using System.ComponentModel.DataAnnotations;
namespace LogbookSqlServer.Models;
public class HouseholdMember
{
    public int Id { get; set; }
    public int BeneficiaryId { get; set; }
    [Required, MaxLength(200)] public string FullName { get; set; } = "";
    [MaxLength(50)] public string? Relationship { get; set; }
    public int? Age { get; set; }
    [MaxLength(150)] public string? Occupation { get; set; }
    public decimal? MonthlyIncome { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Beneficiary? Beneficiary { get; set; }
}
