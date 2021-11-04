using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.DTO
{
    public class seguroDTO
    {
        [Required]
        public int ID { get; set; }
        [Required]
        [StringLength(60)]
        public string descrip { get; set; }
    }
}
