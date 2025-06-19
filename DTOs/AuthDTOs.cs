using System.ComponentModel.DataAnnotations;

namespace bunker_api.DTOs;

public record RegisterRequest(
    [Required] [MinLength(3)] string Username,
    [Required] [EmailAddress] string Email,
    [Required] [MinLength(6)] string Password
);

public record LoginRequest(
    [Required] [EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record UserDto(
    int Id,
    string Email,
    string Username,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);