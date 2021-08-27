using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalSalvador.Services
{
    public class IdentityErrorService
    {
        public static string getDescription(string identityErrorCode)
        {
            string _error = "";

            switch (identityErrorCode)
            {
                case IdentityErrorCodes.DuplicateUserName:
                    _error = "Este usuario ya está registrado.";
                    break;
                case IdentityErrorCodes.DuplicateEmail:
                    _error = "Este email ya ha sido tomado.";
                    break;
                case IdentityErrorCodes.InvalidEmail:
                    _error = "Este correo es invalido.";
                    break;
                case IdentityErrorCodes.PasswordMismatch:
                    _error = "Las contraseñas no coinciden.";
                    break;
                case IdentityErrorCodes.PasswordRequireDigit:
                    _error = "La contraseña requiere digitos.";
                    break;
                case IdentityErrorCodes.PasswordRequireLower:
                    _error = "La contraseña requiere letra en minuscula.";
                    break;
                case IdentityErrorCodes.PasswordTooShort:
                    _error = "La contraseña es muy corta.";
                    break;
                default:
                    _error = "Ha ocurrido un error";
                    break;
            }

            return _error;

        }
    }
}
