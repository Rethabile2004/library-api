using LibraryApi.Data;
using LibraryApi.DTO;
using LibraryApi.Models;
using LibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        public AuthController(AppDbContext context, IConfiguration configuration, ITokenService tokenService)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
        }
        //api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email.ToLower());
            if (existingUser != null)
            {
                // return conflict 409 if the account already exists
                return Conflict(new { message = "A user with this email already exists." });
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var newUser = new User
            {
                Email = registerDto.Email.ToLower(),
                CreatedAt = DateTime.UtcNow,
                FullName = registerDto.FullName,
                PasswordHash = passwordHash
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(newUser);
            var expiryTime = int.Parse(_configuration["JwtSettings:ExpiryHours"]!);
            return Ok(new AuthResponseDto
            {
                Email = newUser.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(expiryTime),
                FullName = newUser.FullName,
                Token = token
            });
        }
        // api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());
            if (user == null)
            {
                // 401 credentials are missing or invalid
                return Unauthorized(new { message = "Invalid email or password." });
            }
            var validPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password,user.PasswordHash);
            if (!validPassword)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            var token = _tokenService.GenerateToken(user);
            var expiryTime = int.Parse(_configuration["JwtSettings:ExpiryHours"]!);
            return Ok(new AuthResponseDto
            {
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(expiryTime),
                FullName = user.FullName,
                Token = token
            }); // 200 success
        }
    }
}