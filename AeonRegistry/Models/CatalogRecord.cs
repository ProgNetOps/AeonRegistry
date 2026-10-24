using AeonRegistry.Enums;

namespace AeonRegistry.Models;

public class CatalogRecord
{
    public int Id { get; set; }
    public required int ArtifactId { get; set; }
    public Artifact? Artifact { get; set; }
    public required string SubmittedById { get; set; }//Foreign key to application user
    public ApplicationUser SubmittedBy { get; set; } = null!;
    public string? VerifiedById { get; set; }//Foreign key to application user
    public ApplicationUser? VerifiedBy { get; set; } = null;
    public required string Status { get; set; } = CatalogStatus.Draft.ToString();
    public required DateTime DateSubmitted { get; set; } = DateTime.UtcNow;
    public ICollection<CatalogNote> Notes { get; set; } = [];

}
