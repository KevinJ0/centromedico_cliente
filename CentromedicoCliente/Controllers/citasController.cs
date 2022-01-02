
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Cliente.DTO;
using CentromedicoCliente.Services.Interfaces;


namespace CentromedicoCliente.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {

        private readonly ICitaService _citaSvc;

        public CitasController(ICitaService citaSvc)
        {
            _citaSvc = citaSvc;

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
        public async Task<ActionResult<Object>> getCitaFormAsync(int medicoID)
        {
            try
            {

                var result = await _citaSvc.getFormCitaAsync(medicoID);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }

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
        public ActionResult<List<citaDTO>> getCitasListByCv(string codVerificacion)
        {
            try
            {
                var result = _citaSvc.getCitasListByCv(codVerificacion);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Devuelve las citas vículadas con el usuario registrado que se encuentran activas. Estas pueden ser las citas que son para esta misma persona
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

            try
            {
                var result = await _citaSvc.getCitasListAsync();
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
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
            try
            {

                citaResultDTO resulta = await _citaSvc.createCitaAsync(formdata);
                return resulta;
            }
            catch (Exception)
            {
                throw;

            }
        }

        enum appointment : int
        {
            me = 0,
            other = 1,
        }
    }

}
