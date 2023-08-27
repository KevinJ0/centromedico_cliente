using Centromedico.Database.DbModels;
using Cliente.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> saveUserInfoAsync(UserInfo formuser);
        Task<bool> RegisterAsync(RegisterDTO formdata);
    }
}
