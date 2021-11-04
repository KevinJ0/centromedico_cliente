using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Helpers
{
    public class TwilioSettings
    {
        string accountSid = "AC8333955ad662b4dd4d537ca87e2b4522";
        string authToken = "e089e3d7f13f938af66e0d11466bec51";

        public  string AccountSid { get { return accountSid; } set { accountSid = value; } }
        public  string AuthToken { get { return authToken; } set { authToken = value; } }
 
    }
}
