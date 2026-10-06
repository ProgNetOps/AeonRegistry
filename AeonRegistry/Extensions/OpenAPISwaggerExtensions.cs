using Microsoft.Extensions.Options;

namespace AeonRegistry.Extensions;
/// <summary>
/// Customizes the look of the API documentation page
/// </summary>
public static class OpenAPISwaggerExtensions
{
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
        });

        return services;
    }
}
