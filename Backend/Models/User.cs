namespace FlowerShop.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // For invalidating sessions on password change or user ban
    public DateTime LastChanged { get; set; } = DateTime.UtcNow;
}
