using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Exceptions
{
    public class ServicesEmptyException : Exception
    {
        public ServicesEmptyException(string message) : base(message)
        {
        }


    }
}
