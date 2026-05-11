using System;
using ecommerce.Contracts.Abstractions;
using Microsoft.AspNetCore.Http;

namespace ecommerce.Contracts.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials
        = new("User.InvalidCredentials", "Invalid email or password", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidToken
        = new("User.InvalidToken", "Invalid Token", StatusCodes.Status400BadRequest);

    public static readonly Error UserNotFound
        = new("User.NotFound", "User Not Found", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedEmail
        = new("User.DuplicatedEmail", "This email address is already registered", StatusCodes.Status409Conflict);

    public static readonly Error DuplicatedPhone
        = new("User.DuplicatedPhone", "This phone number is already registered", StatusCodes.Status409Conflict);

    public static readonly Error ExpiredCode
        = new("User.ExpiredCode", "OTP code has expired, please request a new one", StatusCodes.Status400BadRequest);

    public static readonly Error NotConfirmedAccount
        = new("User.NotConfirmedAccount", "Not Confirmed Account", StatusCodes.Status400BadRequest);

    public static readonly Error InvalidCode
        = new("User.InvalidCode", "Invalid Code", StatusCodes.Status400BadRequest);

    public static readonly Error AlreadyConfirmed
        = new("User.AlreadyConfirmed", Description: "Account is already confirmed", StatusCodes.Status400BadRequest);

    public static readonly Error UpdateFailed
        = new("User.UpdateFailed", Description: "Failed to update user profile", StatusCodes.Status400BadRequest);

    public static readonly Error PasswordChangeFailed
        = new("User.PasswordChangeFailed", Description: "Failed to change user password", StatusCodes.Status400BadRequest);

    public static readonly Error DisabledUser
        = new("User.DisabledUser", "User is disabled", StatusCodes.Status400BadRequest);

    public static readonly Error Forbidden
        = new("User.Forbidden", "You do not have access to this resource", StatusCodes.Status403Forbidden);

    public static readonly Error TooManyRequests
        = new("User.TooManyRequests", "Too many failed attempts. Please try again later.", StatusCodes.Status429TooManyRequests);

    public static readonly Error PasswordMatchesCurrent
        = new("User.PasswordMatchesCurrent", "New password cannot be the same as the current password.", StatusCodes.Status400BadRequest);
}
