using AeonRegistry.Models.Response;

namespace AeonRegistry.Services.Interfaces;

public interface ISiteService
{
    Task<List<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct);
    Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int Id, CancellationToken ct);

}
