using HospitalSalvador.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalSalvador.Context;
using HospitalSalvador.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace HospitalSalvador.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class medicosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly token _token;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;

        public medicosController(RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager,
            token token, MyDbContext db, IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _token = token;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// Método que devuelve al médico por el Id con los datos de interes para el usuario.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /medicos/{id}
        ///
        /// </remarks>
        /// <param name="Id"></param>
        /// <returns>medicoDTO</returns>
        /// <response code="200">Se encontró el médico solicitado y es devuelvo.</response>  
        /// <response code="204">Este médico no tiene los datos requeridos para ser devuelto.</response>  
        /// <response code="500">Error interno del servidor.</response>  
        [HttpGet("{id}")]
        public async Task<ActionResult<medicoDTO>> getMedico(int Id)
        {
            try
            {
           
            var horarios = await _db.horarios_medicos.FirstOrDefaultAsync(x => x.medicosID == Id);

            medicoDTO _medicoDTO = await _db.medicos.ProjectTo<medicoDTO>(_mapper.ConfigurationProvider)
             .FirstOrDefaultAsync(x => x.ID == Id);

            if (horarios == null)
                return NoContent();

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


        /// <summary>
        /// Método que devuelve un lista de médicos según los parametros de filtro que se especifique.
        /// Se necesita al menos 1 parametro para realizar el filtro.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /medicos?nombre=paola?especialidadID=1?seguroID=1
        ///      
        /// </remarks>
        /// <param name="nombre"></param>
        /// <param name="especialidadID"></param>
        /// <param name="seguroID"></param>
        /// <returns>List of medico_directorioDTO</returns>
        /// <response code="200">Devuelve la lista de médicos.</response>  
        /// <response code="204">No encontró ningún médico con estos parametros.</response>  
        [HttpGet]
        public async Task<ActionResult<List<medicoDirectorioDTO>>> filterMedicos([FromQuery] string nombre = "", string especialidadID = "", string seguroID = "")
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
                    return NoContent();

                return medicoslst;

            }
            catch (Exception)
            {
                throw;
            }
        }
       


        List<string> getSunday(horarios_medicos horarios)
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

        List<string> getSaturday(horarios_medicos horarios)
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

       List<string>  getWednesday(horarios_medicos horarios)
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

       List<string>  getFriday(horarios_medicos horarios)
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

       List<string>  getThursday(horarios_medicos horarios)
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

       List<string>  getTuesday(horarios_medicos horarios)
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

       List<string>  getMonday(horarios_medicos horarios)
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

        [HttpGet("[action]")]
        public ActionResult<List<especialidadDTO>> getEspecialidades()
        {
            try
            {

                List<especialidadDTO> especialidadeslst = _db.especialidades
                        .ProjectTo<especialidadDTO>(_mapper.ConfigurationProvider).ToList();

                if (especialidadeslst == null)
                    return NoContent();

                return especialidadeslst;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }


}
