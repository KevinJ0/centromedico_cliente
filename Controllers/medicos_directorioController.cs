using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalSalvador.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class medicos_directorioController : ControllerBase
    {
        // GET: api/<medicos_directorioController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<medicos_directorioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<medicos_directorioController>
        [HttpPost]
        public string get_medicos()
        {
            return "hola";

        }

        // PUT api/<medicos_directorioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<medicos_directorioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
