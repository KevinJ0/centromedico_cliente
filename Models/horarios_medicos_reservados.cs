using System;
using System.Collections.Generic;

#nullable disable

namespace CentromedicoCliente.Models
{
    public partial class horarios_medicos_reservados
    {
        public int medicosID { get; set; }
        //public byte? turno { get; set; }
        public DateTime fecha_hora { get; set; }
    }
}
