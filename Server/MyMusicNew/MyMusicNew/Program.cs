using DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer; // תוספת
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // תוספת
using Microsoft.OpenApi.Models;
using Repositories.Entities;
using Repositories.Interfaces;
using Repositories.Repositories;
using Service.Dto;
using Service.Interfaces;
using Service.Services;
using System.Text; // תוספת

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 1. הגדרת Authentication - זה החלק שבודק את הטוקן
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddProjectServices(connectionString);

builder.Services.AddDbContext<MusicContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("DataContext")
    ));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicPlayer API", Version = "v1" });

    // הגדרת אפשרות להזנת טוקן ב-Swagger
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
// הזרקת השירות שלך - תקין לגמרי!
builder.Services.AddScoped<IToken<User>, TokenService>();

// רישום הרפוזיטורי המיוחד
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- חשוב מאוד: הסדר כאן קריטי! ---
app.UseAuthentication(); // קודם כל בודקים מי המשתמש (חובה להוסיף לפני Authorization)
app.UseAuthorization();  // אחר כך בודקים מה מותר לו

app.MapControllers();

app.Run();