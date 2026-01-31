namespace FinanceReminderHub.Application.DTOs.Requests
{
    public class LembreteRequest
    {
        public int UsuarioID { get; set; }
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public DateTime DataLembrete { get; set; }
        public int IntervaloEmDias { get; set; }
    }
}
