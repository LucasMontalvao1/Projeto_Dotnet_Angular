﻿using ApiWeb.Models;
using System.Collections.Generic;

namespace ApiWeb.Repositorys.Interfaces
{
    public interface ILembreteRepository
    {
        /// <summary>
        /// Obtém os lembretes de um usuário específico pelo seu ID.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de lembretes do usuário.</returns>
        List<Lembrete> GetLembretesByUsuarioId(int usuarioId);

        /// <summary>
        /// Obtém todos os lembretes.
        /// </summary>
        /// <returns>Lista de todos os lembretes.</returns>
        List<Lembrete> GetAllLembretes();
    }
}