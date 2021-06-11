using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpGet("[action]")]
        public async Task<ActionResult<Object>> getNewCitaAsync(int medicoID)
        {

            medicos medico = await _db.medicos.FindAsync(medicoID);

            //Tiene que existir al menos 1 cobertura por defecto que es la privada.
            var coberturaslst = await _db.cobertura_medicos
                .Where(s => s.medicosID == medicoID)
                .Select(c => new { c.seguros.descrip, c.segurosID })
                .ToListAsync();


            var especialidadeslst = await _db.especialidades_medicos
                .Where(x => x.medicosID == medicoID)
                .Select(x => new { x.especialidades.ID, x.especialidades.descrip })
                .ToListAsync();

            var servicioslst = await _db.servicios_medicos
                .Where(x => x.medicosID == medicoID)
                .Select(x => new { x.servicios.ID, x.servicios.descrip })
                .ToListAsync();

            var availableDaylst = await getAvailableDayListAsync(medicoID);
            var availableTimelst = await getNextWorkTimeListAsync(medicoID);


            if (medico == null)
                return BadRequest(new { InvalidDoctor = "El doctor seleccionado no se encuentra habilitado en estos momentos." });

            if (coberturaslst == null)
                return BadRequest(new { InvalidCobertura = "El doctor(a) seleccionado no tiene ninguna cobertura" });

            if (especialidadeslst == null)
                return BadRequest(new { InvalidEspecialidad = "El doctor(a) seleccionado no tiene ninguna especialidad asignada." });

            if (!availableDaylst.Any())
                return BadRequest(new { NoScheduleAvailable = "No hay horario disponible para una cita con este médico 😅." });
            
            if (servicioslst == null)
                return BadRequest(new { InvalidServicio = "El doctor(a) seleccionado no tiene ningún servicios asignado." });


            return Ok(
                new
                {
                    medico = new
                    {
                        ID = medico.ID,
                        nombre = medico.nombre,
                        apellido = medico.apellido,
                    },
                    seguros = coberturaslst,
                    especialidades = especialidadeslst,
                    servicios = servicioslst,
                    diasLaborables = availableDaylst,
                    horasDisponibles = availableTimelst
                });
        }

        //via codigo de verificacion
        [HttpGet("[action]")]
        public async Task<ActionResult<List<citaDTO>>> getCitasListByCvAsync(string codVerificacion)
        {
            try
            {

                List<citaDTO> citaslst = await _db.citas
                    .Where(p => p.cod_verificacionID == codVerificacion && p.estado == true)
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToListAsync();

                if (!citaslst.Any())
                    return NoContent();

                return citaslst;
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.StackTrace + "Error al intentar acceder a la información, por favor intente más tarde." });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpGet("[action]")]
        public async Task<ActionResult<List<citaDTO>>> getCitasListAsync()
        {
            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);

                List<citaDTO> citaslst = await _db.citas
                    .Where(p => p.pacientes.MyIdentityUsers == user && p.estado == true)
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToListAsync();

                if (!citaslst.Any())
                    return NoContent();

                return citaslst;
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.StackTrace + "Error al intentar acceder a la información, por favor intente más tarde." });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpPost("[action]")]
        public async Task<ActionResult<citaResultDTO>> createCitaAsync(citaCreateDTO formdata)
        {

            pacientes paciente = null;
            citas cita;
            horarios_medicos_reservados hora_reserv;
            string codVer;

            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyIdentityUser user = await _userManager.FindByNameAsync(userName);

            try
            {
                //Validate incoming data
                citaValidationResponse dataResponse = validateAndSetData(formdata);

                if (!dataResponse.successful)
                    return BadRequest(new { ErrorDataInput = dataResponse.errorMessage });
                else
                    formdata = dataResponse.citaCreateDTO;


                medicos medico = await _db.medicos.FindAsync(formdata.medicosID);
                if (medico == null)
                    return BadRequest(new { InvalidDoctor = "El doctor seleccionado no se encuentra habilitado en estos momentos." });

                seguros seguro = await _db.seguros.FindAsync(formdata.segurosID);
                if (formdata.segurosID == null)
                    seguro = await _db.seguros.FindAsync(1); //1 by default is NoneInsurace
                else if (seguro == null)
                    return BadRequest(new { InvalidSeguro = "El seguro seleccionado no se encuentra en la base de datos." });

                especialidades especialidad = await _db.especialidades.FindAsync(formdata.especialidadesID);
                if (especialidad == null)
                    return BadRequest(new { InvalidEspecialidad = "La especialidad seleccionada no se encuentra en la base de datos." });

                servicios servicio = await _db.servicios.FindAsync(formdata.serviciosID);
                if (servicio == null)
                    return BadRequest(new { InvalidServicio = "El servicio seleccionado no se encuentra en la base de datos." });

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
                        return BadRequest(new { CoberturaError = "Ha ocurrido un error al tratar de especificar el cobertura del servicio" });
                }


                //Determinar si la hora de la cita está disponible
                if (formdata.fecha_hora < DateTime.Now || formdata.fecha_hora > DateTime.Now.AddDays(30))
                    return BadRequest(new { NoDateError = "El día suministrado no está en el rango de fecha disponible" });

                hora_reserv = await _db.horarios_medicos_reservados.FirstOrDefaultAsync(h => h.fecha_hora == formdata.fecha_hora && h.medicosID == formdata.medicosID);
                if (hora_reserv != null)
                    return BadRequest(new { DateTimeReservedError = "La fecha y hora para la cita programada está reservada, intente con otra por favor." });

                validationResponse AvailableTime = await ValidateAvailableTimeAsync(formdata.fecha_hora, formdata.medicosID);

                if (!AvailableTime.successful)
                    return BadRequest(new { DateTimeError = AvailableTime.errorMessage });


                //Determine if there's an appoiment already
                paciente = _db.pacientes.FirstOrDefault(p => p.MyIdentityUsers == user && p.doc_identidad != null); //this is for add another row with the same ID tutor.

                if (paciente == null)
                    paciente = await _db.pacientes.FirstOrDefaultAsync(p => p.MyIdentityUsers == user); //this is for add another row with the same ID tutor.

                codVer = citaExists(user) ? getCV(user) : generateCV(medico);

                if (formdata.appoiment_type == (int)appointment.me && _db.citas.Where(x => x.medicos == medico && x.pacientes == paciente && x.estado == true && x.especialidades == especialidad).Any())
                    return BadRequest(new { DuplicatedAppointmentError = "Ya hay una cita programada con este doctor(a)" });


                //choosing appoiment type
                switch (formdata.appoiment_type)
                {
                    case (int)appointment.me:

                        if (paciente == null || paciente.doc_identidad == null)
                        {
                            paciente = _mapper.Map<pacientes>(formdata);
                            paciente.MyIdentityUsers = user;
                            _db.pacientes.Add(paciente);
                        }
                        break;

                    case (int)appointment.other:

                        paciente = _mapper.Map<pacientes>(formdata);
                        paciente.MyIdentityUsers = user;
                        _db.pacientes.Add(paciente);
                        break;

                    default:
                        return BadRequest(new { PatientTypeError = "No se ha definido al tipo de paciente que se va a consultar, especifique si es \"Yo\" ó \"Otra persona\"" });

                }

                //calculate costs
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

                //reserve a doctor's schedule
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

                //saving entities
                _db.citas.Add(cita);
                _db.cod_verificacion.Add(codVerificacion);
                _db.horarios_medicos_reservados.Add(hora_reserv);
                await _db.SaveChangesAsync();

                //I need to return a tiket
                return Ok(new citaResultDTO
                {
                    cod_verificacion = cita.cod_verificacionID,
                    servicio = cita.servicios.descrip,
                    consultorio = cita.consultorio,
                    fecha_hora = cita.fecha_hora,
                    medico_nombre_apellido = (medico.nombre + " " + medico.apellido).Trim(),
                    especialidad = especialidad.descrip,
                    seguro = seguro.descrip,
                    pago = cobertura.pago,
                    cobertura = _cobertura,
                    diferencia = _diferencia,
                    doc_identidad = paciente.doc_identidad,
                    paciente_nombre_apellido = (paciente.nombre + " " + paciente.apellido).Trim(),
                    doc_identidad_tutor = paciente.doc_identidad_tutor,
                    paciente_nombre_apellido_tutor = (paciente.nombre_tutor + " " + paciente.apellido_tutor).Trim(),
                    contacto = cita.contacto,

                });

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        public async Task<ActionResult<Object>> getCoberturasAsync(int especialidadID, int servicioID, int medicoID, int seguroID)
        {
            try
            {

                var cobertura = await _db.cobertura_medicos.FirstAsync(x =>
                       x.especialidadesID == especialidadID &&
                       x.medicosID == medicoID &&
                       x.segurosID == seguroID &&
                       x.serviciosID == servicioID);

                //calculate costs
                decimal _porciento = (Decimal.Divide((cobertura.porciento), 100));
                decimal _cobertura = cobertura.pago * _porciento;
                decimal _diferencia = cobertura.pago - _cobertura;

                return new costoServicio
                {
                    porciento = _porciento,
                    cobertura = _cobertura,
                    diferencia = _diferencia
                };
            }
            catch (Exception)
            {
                return NoContent();
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

        private async Task<List<DateTime>> getNextWorkTimeListAsync(int medicoID)
        {
            var maxDate = DateTime.Now.AddDays(30);

            List<DateTime> timelst = new List<DateTime>();
            DateTime _date = DateTime.Now;
            bool nextday = true;
            int dateLimit = 0;

            while (nextday)
            {
                timelst = await getAvailableTimeListAsync(_date, medicoID);
                if (timelst != null)
                {
                    //si labora ese día pero no hay horarios disponible omito esta fecha.
                    if (timelst.Count() > 0)
                        nextday = false;
                }

                _date = _date.AddDays(1);
                dateLimit = _date.CompareTo(maxDate);

                if (dateLimit > 0)
                    return null;

            }
            return timelst;
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        public async Task<List<DateTime>> getAvailableTimeListAsync(DateTime fecha_hora, int medicoID)
        {
            //get the last appointment list schedule of this doctor
            horarios_medicosDTO doctorWorkSchedule = getWorkDaySchedule(fecha_hora.DayOfWeek, medicoID);
            if (doctorWorkSchedule == null)
                return null;

            int appointmentDuration = doctorWorkSchedule.tiempo_cita.Minutes;
            List<DateTime> reservedTimelst = await getReservedTimelstAsync(medicoID, fecha_hora);
            DateTime startTime = fecha_hora.Date.Add(doctorWorkSchedule._from.Value);
            DateTime endTime = fecha_hora.Date.Add(doctorWorkSchedule._until.Value);
            DateTime startFreeTime = fecha_hora.Date.Add(doctorWorkSchedule.free_time_from.Value);
            DateTime endFreeTime = fecha_hora.Date.Add(doctorWorkSchedule.free_time_until.Value);
            List<DateTime> availableTimelst = new List<DateTime>();

            while (startTime < endTime)
            {
                var intime = startTime.CompareTo(DateTime.Now);
                if ((startTime.TimeOfDay.CompareTo(startFreeTime.TimeOfDay) < 0 || startTime.TimeOfDay.CompareTo(endFreeTime.TimeOfDay) > 0) && intime > 0)
                    availableTimelst.Add(startTime);

                startTime = startTime.AddMinutes(appointmentDuration);
            }

            //let's get rid of reserved hours
            if (reservedTimelst != null)
                availableTimelst = availableTimelst.Except(reservedTimelst).ToList();

            return availableTimelst;
        }

        private async Task<List<DateTime>> getAvailableDayListAsync(int medicoID)
        {

            var availableDaylst = new List<DateTime>();

            int maxDays = 30;

            List<DateTime> timelst = new List<DateTime>();
            DateTime currentDate = DateTime.Now;

            for (int i = 0; i < maxDays; i++)
            {

                timelst = await getAvailableTimeListAsync(currentDate, medicoID);

                if (timelst != null)
                {
                    //Store this date if it's available.
                    if (timelst.Count() > 0)
                        availableDaylst.Add(timelst.First().Date);
                }

                currentDate = currentDate.AddDays(1);
            }
            return availableDaylst;
        }

        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        public async Task<validationResponse> ValidateAvailableTimeAsync(DateTime fecha_hora, int medicoID)
        {

            var availableTimelst = await getAvailableTimeListAsync(fecha_hora, medicoID);

            if (availableTimelst == null)
                return new validationResponse
                {
                    errorMessage = "Este doctor(a) no labora el día escogido"
                };

            if (availableTimelst.Contains(fecha_hora))
                return new validationResponse();
            else
                return new validationResponse
                {
                    errorMessage = "La hora provista no se encuentra en el rango de horas disponibles para ser reservada."
                };
        }

        private horarios_medicosDTO getWorkDaySchedule(DayOfWeek dayOfWeek, int medicoID)
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

            if (workDaySchedule == null)
                return null;

            if (workDaySchedule._from == null || workDaySchedule._until == null)
                return null;

            return workDaySchedule;
            //_db.horarios_medicos.Where()
        }

        private async Task<List<DateTime>> getReservedTimelstAsync(int? medicoID, DateTime fecha_hora)
        {

            var reservedTimelst = await _db.horarios_medicos_reservados
                .Where(rt => rt.medicosID == medicoID && rt.fecha_hora.Date == fecha_hora.Date)
                .Select(rt => rt.fecha_hora).ToListAsync();

            return reservedTimelst;
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

        private citaValidationResponse validateAndSetData(citaCreateDTO formdata)
        {

            string _errorMessage = "";
            string doc_identidad = formdata.doc_identidad;
            string doc_identidad_tutor = formdata.doc_identidad_tutor;
            var fechaNacimiento = formdata.fecha_nacimiento;
            var appoiment_type = formdata.appoiment_type;
            formdata.menor_un_año = false;

            int lessPermititted = DateTime.Compare(fechaNacimiento, new DateTime(1910, 1, 1));
            int graterThanToday = DateTime.Compare(fechaNacimiento, DateTime.Today);

            //Valida la fecha de nacimiento 
            if (lessPermititted < 0 || graterThanToday > 0)
            {
                _errorMessage = "La fecha del nacimiento está fuera de rango permitido.";

                return new citaValidationResponse
                {
                    errorMessage = _errorMessage,
                };
            }

            int _edad = DateTime.Today.AddTicks(-fechaNacimiento.Ticks).Year - 1;

            //Valida la edad, tipo de cita y datos de tutor
            if (_edad >= 18 && doc_identidad == null)
                _errorMessage = "No se ha provisto de un documento de identidad.";
            else if (_edad < 18 && doc_identidad_tutor == null) // porque todos los menores de edad necesitan un tutor
                _errorMessage = "No se ha provisto de una identificación por parte del tutor que acompaña al paciente menor de edad.";
            else if (_edad < 18 && appoiment_type != 1)
                _errorMessage = "Tipo de cita escogido erroneo para un menor de edad.";
            else if (_edad >= 18 && appoiment_type != 0)
                _errorMessage = "Tipo de cita escogido erroneo para un mayor de edad.";
            else if (String.IsNullOrWhiteSpace(formdata.nombre_tutor) && _edad < 18)
                _errorMessage = "Es requerido un nombre para el tutor del menor de edad.";
            else if (_edad < 18 && doc_identidad_tutor != null)
                formdata.doc_identidad = null;
            else if (_edad >= 18)
                formdata.doc_identidad_tutor = null;

            //Catch error
            if (!String.IsNullOrEmpty(_errorMessage)) // an error has occured
                return new citaValidationResponse
                {
                    errorMessage = _errorMessage,
                };

            if (_edad < 120 && _edad > 0)
            {
                formdata.edad = _edad;
            }
            else if (_edad == 0) //si es un menor de 1 año
            {
                int _edadMeses = DateTime.Today.AddTicks(-fechaNacimiento.Ticks).Month;

                if (_edadMeses >= 1 && _edadMeses <= 12)
                {
                    formdata.edad = _edadMeses - 1; // 12 month minus 1
                    formdata.menor_un_año = true;
                }
            }

            //return processed data
            return new citaValidationResponse
            {
                citaCreateDTO = formdata,
            };
        }


        public citaValidationResponse validateBirth(citaCreateDTO formdata)
        {
            DateTime fechaNacimiento = formdata.fecha_nacimiento;
            int lessPermititted = DateTime.Compare(fechaNacimiento, new DateTime(1910, 1, 1));
            int graterThanToday = DateTime.Compare(fechaNacimiento, DateTime.Today);
            string _message = "";

            if (lessPermititted < 0 || graterThanToday > 0)
                _message = "La fecha de nacimiento está fuera de rango permitido.";

            return new citaValidationResponse
            {
                errorMessage = _message,
            };
        }

        enum appointment : int
        {
            me = 0,
            other = 1,
        }
    }

    public class citaValidationResponse : validationResponse
    {
        public citaValidationResponse() : base() { }
        public citaCreateDTO citaCreateDTO { get; set; }
    }

    public class validationResponse
    {
        public validationResponse()
        {
            errorMessage = "";
        }

        public bool successful
        {
            get => String.IsNullOrEmpty(errorMessage) ? true : false;
        }
        public string errorMessage { get; set; }
    }
    internal class costoServicio
    {
        public decimal porciento { get; set; }
        public decimal cobertura { get; set; }
        public decimal diferencia { get; set; }
    }
}
