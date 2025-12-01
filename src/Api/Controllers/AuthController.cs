using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    public record LoginRequest(string Username, string Password);

    [AllowAnonymous]
    [HttpPost("token")]
    public IActionResult GetToken([FromBody] LoginRequest request)
    {
        // Very simple demo authentication logic: checks credentials from configuration
        var demoUsername = configuration["Auth:DemoUsername"] ?? "admin";
        var demoPassword = configuration["Auth:DemoPassword"] ?? "Password123!";

        if (!string.Equals(request.Username, demoUsername, StringComparison.OrdinalIgnoreCase) ||
            request.Password != demoPassword)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        var jwtSection = configuration.GetSection("Jwt");
        var key = jwtSection.GetValue<string>("Key") ?? throw new InvalidOperationException("JWT key is not configured.");
        var issuer = jwtSection.GetValue<string>("Issuer") ?? "OnlineCarDealership";
        var audience = jwtSection.GetValue<string>("Audience") ?? "OnlineCarDealershipUsers";
        var minutes = jwtSection.GetValue<int?>("AccessTokenMinutes") ?? 60;

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { access_token = tokenString });
    }
}
