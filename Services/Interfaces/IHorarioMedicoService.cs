using Cliente.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces

{
    public interface IHorarioMedicoService
    {
        public Dictionary<DateTime, int> getHoursList(DateTime fecha_hora, int medicoID);

    }
}