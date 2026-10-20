using Microsoft.Extensions.Options;

namespace AeonRegistry.Extensions;
/// <summary>
/// A static class for extending IServiceCollection
/// </summary>
public static class OpenAPISwaggerExtensions
{
    /// <summary>
    /// Customizes the appearance of the API documentation page
    /// </summary>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(C =>
        {
            C.SwaggerDoc("v1", new OpenApiInfo
            {
                Title="Aeon Registry API",
                Version="v1",
                Description= """
                
                <img src="/images/AeonRegistry.png" height="120">

                ## Aeon Reasearch Division

                Internal API for managing recovered artifacts and research data.
                Provides secure access for field researchers and analysts.

                ### Key Features:
                -Site and Artifact Catalog
                -Research record submissions
                -Secure media storage
                -User role management

                """,
                Contact = new OpenApiContact
                {
                    Name = "Aeon Registry Team",
                    Url = new Uri("https://www.mywebsite.com"),
                    Email = "support@mycompany.com"
                }
            });
            

            C.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
            {
                Name="Authorization",
                Type=SecuritySchemeType.Http,
                Scheme="bearer",
                BearerFormat = "JWT",
                In=ParameterLocation.Header,
                Description=$"Enter 'Bearer' [space] and then your valid JWT token"
            });
            

            C.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });


            //A collection of the Identity endpoint we want to hide from our documentation
            string[] hiddenEndpoints = [
                "api/auth/register",
                "api/auth/refresh",
                "api/auth/confirmemail",
                "api/auth/resendconfirmationemail",
                "api/auth/forgotpassword",
                "api/auth/resetpassword",
                "api/auth/manage",
                "api/auth/manage/info",
                "api/auth/manage/2fa"];

            C.DocInclusionPredicate((docName, apiDesc) =>
            {
                var path = apiDesc.RelativePath?.ToLowerInvariant();

                if(path is null)
                {
                    return false;
                }

                if (hiddenEndpoints.Contains(path, StringComparer.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            });
        });

        return services;
    }
}