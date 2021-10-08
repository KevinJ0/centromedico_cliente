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
using Polly.Retry;
using Polly;
using HospitalSalvador.Services;
using System.Globalization;
using System.Text;

namespace HospitalSalvador.Controllers
{
    [Produces("application/json")]
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
        private readonly RetryPolicy _retryPolicy;
        private readonly int MaxRetries = 3;
        private readonly IEmailService _emailService;
        private readonly IWhatsappService _whatsappService;

        public citasController(RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager,
            token token, MyDbContext db, IMapper mapper,
              IEmailService emailService, IWhatsappService whatsappService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _token = token;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
            _emailService = emailService;
            _whatsappService = whatsappService;
            _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(MaxRetries, time => TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Método que devuelve los datos para llenar el formulario con los datos del médico selecto.
        /// Este método trae los seguros, especialidades y horarios disponibles con los que trabaja el médico solicitado.
        /// En caso de que el médico no tenga algunos de los mencionados requisitos válidos, se procederá a devolver un BadRequest.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/getNewCita?medicoID=2
        ///      
        /// Sample response:
        ///
        ///     {
        ///         "medico": {
        ///             "id": 1,
        ///             "nombre": "Paola",
        ///             "apellido": "Rodriguez Sarmientos"
        ///         },
        ///         "seguros": [
        ///             {
        ///               "descrip": "Privado",
        ///               "segurosID": 1
        ///         },
        ///             {
        ///                 "descrip": "Humano",
        ///                 "segurosID": 2
        ///         },
        ///             {
        ///         "descrip": "ARS",
        ///                 "segurosID": 5
        ///         }
        ///         ],
        ///         "especialidades": [
        ///             {
        ///                 "id": 1,
        ///                 "descrip": "Alergiología"
        ///         },
        ///             {
        ///                 "id": 6,
        ///                 "descrip": "Ginecología"
        ///         }
        ///         ],
        ///         "servicios": [
        ///             {
        ///                 "id": 1,
        ///                 "descrip": "Consulta"
        ///         },
        ///             {
        ///                 "id": 2,
        ///                 "descrip": "Solicitud de receta médica"
        ///         },
        ///             {
        ///                 "id": 3,
        ///                 "descrip": "Consulta de seguimiento"
        ///         }
        ///         ],
        ///         "diasLaborables": [
        ///             "2021-06-11T00:00:00-07:00",
        ///             "2021-06-12T00:00:00-07:00",
        ///             "2021-06-14T00:00:00-07:00",
        ///             "2021-06-15T00:00:00-07:00",
        ///             "2021-06-16T00:00:00-07:00",
        ///             "2021-06-17T00:00:00-07:00",
        ///             "2021-06-18T00:00:00-07:00",
        ///             "2021-06-19T00:00:00-07:00",
        ///             "2021-06-21T00:00:00-07:00",
        ///             "2021-06-22T00:00:00-07:00",
        ///             "2021-06-23T00:00:00-07:00",
        ///             "2021-06-24T00:00:00-07:00",
        ///             "2021-06-25T00:00:00-07:00",
        ///             "2021-06-26T00:00:00-07:00",
        ///             "2021-06-28T00:00:00-07:00",
        ///             "2021-07-10T00:00:00-07:00"
        ///         ],
        ///         "horasDisponibles": [
        ///             "2021-06-11T16:40:00-07:00",
        ///             "2021-06-11T16:40:00-07:30",
        ///             "2021-06-11T16:40:00-08:00",
        ///             "2021-06-11T16:40:00-08:30",
        ///         ]
        ///     }
        ///      
        /// </remarks>
        /// <param name="medicoID"></param>
        /// <returns>Annonymous Object</returns>
        /// <response code="400">Hubo un problema con los datos del médico solicitado.</response>  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpGet("[action]")]
        public async Task<ActionResult<Object>> getNewCitaAsync(int medicoID)
        {

            medicos medico = await _db.medicos.FindAsync(medicoID);

            //Tiene que existir al menos 1 cobertura por defecto que es la privada.
            var coberturaslst = await _db.cobertura_medicos
                .Where(s => s.medicosID == medicoID)
                .Select(c => new { c.seguros.ID, c.seguros.descrip })
                .ToListAsync();

            /* var especialidadeslst = await _db.especialidades_medicos
                 .Where(x => x.medicosID == medicoID)
                 .Select(x => new { x.especialidades.ID, x.especialidades.descrip })
                 .ToListAsync();*/

            var servicioslst = await _db.servicios_medicos
                .Where(x => x.medicosID == medicoID)
                .Select(x => new { x.servicios.ID, x.servicios.descrip })
                .ToListAsync();

            var availableDaylst = await getAvailableDayListAsync(medicoID);
            var availableTimelst = await getNextWorkTimeListAsync(medicoID);


            if (medico == null)
                return BadRequest("El doctor seleccionado no se encuentra habilitado en estos momentos.");

            if (coberturaslst == null)
                return BadRequest("El doctor(a) seleccionado no tiene ninguna cobertura");

            //  if (especialidadeslst == null)
            //        return BadRequest(new { InvalidEspecialidad = "El doctor(a) seleccionado no tiene ninguna especialidad asignada." });

            if (!availableDaylst.Any())
                return BadRequest("No hay horario disponible para una cita con este médico 😅.");

            if (servicioslst == null)
                return BadRequest("El doctor(a) seleccionado no tiene ningún servicios asignado.");


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
                    //especialidades = especialidadeslst,
                    servicios = servicioslst,
                    diasLaborables = availableDaylst
                });
        }

        /// <summary>
        /// Devuelve una lista de citaDTO apartir de un código de verificación que se suministra.
        /// Las citas devuelta están vínculadas al código de verificación y deben de estár en estado de espera por atender.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/getCitasListByCv?codVerificacion=ESRTYY
        ///      
        /// </remarks>
        /// <param name="codVerificacion"></param>
        /// <returns>List of citaDTO</returns>
        /// <response code="204">No hay ninguna cita vículada con este código.</response>  
        /// <response code="400">Código invalido</response>  
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
                return BadRequest("Error al intentar acceder a la información:" + e.Message);
            }
        }

        /// <summary>
        /// Devuelve las citas vículadas con el usuario registrado. Estas pueden ser las citas que son para esta misma persona
        /// ó para un menor de edad y este.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/getCitasList
        ///      
        /// </remarks>
        /// <returns>List of citaDTO</returns>
        /// <response code="204">No hay ninguna cita vículada con este usuario.</response>  
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
                throw new Exception("Error al intentar acceder a la información, por favor intente más tarde." + e.StackTrace);
            }
        }

        /// <summary>
        /// Crea una cita a partir de los datos de la cita, el paciente y del usuario tutor (titular de la cuenta) siendo esta ultima opcional,
        ///y devuelve el un citaResultDTO para mostrar el ticket al usuario.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /citas/createCita
        ///      
        /// </remarks>
        /// <param name="formdata"></param>
        /// <returns>citaResultDTO</returns>
        /// <response code="400">Los datos suministrados son invalidos.</response>  
        /// <response code="401">El documento de identificación del usuario no se encuentra registrado en la Base de Datos.</response>  
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
                medicos medico = await _db.medicos.FindAsync(formdata.medicosID);
                if (medico == null)
                    return BadRequest("El doctor seleccionado no se encuentra habilitado en estos momentos");

                // if (formdata.appointment_type == (int)appointment.me && _db.citas.Where(x => x.medicos == medico && x.pacientes == paciente && x.estado == true /*&& x.especialidades == especialidad*/).Any())
                if (_db.citas.Where(x => x.medicos == medico && x.pacientes.MyIdentityUsers == user && x.estado == true).Any())
                    return BadRequest("Ya hay una cita programada con este doctor(a)");

                seguros seguro = await _db.seguros.FindAsync(formdata.segurosID);
                /* especialidades especialidad = await _db.especialidades.FindAsync(formdata.especialidadesID);
                 if (especialidad == null)
                     return BadRequest(new { InvalidEspecialidad = "La especialidad seleccionada no se encuentra en la base de datos." });*/
                servicios servicio = await _db.servicios.FindAsync(formdata.serviciosID);
                var cobertura = await _db.cobertura_medicos.FirstOrDefaultAsync(x =>
                    //  x.especialidadesID == formdata.especialidadesID &&
                    x.medicosID == formdata.medicosID &&
                    x.segurosID == formdata.segurosID &&
                    x.serviciosID == formdata.serviciosID);

                int nTurn = await getNewTurn(formdata.fecha_hora, formdata.medicosID);

                hora_reserv = await _db.horarios_medicos_reservados.FirstOrDefaultAsync(h => h.fecha_hora == formdata.fecha_hora && h.medicosID == formdata.medicosID);
                validationResponse availableTime = await ValidateAvailableTimeAsync(formdata.fecha_hora, formdata.medicosID);
                citaValidationResponse dataResponse = await ValidateAndSetDataAsync(formdata);


                paciente = _db.pacientes
                    .FirstOrDefault(p => p.MyIdentityUsers == user && !String.IsNullOrWhiteSpace(p.doc_identidad)); //this is for add another row with the same ID tutor.

                /* if (paciente == null)
                     paciente = await _db.pacientes.FirstOrDefaultAsync(p => p.MyIdentityUsers == user); //this is for addind another row with the same ID tutor.
 */
                codVer = citaExists(user) ? getCV(user) : generateCV(medico);

                //VALIDATORS

                /*  if (formdata.userinfo != null)
                  {
                      bool result = await saveUserInfoAsync(formdata.userinfo);
                      if (!result)
                          return BadRequest("Ha ocurrido un error al tratar de guardar los datos del paciente");
                  }*/

                if (String.IsNullOrWhiteSpace(user.doc_identidad))
                    return StatusCode(403, "Este usuario no cuenta con un documento de identidad previamente ingresado");

                //Determinar si la hora de la cita está disponible
                if (formdata.fecha_hora < DateTime.Now || formdata.fecha_hora > DateTime.Now.AddDays(30))
                    return BadRequest("El día suministrado no está en el rango de fecha disponible");


                if (!dataResponse.successful)
                    return BadRequest(dataResponse.errorMessage);
                else
                    formdata = dataResponse.citaCreateDTO;

                if (formdata.segurosID == null)
                    seguro = await _db.seguros.FindAsync(1); //1 by default is None-Insurace
                else if (seguro == null)
                    return BadRequest("El seguro seleccionado no se encuentra en la base de datos");

                if (servicio == null)
                    return BadRequest("El servicio seleccionado no se encuentra en la base de datos");

                if (cobertura == null)
                {
                    //sin seguro
                    cobertura = await _db.cobertura_medicos.FirstOrDefaultAsync(x =>
                    //x.especialidadesID == formdata.especialidadesID &&
                    x.medicosID == formdata.medicosID &&
                    x.serviciosID == formdata.serviciosID &&
                    x.segurosID == seguro.ID); //1 reperesenta el registro de no Seguro/Privado.

                    if (cobertura == null)
                        return BadRequest("Ha ocurrido un error al tratar de especificar el cobertura del servicio");
                }

                if (hora_reserv != null)
                    return BadRequest("La fecha y hora para la cita programada está reservada, intente con otra por favor");

                if (!availableTime.successful)
                    return BadRequest(availableTime.errorMessage);

                if (nTurn == 0)
                    return StatusCode(500, "Ha ocurrido un error al tratar de generar el turno para la cita.");

                //choosing appoiment type
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
                            _db.pacientes.Add(paciente);
                        }
                        else
                            _db.pacientes.Update(paciente);

                        break;

                    case (int)appointment.other:


                        paciente = _mapper.Map<pacientes>(formdata);

                        paciente.doc_identidad_tutor = user.doc_identidad;
                        paciente.nombre_tutor = String.IsNullOrWhiteSpace(user.nombre) ? null : user.nombre;
                        paciente.apellido_tutor = String.IsNullOrWhiteSpace(user.apellido) ? null : user.apellido;
                        paciente.MyIdentityUsers = user;
                        _db.pacientes.Add(paciente);

                        break;

                    default:
                        return BadRequest("No se ha definido al tipo de paciente que se va a consultar, especifique si es \"Yo\" ó \"Otra persona\"");
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


                var citaResult = new citaResultDTO
                {
                    cod_verificacion = cita.cod_verificacionID,
                    servicio = cita.servicios.descrip,
                    consultorio = cita.consultorio,
                    fecha_hora = cita.fecha_hora,
                    medico_nombre_apellido = (medico.nombre + " " + medico.apellido).Trim(),
                    //especialidad = especialidad.descrip,
                    seguro = seguro.descrip,
                    pago = cobertura.pago,
                    cobertura = _cobertura,
                    diferencia = _diferencia,
                    doc_identidad = paciente.doc_identidad,
                    paciente_nombre_apellido = (paciente.nombre + " " + paciente.apellido).Trim(),
                    doc_identidad_tutor = paciente.doc_identidad_tutor,
                    tutor_nombre_apellido = (paciente.nombre_tutor + " " + paciente.apellido_tutor).Trim(),
                    contacto = cita.contacto,
                    correo = user.Email,
                    turno = nTurn

                };
                try
                {
                    //throw new Exception();
                    sendTicketMail(citaResult, user.Email);
                    sendTicketWhatsapp(citaResult, user.contacto);

                }
                catch (Exception)
                {
                    throw;
                }

                //I return a ticket
                return Ok(citaResult);

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void sendTicketWhatsapp(citaResultDTO citaResult, string contacto)
        {

            String message = "";


            message = "*Fecha y hora:* " + new CultureInfo("es-ES").TextInfo.ToTitleCase(citaResult.fecha_hora.ToString("dddd, dd MMMM yyyy hh:mm tt", new CultureInfo("es-ES")))
                    + Environment.NewLine + "*Código:* " + citaResult.cod_verificacion
                    + Environment.NewLine + "Tu médico es *" + citaResult.medico_nombre_apellido + "*"
                    + Environment.NewLine + "*Consultorio:* " + citaResult.consultorio
                    + Environment.NewLine + "*Turno:* " + citaResult.turno
                    + Environment.NewLine + "*Servicio:* " + citaResult.servicio
                    + Environment.NewLine + "*Paciente:* " + citaResult.paciente_nombre_apellido;

            if (citaResult.doc_identidad_tutor != null)
                message = message
                    + Environment.NewLine + "*Doc.identidad tutor:* " + citaResult.doc_identidad_tutor
                    + Environment.NewLine + "*Tutor:* " + citaResult.tutor_nombre_apellido;
            else
                message = message
                + Environment.NewLine + "*Doc.identidad:* " + citaResult.doc_identidad;
            

            message = message
                    + Environment.NewLine + "*Número de contacto:* " + citaResult.contacto
                    + Environment.NewLine + "*Seguro:* " + citaResult.seguro
                    + Environment.NewLine + "*Diferencia:* RD$" + Decimal.Round(citaResult.diferencia, 2);

            _whatsappService.Send("14155238886", contacto, message);

        }


        private void sendTicketMail(citaResultDTO citaResult, string to)
        {

            _retryPolicy.Execute(() =>
            {
                StringBuilder message = new StringBuilder("");
                var reader = new System.IO.StreamReader("Helpers/TicketTemplateFirst.html", System.Text.Encoding.UTF8);
                var firstTicket = reader.ReadToEnd();
                reader = new System.IO.StreamReader("Helpers/TicketTemplateSecond.html", System.Text.Encoding.UTF8);
                var secondTicket = reader.ReadToEnd();
                reader.Close();

                string showDocIdent = "inherit";
                string showDocIdentTutor = "none";
                if (citaResult.doc_identidad_tutor != null)
                {
                    showDocIdent = "none";
                    showDocIdentTutor = "inherit";
                }
                message.Append(firstTicket);
                message.Append($@"
                     <h2 style=""
                        margin-bottom: 2rem;
                        font-weight: normal;
                        font-size: clamp(0.7rem, 6vw, 1.3rem);
                        padding: 1rem;
                        text-align: center;
                        padding-top: 0;
                        "" >{new CultureInfo("es-ES").TextInfo.ToTitleCase(citaResult.fecha_hora.ToString("dddd, dd MMMM yyyy hh:mm tt", new CultureInfo("es-ES")))}</h2>
                        <div class=""user_data"">
                          <div class=""flex-container"">
                            <p><b>Código:</b>  {citaResult.cod_verificacion}</p>
                            <p><b>Tu médico es</b> {citaResult.medico_nombre_apellido}</p>
                            <p><b>Consultorio: </b>{citaResult.consultorio}</p>
                            <p><b>Turno: </b>{citaResult.turno}</p>
                            <p><b>Servicio: </b>{citaResult.servicio}</p>
                            <p  style=""display: {showDocIdent};""><b>Doc. identidad: </b><span>{citaResult.doc_identidad}</span></p>
                            <p><b>Paciente: </b>{citaResult.paciente_nombre_apellido}</p>
                            <p style=""display: {showDocIdentTutor};""><b>Doc. identidad tutor: </b><span>{citaResult.doc_identidad_tutor}</span></p>
                            <p style=""display: {showDocIdentTutor};""><b>Tutor: </b><span>{citaResult.tutor_nombre_apellido}</span></p>
                            <p><b>Número de contacto: </b>{citaResult.contacto}</p>
                            <p><b>Seguro: </b>{citaResult.seguro}</p>
                            <p><b>Diferencia: </b>RD${Decimal.Round(citaResult.diferencia, 2)}</p>
                          </div>");
                message.Append(secondTicket);

                _emailService.Send(to, "Cita Médica - Hospital Salvador", message.ToString(), _configuration["AppSettings:EmailFrom"]);
            });
        }

        private async Task<int> getNewTurn(DateTime fecha_hora, int medicoID)
        {
            try
            {
                var TimeTurnDic = await getAvailableTimeTurnDicAsync(fecha_hora, medicoID);
                int nTurn = TimeTurnDic.First(x => x.Key.Equals(fecha_hora)).Value;
                return nTurn;
            }
            catch (Exception)
            {

                return 0;
            }
        }



        /// <summary>
        /// Método que devuelve el costo y destalles de la consulta de un médico.
        /// Todos los parametro deben de ser validos para poder procesar la respuesta.
        /// En caso de que se solicite los datos para los no asegurados o privados es requerido que el ID del seguro sea 1.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/getCoberturas?especialidadID=1&amp;servicioID=1&amp;medicoID=2&amp;seguroID=2
        ///      
        /// </remarks>
        /// <param name="seguroID"></param>
        /// <param name="servicioID"></param>
        /// <param name="medicoID"></param>
        /// <returns>costoServicio</returns>
        /// <response code="204">No hay cobertura disponibles con estos parametros.</response>
        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        public async Task<ActionResult<coberturaDTO>> getCoberturasAsync(int servicioID, int medicoID, int seguroID)
        {
            try
            {

                var cobertura = await _db.cobertura_medicos.FirstAsync(x =>
                       //x.especialidadesID == especialidadID &&
                       x.medicosID == medicoID &&
                       x.segurosID == seguroID &&
                       x.serviciosID == servicioID);

                //calculate costs
                decimal _porciento = (Decimal.Divide((cobertura.porciento), 100));
                decimal _cobertura = cobertura.pago * _porciento;
                decimal _diferencia = cobertura.pago - _cobertura;
                decimal _pago = cobertura.pago;

                return new coberturaDTO
                {
                    porciento = _porciento,
                    pago = _pago,
                    cobertura = _cobertura,
                    diferencia = _diferencia
                };
            }
            catch (Exception)
            {
                return NoContent();
            }
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

        /// <summary>
        /// Devuelve un diccionario datetime y init que representa los horarios disponibles de este médico , tomando como referencia 
        /// el día que se le suministra y el turno que le concierne a la fecha. Retorna null si el médico no labora ese día.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /citas/ValidateDate?fecha_hora=2021-06-21T08:00:00&amp;medicoID=1
        ///      
        /// </remarks>
        /// <param name="fecha_hora"></param>
        /// <param name="medicoID"></param>
        /// <returns>List of DateTime</returns>
        /// <response code="204">No hay horarios disponibles para hacer consultas este día.</response>  
        /// <response code="400">El médico no labora este día.</response>  
        [HttpGet("[action]")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        public async Task<ActionResult<Dictionary<DateTime, int>>> getTimeListAsync(DateTime fecha_hora, int medicoID)
        {
            //get the  appointment list schedule of this doctor
            var dateTimeDic = await getAvailableTimeTurnDicAsync(fecha_hora, medicoID);

            if (dateTimeDic == null)
                return BadRequest("Este médico no labora el día escogido: " + fecha_hora.Date.ToShortDateString());
            else if (!dateTimeDic.Any())
                return NoContent();

            return dateTimeDic;
        }

        private async Task<List<DateTime>> getAvailableTimeListAsync(DateTime fecha_hora, int medicoID)
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

        private async Task<Dictionary<DateTime, int>> getAvailableTimeTurnDicAsync(DateTime fecha_hora, int medicoID)
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
            Dictionary<DateTime, int> availableTimelst = new Dictionary<DateTime, int>();
            Dictionary<DateTime, int> b = new Dictionary<DateTime, int>();
            int nTurn = 0;

            while (startTime < endTime)
            {
                var intime = startTime.CompareTo(DateTime.Now); // compare today's date in order to not allow lower date than today 
                if ((startTime.TimeOfDay.CompareTo(startFreeTime.TimeOfDay) < 0 || startTime.TimeOfDay.CompareTo(endFreeTime.TimeOfDay) > 0) && intime > 0)
                {
                    nTurn++;
                    availableTimelst.Add(startTime, nTurn);

                }
                startTime = startTime.AddMinutes(appointmentDuration);
            }

            //let's get rid of reserved hours
            if (reservedTimelst != null)
            {
                availableTimelst = availableTimelst
                .Where(kvp => !reservedTimelst.Contains(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            return availableTimelst;
        }

        private async Task<List<DateTime>> getAvailableDayListAsync(int medicoID)
        {

            List<DateTime> holyDayslst = await _db.dias_feriados.Select(x => x.fecha).ToListAsync();
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
                    if (timelst.Except(holyDayslst).Count() > 0)
                        availableDaylst.Add(timelst.First().Date);
                }

                currentDate = currentDate.AddDays(1);
            }
            return availableDaylst.Except(holyDayslst).ToList();
        }

        /// <summary>
        /// Método que valida si la fecha y hora suministradas están disponibles en la base de datos. 
        /// </summary>
        /// <param name="fecha_hora"></param>
        /// <param name="medicoID"></param>
        /// <returns>validationResponse</returns>
        private async Task<validationResponse> ValidateAvailableTimeAsync(DateTime fecha_hora, int medicoID)
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

        private bool citaExists(MyIdentityUser user)
        {

            if (_db.citas.FirstOrDefault(x => x.pacientes.MyIdentityUsers == user && x.estado == true) != null)
            {
                return true;
            }

            return false;
        }

        private async Task<citaValidationResponse> ValidateAndSetDataAsync(citaCreateDTO formdata)
        {
            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyIdentityUser user = await _userManager.FindByNameAsync(userName);
            string _errorMessage = "";
            //string doc_identidad = formdata.doc_identidad; es algo no que necesito validar por el momento
            //string doc_identidad_tutor = formdata.doc_identidad_tutor;
            var fechaNacimiento = formdata.fecha_nacimiento;
            var appointment_type = formdata.appointment_type;
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

            if (_edad < 18 && appointment_type != 1)
                _errorMessage = "El tipo de cita escogido no es valido para un menor de edad.";
            else if (_edad >= 18 && appointment_type != 0)
                _errorMessage = "El tipo de cita escogido no es valido para un mayor de edad.";
            else if (String.IsNullOrWhiteSpace(user.nombre) && _edad < 18)
                _errorMessage = "Es requerido un nombre para el tutor del menor de edad.";


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

        public static citaValidationResponse validateBirth(DateTime _fechaNacimiento)
        {
            DateTime fechaNacimiento = _fechaNacimiento;
            int lessPermititted = DateTime.Compare(fechaNacimiento, new DateTime(1910, 1, 1));
            int graterThanToday = DateTime.Compare(fechaNacimiento, DateTime.Today);
            string _message = "";
            int _edad = DateTime.Today.AddTicks(-_fechaNacimiento.Ticks).Year - 1;

            if (_edad < 18)
                _message = "Es necesario que el usuario sea mayor de edad.";


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
    public class costoServicio
    {
        public decimal porciento { get; set; }
        public decimal cobertura { get; set; }
        public decimal diferencia { get; set; }
    }
}
