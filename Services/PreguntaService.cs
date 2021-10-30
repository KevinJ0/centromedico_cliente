using CentromedicoCliente.Models.DTO;
using CentromedicoCliente.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
 
 
namespace CentromedicoCliente.Services
{
    public class PreguntaService : IPreguntaService
    {
        private readonly IEmailService _emailService;

        public PreguntaService(IEmailService emailService)
        {
            _emailService = emailService;

        }
        public ActionResult SendQuestion(correoPreguntaDTO formdata)
        {
            string reason = "";

            switch (formdata.motivo)
            {
                case (int)contactReason.donation:
                    reason = "Donación";
                    break;
                case (int)contactReason.social_media:
                    reason = "Medios de comunicación";
                    break;
                default:
                    reason = "General";
                    break;
            }

            try
            {
                string msg = $@"<html>
                Enviado por: {formdata.correo}<br>
                Motivo: {reason}<br>                
                Número de contacto: {formdata.contacto}<br>
                Nombre: {formdata.nombre}<br><br>
                {formdata.mensaje}</html>";

                _emailService.Send("itlaprueba4@gmail.com", "Pregunta de cliente", msg);

                return new OkObjectResult("Mensaje enviado con exito.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        enum contactReason : int
        {
            general = 0,
            donation = 1,
            social_media = 2,
        }

    }
}
