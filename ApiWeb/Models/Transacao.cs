namespace ApiWeb.Models
{
    public class Transacao
    {
        public int TransacaoID { get; set; }
        public int UsuarioID { get; set; }
        public int CategoriaID { get; set; }
        public string Tipo { get; set; } // Entrada ou Saída
        public decimal Valor { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
        public DateTime CriadoEm { get; set; }

        // Propriedades de navegação
        public Categoria Categoria { get; set; }
        public User Usuario { get; set; }
    }
}
