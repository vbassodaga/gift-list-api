using HousewarmingRegistry.API.Data;
using HousewarmingRegistry.API.Models;

namespace HousewarmingRegistry.API.Helpers;

public static class PermissionHelper
{
    public static async Task<User?> GetUserByIdAsync(RegistryDbContext context, int userId)
    {
        return await context.Users.FindAsync(userId);
    }

    public static bool IsAdmin(User? user)
    {
        return user != null && user.IsAdmin;
    }

    public static bool CanManageGifts(User? user)
    {
        return IsAdmin(user);
    }

    public static bool CanPurchaseGifts(User? user)
    {
        return user != null && !IsAdmin(user);
    }
}

