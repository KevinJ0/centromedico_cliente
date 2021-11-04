using Centromedico.Database.DbModels;
using Cliente.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<IdentityResult> Add(RegisterDTO formdata);
    }
}
