namespace Leasora.Api.Models;

// Each record connects one officer to one service district.
public class OfficerServiceDistrict
{
    // References the officer profile's UserId.
    public string OfficerUserId { get; set; } = string.Empty;

    // Provides access to the officer profile when loaded.
    public FinanceOfficerProfile OfficerProfile { get; set; } = null!;

    // References the district the officer serves.
    public int DistrictId { get; set; }

    // Provides access to the district details when loaded.
    public District District { get; set; } = null!;
}