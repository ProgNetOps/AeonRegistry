using System.ComponentModel.DataAnnotations;

namespace AeonRegistry.Models;

public class CatalogNote
{
    public int Id { get; set; }
    public required int CatalogRecordId { get; set; }
    public CatalogRecord? CatalogRecord { get; set; }
    public string AuthorId { get; set; } = string.Empty;//Foreign key to application user
    public ApplicationUser? Author { get; set; } = null!;
    [Required,MaxLength(1000)]
    public string Content { get; set; } = string.Empty;
    public required DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
