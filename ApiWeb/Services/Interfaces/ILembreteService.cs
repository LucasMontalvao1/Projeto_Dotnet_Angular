using ApiWeb.Models;
using System.Collections.Generic;

namespace ApiWeb.Services.Interfaces
{
    public interface ILembreteService
    {
        List<Lembrete> GetLembretesByUsuarioId(int usuarioId);

        List<Lembrete> GetAllLembretes();

        Lembrete AddLembrete(Lembrete lembrete, string mensagem);

        Lembrete UpdateLembrete(Lembrete lembrete);

        bool DeleteLembrete(int lembreteId);
    }

}
