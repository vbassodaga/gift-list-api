namespace HousewarmingRegistry.API.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.SimpleUser;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual ICollection<Gift> PurchasedGifts { get; set; } = new List<Gift>();

    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdmin => Role == UserRole.Admin;
}

