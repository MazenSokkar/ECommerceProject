using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;
using ecommerce.Contracts.Auth;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Registration successful. Please check your email to confirm your account."))
            : result.ToProblem();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ConfirmEmailAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Email confirmed successfully."))
            : result.ToProblem();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Login successful."))
            : result.ToProblem();
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var jti = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;
        var expString = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp)?.Value;

        if (string.IsNullOrEmpty(jti) || !long.TryParse(expString, out var expTicks))
            return BadRequest();

        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expTicks).UtcDateTime;

        var result = await authService.LogoutAsync(jti, expiresAt, cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Logged out successfully."))
            : result.ToProblem();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ForgotPasswordAsync(request, cancellationToken);

        return result.IsSuccess
            // Always return success even if user not found to prevent enumeration
            ? Ok(ResponseGenerator.GenerateSuccessResponse("If your email is registered, you will receive an OTP shortly."))
            : result.ToProblem();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var result = await authService.ResetPasswordAsync(request, cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Password reset successfully. You can now login with your new password."))
            : result.ToProblem();
    }
}
