using System;

namespace ecommerce.Contracts.Abstractions.Constants;

public static class RegexPatterns
{
    public const string Password = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[#?!@$%^&*\-]).{8,}$";
}