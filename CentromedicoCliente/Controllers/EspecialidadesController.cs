using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
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
    [Produces("application/json")]
    [ApiController]
    public class EspecialidadesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;

        public EspecialidadesController(
            IConfiguration configuration,
            UserManager<MyIdentityUser> userManager,
             MyDbContext db, IMapper mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
            _db = db;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public ActionResult<List<especialidadDTO>> getAllEspecialidades()
        {
            try
            {
                List<especialidadDTO> especialidadeslst = _db.especialidades
                    .ProjectTo<especialidadDTO>(_mapper.ConfigurationProvider)
                    .ToList();

                if (!especialidadeslst.Any())
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
