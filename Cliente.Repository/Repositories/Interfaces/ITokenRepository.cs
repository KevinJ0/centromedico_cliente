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
    public interface ITokenRepository
    {
        Task<IdentityResult> Add(RegisterDTO formdata);
        void Remove(token oldrt);
        void Add(token newRtoken);
        IQueryable getAllByUserId(string id);
    }
}
