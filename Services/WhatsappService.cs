using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System;
using CentromedicoCliente.Services.Helpers;

namespace CentromedicoCliente.Services
{


    public interface IWhatsappService
    {
        void Send(string from, string to, string msj);
    }

    public class WhatsappService : IWhatsappService
    {
        private readonly TwilioSettings _twilioSettings;

        public WhatsappService(TwilioSettings twilioSettings)
        {
            _twilioSettings = twilioSettings;
        }

        public void Send(string from, string to, string msj)
        {
            var accountSid = _twilioSettings.AccountSid;
            //Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = _twilioSettings.AuthToken;
            //Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber($"whatsapp:+1{to}"));
            messageOptions.From = new PhoneNumber($"whatsapp:+{from}");
            messageOptions.Body = msj;
            var message = MessageResource.Create(messageOptions);

        }

    }
}
