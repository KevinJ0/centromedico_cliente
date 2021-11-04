using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CentromedicoCliente.Services.Interfaces;
using Centromedico.Database.DbModels;
using Cliente.DTO;
using Centromedico.Database.Context;

namespace CentromedicoCliente.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicosController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMedicoService _medicoSvc;


        public MedicosController(IMedicoService medicoSvc, 
            IConfiguration configuration,
             MyDbContext db, IMapper mapper)
        {
            _configuration = configuration;
            _db = db;
            _medicoSvc = medicoSvc;
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

                var r = await _medicoSvc.getById(Id);
                return r;

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
        public ActionResult<List<medicoDirectorioDTO>> filterMedicos([FromQuery] string nombre = "", string especialidadID = "", string seguroID = "")
        {
            try
            {
                var medicoslst = _medicoSvc.getAllMedical(nombre,especialidadID,seguroID);
                return medicoslst;
            }
            catch (Exception)
            {
                throw;
            }
        }
       



    }


}
