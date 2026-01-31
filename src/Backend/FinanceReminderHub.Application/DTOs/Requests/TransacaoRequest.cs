namespace FinanceReminderHub.Application.DTOs.Requests
{
    public class TransacaoRequest
    {
        public int UsuarioID { get; set; }
        public int CategoriaID { get; set; }
        public string Tipo { get; set; } // "Entrada" or "Saida"
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
    }
}
