using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Models.DTO
{
    public class pruebaDTO
    {
        public int ID { get; set; }
        public string descrip { get; set; }
        public DateTime? fecha_recoleccion { get; set; }
        public DateTime? fecha_emision { get; set; }
     //   public int resultadosID { get; set; }
        public string url { get; set; }
    }
}
