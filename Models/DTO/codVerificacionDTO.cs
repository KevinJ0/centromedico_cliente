using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSalvador.Models.DTO
{
    public class codVerificacionDTO
    {
        [Required]
        public string value { get; set; }
    }
}
