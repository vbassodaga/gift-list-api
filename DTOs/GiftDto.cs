namespace HousewarmingRegistry.API.DTOs;

public class GiftDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPurchased { get; set; }
    public int? PurchasedByUserId { get; set; }
    public string? PurchasedBy { get; set; } // Full name of purchaser
    public DateTime CreatedAt { get; set; }
}

public class CreateGiftDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class UpdateGiftDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}

