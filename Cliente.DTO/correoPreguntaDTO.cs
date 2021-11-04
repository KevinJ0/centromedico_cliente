using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.DTO
{
    public class correoPreguntaDTO
    {

        [Required]
        public string nombre { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress]
        public string correo { get; set; }
        [StringLength(15)]
        [Required]
        public string contacto { get; set; }
        [Required]
        public int motivo { get; set; }
        [Required]
        public string mensaje { get; set; }

    }
}
