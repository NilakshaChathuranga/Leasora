using System.ComponentModel.DataAnnotations;

namespace Leasora.Api.Models;

public class FinanceOfficerProfile
{
    [Key]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;

    public Guid FinanceCompanyId { get; set; }

    public FinanceCompany FinanceCompany { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string EmployeeId { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string CorporateEmail { get; set; } = string.Empty;

    public OfficerVerificationStatus VerificationStatus { get; set; }
        = OfficerVerificationStatus.Pending;
    [MaxLength(500)]
    public string? VerificationProofStorageKey { get; set; }

    public DateTimeOffset? SubmittedAtUtc { get; set; }

    public DateTimeOffset? ReviewedAtUtc { get; set; }

    public string? ReviewedByUserId { get; set; }

    public ApplicationUser? ReviewedByUser { get; set; }

    [MaxLength(1000)]
    public string? ReviewNotes { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
        = DateTimeOffset.UtcNow;
}