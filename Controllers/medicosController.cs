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


        // GET api/<medicosController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<medicoDTO>> get_medico(int Id)
        {

            var horarios = await _db.horarios_medicos.FirstOrDefaultAsync(x => x.medicosID == Id);

            medicoDTO _medicoDTO = await _db.medicos.ProjectTo<medicoDTO>(_mapper.ConfigurationProvider)
             .FirstOrDefaultAsync(x => x.ID == Id);


            if (horarios == null)
                return NoContent();


            Dictionary<string, string> schedulelst = new Dictionary<string, string>();

            string lunesHoras = getMonday(horarios);
            string martesHoras = getTuesday(horarios);
            string miercolesHoras = getWednesday(horarios);
            string juevesHoras = getThursday(horarios);
            string viernesHoras = getFriday(horarios);
            string sabadosHoras = getSaturday(horarios);
            string domingosHoras = getSunday(horarios);

            schedulelst.Add("Lunes", lunesHoras);
            schedulelst.Add("Martes", martesHoras);
            schedulelst.Add("Miércoles", miercolesHoras);
            schedulelst.Add("Jueves", juevesHoras);
            schedulelst.Add("Viernes", viernesHoras);
            schedulelst.Add("Sabados", sabadosHoras);
            schedulelst.Add("Domingos", domingosHoras);

            _medicoDTO.horarios = schedulelst;

            return _medicoDTO;
        }

        private string getSunday(horarios_medicos horarios)
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

        private string getSaturday(horarios_medicos horarios)
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

        private string getWednesday(horarios_medicos horarios)
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

        private string getFriday(horarios_medicos horarios)
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

        private string getThursday(horarios_medicos horarios)
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

        private string getTuesday(horarios_medicos horarios)
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



        private string getMonday(horarios_medicos horarios)
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

        private string getHoursList(TimeSpan WDStartH,
        TimeSpan WDEndH, TimeSpan FreeTimeFrom, TimeSpan FreeTimeUntil)
        {
            //primera tanda 
            var _time = WDStartH.ToString();
            string _hours = Convert.ToDateTime(_time).ToString("hh:mm:sstt");
            _time = FreeTimeFrom.ToString();
            _hours = _hours + " - " + Convert.ToDateTime(_time).ToString("hh:mm:sstt");
            //segunda tanda 
            _time = FreeTimeUntil.ToString();
            _hours = _hours + "  " + Convert.ToDateTime(_time).ToString("hh:mm:sstt");
            _time = WDEndH.ToString();
            _hours = _hours + " - " + Convert.ToDateTime(_time).ToString("hh:mm:sstt");
            return _hours;
        }


        [HttpGet]
        public async Task<List<medico_directorioDTO>> get_medicos([FromQuery] string nombre = "", string especialidadID = "", string seguroID = "")
        {
            List<medico_directorioDTO> medicoslst = await _db.medicos
                .Where(m => m.estado == true)
                .Where(x => x.especialidades_medicos.Where(p => p.especialidadesID.ToString().Contains(especialidadID)).Any())
                .Where(x => x.cobertura_medicos.Where(p => p.segurosID.ToString().Contains(seguroID)).Any())
                .Where(x => (x.nombre + x.apellido).Contains(nombre))
                .ProjectTo<medico_directorioDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return medicoslst;
        }


        // PUT api/<medicosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<medicosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
