using Leasora.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Leasora.Api.Data;

// Adds missing Sri Lankan provinces and districts to the database.
public static class LocationSeeder
{
    public static async Task SeedAsync(LeasoraDbContext db)
    {
        // Groups each province with the districts that belong to it.
        (string ProvinceName, string[] DistrictNames)[] locations =
        [
            ("Western", ["Colombo", "Gampaha", "Kalutara"]),
            ("Central", ["Kandy", "Matale", "Nuwara Eliya"]),
            ("Southern", ["Galle", "Matara", "Hambantota"]),
            ("Northern", [
                "Jaffna", "Kilinochchi", "Mannar",
                "Vavuniya", "Mullaitivu"
            ]),
            ("Eastern", ["Batticaloa", "Ampara", "Trincomalee"]),
            ("North Western", ["Kurunegala", "Puttalam"]),
            ("North Central", ["Anuradhapura", "Polonnaruwa"]),
            ("Uva", ["Badulla", "Monaragala"]),
            ("Sabaragamuwa", ["Ratnapura", "Kegalle"])
        ];

        // Loads existing records so repeated runs can skip them.
        var provinces = await db.Provinces.ToListAsync();
        var districts = await db.Districts.ToListAsync();

        foreach (var location in locations)
        {
            // Finds an existing province without changing its ID.
            var province = provinces.SingleOrDefault(existing =>
                string.Equals(
                    existing.Name,
                    location.ProvinceName,
                    StringComparison.OrdinalIgnoreCase));

            if (province is null)
            {
                // The database will generate the new province's ID.
                province = new Province
                {
                    Name = location.ProvinceName
                };

                db.Provinces.Add(province);
                provinces.Add(province);
            }

            foreach (var districtName in location.DistrictNames)
            {
                // Checks both the province and district name.
                var districtExists = districts.Any(existing =>
                    existing.ProvinceId == province.Id &&
                    string.Equals(
                        existing.Name,
                        districtName,
                        StringComparison.OrdinalIgnoreCase));

                if (districtExists)
                {
                    continue;
                }

                // Links the district to its province.
                // EF Core sets ProvinceId when saving new records.
                var district = new District
                {
                    Name = districtName,
                    Province = province
                };

                db.Districts.Add(district);
                districts.Add(district);
            }
        }

        // Saves all missing provinces and districts together.
        await db.SaveChangesAsync();
    }
}