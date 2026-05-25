using LibraryApi.Models;

namespace LibraryApi.Services
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}
