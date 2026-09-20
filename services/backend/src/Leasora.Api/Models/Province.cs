using System.ComponentModel.DataAnnotations;

namespace Leasora.Api.Models;

// Represents a Sri Lankan province that contains districts.
public class Province
{
    // Unique identifier used to link districts to this province.
    public int Id { get; set; }

    // Province name, such as "Western".
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}