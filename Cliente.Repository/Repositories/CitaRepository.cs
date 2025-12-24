using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Cliente.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cliente.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Cliente.Repository.Repositories
{
    public class CitaRepository : ICitaRepository
    {

        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CitaRepository(IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager, MyDbContext db, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }



        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {

            List<citaDTO> citaslst = _db.citas
                .Where(p => p.cod_verificacionID == codVerificacion && p.estado == true)
                .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToList();
            //falta correcta implementacion
            throw new NotImplementedException();

            return citaslst;

        }

        public async Task<List<citaDTO>> getCitasListByUserAsync()
        {

            try
            {
                MyIdentityUser user = await _userManager
                    .FindByNameAsync(_httpContextAccessor.HttpContext.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                List<citaDTO> citaslst = await _db.citas
                    .Include(c => c.medicos)
                    .ThenInclude(m => m.turnos)
                    .Where(p => p.pacientes.MyIdentityUsers == user && p.estado == true)
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToListAsync();
                citaslst.ForEach(citaDTO =>
                {
                    //datos extras de los turnos
                    if (int.TryParse(citaDTO.medicosID.ToString(), out int _medicoID))
                    {
                        citaDTO.turno_paciente.cant_pacientes_adelante = getCantCitasPendientes(citaDTO.fecha_hora, _medicoID);
                        citaDTO.turno_paciente.ultima_entrada = getLastTurnByDate(citaDTO.fecha_hora, _medicoID);
                        citaDTO.turno_paciente.primera_entrada = getFirstTurnByDate(citaDTO.fecha_hora, _medicoID);
                    }
                });
                return citaslst;
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar acceder a la información, por favor intente más tarde." + e.StackTrace);
            }
        }


        private DateTime? getFirstTurnByDate(DateTime fecha_hora, int medicoID)
        {
            var res = _db.citas
                         .Where(x => x.fecha_hora.Date == fecha_hora.Date
                                  && x.medicosID == medicoID
                                  && x.deleted)
                         .Min(c => c.deleted_date);

            return res;

        }
        private DateTime? getLastTurnByDate(DateTime fecha_hora, int medicoID)
        {
            var res = _db.citas
                         .Where(x => x.fecha_hora.Date == fecha_hora.Date
                                  && x.medicosID == medicoID
                                  && x.deleted)
                         .Max(c => c.deleted_date);

            return res;

        }

        public string getCV(string docIdentidad)
        {
            cod_verificacion codV = _db.cod_verificacion
                .Include("citas")
                .Where(x => x.citas.pacientes.MyIdentityUsers.doc_identidad == docIdentidad
                            && x.citas.estado == true)
                .FirstOrDefault();

            return codV?.value;
        }

        public void Add(citas entity)
        {
            _db.citas.Add(entity);
        }
        public bool ExistByUser(medicos medico, MyIdentityUser user)
        {
            try
            {
                citas result = _db.citas.FirstOrDefault(x => x.medicos == medico
                  && x.pacientes.MyIdentityUsers == user && x.estado == true);

                if (result != null)
                    return true;

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool ExistByDocIdentidadAndMedico(medicos medico, string docIdentidad)
        {
            try
            {
                citas result = _db.citas.FirstOrDefault(x => x.medicos == medico
                  && x.pacientes.doc_identidad == docIdentidad && x.estado == true && x.pacientes.confirm_doc_identidad);

                if (result != null)
                    return true;

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }



        public bool ExistByDocIdentidad(string docIdentidad)
        {

            try
            {
                if (_db.citas.FirstOrDefault(x => x.pacientes.doc_identidad == docIdentidad
                                                  && x.estado == true) != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int getCantCitasPendientes(DateTime fecha_hora_cita, int medicosID)
        {
            var res = _db.citas.Where(x =>
                        !x.deleted
                        && x.fecha_hora.Date == fecha_hora_cita.Date
                        && x.medicosID == medicosID
                        && x.fecha_hora.TimeOfDay < fecha_hora_cita.TimeOfDay
                        ).Count();

            return res;
        }
    }
}
