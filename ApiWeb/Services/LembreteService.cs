using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace ApiWeb.Services
{
    public class LembreteService : ILembreteService
    {
        private readonly ILembreteRepository _lembreteRepository;

        public LembreteService(ILembreteRepository lembreteRepository)
        {
            _lembreteRepository = lembreteRepository;
        }

        /// <summary>
        /// Obtém os lembretes de um usuário específico pelo seu ID.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de lembretes do usuário.</returns>
        public List<Lembrete> GetLembretesByUsuarioId(int usuarioId)
        {
            try
            {
                return _lembreteRepository.GetLembretesByUsuarioId(usuarioId);
            }
            catch (Exception ex)
            {
                // Aqui você pode adicionar um log ou tratamento adicional
                throw new Exception($"Erro ao obter lembretes para o usuário {usuarioId}: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém todos os lembretes sem filtrar por usuário.
        /// </summary>
        /// <returns>Lista de todos os lembretes.</returns>
        public List<Lembrete> GetAllLembretes()
        {
            try
            {
                return _lembreteRepository.GetAllLembretes(); // Método que deve ser implementado no repositório
            }
            catch (Exception ex)
            {
                // Aqui você pode adicionar um log ou tratamento adicional
                throw new Exception($"Erro ao obter todos os lembretes: {ex.Message}");
            }
        }
    }
}
