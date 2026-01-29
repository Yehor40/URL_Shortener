using System.ComponentModel.DataAnnotations;

namespace URL_Shortener.Domain.Entities;

public class AboutInfo
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; }
}
