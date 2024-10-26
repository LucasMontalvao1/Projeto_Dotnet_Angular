using ApiWeb.Models;

namespace ApiWeb.Repositorys.Interfaces
{
    public interface IAuthRepository
    {
        User ValidarUsuario(string username, string password);

        Task<bool> UsuarioExiste(int usuarioId);
    }
}
