using System.ComponentModel.DataAnnotations;

namespace Leasora.Api.Contracts.Auth;

// Contains the information required to register a customer.
public sealed class RegisterCustomerRequest
{
    // Customer's name displayed in the application.
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    // Email will also be used as the customer's login username.
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    // Identity will perform additional password-security validation.
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    // Must contain the same value as Password.
    [Required]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}