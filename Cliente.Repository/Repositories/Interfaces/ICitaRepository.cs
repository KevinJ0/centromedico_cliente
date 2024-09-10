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
        public Task<List<citaDTO>> getCitasListByUserAsync();
        public List<citaDTO> getCitasListByCv(string codVerificacion);
        public bool ExistByDocIdentidadAndMedico(medicos medico, string docIdentidad);
        public bool ExistByDocIdentidad(string docIdentidad);
        public void Add(citas entity);
        public string getCV(string docIdentidad);
        public bool ExistByUser(medicos medico, MyIdentityUser user);
    }
}
