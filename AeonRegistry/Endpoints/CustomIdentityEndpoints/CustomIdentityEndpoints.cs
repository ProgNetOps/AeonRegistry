using AeonRegistry.Endpoints.CustomIdentityEndpoints.Models;
using AeonRegistry.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AeonRegistry.Endpoints.CustomIdentityEndpoints;

public static class CustomIdentityEndpoints
{
    public static IEndpointRouteBuilder MapCustomIdentityEndpoints(this IEndpointRouteBuilder route)
    {
        //Step1: Make a group
        var group = route.MapGroup("/api/auth")
            .WithTags("Admin");//The tag ensures the endpoints are arranged in the named group in the documentation page

        //Step2: Make endpoints in the group
        group.MapPost("/register-admin", RegisterUser)
            .WithName("RegisterAdmin")
            .WithSummary("Register A User")
            .WithDescription("Registers a user with admin role");
        //.RequireAuthorization("AdminPolicy");


        //Step3: Implement route handlers


        //Step4: return the route
        return route;

    }

    //route handler
    private static async Task<IResult> RegisterUser(RegisterUserRequest dto,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender,
        IConfiguration config)
    {

        //See if user email sent already exists
        if(await userManager.FindByEmailAsync(dto.Email) is not null) 
        {
            return Results.BadRequest(new { Error = $"User with email {dto.Email} already exists" });
        }
        else
        {
            //Create an ApplicationUser instance
            ApplicationUser user = new()
            {
                UserName=dto.Email,
                Email=dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            //Create a temporary password that will be emailed to the user
            //The temporary password must meet the password policy
            var tempPassword = "TempPassword123!";

            //create the user with the  temppassword
            var created = await userManager.CreateAsync(user, tempPassword);
        
            if(created.Succeeded is false)
            {
                return Results.BadRequest(new { Errors = created.Errors });
            }

            if (await roleManager.RoleExistsAsync("Researcher") is true)
            {
                await userManager.AddToRoleAsync(user, "Researcher");
            }

            //Generate password reset token
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            //Encode the token so it can be emailed for reset
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                
            //Send email to user to change password [User can't login because they don't have a password, they must change the password]
            var baseURL = config["BaseURL"] ?? "https://localhost:7230";

            await emailSender.SendEmailAsync(
                dto.Email,
                "Welcome to the Aeon Registry",
                $"""
                Your account has been created. Please change your password by visiting: {baseURL}/Setpassword.html

                {baseURL}/Setpassword.html?email={dto.Email}&resetCode={encodedToken}

                """
                );

            return Results.Ok(new {Message = $"User {user.Email} created. Password reset link set."});
        }

    }
}
