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
            C.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
            {
                Title="Aeon Registry API",
                Version="v1",
                Description="Test Description"
            });
        });



        return services;
    }
}
