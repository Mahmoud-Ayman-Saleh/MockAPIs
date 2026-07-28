using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MockAPIs.API.Middleware;
using MockAPIs.BLL.Interfaces;
using MockAPIs.BLL.Services;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Interfaces;
using MockAPIs.DAL.Models;
using MockAPIs.DAL.Repositories;
using MockAPIs.DAL.Repositories.Interfaces;

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ─── CORS Policy ──────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

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
builder.Services.AddScoped<IResourceRepository, ResourceRepository>();
builder.Services.AddScoped<IResourceServices, ResourceService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFieldRepository, FieldRepository>();
builder.Services.AddScoped<IFieldService, FieldService>();
builder.Services.AddScoped<IEndpointConfigRepository, EndpointConfigRepository>();
builder.Services.AddScoped<IEndpointConfigService, EndpointConfigService>();
builder.Services.AddScoped<IDataRepository, DataRepository>();
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddScoped<IMockRuntimeRepository, MockRuntimeRepository>();
builder.Services.AddScoped<IMockRuntimeService, MockRuntimeService>();

var app = builder.Build();

app.UseForwardedHeaders();

// ─── CORS must be early in the pipeline ───────────────────────
app.UseCors("AllowAll");

// Only redirect to HTTPS in production (avoids breaking local dev on HTTP)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// ─── Apply Migrations & Seed Roles ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    string[] roles = ["Admin", "User"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}

app.MapOpenApi();

app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MockAPIs API");
        options.RoutePrefix = string.Empty;
    }
);

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
