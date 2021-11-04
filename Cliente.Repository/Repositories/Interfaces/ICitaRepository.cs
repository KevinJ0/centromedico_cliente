using Centromedico.Database.DbModels;
using Cliente.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories.Interfaces
{
    public interface ICitaRepository
    {
        public Task<List<citaDTO>> getCitasListAsync();
        public List<citaDTO> getCitasListByCv(string codVerificacion);
        public bool Exist(medicos medico, MyIdentityUser user);
        public bool Exist(MyIdentityUser user);
        public void Add(citas entity);
    }
}
