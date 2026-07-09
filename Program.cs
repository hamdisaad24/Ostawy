using API.SeedingData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Helpers;
using Ostawy.Interfaces;
using Ostawy.Models;
using Ostawy.Repositories;
using Ostawy.SeedingData;
using Ostawy.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(
    option =>
    {
        option.Password.RequiredLength = 8;
        option.Password.RequireNonAlphanumeric = false;
        option.Password.RequireDigit = false;
        option.Password.RequireUppercase = false;
        option.Password.RequireLowercase = false;
    })
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.Configure<PaymobSettings>(
    builder.Configuration.GetSection(PaymobSettings.SectionName));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<PlanService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<PlanRepository>();
builder.Services.AddScoped<ProfessionService>();
builder.Services.AddScoped<ProfessionRepository>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
await RoleSeeder.SeedRolesAsync(app);

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AdminSeeder.SeedAsync(services);
    var context = services.GetRequiredService<ApplicationDbContext>();
    await RequestSeeder.SeedAsync(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=index}/{id?}")
    .WithStaticAssets();


app.Run();
