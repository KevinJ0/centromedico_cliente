﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 
#nullable disable

namespace Cliente.DTO
{
   
    public partial class coberturaMedicoDTO
    {
        [Key]
        public int medicosID { get; set; }
        public byte porciento { get; set; }
        [Key]
        public int segurosID { get; set; }
        public int serviciosID { get; set; }
        [Column(TypeName = "money")]
        public decimal pago { get; set; }

    
    }
}
