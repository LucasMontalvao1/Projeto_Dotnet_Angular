using ApiWeb.Models;

namespace ApiWeb.Services.Interfaces
{
    public interface IAuthService
    {
        User ValidarUsuario(string username, string password);

        string GenerateToken(User user);

        bool ValidateToken(string token);
    }
}
