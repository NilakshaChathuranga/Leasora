using Leasora.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Leasora.Api.Data;

// Creates the application's roles if they do not already exist.
public static class RoleSeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager)
    {
        // Reuses the role names defined in ApplicationRoles.
        string[] roleNames =
        [
            ApplicationRoles.Customer,
            ApplicationRoles.FinanceOfficer,
            ApplicationRoles.Admin
        ];

        foreach (var roleName in roleNames)
        {
            // Skip existing roles when the seeder runs again.
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            // Creates and saves the missing role through Identity.
            var result = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            // Report failures instead of silently leaving roles missing.
            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Failed to create role '{roleName}': {errors}");
            }
        }
    }
}