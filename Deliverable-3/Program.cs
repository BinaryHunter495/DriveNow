using DriveNow.Data;
using DriveNow.Models;
using DriveNow.Repositories;
using DriveNow.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "DriveNow.Auth";
    options.SlidingExpiration = true;
});

// Repository Pattern + Dependency Injection
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Create the database for this coursework project, create roles, seed cars,
// and optionally provision an administrator from environment/user-secret settings.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "Admin", "Customer" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // Keep an administrator created in the earlier deliverable, if one exists.
    var legacyAdmin = await userManager.FindByEmailAsync("admin@drivenow.com");
    if (legacyAdmin != null && !await userManager.IsInRoleAsync(legacyAdmin, "Admin"))
        await userManager.AddToRoleAsync(legacyAdmin, "Admin");

    // For a fresh installation, set AdminSeed__Email and AdminSeed__Password
    // as environment variables or User Secrets. No default password is committed.
    var adminEmail = builder.Configuration["AdminSeed:Email"];
    var adminPassword = builder.Configuration["AdminSeed:Password"];
    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var created = await userManager.CreateAsync(admin, adminPassword);
            if (!created.Succeeded)
                throw new InvalidOperationException("Admin seed failed: " + string.Join("; ", created.Errors.Select(e => e.Description)));
        }
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    if (!await db.Cars.AnyAsync())
    {
        db.Cars.AddRange(
            new Car { Name = "Toyota Corolla", Rate = 6000 },
            new Car { Name = "Honda Civic", Rate = 8000 },
            new Car { Name = "Toyota Prado", Rate = 12000 });
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
