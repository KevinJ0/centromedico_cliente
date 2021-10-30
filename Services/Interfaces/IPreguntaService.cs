using CentromedicoCliente.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces
{
    public interface IPreguntaService
    {
        ActionResult SendQuestion(correoPreguntaDTO formdata);
    }
}
