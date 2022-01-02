using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cliente.DTO;
using CentromedicoCliente.Services.Interfaces;
using Cliente.Repository.Repositories.Interfaces;
using System.Linq;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CentromedicoCliente.Exceptions;
using CentromedicoCliente.Services;

namespace CentromedicoCliente.Services
{
    public class CitaService : ICitaService
    {
        private readonly ICitaRepository _citaRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IHorarioMedicoRepository _horarioMedicoRepo;
        private readonly IServicioRepository _servicioRepo;
        private readonly ICoberturaRepository _coberturaRepo;
        private readonly IHorarioMedicoReservaRepository _horarioMRRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMedicoRepository _medicoRepo;
        private readonly ISeguroRepository _seguroRepo;
        private readonly INotificationService _notificationService;
        private readonly IPacienteRepository _pacienteRepo;

        public CitaService(
            IHorarioMedicoReservaRepository horarioMRRepo,
            IServicioRepository servicioRepo,
            IMedicoRepository medicoRepo,
            IPacienteRepository pacienteRepo,
            ISeguroRepository seguroRepo,
            INotificationService notificationService,
            ICoberturaRepository coberturaRepo,
            ICitaRepository citaRepo,
            IHttpContextAccessor httpContextAccessor,
            IHorarioMedicoRepository horarioMedicoRepo,
            UserManager<MyIdentityUser> userManager,
            MyDbContext db, IMapper mapper)
        {
            _horarioMRRepo = horarioMRRepo;
            _horarioMedicoRepo = horarioMedicoRepo;
            _coberturaRepo = coberturaRepo;
            _servicioRepo = servicioRepo;
            _pacienteRepo = pacienteRepo;
            _seguroRepo = seguroRepo;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _citaRepo = citaRepo;
            _db = db;
            _mapper = mapper;
            _medicoRepo = medicoRepo;
        }

        public async Task<citaResultDTO> createCitaAsync(citaCreateDTO formdata)
        {

            pacientes paciente = null;
            citas cita;
            horarios_medicos_reservados horaReservacion;
            string codVer;
            seguros seguro;
            MyIdentityUser user = await _userManager
                    .FindByNameAsync(_httpContextAccessor.HttpContext.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value);
            try
            {

                //Validate incoming data
                medicos medico = _medicoRepo.getById(formdata.medicosID);
                if (medico == null)
                    throw new EntityNotFoundException("El doctor seleccionado no se encuentra habilitado en estos momentos.");

                if (_citaRepo.Exist(medico, user))
                    throw new BadRequestException("Ya hay una cita programada con este doctor(a)");

                if (formdata.segurosID == null)
                    seguro = await _seguroRepo.getByIdAsync(1);//1 by default is None-Insurace
                else
                    seguro = await _seguroRepo.getByIdAsync(formdata.segurosID);
                /* especialidades especialidad = await _db.especialidades.FindAsync(formdata.especialidadesID);
                 if (especialidad == null)
                     throw  new BadRequestException(new { InvalidEspecialidad = "La especialidad seleccionada no se encuentra en la base de datos." });*/

                servicios servicio = await _servicioRepo.getByIdAsync(formdata.segurosID);

                horaReservacion = await _horarioMedicoRepo.getReservedHourAsync(formdata.medicosID, formdata.fecha_hora);

                var availableDateHourlst = getAvailableDateHour(formdata.fecha_hora, formdata.medicosID);

                var patientData = await getPatientAgeAsync(formdata.fecha_nacimiento, formdata.appointment_type);
                formdata.edad = patientData.Item1;
                formdata.menor_un_año = patientData.Item2;

                paciente = _pacienteRepo.getWithDocIdent(user);

                codVer = _citaRepo.Exist(user) ? _citaRepo.getCV(user) : generateCV(medico.nombre, medico.apellido);

                int nTurn = getNewTurn(formdata.fecha_hora, formdata.medicosID);

                //VALIDATORS

                if (String.IsNullOrWhiteSpace(user.doc_identidad))
                    throw new BadRequestException("Este usuario no cuenta con un documento de identidad previamente ingresado.");

                //Determinar si la hora de la cita está disponible en el rango de fechas hábiles
                if (formdata.fecha_hora < DateTime.Now || formdata.fecha_hora > DateTime.Now.AddDays(30))
                    throw new BadRequestException("El día suministrado no está en el rango de fecha disponible");

                if (seguro == null)
                    throw new BadRequestException("El seguro seleccionado no se encuentra en la base de datos.");

                if (servicio == null)
                    throw new BadRequestException("El servicio seleccionado no se encuentra en la base de datos.");

                coberturaMedicoDTO cobertura = await _coberturaRepo.getAsync(formdata.medicosID, formdata.segurosID, formdata.serviciosID);

                if (cobertura == null)
                    throw new BadRequestException("Ha ocurrido un error al tratar de especificar el cobertura del servicio.");

                if (horaReservacion != null)
                    throw new BadRequestException("La fecha y hora para la cita programada está reservada, intente con otra por favor.");

                if (availableDateHourlst == null)
                    throw new ArgumentException("Este doctor(a) no labora el día escogido.");

                if (!availableDateHourlst.Contains(formdata.fecha_hora))
                    throw new ArgumentException("La hora provista no se encuentra en el rango de horas disponibles para ser reservada.");

                if (nTurn == 0)
                    throw new Exception("Ha ocurrido un error al tratar de generar el turno para la cita.");

                //Checking appoiment type
                switch (formdata.appointment_type)
                {
                    case (int)appointment.me:

                        bool addNewPaciente = false;

                        if (paciente == null || paciente.doc_identidad == null) //para no repetir el paciente en la db más de una vez.
                            addNewPaciente = true;

                        _mapper.Map<MyIdentityUser, pacientes>(user, paciente);
                        if (addNewPaciente)
                        {
                            paciente = _mapper.Map<pacientes>(user);
                            _pacienteRepo.Add(paciente);
                        }
                        else
                            _pacienteRepo.Update(paciente);
                        break;

                    case (int)appointment.other:

                        paciente = _mapper.Map<pacientes>(formdata);

                        paciente.doc_identidad_tutor = user.doc_identidad;
                        paciente.nombre_tutor = String.IsNullOrWhiteSpace(user.nombre) ? null : user.nombre;
                        paciente.apellido_tutor = String.IsNullOrWhiteSpace(user.apellido) ? null : user.apellido;
                        paciente.MyIdentityUsers = user;
                        _pacienteRepo.Add(paciente);

                        break;

                    default:
                        throw new ArgumentException("No se ha definido al tipo de paciente que se va a consultar, especifique si es \"Yo\" ó \"Otra persona\"");
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
                    //especialidades = especialidad,
                    consultorio = medico.consultorio,
                    cobertura = _cobertura,
                    pago = cobertura.pago,
                    diferencia = _diferencia,
                    nota = formdata.nota,
                    contacto = formdata.contacto,
                    contacto_whatsapp = formdata.contacto_whatsapp,
                    fecha_hora = formdata.fecha_hora,
                    turno = nTurn
                };

                //reserve a doctor's schedule
                horaReservacion = new horarios_medicos_reservados
                {
                    medicosID = formdata.medicosID,
                    fecha_hora = formdata.fecha_hora,
                };

                cod_verificacion codVerificacion = new cod_verificacion
                {
                    value = codVer,
                    citas = cita,
                };

                //Saving entities
                _citaRepo.Add(cita);
                _db.cod_verificacion.Add(codVerificacion);
                _horarioMRRepo.Add(horaReservacion);

                _db.SaveChanges();


                var citaResult = _mapper.Map<citaResultDTO>(cita);

                try
                {
                    _notificationService.sendTicketMail(citaResult, user.Email);
                    _notificationService.sendTicketWhatsapp(citaResult, user.contacto);

                }
                catch (Exception)
                {

                    //log
                }

                //I return a ticket
                return citaResult;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<List<citaDTO>> getCitasListAsync()
        {

            try
            {
                List<citaDTO> citaslst = await _citaRepo.getCitasListAsync();

                if (!citaslst.Any())
                    throw new EntityNotFoundException("No hay contenido que mostrar.");

                return citaslst;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {
            try
            {

                List<citaDTO> citaslst = _citaRepo.getCitasListByCv(codVerificacion);

                if (!citaslst.Any())
                    throw new EntityNotFoundException("Este usuario no contiene citas activas.");

                return citaslst;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<Object> getFormCitaAsync(int medicoID)
        {

            medicos medico = _medicoRepo.getMedicoServices(medicoID);

            //Tiene que existir al menos 1 cobertura por defecto que es la privada.
            var coberturaslst = await _coberturaRepo.getAllByDoctorIdAsync(medicoID);

            var servicioslst = await _servicioRepo.getAllByDoctorIdAsync(medicoID);

            var availableDaylst = await _horarioMedicoRepo.getAvailableDayListAsync(medicoID);

            if (medico == null)
                throw new EntityNotFoundException("El doctor seleccionado no se encuentra habilitado en estos momentos.");

            if (coberturaslst == null)
                throw new BadRequestException("El doctor(a) seleccionado no tiene ninguna cobertura.");

            if (!availableDaylst.Any())
                throw new EntityNotFoundException("No hay horario disponible para una cita con este médico.");

            if (servicioslst == null)
                throw new BadRequestException("El doctor(a) seleccionado no tiene ningún servicios asignado.");

            return
                new
                {
                    medico = new
                    {
                        ID = medico.ID,
                        nombre = medico.nombre,
                        apellido = medico.apellido,
                    },
                    //seguros= coberturaslst,
                    //especialidades = especialidadeslst,
                    servicios = servicioslst,
                    diasLaborables = availableDaylst
                };

        }


        private string generateCV(string value1, string value2)
        {

            Random rand = new Random();
            string initialName = value1.Substring(0, 1);
            string initialLastName = value2.Substring(0, 1);
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
                    isUnique = true;
            }
            return newCod;
        }

        private List<DateTime> getAvailableDateHour(DateTime fecha_hora, int medicoID)
        {

            try
            {
                var availableTimelst = _horarioMedicoRepo.getAvailableHoursTurnDic(fecha_hora, medicoID)?.Select(x => x.Key).ToList(); ;

                return availableTimelst;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async Task<Tuple<int, bool>> getPatientAgeAsync(DateTime fechaNacimiento, int appointmentType)
        {

            try
            {

                MyIdentityUser user = await _userManager
                   .FindByNameAsync(_httpContextAccessor.HttpContext.User
                   .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                int lessPermititted = DateTime.Compare(fechaNacimiento, new DateTime(1910, 1, 1));
                int graterThanToday = DateTime.Compare(fechaNacimiento, DateTime.Today);

                //Valida la fecha de nacimiento 
                if (lessPermititted < 0 || graterThanToday > 0)
                    throw new ArgumentException("La fecha del nacimiento está fuera de rango permitido.");

                int _edad = DateTime.Today.AddTicks(-fechaNacimiento.Ticks).Year - 1;


                //Valida la edad, tipo de cita y datos de tutor
                if (_edad < 18 && appointmentType is not (int)appointment.other)
                    throw new ArgumentException("El tipo de cita escogido no es valido para un menor de edad.");
                else if (_edad >= 18 && appointmentType is not (int)appointment.me)
                    throw new ArgumentException("El tipo de cita escogido no es valido para un mayor de edad.");
                else if (String.IsNullOrWhiteSpace(user.nombre) && _edad < 18)
                    throw new ArgumentException("Es requerido un nombre para el tutor del menor de edad.");


                if (_edad == 0) //si es un menor de 1 año
                {
                    int _edadMeses = DateTime.Today.AddTicks(-fechaNacimiento.Ticks).Month;

                    return new Tuple<int, bool>((_edadMeses - 1), true); // 12 month minus 1
                }

                return new Tuple<int, bool>(_edad, false); // 12 month minus 1

            }
            catch (Exception)
            {
                throw;
            }
        }

        private int getNewTurn(DateTime fecha_hora, int medicoID)
        {
            try
            {
                var TimeTurnDic = _horarioMedicoRepo.getAvailableHoursTurnDic(fecha_hora, medicoID);

                if (TimeTurnDic?.Count < 1)
                    return 0;

                int nTurn = TimeTurnDic.First(x => x.Key.Equals(fecha_hora)).Value;

                return nTurn;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        enum appointment : int
        {
            me = 0,
            other = 1,
        }

    }
}
