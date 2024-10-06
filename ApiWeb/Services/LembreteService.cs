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
        private readonly IRabbitMqService _rabbitMqService;

        public LembreteService(ILembreteRepository lembreteRepository, IRabbitMqService rabbitMqService)
        {
            _lembreteRepository = lembreteRepository;
            _rabbitMqService = rabbitMqService;
        }

        public List<Lembrete> GetLembretesByUsuarioId(int usuarioId)
        {
            try
            {
                return _lembreteRepository.GetLembretesByUsuarioId(usuarioId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter lembretes para o usuário {usuarioId}: {ex.Message}");
            }
        }

        public List<Lembrete> GetAllLembretes()
        {
            try
            {
                return _lembreteRepository.GetAllLembretes();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao obter todos os lembretes: {ex.Message}");
            }
        }

        public Lembrete AddLembrete(Lembrete lembrete, string mensagem)
        {
            try
            {
                var novoLembrete = _lembreteRepository.AddLembrete(lembrete);
                _rabbitMqService.PublishReminderAdded(mensagem); 
                return novoLembrete;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao adicionar lembrete: {ex.Message}");
            }
        }

        public void IniciarConsumoDeLembretes()
        {
            _rabbitMqService.SubscribeToReminderQueue();
        }

        public Lembrete UpdateLembrete(Lembrete lembrete)
        {
            try
            {
                return _lembreteRepository.UpdateLembrete(lembrete);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar lembrete: {ex.Message}");
            }
        }

        public bool DeleteLembrete(int lembreteId)
        {
            try
            {
                return _lembreteRepository.DeleteLembrete(lembreteId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao deletar lembrete: {ex.Message}");
            }
        }
    }
}
