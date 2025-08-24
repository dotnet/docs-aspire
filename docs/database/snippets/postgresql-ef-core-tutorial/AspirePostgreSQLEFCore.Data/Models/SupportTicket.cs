using System.ComponentModel.DataAnnotations;

namespace AspirePostgreSQLEFCore.Data.Models;

public sealed class SupportTicket
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [Required]
    public string Status { get; set; } = "Open";

    public string Priority { get; set; } = "Medium";
}