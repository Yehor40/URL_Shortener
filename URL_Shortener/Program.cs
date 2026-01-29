using FluentValidation;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using URL_Shortener.Domain.Entities;
using URL_Shortener.Infrastructure.Persistence;
using URL_Shortener.Api.Middleware;

// Load .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/url_shortener.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Data Source=url_shortener.db";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(connectionString);
    // SQLite performance optimization
});

// Register MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(URL_Shortener.Application.Common.Behaviours.ValidationBehavior<,>));
});

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortener API V1");
        c.RoutePrefix = "swagger"; // Explicitly setting it to swagger
    });
    
    // Redirect root to swagger
    app.MapGet("/", context => 
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

// app.UseHttpsRedirection(); // Disable for troubleshooting local port issues
app.UseStaticFiles();

app.UseSerilogRequestLogging();

app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));
            
        var adminEmail = "admin@example.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        
        var userEmail = "user@example.com";
        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var user = new ApplicationUser { UserName = userEmail, Email = userEmail, EmailConfirmed = true };
            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
        }

        if (!context.AboutInfos.Any())
        {
            context.AboutInfos.Add(new AboutInfo 
            { 
                Content = "Refactored with CQRS, MediatR, and Serilog logging.",
                UpdatedAtUtc = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while seeding the database.");
    }
}

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}