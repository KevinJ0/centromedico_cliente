using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using HospitalSalvador.Context;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
using HospitalSalvador.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace HospitalSalvador
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
    public class analisis_categoriaController : Controller
    {
        private readonly MyDbContext db;

        public analisis_categoriaController(MyDbContext context)
        {
            db = context;
        }



        // GET: api/analisis_categoria
        [HttpGet]
        public IEnumerable<analisisCategoriaDto> analisis_categoria()
        {
            List<analisisCategoriaDto> lst = (from d in db.analisis_categoria
                                              orderby d.ID descending
                                              select new analisisCategoriaDto
                                              {
                                                  ID = d.ID,
                                                  descrip = d.descrip,
                                                  estados = d.estados
                                              }).ToList();
            return lst;
        }

        // GET: api/analisis_categoria/5
        [HttpGet("{id}")]
        public async Task<ActionResult<analisis_categoria>> Getanalisis_categoria(int id)
        {
            var analisis_categoria = await db.analisis_categoria.FindAsync(id);

            if (analisis_categoria == null)
            {
                return NotFound();
            }

            return analisis_categoria;
        }



        // POST: api/analisis_categoria
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("[action]")]
        public analisisCategoriaDto Add([FromBody] analisisCategoriaDto model)
        {
            analisis_categoria data = new analisis_categoria();
            analisisCategoriaDto viewModel = new analisisCategoriaDto();
            MyResponse or = new MyResponse();
            try
            {
                data.descrip = model.descrip;
                data.estados = true;

                db.analisis_categoria.Add(data);
                db.SaveChanges();
                or.Success = 1;

                data = db.analisis_categoria.Last<analisis_categoria>();
            }
            catch (Exception err)
            {
                or.Success = 0;
                or.Message = err.Message;
            }

            viewModel.ID = data.ID;
            viewModel.descrip = data.descrip;
            viewModel.estados = data.estados;
            return viewModel;
        }



        // DELETE: api/analisis_categoria/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var analisis_categoria = await db.analisis_categoria.FindAsync(id);
            if (analisis_categoria == null)
            {
                return NotFound();
            }

            db.analisis_categoria.Remove(analisis_categoria);
            await db.SaveChangesAsync();

            return NoContent();
        }

        private bool analisis_categoriaExists(int id)
        {
            return db.analisis_categoria.Any(e => e.ID == id);
        }
    }
}
