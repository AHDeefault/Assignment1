
using ApiContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository userRepo;
    private readonly IConfiguration configuration;

    public AuthController(
        IUserRepository userRepo,
        IConfiguration configuration)
    {
        this.userRepo = userRepo;
        this.configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userRepo.GetManyAsync()
            .FirstOrDefaultAsync(u =>
                u.Username.ToLower() == request.UserName.ToLower());

        if (user == null || user.Password != request.Password)
        {
            return Unauthorized("Username or password is incorrect.");
        }

        string jwtKey = configuration["Jwt:Key"]!;
        string issuer = configuration["Jwt:Issuer"]!;
        string audience = configuration["Jwt:Audience"]!;

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("Id", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        string tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new
        {
            Token = tokenString,
            User = new UserDto
            {
                Id = user.Id,
                UserName = user.Username
            }
        });
    }
}
