using LogbookSqlServer.Data;
using LogbookSqlServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LogbookSqlServer.Controllers;

[ApiController]
[Route("api/beneficiaries/{beneficiaryId:int}/media")]
[Authorize]
public class BeneficiaryMediaController(LogbookDbContext db, IWebHostEnvironment env) : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    [HttpGet]
    public async Task<IActionResult> GetAll(int beneficiaryId)
    {
        if (!await db.Beneficiaries.AnyAsync(x => x.Id == beneficiaryId)) return NotFound(new { message = "ไม่พบข้อมูลบุคคล" });
        return Ok(await db.BeneficiaryMedia.AsNoTracking().Where(x => x.BeneficiaryId == beneficiaryId).OrderByDescending(x => x.UploadedAt).ToListAsync());
    }

    [HttpPost]
    [RequestSizeLimit(10_500_000)]
    public async Task<IActionResult> Upload(int beneficiaryId, [FromForm] IFormFile? file, [FromForm] string category, [FromForm] string? agency, [FromForm] string? description, [FromForm] DateTime? assistanceDate)
    {
        if (!await db.Beneficiaries.AnyAsync(x => x.Id == beneficiaryId)) return NotFound(new { message = "ไม่พบข้อมูลบุคคล" });
        if (file == null || file.Length == 0) return BadRequest(new { message = "กรุณาเลือกรูปภาพ" });
        if (file.Length > 10 * 1024 * 1024) return BadRequest(new { message = "รูปภาพต้องมีขนาดไม่เกิน 10 MB" });
        var ext = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(ext)) return BadRequest(new { message = "รองรับเฉพาะไฟล์ JPG, PNG และ WEBP" });
        if (category is not ("House" or "Assistance")) return BadRequest(new { message = "ประเภทภาพไม่ถูกต้อง" });

        var folder = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads", "beneficiaries", beneficiaryId.ToString());
        Directory.CreateDirectory(folder);
        var filename = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
        var fullPath = Path.Combine(folder, filename);
        await using (var stream = System.IO.File.Create(fullPath)) await file.CopyToAsync(stream);
        var item = new BeneficiaryMedia { BeneficiaryId = beneficiaryId, Category = category, Agency = string.IsNullOrWhiteSpace(agency) ? null : agency.Trim(), Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(), AssistanceDate = assistanceDate, ImagePath = $"/uploads/beneficiaries/{beneficiaryId}/{filename}", UploadedAt = DateTime.UtcNow };
        db.BeneficiaryMedia.Add(item);
        await db.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{mediaId:int}")]
    public async Task<IActionResult> Delete(int beneficiaryId, int mediaId)
    {
        var item = await db.BeneficiaryMedia.FirstOrDefaultAsync(x => x.Id == mediaId && x.BeneficiaryId == beneficiaryId);
        if (item == null) return NotFound(new { message = "ไม่พบรูปภาพ" });
        var path = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), item.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        db.BeneficiaryMedia.Remove(item);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
