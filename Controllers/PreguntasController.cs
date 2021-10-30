using CentromedicoCliente.Models.DTO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CentromedicoCliente.Context;
using CentromedicoCliente.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using CentromedicoCliente.Services;
using CentromedicoCliente.Services.Interfaces;

namespace CentromedicoCliente.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IPreguntaService _preguntaSvc;

        public PreguntasController(IPreguntaService preguntaSvc, IEmailService emailService)
        {
            _preguntaSvc = preguntaSvc;
            _emailService = emailService;

        }

        [HttpPost("[action]")]
        public ActionResult SendQuestion(correoPreguntaDTO formdata)
        {
            try
            {

              return  _preguntaSvc.SendQuestion(formdata);
            }
            catch (Exception)
            {

                throw;
            }


        }

      
    }
}
