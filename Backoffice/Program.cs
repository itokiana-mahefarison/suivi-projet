using Backoffice.Config.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Backoffice.Services;
using Backoffice.Services.Interfaces;
using Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "ProjectManager.Auth";
    });

// Add AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Check if Roles table is empty and seed initial roles
    if (!db.Roles.Any())
    {
        db.Roles.AddRange(
            new Shared.Models.Role
            {
                Label = "Administrator",
                CreatedAt = DateTime.UtcNow
            },
            new Shared.Models.Role
            {
                Label = "Project Manager",
                CreatedAt = DateTime.UtcNow
            },
            new Shared.Models.Role
            {
                Label = "Developer",
                CreatedAt = DateTime.UtcNow
            }
        );
        db.SaveChanges();
    }

    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Name = "Admin",
            Email = "admin@admin.com",
            Password = BCrypt.Net.BCrypt.HashPassword("admin"),
            RoleId = 1
        });

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
