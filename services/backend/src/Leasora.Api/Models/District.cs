using System.ComponentModel.DataAnnotations;

namespace Leasora.Api.Models;

// Represents a district belonging to a Sri Lankan province.
public class District
{
    // Unique identifier for this district.
    public int Id { get; set; }

    // District name displayed in the app, such as "Colombo".
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Stores the identifier of the province this district belongs to.
    public int ProvinceId { get; set; }

    // Allows access to the related province when it is loaded.
    public Province Province { get; set; } = null!;
}