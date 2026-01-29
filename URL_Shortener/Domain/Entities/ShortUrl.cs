using System.ComponentModel.DataAnnotations;

namespace URL_Shortener.Domain.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(2048)]
    public string OriginalUrl { get; set; }

    [Required]
    [MaxLength(20)]
    public string ShortCode { get; set; }

    [Required]
    public string CreatedByUserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public ApplicationUser CreatedByUser { get; set; }
}