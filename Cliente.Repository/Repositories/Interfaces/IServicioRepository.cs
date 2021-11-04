using Centromedico.Database.DbModels;
using Cliente.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories.Interfaces
{
    public interface IServicioRepository
    {
        public Task<List<servicio_coberturasDTO>> getAllByDoctorIdAsync(int medicoID);
        Task<servicios> getByIdAsync(int? segurosID);
    }
    }
