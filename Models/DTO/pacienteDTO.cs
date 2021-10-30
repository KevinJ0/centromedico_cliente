using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CentromedicoCliente.Models.DTO
{
    public partial class pacienteDTO
    {
        bool menor;

        [Required]
        [StringLength(40)]
        public string nombre { get; set; }
        [StringLength(50)]
        public string apellido { get; set; }
        [Required]
        [StringLength(1)]
        public string sexo { get; set; }
        [Column(TypeName = "date")]
        public DateTime fecha_nacimiento
        { get; set;}
        [StringLength(15)]
        public string doc_identidad { get; set; }
        [StringLength(40)]
        public string nombre_tutor { get; set; }
        [StringLength(45)]
        public string apellido_tutor { get; set; }
        public bool extranjero { get; set; }
        [StringLength(10)]
        public string contacto { get; set; }
        [StringLength(15)]
        public string doc_identidad_tutor { get; set; }
        public int? edad
        {
            get; set;
        }
        public bool menor_un_año { get {

                return menor;
            } }
        public string MyIdentityUserID { get; set; }

    }
}
