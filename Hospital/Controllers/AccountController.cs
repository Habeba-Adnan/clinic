using Hospital.DTOs;
using Hospital.Models;
using Hospital.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountRepo _accountRepo;
        private readonly IConfiguration _configuration;

        public AccountController(AccountRepo accountRepo, IConfiguration configuration)
        {
            _accountRepo = accountRepo;
            _configuration = configuration;
        }
        [HttpPost("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            if (loginDTO == null || string.IsNullOrWhiteSpace(loginDTO.Username) || string.IsNullOrWhiteSpace(loginDTO.Password))
            {
                return BadRequest(new
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid login credentials."
                });
            }

            Admin admin = _accountRepo.GetAdmin(loginDTO.Username);

            if (admin == null || admin.Password != loginDTO.Password || admin.Username != loginDTO.Username)
            {
                return Unauthorized(new
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Invalid username or password."
                });
            }

            var token = GenerateJwtToken(admin);

            return Ok(new
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Login successful.",
                Token = token,
                Expired = DateTime.Now.AddHours(1),
                Roles = "Admin"
            });
        }

        private string GenerateJwtToken(Admin admin)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, admin.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("AdminId", admin.Id.ToString())
    };

            // Retrieve the secret key from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecritKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Define the token
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIss"],
                audience: _configuration["JWT:ValidAud"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}

