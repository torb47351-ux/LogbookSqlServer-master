using System.ComponentModel.DataAnnotations;
namespace LogbookSqlServer.Models;
public class BeneficiaryMedia
{
    public int Id { get; set; }
    public int BeneficiaryId { get; set; }
    public Beneficiary? Beneficiary { get; set; }
    [Required, MaxLength(20)] public string Category { get; set; } = "Assistance";
    [MaxLength(200)] public string? Agency { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    public DateTime? AssistanceDate { get; set; }
    [Required, MaxLength(500)] public string ImagePath { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
