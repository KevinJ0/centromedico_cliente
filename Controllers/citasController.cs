using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalSalvador.Context;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
    [ApiController]
    public class citasController : ControllerBase
    {

        // jwt and refresh token

        private readonly IMapper _mapper;
        private readonly token _token;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;


        public citasController(RoleManager<IdentityRole> roleManager,
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




        [HttpPost]
        public async Task<ActionResult<List<citaDTO>>> Getcitas(codVerificacionDTO formdata)
        {
 
            try
            {

                List<citaDTO> citaslst = await _db.citas.Where(p => p.cod_verificacionID == formdata.value && p.estado == true)
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToListAsync();
                
                if (!citaslst.Any())
                    return Unauthorized(new { LoginError = "Código de verificación no existe" });

                return citaslst;
            }
            catch (Exception e)
            {

                return BadRequest(new { Error = e.StackTrace + "Error al intentar acceder a la información, por favor intente más tarde." });
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<citas>> create_cita(citaCreateDTO formdata)
        {

            pacientes paciente = null;
            citas cita;
            horarios_medicos_reservados hora_reserv;
            string codVer;
            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyIdentityUser user = await _userManager.FindByNameAsync(userName);

            try
            {

                medicos medico = await _db.medicos.FindAsync(formdata.medicosID);
                if (medico == null)
                    return BadRequest(new { InvalidDoctorError = "El doctor seleccionado no se encuentra habilitado en estos momentos." });

                seguros seguro = await _db.seguros.FindAsync(formdata.segurosID);
                if (formdata.segurosID == null)
                    seguro = await _db.seguros.FindAsync(1); //1 by default is NoneInsurace
                else if (seguro == null)
                    return BadRequest(new { InvalidSeguroError = "El seguro seleccionado no se encuentra en la base de datos." });

                especialidades especialidad = await _db.especialidades.FindAsync(formdata.especialidadesID);
                if (especialidad == null)
                    return BadRequest(new { InvalidEspecialidadError = "La especialidad seleccionada no se encuentra en la base de datos." });

                servicios servicio = await _db.servicios.FindAsync(formdata.serviciosID);
                if (servicio == null)
                    return BadRequest(new { InvalidServicioError = "El servicio seleccionado no se encuentra en la base de datos." });


                //await _db.turnos.Where(x => x.medicosID == formdata.medicosID).MaxAsync(x => x.turno_atendido);

                var cobertura = await _db.cobertura_medicos.FirstOrDefaultAsync(x =>
                    x.especialidadesID == formdata.especialidadesID &&
                    x.medicosID == formdata.medicosID &&
                    x.segurosID == formdata.segurosID &&
                    x.serviciosID == formdata.serviciosID);

                if (cobertura == null)
                {
                    //sin seguro
                    cobertura = await _db.cobertura_medicos.FirstOrDefaultAsync(x =>
                    x.especialidadesID == formdata.especialidadesID &&
                    x.medicosID == formdata.medicosID &&
                    x.serviciosID == formdata.serviciosID &&
                    x.segurosID == seguro.ID); //1 reperesenta el registro de no Seguro.

                    if (cobertura == null)
                        return BadRequest(new { InvalidCoberturaError = "Ha ocurrido un error al tratar de especificar el cobertura del servicio" });
                }


                //Determinar si la hora de la cita está disponible
                if (formdata.fecha_hora < DateTime.Now || formdata.fecha_hora > DateTime.Now.AddDays(30))
                    throw new ArgumentOutOfRangeException("El día suministrado no está en el rango de fecha disponible.");

                hora_reserv = await _db.horarios_medicos_reservados.FirstOrDefaultAsync(h => h.fecha_hora == formdata.fecha_hora && h.medicosID == formdata.medicosID);
                if (hora_reserv != null)
                    return BadRequest(new { InvalidDateTimeError = "La fecha y hora para la cita programada está reservada, intente con otra por favor." });

                List<DateTime> AvailableTimelst = await  getAvailableTimeRangeAsync(formdata.fecha_hora, formdata.medicosID);

                if (AvailableTimelst.Count == 0)
                    return BadRequest(new { InvalidDateTimeError = "No se encuentra ningún horario disponibles para esta fecha" });



                //Determine if there's an appoiment already
                paciente = _db.pacientes.FirstOrDefault(p => p.MyIdentityUsers == user && p.doc_identidad != null); //this is for add another row with the same ID tutor.

                if (paciente == null)
                    paciente = await _db.pacientes.FirstOrDefaultAsync(p => p.MyIdentityUsers == user); //this is for add another row with the same ID tutor.

                codVer = citaExists(user) ? getCV(user) : generateCV(medico);

                if (formdata.appoiment_type == (int)appoiment.me && _db.citas.Where(x => x.medicos == medico && x.pacientes == paciente && x.estado == true && x.especialidades == especialidad).Any())
                    throw new Exception("Ya hay una cita pragramada con este Doctor(a)");


                //choosing appoiment type
                switch (formdata.appoiment_type)
                {
                    case (int)appoiment.me:

                        if (paciente == null || paciente.doc_identidad == null)
                        {
                            paciente = _mapper.Map<pacientes>(formdata);
                            paciente.MyIdentityUsers = user;
                            _db.pacientes.Add(paciente);
                        }
                        break;

                    case (int)appoiment.other:

                        paciente = _mapper.Map<pacientes>(formdata);
                        paciente.MyIdentityUsers = user;
                        _db.pacientes.Add(paciente);
                        break;

                    default:
                        throw new ArgumentException("No se ha definido al tipo de paciente que se va a consultar, especifique si es \"Yo\" ó \"Otra persona\"");
                }


                decimal coberturaPorciento = (Decimal.Divide((cobertura.porciento), 100));
                decimal _cobertura = cobertura.pago * coberturaPorciento;
                decimal _diferencia = cobertura.pago - _cobertura;

                cita = new citas
                {
                    cod_verificacionID = codVer,
                    pacientes = paciente,
                    medicos = medico,
                    seguros = seguro,
                    servicios = servicio,
                    especialidades = especialidad,
                    consultorio = medico.consultorio,
                    cobertura = _cobertura,
                    pago = cobertura.pago,
                    diferencia = _diferencia,
                    nota = formdata.nota,
                    contacto = formdata.contacto,
                    contacto_whatsapp = formdata.contacto_whatsapp,
                    contacto_llamada = formdata.contacto_llamada,
                    tipo_contacto = formdata.tipo_contacto,
                    fecha_hora = formdata.fecha_hora,

                };

                hora_reserv = new horarios_medicos_reservados
                {
                    medicosID = formdata.medicosID,
                    fecha_hora = formdata.fecha_hora,
                };

                cod_verificacion codVerificacion = new cod_verificacion
                {
                    value = codVer,
                    citas = cita,
                };

                _db.citas.Add(cita);
                _db.cod_verificacion.Add(codVerificacion);
                _db.horarios_medicos_reservados.Add(hora_reserv);
                await _db.SaveChangesAsync();

                return cita;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool citaExists(MyIdentityUser user)
        {

            if (_db.citas.FirstOrDefault(x => x.pacientes.MyIdentityUsers == user && x.estado == true) != null)
            {
                return true;
            }

            return false;
        }

        private async Task<List<DateTime>> getReservedTimeRangeAsync(int? medicoID, DateTime fecha_hora)
        {

            var reservedTimelst = await _db.horarios_medicos_reservados.
                                   Where(rt => rt.medicosID == medicoID && rt.fecha_hora.Date == fecha_hora.Date).
                                   Select(rt => rt.fecha_hora).ToListAsync();

            return reservedTimelst;
        }

        private async Task<List<DateTime>> getAvailableTimeRangeAsync(DateTime fecha_hora, int medicoID)
        {

            horarios_medicosDTO doctorWorkSchedule = getWorkDaySchedule(fecha_hora.DayOfWeek, medicoID);
            int appointmentDuration = doctorWorkSchedule.tiempo_cita.Minutes;
            List<DateTime> reservedTimeRange = await getReservedTimeRangeAsync(medicoID, fecha_hora);
            DateTime startTime = fecha_hora.Date.Add(doctorWorkSchedule._from.Value);
            DateTime endTime = fecha_hora.Date.Add(doctorWorkSchedule._until.Value);
            DateTime startFreeTime = fecha_hora.Date.Add(doctorWorkSchedule.free_time_from.Value);
            DateTime endFreeTime = fecha_hora.Date.Add(doctorWorkSchedule.free_time_until.Value);
            List<DateTime> availableTimeRange = new List<DateTime>();

            while (startTime < endTime)
            {
                var intime = startTime.CompareTo(DateTime.Now);
                if ((startTime.TimeOfDay.CompareTo(startFreeTime.TimeOfDay) < 0 || startTime.TimeOfDay.CompareTo(endFreeTime.TimeOfDay) > 0) && intime > 0)
                    availableTimeRange.Add(startTime);

                startTime = startTime.AddMinutes(appointmentDuration);
            }

            //let's get rid of reserved hours
            if (reservedTimeRange != null)
            {
                availableTimeRange = availableTimeRange.Except(reservedTimeRange).ToList();
            }

            if (availableTimeRange.Contains(fecha_hora))
                return availableTimeRange;
            else
                throw new ArgumentOutOfRangeException("La fecha provista no se encuentra en el rango de fechas disponibles para ser reservada.");
        }

        private horarios_medicosDTO getWorkDaySchedule(DayOfWeek dayOfWeek, int? medicoID)
        {
            IQueryable<horarios_medicosDTO> hoursQr = null;

            if (dayOfWeek == DayOfWeek.Monday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.monday_from,
                               _until = h.monday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            if (dayOfWeek == DayOfWeek.Tuesday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.tuesday_from,
                               _until = h.tuesday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Wednesday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.wednesday_from,
                               _until = h.wednesday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Thursday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.thursday_from,
                               _until = h.thursday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Friday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.friday_from,
                               _until = h.friday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Saturday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.saturday_from,
                               _until = h.saturday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            else if (dayOfWeek == DayOfWeek.Sunday)
                hoursQr = (from h in _db.horarios_medicos
                           where h.medicosID == medicoID
                           select new horarios_medicosDTO
                           {
                               _from = h.sunday_from,
                               _until = h.sunday_until,
                               free_time_from = h.free_time_from,
                               free_time_until = h.free_time_until,
                               tiempo_cita = h.tiempo_cita
                           });

            var workDaySchedule = hoursQr.FirstOrDefault();
            if (workDaySchedule._from == null || workDaySchedule._until == null)
                throw new Exception("Este doctor(a) no labora el día escogido");


            return workDaySchedule;
            //_db.horarios_medicos.Where()
        }



        private object getDoctorSchedule(DayOfWeek dayOfWeek)
        {
            throw new NotImplementedException();
        }



        private string getCV(MyIdentityUser user)
        {
            cod_verificacion codV = _db.cod_verificacion
                .Include("citas")
                .Where(x => x.citas.pacientes.MyIdentityUsers == user && x.citas.estado == true)
                .FirstOrDefault();

            return codV.value;
        }



        private string generateCV(medicos md)
        {


            Random rand = new Random();
            string initialName = md.nombre.Substring(0, 1);
            string initialLastName = md.apellido.Substring(0, 1);
            string newCod = "";
            bool isUnique = false;

            while (!isUnique)
            {
                for (int i = 0; i < 4; i++)
                {
                    int numero = rand.Next(26);
                    newCod = newCod + Char.ToString((char)(((int)'A') + numero));
                }

                newCod = initialName + initialLastName + "" + newCod;

                if (!_db.citas.Where(x => x.cod_verificacionID == newCod).Any())
                {
                    isUnique = true;
                }
            }
            return newCod;
        }


        [HttpPost("[action]")]
        public pacientes getPacienteByCV(string cod_vr)
        {

            var _citaID = _db.cod_verificacion.FirstOrDefault(c => c.value == cod_vr);
            pacientes paciente = null;

            if (_citaID != null)
                paciente = _db.citas
                 .Include("pacientes")
                 .FirstOrDefault(x => x.ID == _citaID.citasID && x.estado == true).pacientes;
            else
                throw new ArgumentException("El código de verificación no es valido.");

            return paciente;

        }


        [HttpPost("[action]")]
        public async Task<ActionResult<citaCreateDTO>> add_cita(citaCreateDTO formdata)
        {
            throw new NotImplementedException();
        }





        // PUT: api/citas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putcitas(int id, citas citas)
        {
            if (id != citas.ID)
            {
                return BadRequest();
            }

            _db.Entry(citas).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!citasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /* // POST: api/citas
         // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
         [HttpPost]
         public async Task<ActionResult<citas>> Postcitas(citas citas)
         {
             _db.citas.Add(citas);
             await _db.SaveChangesAsync();

             return CreatedAtAction("Getcitas", new { id = citas.ID }, citas);
         }*/

        // DELETE: api/citas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletecitas(int id)
        {
            var citas = await _db.citas.FindAsync(id);
            if (citas == null)
            {
                return NotFound();
            }

            _db.citas.Remove(citas);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private bool citasExists(int id)
        {
            return _db.citas.Any(e => e.ID == id);
        }
        enum appoiment : int
        {
            me = 0,
            other = 1,
        }
    }
}
