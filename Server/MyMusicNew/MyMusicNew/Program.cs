using DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Entities;
using Repositories.Interfaces;
using Repositories.Repositories;
using Service.Dto;
using Service.Interfaces;
using Service.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// שליפת מחרוזת החיבור מה-appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// --- 1. הגדרת Authentication (אימות) ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        // התיקון הקריטי למניעת 401 בגלל הפרשי זמנים ב-Localhost
        ClockSkew = TimeSpan.Zero
    };
});

// --- 2. רישום שירותי הפרויקט (Extensions) ---
builder.Services.AddProjectServices(connectionString);

// --- 3. הגדרת בסיס הנתונים ---
builder.Services.AddDbContext<MusicContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("DataContext")
    ));

// --- 4. הגדרת Controllers ו-JSON ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// --- 5. הגדרת Swagger עם תמיכה ב-JWT ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicPlayer API", Version = "v1" });

    // הוספת כפתור ה-Authorize ל-Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// רישום ה-TokenService (אם לא רשום בתוך AddProjectServices)
builder.Services.AddScoped<IToken<User>, TokenService>();

var app = builder.Build();

// --- 6. הגדרת Middleware (סדר הפעולות) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// חובה: Authentication לפני Authorization!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();