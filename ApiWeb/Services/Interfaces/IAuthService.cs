using ApiWeb.Models;

namespace ApiWeb.Services.Interfaces
{
    public interface IAuthService
    {
        User ValidarUsuario(User user); 

        string GenerateToken(User user);

        bool ValidateToken(string token); 
    }
}
