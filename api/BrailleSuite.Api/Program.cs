using System.Text;
using BrailleSuite.Api.Features.Auth;
using BrailleSuite.Api.Features.Documents.Documents;
using BrailleSuite.Api.Features.Documents.Folders;
using BrailleSuite.Api.Features.Translation;
using BrailleSuite.Api.Shared.Common;
using BrailleSuite.Api.Shared.Infrastructure;
using BrailleSuite.Api.Shared.Infrastructure.Braille;
using BrailleSuite.Api.Shared.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;
FontManager.RegisterFontFromStream(
    File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Assets", "Fonts", "DejaVuSans.ttf")));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AppDb")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, ClaimsCurrentUserContext>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddSingleton<ILibLouisTranslator, LibLouisTranslator>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

const string WebAppCorsPolicy = "WebApp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(WebAppCorsPolicy, policy => policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(WebAppCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapFoldersEndpoints();
app.MapDocumentsEndpoints();

app.Run();
