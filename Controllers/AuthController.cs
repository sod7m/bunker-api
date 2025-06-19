using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bunker_api.Data;
using bunker_api.DTOs;
using bunker_api.Models;
using bunker_api.Services;
using BCrypt.Net;

namespace bunker_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthController(ApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Перевірка чи існує користувач з такою поштою
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest("User with this email already exists");
        }

        // Перевірка чи існує користувач з таким нікнеймом
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest("User with this username already exists");
        }

        // Створення нового користувача
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Генерація токенів
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var response = new AuthResponse(
            token,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Email, user.Username, user.CreatedAt, user.LastLoginAt) // <-- без IsEmailVerified
        );

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Знаходження користувача по email або username
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return BadRequest("Invalid email/username or password");
        }

        // Оновлення часу останнього входу
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Генерація токенів
        var token = _jwtService.GenerateToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var response = new AuthResponse(
            token,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Email, user.Username, user.CreatedAt, user.LastLoginAt) // <-- без IsEmailVerified
        );

        return Ok(response);
    }
}