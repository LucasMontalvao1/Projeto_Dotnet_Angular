using ApiWeb.Models;

namespace ApiWeb.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);

        bool ValidateToken(string token);
    }
}
