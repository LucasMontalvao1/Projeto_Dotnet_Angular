namespace ApiWeb.Models.DTOs
{
    public class TransacaoDto
    {
        public int UsuarioID { get; set; }
        public int CategoriaID { get; set; }
        public string Tipo { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}
