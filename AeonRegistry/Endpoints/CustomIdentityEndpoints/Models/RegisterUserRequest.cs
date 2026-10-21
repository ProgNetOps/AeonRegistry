
namespace AeonRegistry.Endpoints.CustomIdentityEndpoints.Models;

public class RegisterUserRequest
{
    public required string? Email { get; set; }
    public required string? FirstName { get; set; }
    public required string? LastName { get; set; }
}
