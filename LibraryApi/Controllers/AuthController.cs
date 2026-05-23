using LibraryApi.Data;
using LibraryApi.DTO;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        //api/auth/controller
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>>Register(RegisterDto registerDto)
        {
            var existingUser= await _context.Users.FirstOrDefaultAsync(u=>u.Email==registerDto.Email.ToLower());
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
            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }
    }
}
