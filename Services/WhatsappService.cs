using Microsoft.Extensions.Options;
using HospitalSalvador.Helpers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace HospitalSalvador.Services
{


    public interface IWhatsappService
    {
        void Send(string from, string to, string msj);
    }

    public class WhatsappService : IWhatsappService
    {
        private readonly TwilioSettings _twilioSettings;

        public WhatsappService(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        public void Send(string from, string to, string msj)
        {

            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber($"whatsapp:+1{to}"));
            messageOptions.From = new PhoneNumber($"whatsapp:+{from}");
            messageOptions.Body = msj;

            var message = MessageResource.Create(messageOptions);

        }

    }
}
