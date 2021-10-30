﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CentromedicoCliente.Models
{
    [Index(nameof(secretariasID), Name = "IX_balance_caja_secretariasID")]
    public partial class balance_caja
    {
        [Key]
        public int medicosID { get; set; }
        [Key]
        [Column(TypeName = "date")]
        public DateTime fecha { get; set; }
        public int secretariasID { get; set; }
        [Column(TypeName = "money")]
        public decimal? balance_inicial { get; set; }
        [StringLength(40)]
        public string secretaria_nombre { get; set; }

        [ForeignKey(nameof(medicosID))]
        [InverseProperty("balance_caja")]
        public virtual medicos medicos { get; set; }
        [ForeignKey(nameof(secretariasID))]
        [InverseProperty("balance_caja")]
        public virtual secretarias secretarias { get; set; }
    }
}
