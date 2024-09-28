using ApiWeb.Models;
using System.Collections.Generic;

namespace ApiWeb.Services.Interfaces
{
    public interface ILembreteService
    {
        /// <summary>
        /// Obtém os lembretes de um usuário específico pelo seu ID.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de lembretes do usuário.</returns>
        List<Lembrete> GetLembretesByUsuarioId(int usuarioId);

        /// <summary>
        /// Obtém todos os lembretes sem filtrar por usuário.
        /// </summary>
        /// <returns>Lista de todos os lembretes.</returns>
        List<Lembrete> GetAllLembretes();
    }
}
