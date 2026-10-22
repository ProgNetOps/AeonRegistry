using AeonRegistry.Endpoints.CustomIdentityEndpoints.Models;
using AeonRegistry.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace AeonRegistry.Endpoints.CustomIdentityEndpoints;

public static class CustomIdentityEndpoints
{
    public static IEndpointRouteBuilder MapCustomIdentityEndpoints(this IEndpointRouteBuilder route)
    {
        //Step1: Make a group
        var group = route.MapGroup("/api/auth")
            .WithTags("Admin");//The tag ensures the endpoints are arranged in the named group in the documentation page

        #region Create Endpoints
        //Step2: Make endpoints in the group
        group.MapPost("/register-admin", RegisterUser)
            .WithName("RegisterAdmin")
            .WithDescription("Registers a user with admin role")
            .WithSummary("Register A User")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        //.RequireAuthorization("AdminPolicy");

        group.MapPost("/reset-password",ResetPassword)
            .WithName("ResetPassword")
            .WithDescription("Custom Reset Password for a user")
            .WithSummary("Custom Reset Password")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithDescription("Custom Forgot Password flow")
            .WithSummary("Custom Forgot Password")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/manage/profile", GetProfileInfo)
            .WithName("GetProfileInfo")
            .WithDescription("Get current user profile info")
            .WithSummary("Get the current user profile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();


        #endregion

        //Step3: Implement route handlers


        //Step4: return the route
        return route;

    }

    #region Create Endpoint Handlers
    private static async Task<IResult> RegisterUser(
        RegisterUserRequest request,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender,
        IConfiguration config)
    {

        //See if user email sent already exists
        if(await userManager.FindByEmailAsync(request.Email) is not null) 
        {
            return Results.BadRequest(new { Error = $"User with email {request.Email} already exists" });
        }
        else
        {
            //Create an ApplicationUser instance
            ApplicationUser user = new()
            {
                UserName= request.Email,
                Email= request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
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
                request.Email,
                "Welcome to the Aeon Registry",
                $"""
                Your account has been created. Please change your password by visiting: {baseURL}/Setpassword.html

                {baseURL}/Setpassword.html?email={request.Email}&resetCode={encodedToken}

                """
                );

            return Results.Ok(new {Message = $"User {user.Email} created. Password reset link set."});
        }

    }


    private static async Task<IResult> ResetPassword(
        ResetPasswordRequest request,
        UserManager<ApplicationUser> userManager)
    {

        if(string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.ResetCode)||
            string.IsNullOrEmpty(request.NewPassword))
        {
            return Results.BadRequest(new {Message = "All fields are required"});
        }
        else
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Results.BadRequest(new { Message = "User not found." });
            }

            try
            {
                //Decode the token using the same encoding algorithm
                var decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(request.ResetCode));

                var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

                if (result.Succeeded)
                {
                    return Results.Ok(new { Message = "Password reset successful" });
                }
                else
                {
                    return Results.BadRequest(new { Message = "Error" });
                }
            }
            catch (FormatException)
            {
                return Results.BadRequest(new { Message = "Invalid token" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new {Message=$"Error: {ex.Message}"});
            }
        }
    }


    private static async Task<IResult> ForgotPassword(
        ForgotPasswordRequest request,
        UserManager<ApplicationUser> userManager,
        IEmailSender emailSender,
        IConfiguration config)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return Results.BadRequest(new { Message = "Email Address is required" });
        }

        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.Ok(new { Message = "If the user exists, a Forgot Password link will be sent" });
        }

        //Generate a reset token
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var baseURL = config["BaseURL"] ?? "https://localhost:7230";
        var resetLink = $"{baseURL}/reset-password.html?email={request.Email}&resetCode={encodedToken}";

        await emailSender.SendEmailAsync(
            request.Email,
            "Reset Your Password",
            $"""
                To reset your password: use the link:

                {resetLink}

                """
            );

        return Results.Ok(new { Message = "If the user exists, a Forgot Password link will be sent" });
    }


    private static async Task<IResult> GetProfileInfo(
        ClaimsPrincipal principal,
        UserManager<ApplicationUser> userManager)
    {
        //The current logged-in user
        var user = await userManager.GetUserAsync(principal);

        if(user is null)
        {
            return Results.NotFound();
        }

        var response = new UserProfileResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            FullName = user.FullName
        };

        return Results.Ok(response);
    }

    #endregion

}
