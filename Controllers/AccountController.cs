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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
        private readonly MyDbContext _db;
        private readonly IMapper _mapper;


        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<MyIdentityUser> userManager,
      SignInManager<MyIdentityUser> signManager, MyDbContext context, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _db = context;
            _mapper = mapper;

        }

        /// <summary>
        /// Método que almacena los datos personales del usuario/paciente en la base de datos, que será más tarde utilizados para futuras acciones.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Account/setUserInfo
        ///      {
        ///         nombre = "Kevin",
        ///         email = "Rosario",
        ///         contacto = "8095509090",
        ///         doc_identidad = "402999413213",
        ///         sexo = "f" | "m",
        ///         fecha_nacimiento = "1/1/1998"
        ///      }
        /// </remarks>
        /// <param name="formuser"></param>
        /// <returns>ActionResult</returns>
        /// <response code="400">La fecha suministrada no es válida.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpPost("[action]")]
        public async Task<ActionResult> setUserInfoAsync(UserInfo formuser)
        {
            try
            {

                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);

                //si no ha sido confirmado por el auxiliar médico
                if (!user.confirm_doc_identidad)
                {

                    user.nombre = formuser.nombre;
                    user.apellido = formuser.apellido;
                    user.sexo = formuser.sexo;
                    user.contacto = formuser.contacto;
                    user.doc_identidad = formuser.doc_identidad;
                    user.fecha_nacimiento = formuser.fecha_nacimiento;
                    var dataResponse = Controllers.citasController.validateBirth(formuser.fecha_nacimiento);

                    if (!dataResponse.successful)
                    {
                        return BadRequest("La fecha de nacimiento no es valida, debe ser mayor de edad.");
                    }

                }
                else
                {
                    user.contacto = formuser.contacto;
                }

                _db.SaveChanges();

                return Ok();
            }
            catch (Exception)
            {
                throw;
            }

        }

       

        /// <summary>
        /// Método que devuelve los datos personales del usuario/paciente.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /Account/getUserInfo
        ///      {
        ///         nombre = "Pedro",
        ///         email = "Roland",
        ///         contacto = "8095559988",
        ///         doc_identidad = "RD2288354523",
        ///         sexo = "m",
        ///         fecha_nacimiento = "1/1/1998",
        ///         confirm_doc_identidad = "false"
        ///      }
        /// </remarks>
        /// <returns>UserInfo</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpGet("[action]")]
        public async Task<UserInfo> getUserInfoAsync()
        {

            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyIdentityUser user = await _userManager.FindByNameAsync(userName);

            UserInfo userInfo = _mapper.Map<UserInfo>(user);
            return userInfo;
        }


        /// <summary>
        /// Método que devulve true or false en caso de que el documento de identidad ya haya sido verificado por el personal médico de manera física.
        /// </summary>
        /// <returns>bool</returns>
        /// <response code="500">Ha ocurrido un error al tratar de hacer la solicitud.</response>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Patient")]
        [HttpGet("[action]")]
        public async Task<bool> isUserDocIdentConfirmAsync()
        {
           
            
            try
            {
                string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                MyIdentityUser user = await _userManager.FindByNameAsync(userName);
                return user.confirm_doc_identidad ? true : false;
            }
            catch (Exception)
            {

                throw new Exception("Ha ocurrido un error al tratar de hacer la solicitud.");
            }

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
