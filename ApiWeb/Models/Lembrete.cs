namespace ApiWeb.Models
{
    public class Lembrete
    {
        public int LembreteID { get; set; }

        public int UsuarioID { get; set; }

        public string Titulo { get; set; }

        public string Descricao { get; set; }

        public DateTime DataLembrete { get; set; }

        public int IntervaloEmDias { get; set; }

        public DateTime CriadoEm { get; set; }
    }
}
