using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
using HospitalSalvador.Context;
using Microsoft.Extensions.Configuration;
using AutoMapper;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalSalvador.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {

        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<MyIdentityUser> _signManager;
        private readonly IConfiguration _configuration;
        private readonly MyDbContext db;
        private readonly IMapper _mapper;


        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<MyIdentityUser> userManager,
      SignInManager<MyIdentityUser> signManager, MyDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _configuration = configuration;
            db = context;
            _mapper = mapper;

        }

        /// <summary>
        /// Método que crea un usuario con el rol de Client por defecto y
        /// recibe por parametro el nombre de un rol que deseas agregar al usuario.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     POST /Account/Register
        ///      {
        ///         username = UserName,
        ///         email = Email,
        ///         status = 1,
        ///         message = "Registration Successful"
        ///      }
        /// </remarks>
        /// <param name="formdata"></param>
        /// <returns>citaResultDTO</returns>
        /// <response code="400">Los datos suministrados son invalidos.</response>
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO formdata)
        {
            // Will hold all the errors related to registration
            List<string> errorList = new List<string>();
            IdentityRole identityRole;
            var user = _mapper.Map<MyIdentityUser>(formdata);
            if (user.Email == null)
            {
                user.Email = formdata.UserCredential + "@hospital.com";
                identityRole = new IdentityRole { Name = "Client" };
                await _roleManager.CreateAsync(identityRole);
            }

            user.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.CreateAsync(user, formdata.Password);

            if (result.Succeeded)
            {

                // get user Role
               identityRole = new IdentityRole { Name = formdata.RoleName };
                await _roleManager.CreateAsync(identityRole);
                await _userManager.AddToRoleAsync(user, formdata.RoleName);
                await _userManager.AddToRoleAsync(user, "Client");

                return Ok(new
                {
                    username = user.UserName,
                    email = user.Email,
                    status = 1,
                    message = "Registration Successful"
                });

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    //ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }

            return BadRequest(new JsonResult(errorList));

        }

    }
}
