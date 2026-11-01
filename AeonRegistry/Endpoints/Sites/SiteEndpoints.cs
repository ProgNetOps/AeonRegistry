using AeonRegistry.Filters;
using AeonRegistry.Models.Response;
using AeonRegistry.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AeonRegistry.Endpoints.Sites;

public static class SiteEndpoints
{
    //Endpoint Groups for Sites
    public static IEndpointRouteBuilder MapSiteEndpoints(
        this IEndpointRouteBuilder route)
    {
        //first group
        var publicGroup = route.MapGroup("/api/public/sites")
            .AllowAnonymous()
            .WithSummary("Public Site Endpoints")
            .WithDescription("Endpoints that expose public sites")
            .WithTags("Sites - Public")
            .AddEndpointFilter<ExceptionHandlingFilter>();

        var privateGroup = route.MapGroup("api/private/sites")
            .RequireAuthorization()
            .WithTags("Sites - Private")
            .WithSummary("Authorized Site Endpoint")
            .WithDescription("Endpoints that require authentication");


        //then endpoints
        publicGroup.MapGet("", GetAllPublicSites)
            .WithName(nameof(GetAllPublicSites))
            .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get All Sites (Public)")
            .WithDescription("Returns all sites with only htheir public data");
        
        publicGroup.MapGet("/{id:int}", GetPublicSiteById)
            .WithName(nameof(GetPublicSiteById))
            .Produces<PublicSiteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get Site by Id (public)")
            .WithDescription("Returns a single site with public data");

        privateGroup.MapGet("", GetAllPrivateSites)
            .WithName(nameof(GetAllPrivateSites))
            .Produces<List<PrivateSiteResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get All Sites - Private")
            .WithDescription("Returns all sites requires Authentication");

        privateGroup.MapGet("/{id:int}", GetPrivateSiteById)
            .WithName(nameof(GetPrivateSiteById))
            .Produces<PrivateSiteResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithSummary("Get Site by Id - Private")
            .WithDescription("Returns all sites requires Authentication");

        return route;
    }



    //Method Handlers for Sites
    private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(
        ISiteService siteService, CancellationToken ct)
    {
        var sites = await siteService.GetAllPublicSitesAsync(ct);
        return TypedResults.Ok(sites);
    }

    private static async Task<Results<Ok<PublicSiteResponse>, NotFound>> GetPublicSiteById
        (int id, ISiteService service, CancellationToken ct)
    {
        var site = await service.GetPublicSiteByIdAsync(id, ct);

        return site is null ?
            TypedResults.NotFound() :
            TypedResults.Ok(site);
    }

    private static async Task<Ok<List<PrivateSiteResponse>>> GetAllPrivateSites(
       ISiteService siteService, CancellationToken ct)
    {
        var sites = await siteService.GetAllPrivateSitesAsync(ct);
        return TypedResults.Ok(sites);
    }

    private static async Task<Results<Ok<PrivateSiteResponse>, NotFound>> GetPrivateSiteById
        (int id, ISiteService service, CancellationToken ct)
    {
        var site = await service.GetPrivateSiteByIdAsync(id, ct);

        return site is null ?
            TypedResults.NotFound() :
            TypedResults.Ok(site);
    }

}
