using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HousewarmingRegistry.API.Data;
using HousewarmingRegistry.API.DTOs;
using HousewarmingRegistry.API.Models;
using HousewarmingRegistry.API.Helpers;

namespace HousewarmingRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GiftsController : ControllerBase
{
    private readonly RegistryDbContext _context;

    public GiftsController(RegistryDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GiftDto>>> GetGifts()
    {
        var gifts = await _context.Gifts
            .Include(g => g.PurchasedByUser)
            .OrderBy(g => g.CreatedAt)
            .Select(g => new GiftDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                ImageUrl = g.ImageUrl,
                IsPurchased = g.IsPurchased,
                PurchasedByUserId = g.PurchasedByUserId,
                PurchasedBy = g.PurchasedByUser != null ? g.PurchasedByUser.FullName : null,
                CreatedAt = g.CreatedAt
            })
            .ToListAsync();

        return Ok(gifts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GiftDto>> GetGift(int id)
    {
        var gift = await _context.Gifts
            .Include(g => g.PurchasedByUser)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (gift == null)
        {
            return NotFound();
        }

        var giftDto = new GiftDto
        {
            Id = gift.Id,
            Name = gift.Name,
            Description = gift.Description,
            ImageUrl = gift.ImageUrl,
            IsPurchased = gift.IsPurchased,
            PurchasedByUserId = gift.PurchasedByUserId,
            PurchasedBy = gift.PurchasedByUser != null ? gift.PurchasedByUser.FullName : null,
            CreatedAt = gift.CreatedAt
        };

        return Ok(giftDto);
    }

    [HttpPost]
    public async Task<ActionResult<GiftDto>> CreateGift(CreateGiftDto createGiftDto, [FromQuery] int userId)
    {
        var user = await PermissionHelper.GetUserByIdAsync(_context, userId);
        if (!PermissionHelper.CanManageGifts(user))
        {
            return Forbid("Only admins can create gifts.");
        }

        var gift = new Gift
        {
            Name = createGiftDto.Name,
            Description = createGiftDto.Description,
            ImageUrl = createGiftDto.ImageUrl,
            IsPurchased = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Gifts.Add(gift);
        await _context.SaveChangesAsync();

        var giftDto = new GiftDto
        {
            Id = gift.Id,
            Name = gift.Name,
            Description = gift.Description,
            ImageUrl = gift.ImageUrl,
            IsPurchased = gift.IsPurchased,
            PurchasedByUserId = gift.PurchasedByUserId,
            PurchasedBy = null,
            CreatedAt = gift.CreatedAt
        };

        return CreatedAtAction(nameof(GetGift), new { id = gift.Id }, giftDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGift(int id, UpdateGiftDto updateGiftDto, [FromQuery] int userId)
    {
        var user = await PermissionHelper.GetUserByIdAsync(_context, userId);
        if (!PermissionHelper.CanManageGifts(user))
        {
            return Forbid("Only admins can update gifts.");
        }

        var gift = await _context.Gifts.FindAsync(id);

        if (gift == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(updateGiftDto.Name))
            gift.Name = updateGiftDto.Name;
        if (!string.IsNullOrEmpty(updateGiftDto.Description))
            gift.Description = updateGiftDto.Description;
        if (!string.IsNullOrEmpty(updateGiftDto.ImageUrl))
            gift.ImageUrl = updateGiftDto.ImageUrl;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/purchase")]
    public async Task<IActionResult> MarkAsPurchased(int id, MarkPurchasedDto markPurchasedDto)
    {
        var gift = await _context.Gifts.FindAsync(id);

        if (gift == null)
        {
            return NotFound();
        }

        if (gift.IsPurchased)
        {
            return BadRequest("This gift has already been purchased.");
        }

        // Verify user exists and is not admin
        var user = await PermissionHelper.GetUserByIdAsync(_context, markPurchasedDto.UserId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        if (PermissionHelper.IsAdmin(user))
        {
            return BadRequest("Admins cannot mark gifts as purchased.");
        }

        gift.IsPurchased = true;
        gift.PurchasedByUserId = markPurchasedDto.UserId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id}/unpurchase")]
    public async Task<IActionResult> MarkAsUnpurchased(int id, [FromQuery] int userId)
    {
        var gift = await _context.Gifts.FindAsync(id);

        if (gift == null)
        {
            return NotFound();
        }

        var user = await PermissionHelper.GetUserByIdAsync(_context, userId);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        // Only the user who purchased the gift or an admin can unpurchase
        if (!PermissionHelper.IsAdmin(user) && gift.PurchasedByUserId != userId)
        {
            return Forbid("You can only unpurchase gifts that you purchased.");
        }

        gift.IsPurchased = false;
        gift.PurchasedByUserId = null;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGift(int id, [FromQuery] int userId)
    {
        var user = await PermissionHelper.GetUserByIdAsync(_context, userId);
        if (!PermissionHelper.CanManageGifts(user))
        {
            return Forbid("Only admins can delete gifts.");
        }

        var gift = await _context.Gifts.FindAsync(id);
        if (gift == null)
        {
            return NotFound();
        }

        _context.Gifts.Remove(gift);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

