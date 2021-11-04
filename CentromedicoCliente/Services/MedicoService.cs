using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoCliente.Services.Interfaces;
using Cliente.DTO;
using Cliente.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services
{
    public class MedicoService : IMedicoService
    {

        private readonly MyDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMedicoRepository _medicoRepos;

        public MedicoService(IMedicoRepository medicoRepos, MyDbContext db, IMapper mapper)
        {
            _medicoRepos = medicoRepos;
            _db = db;
            _mapper = mapper;
        }


        public ActionResult<List<medicoDirectorioDTO>> getAllMedical(string nombre, string especialidadID, string seguroID)
        {
            try
            {

                List<medicoDirectorioDTO> medicoslst = new List<medicoDirectorioDTO>();

                if (String.IsNullOrWhiteSpace(nombre))
                    nombre = String.Empty;
                if (String.IsNullOrWhiteSpace(especialidadID) || especialidadID == "0")
                    especialidadID = String.Empty;
                if (String.IsNullOrWhiteSpace(seguroID) || seguroID == "0")
                    seguroID = String.Empty;

                medicoslst = _db.medicos
                        .Where(m => m.estado == true)
                        .Where(x =>
                        x.especialidades_medicos.Where(p => p.especialidadesID.ToString().Contains(especialidadID)).Any()
                        && x.cobertura_medicos.Where(p => p.segurosID.ToString().Contains(seguroID)).Any()
                        && (x.nombre + x.apellido).ToLower().Contains(nombre))
                        .ProjectTo<medicoDirectorioDTO>(_mapper.ConfigurationProvider).ToList();

                if (!medicoslst.Any())
                    return new NoContentResult();

                return medicoslst;

            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<ActionResult<medicoDTO>> getById(int Id)
        {
            try
            {

                var horarios = await _db.horarios_medicos.FirstOrDefaultAsync(x => x.medicosID == Id);

                medicoDTO _medicoDTO = _mapper.Map<medicoDTO>(_medicoRepos.getById(Id));

                if (horarios == null || _medicoDTO is null)
                    return new NoContentResult();

                Dictionary<string, List<string>> schedulelst = new Dictionary<string, List<string>>();

                List<string> lunesHoras = getMonday(horarios);
                List<string> martesHoras = getTuesday(horarios);
                List<string> miercolesHoras = getWednesday(horarios);
                List<string> juevesHoras = getThursday(horarios);
                List<string> viernesHoras = getFriday(horarios);
                List<string> sabadosHoras = getSaturday(horarios);
                List<string> domingosHoras = getSunday(horarios);

                schedulelst.Add("Lunes", lunesHoras);
                schedulelst.Add("Martes", martesHoras);
                schedulelst.Add("Miercoles", miercolesHoras);
                schedulelst.Add("Jueves", juevesHoras);
                schedulelst.Add("Viernes", viernesHoras);
                schedulelst.Add("Sabados", sabadosHoras);
                schedulelst.Add("Domingos", domingosHoras);

                _medicoDTO.horarios = schedulelst;

                return _medicoDTO;

            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un error al tratar de conseguir al médico: " + e.Message);
            }
        }


        private List<string> getSunday(horarios_medicos horarios)
        {
            if (horarios.sunday_from != null && horarios.sunday_until != null)
            {
                return getHoursList(
               horarios.sunday_from.Value,
               horarios.sunday_until.Value,
               horarios.free_time_from.Value,
               horarios.free_time_until.Value
               );
            }
            else
                return null;
        }

        private List<string> getSaturday(horarios_medicos horarios)
        {
            if (horarios.saturday_from != null && horarios.saturday_until != null)
            {
                return getHoursList(
                horarios.saturday_from.Value,
                horarios.saturday_until.Value,
                horarios.free_time_from.Value,
                horarios.free_time_until.Value
                );
            }
            else
                return null;
        }

        private List<string> getWednesday(horarios_medicos horarios)
        {
            if (horarios.wednesday_from != null && horarios.wednesday_until != null)
            {
                return getHoursList(
                 horarios.wednesday_from.Value,
                 horarios.wednesday_until.Value,
                 horarios.free_time_from.Value,
                 horarios.free_time_until.Value
                 );

            }
            else
                return null;
        }

        private List<string> getFriday(horarios_medicos horarios)
        {
            if (horarios.friday_from != null && horarios.friday_until != null)
            {
                return getHoursList(
                 horarios.friday_from.Value,
                 horarios.friday_until.Value,
                 horarios.free_time_from.Value,
                 horarios.free_time_until.Value
                 );

            }
            else
                return null;
        }

        private List<string> getThursday(horarios_medicos horarios)
        {
            if (horarios.thursday_from != null && horarios.thursday_until != null)
            {
                return getHoursList(
                  horarios.thursday_from.Value,
                  horarios.thursday_until.Value,
                  horarios.free_time_from.Value,
                  horarios.free_time_until.Value
                  );
            }
            else
                return null;
        }

        private List<string> getTuesday(horarios_medicos horarios)
        {
            if (horarios.tuesday_from != null && horarios.tuesday_until != null)
            {
                return getHoursList(
                    horarios.tuesday_from.Value,
                    horarios.tuesday_until.Value,
                    horarios.free_time_from.Value,
                    horarios.free_time_until.Value
                    );
            }
            else
                return null;
        }

        private List<string> getMonday(horarios_medicos horarios)
        {
            if (horarios.monday_from != null && horarios.monday_until != null)
            {
                return getHoursList(
                   horarios.monday_from.Value,
                   horarios.monday_until.Value,
                   horarios.free_time_from.Value,
                   horarios.free_time_until.Value
                   );
            }
            else
                return null;
        }

        private List<string> getHoursList(TimeSpan WDStartH,
        TimeSpan WDEndH, TimeSpan FreeTimeFrom, TimeSpan FreeTimeUntil)
        {
            //primera tanda 
            List<string> hourslst = new List<string>();
            var _time = WDStartH.ToString();
            string _hour = Convert.ToDateTime(_time).ToString("h:mm:tt");
            _time = FreeTimeFrom.ToString();
            _hour = _hour + " - " + Convert.ToDateTime(_time).ToString("h:mm:tt");
            hourslst.Add(_hour);
            //segunda tanda 
            _time = FreeTimeUntil.ToString();
            _hour = Convert.ToDateTime(_time).ToString("h:mm:tt");
            _time = WDEndH.ToString();
            _hour = _hour + " - " + Convert.ToDateTime(_time).ToString("h:mm:tt");
            hourslst.Add(_hour);
            return hourslst;
        }


    }
}
