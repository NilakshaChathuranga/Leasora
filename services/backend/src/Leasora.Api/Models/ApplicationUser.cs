using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Leasora.Api.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }
        = DateTimeOffset.UtcNow;
}