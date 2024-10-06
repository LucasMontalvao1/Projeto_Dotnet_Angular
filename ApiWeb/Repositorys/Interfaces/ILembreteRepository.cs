using ApiWeb.Models;
using System.Collections.Generic;

namespace ApiWeb.Repositorys.Interfaces
{
    public interface ILembreteRepository
    {
        List<Lembrete> GetLembretesByUsuarioId(int usuarioId);

        List<Lembrete> GetAllLembretes();

        Lembrete AddLembrete(Lembrete lembrete);

        Lembrete UpdateLembrete(Lembrete lembrete);

        bool DeleteLembrete(int lembreteId);
    }
}
