using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels ;
using CentromedicoCliente.Services.Interfaces;
using Cliente.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CentromedicoCliente.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegurosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        private readonly ISeguroService _seguroSvc;

        public SegurosController(ISeguroService seguroSvc, MyDbContext db, IMapper mapper)
        {
            _seguroSvc = seguroSvc;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public ActionResult<List<seguroDTO>> getSeguros(int medicoID)
        {
            try
            {
                List<seguroDTO> segurosDtolst =  _seguroSvc.getAllByDoctorId(medicoID);

                if (!segurosDtolst.Any())
                    return new NoContentResult();

                return segurosDtolst;
            }
            catch (Exception)
            {
                throw;
            }

             
        }


        //en desuso
        [HttpGet("[action]")]
        public ActionResult<List<seguroDTO>> getAllSeguros()
        {


            try
            {
                List<seguroDTO> seguroslst = _db.seguros
                    .ProjectTo<seguroDTO>(_mapper.ConfigurationProvider)
                    .ToList();

                if (!seguroslst.Any())
                    return NoContent();

                return seguroslst;

            }
            catch (Exception)
            {
                throw;
            }

        }


        // GET: api/<segurosController>
        [HttpGet("[action]")]
        public ActionResult<List<seguroDTO>> getSegurosByServicio(int medicoID, int servicioID)
        {

            try
            {
                List<seguroDTO> segurosDtolst = _seguroSvc.getSegurosByServicio(medicoID, servicioID);

                if (!segurosDtolst.Any())
                    return new NoContentResult();

                return segurosDtolst;
            }
            catch (Exception)
            {
                throw;
            }
             

        }

    }
}
