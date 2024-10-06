using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services.Interfaces;
using System;

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

        public User ValidarUsuario(string username, string password)
        {
            try
            {
                return _authRepository.ValidarUsuario(username, password);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar usuário: {ex.Message}");
            }
        }

        public string GenerateToken(User user)
        {
            try
            {
                return _tokenRepository.CreateToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao gerar token: {ex.Message}");
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                return _tokenRepository.IsValidToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao validar token: {ex.Message}");
            }
        }
    }
}
