using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Cliente.DTO;
using Cliente.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cliente.Repository.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        public MedicoRepository(MyDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public medicos getMedicoWithExtras(int medicoID)
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

        public medicoDTO getMedicoDtoWithExtras(int medicoID)
        {
            try
            {
                medicoDTO medicoDto = _mapper.Map<medicoDTO>(_db.medicos
                    .Include(m => m.especialidades_medicos).ThenInclude(es => es.especialidades)
                    //.Include(m => m.cobertura_medicos).ThenInclude(cober => cober.seguros)
                    .Include(m => m.servicios_medicos).ThenInclude(cober => cober.servicios)
                    .FirstOrDefault(x => x.ID == medicoID));

                var coberturas = _db.cobertura_medicos.Include(m => m.seguros).AsNoTracking()
                        .Where(m => m.medicosID == medicoID)
                        .ToList();

                // Mapea manualmente las coberturas
                /*medicoDto.cobertura_medicos = coberturas.Select(c => new coberturaMedicoDTO
                {
                    medicosID = c.medicosID,
                    segurosID = c.segurosID,
                    serviciosID = c.serviciosID,
                    pago = c.pago,
                    porciento = c.porciento
                }).ToList();*/

                // Mapea manualmente las seguros
                medicoDto.seguros = coberturas.Select(c => new seguroDTO
                {
                    ID = c.seguros.ID,
                    descrip = c.seguros.descrip
                }).GroupBy(s => new { s.ID, s.descrip })
                .Select(g => g.First())
                .ToList();

                return medicoDto;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public medicos getById(int medicoID)
        {
            try
            {
                medicos medico = _db.medicos
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
