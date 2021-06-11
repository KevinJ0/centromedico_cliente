using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSalvador.Models.DTO
{
    public class resultadoDTO
    {
        public int ID { get; set; }
        [Column(TypeName = "date")]
        public DateTime? fecha_entrada { get; set; }
        [Column(TypeName = "date")]
        public DateTime? fecha_reporte { get; set; }
        public string descrip { get; set; }
        public string url { get; set; }


    }
}
