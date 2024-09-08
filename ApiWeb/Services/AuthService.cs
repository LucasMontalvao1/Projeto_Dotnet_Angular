using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services.Interfaces;

namespace ApiWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenRepository _tokenRepository;

        public AuthService(IAuthRepository authRepository, ITokenRepository tokenRepository)
        {
            _authRepository = authRepository;
            _tokenRepository = tokenRepository;
        }

        public User ValidarUsuario(User user)
        {
            return _authRepository.ValidarUsuario(user);
        }

        public string GenerateToken(User user)
        {
            return _tokenRepository.CreateToken(user);
        }

        public bool ValidateToken(string token)
        {
            return _tokenRepository.IsValidToken(token);
        }
    }
}
