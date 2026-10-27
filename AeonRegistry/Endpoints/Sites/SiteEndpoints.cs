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
            .WithTags("Sites - Public");


        //then endpoints
        publicGroup.MapGet("", GetAllPublicSites)
            .WithName(nameof(GetAllPublicSites))
            .Produces<List<PublicSiteResponse>>(StatusCodes.Status200OK)
            .WithSummary("Get All Sites (Public)")
            .WithDescription("Returns all sites with only htheir public data");

        return route;
    }



    //Method Handlers for Sites
    private static async Task<Ok<List<PublicSiteResponse>>> GetAllPublicSites(
        ISiteService siteService, CancellationToken ct)
    {
        var sites = await siteService.GetAllPublicSitesAsync(ct);
        return TypedResults.Ok(sites);
    }
}
