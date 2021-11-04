using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Cliente.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cliente.Repository.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly MyDbContext _db;
        public MedicoRepository( MyDbContext db)
        {
            _db = db;
        }

        public medicos getById(int medicoID)
        {
            try
            {
                medicos medico = _db.medicos
                    .Include(m => m.especialidades_medicos).ThenInclude(es => es.especialidades)
                    .Include(m => m.cobertura_medicos).ThenInclude(cober => cober.seguros)
                    .Include(m => m.servicios_medicos).ThenInclude(cober => cober.servicios)
                    .FirstOrDefault(x => x.ID == medicoID);

                return medico;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
