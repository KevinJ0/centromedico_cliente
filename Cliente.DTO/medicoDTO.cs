using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cliente.DTO
{
    public class medicoDTO
    {

        [Key]
        public int ID { get; set; }
        [StringLength(10)]
        public string exequatur { get; set; }
        [Required]
        [StringLength(10)]
        public string colegiatura { get; set; }
        [Required]
        [StringLength(30)]
        public string nombre { get; set; }
        [StringLength(40)]
        public string apellido { get; set; }
        [Required]
        [StringLength(1)]
        public string sexo { get; set; }
        [StringLength(50)]
        public string correo { get; set; }
        public string url_twitter { get; set; }
        public string url_facebook { get; set; }
        public string url_instagram { get; set; }
        [StringLength(10)]
        public string telefono1 { get; set; }
        [StringLength(2)]
        public string telefono1_contact { get; set; }
        [StringLength(10)]
        public string telefono2 { get; set; }
        [StringLength(2)]
        public string telefono2_contact { get; set; }
        public int? consultorio { get; set; }
        [Required]
        public bool? estado { get; set; }
        public string ProfilePhoto { get; set; }
        public List<string> especialidades { get; set; }
        public List<seguroDTO> seguros { get; set; }
        public List<coberturaMedicoDTO> cobertura_medicos{ get; set; }

        public List<string> servicios { get; set; }
        //public virtual horarios_medicos horarios_medicos { get; set; }

        public Dictionary<string, List<string>> horarios {get; set;}

     
    }
   
}
