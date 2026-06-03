using Asp.Versioning;
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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(AppDbContext context, IConfiguration configuration, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
            _logger = logger;
        }
        //api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email.ToLower());
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt with already existing email: {Email}", registerDto.Email);
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

            _logger.LogInformation("New user registered Email: {Email} (UserId: {UserId})", newUser.Email, newUser.Id);
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
                _logger.LogWarning("Failed login attempt email: {Email} not found.", loginDto.Email);
                // 401 credentials are missing or invalid
                return Unauthorized(new { message = "Invalid email or password." });
            }
            var validPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!validPassword)
            {
                _logger.LogWarning("Failed login attempt incorrect password for Email: {Email}", loginDto.Email);
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = _tokenService.GenerateToken(user);
            var expiryTime = int.Parse(_configuration["JwtSettings:ExpiryHours"]!);
            _logger.LogInformation("User logged in: {Email} (UserId: {UserId})", user.Email, user.Id);
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