using System.ComponentModel.DataAnnotations;

namespace ApiWeb.Models
{
    public class User
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Foto { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Cargo { get; set; }

        [StringLength(100)]
        public string Matricula { get; set; }
    }
}
