using LogbookSqlServer.Data;
using LogbookSqlServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace LogbookSqlServer.Controllers;

[ApiController]
[Route("api/beneficiaries/{beneficiaryId:int}/members")]
[Authorize]
public class HouseholdMembersController(LogbookDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> All(int beneficiaryId)
    {
        if (!await db.Beneficiaries.AnyAsync(x => x.Id == beneficiaryId)) return NotFound(new { message = "ไม่พบข้อมูลครอบครัว" });
        return Ok(await db.HouseholdMembers.AsNoTracking().Where(x => x.BeneficiaryId == beneficiaryId).OrderBy(x => x.Id).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(int beneficiaryId, HouseholdMember input)
    {
        if (!await db.Beneficiaries.AnyAsync(x => x.Id == beneficiaryId)) return NotFound(new { message = "ไม่พบข้อมูลครอบครัว" });
        if (string.IsNullOrWhiteSpace(input.FullName)) return BadRequest(new { message = "กรุณากรอกชื่อสมาชิกในครอบครัว" });
        if (input.Age is < 0 or > 120) return BadRequest(new { message = "อายุต้องอยู่ระหว่าง 0 ถึง 120 ปี" });
        if (input.MonthlyIncome < 0) return BadRequest(new { message = "รายได้ต้องไม่ติดลบ" });
        input.Id = 0; input.BeneficiaryId = beneficiaryId; input.FullName = input.FullName.Trim(); input.CreatedAt = DateTime.UtcNow;
        db.HouseholdMembers.Add(input);
        await db.SaveChangesAsync();
        var count = await db.HouseholdMembers.CountAsync(x => x.BeneficiaryId == beneficiaryId);
        var family = await db.Beneficiaries.FindAsync(beneficiaryId);
        if (family != null) { family.HouseholdMembers = count + 1; family.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); }
        return Created($"/api/beneficiaries/{beneficiaryId}/members/{input.Id}", input);
    }

    [HttpDelete("{memberId:int}")]
    public async Task<IActionResult> Delete(int beneficiaryId, int memberId)
    {
        var member = await db.HouseholdMembers.FirstOrDefaultAsync(x => x.BeneficiaryId == beneficiaryId && x.Id == memberId);
        if (member == null) return NotFound(new { message = "ไม่พบสมาชิกในครอบครัว" });
        db.HouseholdMembers.Remove(member);
        await db.SaveChangesAsync();
        var count = await db.HouseholdMembers.CountAsync(x => x.BeneficiaryId == beneficiaryId);
        var family = await db.Beneficiaries.FindAsync(beneficiaryId);
        if (family != null) { family.HouseholdMembers = count + 1; family.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(); }
        return NoContent();
    }
}
