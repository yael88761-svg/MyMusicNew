using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories.Entities;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class TokenService(IConfiguration config) : IToken<User>
    {
        private readonly IConfiguration _config = config;

        public string CreateToken(User user)
        {
            // 1. שליפת המפתח מהקונפיגורציה
            string secretKey = _config["Jwt:Key"];

            // בדיקה קריטית: אם המפתח לא נמצא, אנחנו רוצים לדעת מזה מיד ולא לקבל 401 מסתורי
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new Exception("שגיאה: המפתח Jwt:Key לא נמצא ב-appsettings.json! ודאי שהקובץ שמור ומוגדר כ-Copy to Output Directory.");
            }

            // 2. המרה למערך בייטים
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var key = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. יצירת ה-Claims
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
