using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace X.Sharp.Web.Controllers
{
    public class JwtDemoController : Controller
    {
        public IActionResult CreateToken()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "laohu"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(60),
                Issuer = "Example",
                Audience = "https://localhost:7154",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("xxxxxxxxxxxxxxxx")),
                    SecurityAlgorithms.HmacSha256),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);

            var reader = tokenHandler.ReadJwtToken(tokenStr);

            return Json(tokenStr);
        }
        public IActionResult ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameter = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "Example",
                ValidAudience = "https://localhost:7154",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("xxxxxxxxxxxxxxxx"))
            };
            var principal =  tokenHandler.ValidateToken(token, validationParameter, out var result);

            return Json(result);
        }
    }
}