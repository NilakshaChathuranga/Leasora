using System.ComponentModel.DataAnnotations;
using Leasora.Api.Contracts.Auth;
using Leasora.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Leasora.Api.Endpoints;

// Contains all authentication-related API endpoints.
public static class AuthEndpoints
{
    // Registers the authentication routes in the application.
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        // All routes in this group start with /api/auth.
        var authGroup = endpoints
            .MapGroup("/api/auth")
            .WithTags("Authentication");

        // Creates the customer registration endpoint.
        authGroup.MapPost(
            "/customers/register",
            RegisterCustomerAsync);

        return endpoints;
    }

    // Creates a new customer account.
    private static async Task<IResult> RegisterCustomerAsync(
        RegisterCustomerRequest request,
        UserManager<ApplicationUser> userManager)
    {
        // Checks the validation attributes in RegisterCustomerRequest.
        var validationErrors = Validate(request);

        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors);
        }

        // Removes accidental spaces from the email.
        var email = request.Email.Trim();

        // Prevents registering the same email more than once.
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return Results.Conflict(new
            {
                message = "An account with this email already exists."
            });
        }

        // Creates the user object that will be stored by ASP.NET Identity.
        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = email,
            UserName = email
        };

        // Hashes the password securely and saves the user.
        var createResult = await userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
        {
            return Results.ValidationProblem(
                ToValidationErrors(createResult.Errors));
        }

        // Gives every account created here the Customer role.
        var roleResult = await userManager.AddToRoleAsync(
            user,
            ApplicationRoles.Customer);

        if (!roleResult.Succeeded)
        {
            // Removes the incomplete account if role assignment fails.
            await userManager.DeleteAsync(user);

            return Results.ValidationProblem(
                ToValidationErrors(roleResult.Errors));
        }

        // Returns HTTP 201 without returning the password.
        return Results.Created(
            $"/api/users/{user.Id}",
            new
            {
                user.Id,
                user.FullName,
                user.Email,
                Role = ApplicationRoles.Customer
            });
    }

    // Converts Data Annotation errors into an API-friendly format.
    private static Dictionary<string, string[]> Validate(
        RegisterCustomerRequest request)
    {
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        return results
            .SelectMany(result =>
                (result.MemberNames.Any()
                    ? result.MemberNames
                    : new[] { "request" })
                .Select(member => new
                {
                    Member = member,
                    Message = result.ErrorMessage ?? "Invalid value."
                }))
            .GroupBy(error => error.Member)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.Message)
                    .ToArray());
    }

    // Converts ASP.NET Identity errors into an API-friendly format.
    private static Dictionary<string, string[]> ToValidationErrors(
        IEnumerable<IdentityError> errors)
    {
        return errors
            .GroupBy(error => error.Code)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.Description)
                    .ToArray());
    }
}