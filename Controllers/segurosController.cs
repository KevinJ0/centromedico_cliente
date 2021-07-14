using AutoMapper;
using AutoMapper.QueryableExtensions;
using HospitalSalvador.Context;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalSalvador.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class segurosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly token _token;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;

        public segurosController(RoleManager<IdentityRole> roleManager,
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

        [HttpGet("[action]")]
        public ActionResult<List<seguroDTO>> getSeguros(int medicoID)
        {
            try
            {
                List<seguroDTO> seguroslst = _db.cobertura_medicos
                    .Where(x => x.medicosID == medicoID)
                    .Select(x => x.seguros)
                    .ProjectTo<seguroDTO>(_mapper.ConfigurationProvider)
                    .ToList();

                if (!seguroslst.Any())
                    return NoContent();

                return seguroslst;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        // GET: api/<segurosController>
        [HttpGet("[action]")]
        public ActionResult<List<seguroDTO>> getSegurosByServicio(int medicoID, int servicioID)
        {
            try
            {
                List<seguroDTO> seguroslst = _db.cobertura_medicos
                    .Where(x => x.medicosID == medicoID && x.serviciosID == servicioID)
                    .Select(x=>x.seguros)
                    .ProjectTo<seguroDTO>(_mapper.ConfigurationProvider)
                    .ToList();

                if (!seguroslst.Any())
                    return NoContent();

                return seguroslst;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

    }
}
