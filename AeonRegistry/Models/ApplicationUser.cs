using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AeonRegistry.Models;
/// <summary>
/// Class that extends identity user
/// </summary>
public class ApplicationUser:IdentityUser
{
    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public ICollection<CatalogRecord> SubmittedCatalogRecords { get; set; } = [];
    public ICollection<CatalogRecord> VerifiedCatalogRecords { get; set; } = [];
    public ICollection<ArtifactMediaFile> UploadedMediaFiles { get; set; } = [];
}
