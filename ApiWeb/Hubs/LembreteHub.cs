using ApiWeb.Models;
using Microsoft.AspNetCore.SignalR;

namespace ApiWeb.Hubs
{
    public class LembreteHub : Hub
    {
        public async Task SendReminder(Lembrete lembrete)
        {
            await Clients.All.SendAsync("ReceiveReminder", lembrete);
        }

    }
}
