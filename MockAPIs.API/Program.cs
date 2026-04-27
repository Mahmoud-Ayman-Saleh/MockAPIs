using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MockAPIs.BLL.Interfaces;
using MockAPIs.BLL.Services;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;
using MockAPIs.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ─── Database ─────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── ASP.NET Identity ─────────────────────────────────────────
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(op =>
{
    op.Password.RequireDigit = true;
    op.Password.RequireLowercase = true;
    op.Password.RequireUppercase = true;
    op.Password.RequireNonAlphanumeric = true;
    op.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ─── JWT Authentication ───────────────────────────────────────
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme =
    op.DefaultChallengeScheme =
    op.DefaultForbidScheme =
    op.DefaultScheme =
    op.DefaultSignInScheme =
    op.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(op =>
{
    op.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration["JWT:SigningKey"]!
            )
        ),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ─── DI: Application Services ────────────────────────────────
builder.Services.AddScoped<IAuthServices, AuthService>();
builder.Services.AddScoped<IProjectServices, ProjectServices>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

var app = builder.Build();

// ─── Seed Roles ───────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    string[] roles = ["Admin", "User"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "MockAPIs API");
        }
    );
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
