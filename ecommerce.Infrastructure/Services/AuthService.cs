using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;
using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Auth;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using ecommerce.Infrastructure.Options;
using MapsterMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace ecommerce.Infrastructure.Services;

public class AuthService(
    IAuthRepository authRepository,
    IAddressRepository addressRepository,
    IUnitOfWork unitOfWork,
    IJwtProvider jwtProvider,
    IEmailService emailService,
    IMapper mapper,
    IDistributedCache cache,
    IOptions<AuthSettings> authSettings) : IAuthService
{
    private readonly IAuthRepository _authRepository = authRepository;
    private readonly IAddressRepository _addressRepository = addressRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IEmailService _emailService = emailService;
    private readonly IMapper _mapper = mapper;
    private readonly IDistributedCache _cache = cache;
    private readonly AuthSettings _authSettings = authSettings.Value;
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _authRepository.CheckEmailAvailabilityAsync(request.Email))
            return Result.Failure(UserErrors.DuplicatedEmail);

        if (await _authRepository.CheckPhoneAvailabilityAsync(request.Phone))
            return Result.Failure(UserErrors.DuplicatedPhone);

        var user = _mapper.Map<ApplicationUser>(request);
        user.UserName = request.Email;

        var createResult = await _authRepository.CreateUserAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var error = createResult.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        await _authRepository.AssignRoleAsync(user, DefaultRoles.Customer);

        var address = _mapper.Map<Address>(request.Address);
        address.UserId = user.Id;
        await _addressRepository.AddAsync(address, cancellationToken);
        await _unitOfWork.Complete();

        var otp = GenerateOtp();
        await _authRepository.SetOtpAsync(user, otp, _authSettings.OtpExpiryMinutes);

        await _emailService.SendEmailAsync(
            email: user.Email!,
            subject: "Confirm your GiftWave account",
            template: "EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{UserName}}", user.FirstName },
                { "{{OTP}}",      otp }
            },
            cancellationToken: cancellationToken
        );

        return Result.Success();
    }

    public async Task<Result<AuthResponse>> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        if (user.EmailConfirmed)
            return Result.Failure<AuthResponse>(UserErrors.AlreadyConfirmed);

        var (storedOtp, expiry) = await _authRepository.GetOtpAsync(user);

        if (storedOtp is null || expiry is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCode);

        if (DateTime.UtcNow > expiry)
            return Result.Failure<AuthResponse>(UserErrors.ExpiredCode);

        if (storedOtp != request.Code)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCode);

        await _authRepository.ConfirmUserEmailAsync(user);
        await _authRepository.RemoveOtpAsync(user);

        var roles = await _authRepository.GetRolesAsync(user);
        var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, []);

        return Result.Success(new AuthResponse(
            user.FirstName,
            user.LastName,
            user.Email!,
            roles,
            token,
            expiresIn
        ));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = request.Identifier.Contains('@')
            ? await _authRepository.FindByEmailAsync(request.Identifier)
            : await _authRepository.FindByPhoneAsync(request.Identifier);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var lockoutKey = $"Lockout:{user.Email}";
        var attemptsStr = await _cache.GetStringAsync(lockoutKey, cancellationToken);
        var attempts = int.TryParse(attemptsStr, out var a) ? a : 0;

        if (attempts >= _authSettings.MaxLoginAttempts)
            return Result.Failure<AuthResponse>(UserErrors.TooManyRequests);

        var signInResult = await _authRepository.CheckPasswordAsync(user, request.Password, false, false);
        if (!signInResult.Succeeded)
        {
            attempts++;
            await _cache.SetStringAsync(lockoutKey, attempts.ToString(), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(_authSettings.LockoutMinutes)
            }, cancellationToken);

            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        await _cache.RemoveAsync(lockoutKey, cancellationToken);

        var roles = await _authRepository.GetRolesAsync(user);
        var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, []);

        return Result.Success(new AuthResponse(
            user.FirstName,
            user.LastName,
            user.Email!,
            roles,
            token,
            expiresIn
        ));
    }

    public async Task<Result> LogoutAsync(string jti, DateTime expiresAt, CancellationToken cancellationToken = default)
    {
        var remainingLifetime = expiresAt - DateTime.UtcNow;
        if (remainingLifetime > TimeSpan.Zero)
        {
            await _cache.SetStringAsync($"Blacklist:{jti}", "true", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = remainingLifetime
            }, cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Success();

        var otp = GenerateOtp();
        await _authRepository.SetOtpAsync(user, otp, _authSettings.OtpExpiryMinutes);

        await _emailService.SendEmailAsync(
            email: user.Email!,
            subject: "Reset your GiftWave password",
            template: "PasswordReset",
            templateModel: new Dictionary<string, string>
            {
                { "{{UserName}}", user.FirstName },
                { "{{OTP}}",      otp }
            },
            cancellationToken: cancellationToken
        );

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _authRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(UserErrors.InvalidCode);

        var (storedOtp, expiry) = await _authRepository.GetOtpAsync(user);

        if (storedOtp is null || expiry is null || DateTime.UtcNow > expiry || storedOtp != request.Code)
            return Result.Failure(UserErrors.InvalidCode);

        // Check if new password matches current
        var isCurrentPassword = await _authRepository.CheckPasswordAsync(user, request.NewPassword, false, false);
        if (isCurrentPassword.Succeeded)
            return Result.Failure(UserErrors.PasswordMatchesCurrent);

        var resetResult = await _authRepository.ResetPasswordAsync(user, request.NewPassword);
        if (!resetResult.Succeeded)
            return Result.Failure(new Error("PasswordResetFailed", resetResult.Errors.First().Description, 400));

        await _authRepository.RemoveOtpAsync(user);

        // Global token invalidation
        var nowUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await _cache.SetStringAsync($"SessionRevokedBefore:{user.Id}", nowUnixSeconds.ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_authSettings.SessionRevokedCacheDays)
        }, cancellationToken);

        return Result.Success();
    }

    private static string GenerateOtp() =>
        Random.Shared.Next(100_000, 999_999).ToString();
}
