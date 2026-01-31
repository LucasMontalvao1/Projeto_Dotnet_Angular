namespace FinanceReminderHub.Application.DTOs.Responses
{
    public class TransacaoResponse
    {
        public int TransacaoID { get; set; }
        public int UsuarioID { get; set; }
        public int CategoriaID { get; set; }
        public string CategoriaNome { get; set; } // Enriched data
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
