using System.ComponentModel.DataAnnotations;

namespace bunker_api.Models;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastLoginAt { get; set; }
    
    public bool IsEmailVerified { get; set; } = false; // Повертаємо поле зі значенням false за замовчуванням
}