using LogbookSqlServer.Data;
using LogbookSqlServer.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<LogbookDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => {
 o.Cookie.Name="Logbook.Auth"; o.Cookie.HttpOnly=true; o.Cookie.SameSite=SameSiteMode.Lax; o.Cookie.SecurePolicy=CookieSecurePolicy.SameAsRequest; o.ExpireTimeSpan=TimeSpan.FromHours(8); o.SlidingExpiration=true;
 o.Events.OnRedirectToLogin=c=>{c.Response.StatusCode=401;return Task.CompletedTask;}; o.Events.OnRedirectToAccessDenied=c=>{c.Response.StatusCode=403;return Task.CompletedTask;};
});
builder.Services.AddAuthorization(); builder.Services.AddSingleton<IPasswordHasher<AppUser>,PasswordHasher<AppUser>>(); builder.Services.AddControllers();
var app=builder.Build(); app.UseDefaultFiles(); app.UseStaticFiles(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers();
using(var scope=app.Services.CreateScope()) { var db=scope.ServiceProvider.GetRequiredService<LogbookDbContext>(); await db.Database.EnsureCreatedAsync();
 var name=Environment.GetEnvironmentVariable("LOGBOOK_ADMIN_USERNAME"); var password=Environment.GetEnvironmentVariable("LOGBOOK_ADMIN_PASSWORD");
 if(!string.IsNullOrWhiteSpace(name)&&!string.IsNullOrWhiteSpace(password)) { if(password.Length<12) throw new InvalidOperationException("LOGBOOK_ADMIN_PASSWORD ต้องมีอย่างน้อย 12 ตัวอักษร"); if(!await db.Users.AnyAsync(x=>x.Username==name.Trim())) { var user=new AppUser{Username=name.Trim(),DisplayName="ผู้ดูแลระบบ",Role="Admin"}; user.PasswordHash=scope.ServiceProvider.GetRequiredService<IPasswordHasher<AppUser>>().HashPassword(user,password); db.Users.Add(user); await db.SaveChangesAsync(); } }
}
app.Run();
