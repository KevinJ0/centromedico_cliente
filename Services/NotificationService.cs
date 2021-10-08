using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly.Retry;
using Polly;
using System.Globalization;
using System.Text;
using HospitalSalvador.Models.DTO;
using Microsoft.Extensions.Configuration;

namespace HospitalSalvador.Services
{
    public interface INotificationService
    {
        void sendTicketWhatsapp(citaResultDTO citaResult, string contacto);
        void sendTicketMail(citaResultDTO citaResult, string to);
    }

    public class NotificationService : INotificationService
    {
        private readonly RetryPolicy _retryPolicy;
        private readonly int MaxRetries = 3;
        private readonly IEmailService _emailService;
        private readonly IWhatsappService _whatsappService;
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration, IEmailService emailService, IWhatsappService whatsappService)
        {
            _configuration = configuration;
            _emailService = emailService;
            _whatsappService = whatsappService;
            _retryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetry(MaxRetries, time => TimeSpan.FromSeconds(1));
        }


        public void sendTicketWhatsapp(citaResultDTO citaResult, string contacto)
        {
            String message = "";
            message = "Usted ha hecho una cita médica en Hospital Salvador, a continuación los datos de su cita:"
                    + Environment.NewLine + Environment.NewLine + "*Fecha y hora:* " + new CultureInfo("es-ES").TextInfo.ToTitleCase(citaResult.fecha_hora.ToString("dddd, dd MMMM yyyy hh:mm tt", new CultureInfo("es-ES")))
                    + Environment.NewLine + "*Código:* " + citaResult.cod_verificacion
                    + Environment.NewLine + "Tu médico es *" + citaResult.medico_nombre_apellido + "*"
                    + Environment.NewLine + "*Consultorio:* " + citaResult.consultorio
                    + Environment.NewLine + "*Turno:* " + citaResult.turno
                    + Environment.NewLine + "*Servicio:* " + citaResult.servicio
                    + Environment.NewLine + "*Paciente:* " + citaResult.paciente_nombre_apellido;

            if (citaResult.doc_identidad_tutor != null)
                message = message
                    + Environment.NewLine + "*Doc.identidad tutor:* " + citaResult.doc_identidad_tutor
                    + Environment.NewLine + "*Tutor:* " + citaResult.tutor_nombre_apellido;
            else
                message = message
                + Environment.NewLine + "*Doc.identidad:* " + citaResult.doc_identidad;


            message = message
                    + Environment.NewLine + "*Número de contacto:* " + citaResult.contacto
                    + Environment.NewLine + "*Seguro:* " + citaResult.seguro
                    + Environment.NewLine + "*Diferencia:* RD$" + Decimal.Round(citaResult.diferencia, 2);

            _whatsappService.Send("14155238886", contacto, message);

        }


        public void sendTicketMail(citaResultDTO citaResult, string to)
        {

            _retryPolicy.Execute(() =>
            {
                StringBuilder message = new StringBuilder("");
                var reader = new System.IO.StreamReader("Helpers/TicketTemplateFirst.html", System.Text.Encoding.UTF8);
                var firstTicket = reader.ReadToEnd();
                reader = new System.IO.StreamReader("Helpers/TicketTemplateSecond.html", System.Text.Encoding.UTF8);
                var secondTicket = reader.ReadToEnd();
                reader.Close();

                string showDocIdent = "inherit";
                string showDocIdentTutor = "none";
                if (citaResult.doc_identidad_tutor != null)
                {
                    showDocIdent = "none";
                    showDocIdentTutor = "inherit";
                }
                message.Append(firstTicket);
                message.Append($@"
                     <h2 style=""
                        margin-bottom: 2rem;
                        font-weight: normal;
                        font-size: clamp(0.7rem, 6vw, 1.3rem);
                        padding: 1rem;
                        text-align: center;
                        padding-top: 0;
                        "" >{new CultureInfo("es-ES").TextInfo.ToTitleCase(citaResult.fecha_hora.ToString("dddd, dd MMMM yyyy hh:mm tt", new CultureInfo("es-ES")))}</h2>
                        <div class=""user_data"">
                          <div class=""flex-container"">
                            <p><b>Código:</b>  {citaResult.cod_verificacion}</p>
                            <p><b>Tu médico es</b> {citaResult.medico_nombre_apellido}</p>
                            <p><b>Consultorio: </b>{citaResult.consultorio}</p>
                            <p><b>Turno: </b>{citaResult.turno}</p>
                            <p><b>Servicio: </b>{citaResult.servicio}</p>
                            <p  style=""display: {showDocIdent};""><b>Doc. identidad: </b><span>{citaResult.doc_identidad}</span></p>
                            <p><b>Paciente: </b>{citaResult.paciente_nombre_apellido}</p>
                            <p style=""display: {showDocIdentTutor};""><b>Doc. identidad tutor: </b><span>{citaResult.doc_identidad_tutor}</span></p>
                            <p style=""display: {showDocIdentTutor};""><b>Tutor: </b><span>{citaResult.tutor_nombre_apellido}</span></p>
                            <p><b>Número de contacto: </b>{citaResult.contacto}</p>
                            <p><b>Seguro: </b>{citaResult.seguro}</p>
                            <p><b>Diferencia: </b>RD${Decimal.Round(citaResult.diferencia, 2)}</p>
                          </div>");
                message.Append(secondTicket);

                _emailService.Send(to, "Cita Médica - Hospital Salvador", message.ToString(), _configuration["EmailSettings:EmailFrom"]);
            });
        }
    }
}
