using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Models.DTO
{
    public class coberturaDTO
    {
       
        public decimal porciento  { get; set; }
        [Column(TypeName = "money")]
        public decimal pago { get; set; }
        [Column(TypeName = "money")]
        public decimal cobertura { get; set; }
        [Column(TypeName = "money")]
        public decimal diferencia { get; set; }
    }
}
