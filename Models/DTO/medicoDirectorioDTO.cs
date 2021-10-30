using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Models.DTO
{
    public class medicoDirectorioDTO
    {

        public int ID { get; set; }
        public string exequatur { get; set; }
        public string colegiatura { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string sexo { get; set; }
        public string correo { get; set; }
        public string url_twitter { get; set; }
        public string url_facebook { get; set; }
        public string url_instagram { get; set; }
        public string telefono1 { get; set; }
        public string telefono2 { get; set; }
        public int? consultorio { get; set; }
        public string ProfilePhoto { get; set; }
        public string telefono1_contact { get; set; }
        public string telefono2_contact { get; set; }
        public List<string> especialidades { get; set; }
    }
}
