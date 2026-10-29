using AeonRegistry.Models.Response;
using AeonRegistry.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AeonRegistry.Services;

public class SiteService(ApplicationDbContext db) : ISiteService
{
    public async Task<List<PublicSiteResponse>> GetAllPublicSitesAsync(CancellationToken ct)
    {
        return await db.Sites
            .AsNoTracking()
            .Select(s => new PublicSiteResponse
            {
                Id= s.Id,
                Name = s.Name,
                Location = s.Location,
                Coordinates = s.Coordinates,
                Latitude = s.Latitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative
            })
            .ToListAsync(ct);
    }

    public async Task<PublicSiteResponse?> GetPublicSiteByIdAsync(int Id, CancellationToken ct)
    {
        return await db.Sites
            .AsNoTracking()
            .Where(s => s.Id == Id)
            .Select(s => new PublicSiteResponse
            {
                Id = s.Id,
                Name = s.Name,
                Location = s.Location,
                Coordinates = s.Coordinates,
                Latitude = s.Latitude,
                Description = s.Description,
                PublicNarrative = s.PublicNarrative
            })
            .FirstOrDefaultAsync();
    }
}
