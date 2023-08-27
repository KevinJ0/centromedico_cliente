using Centromedico.Database.DbModels;
using Cliente.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces
{
    public interface ICitaService
    {
        Task<citaFormDTO> getFormCitaAsync(int medicoID);
        List<citaDTO> getCitasListByCv(string codVerificacion);
        Task<List<citaDTO>> getCitasListAsync();
        Task<citaResultDTO> createCitaAsync(citaCreateDTO formdata);

    }
}
