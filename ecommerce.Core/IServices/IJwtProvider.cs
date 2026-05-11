using System;
using ecommerce.Core.Entities;

namespace ecommerce.Core.IServices;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);

    string? ValidateToken(string token);
}
