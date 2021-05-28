using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO formdata) 
        {
            // Will hold all the errors related to registration
            List<string> errorList = new List<string>();

            var user = _mapper.Map<MyIdentityUser>(formdata);
            user.SecurityStamp = Guid.NewGuid().ToString();


            var result = await _userManager.CreateAsync(user, formdata.Password);

            if (result.Succeeded)
            {

                // get user Role
               IdentityRole identityRole = new IdentityRole { Name = formdata.RoleName };
                await _roleManager.CreateAsync(identityRole);
                await _userManager.AddToRoleAsync(user, formdata.RoleName);
                 
                return Ok(new {username = user.UserName, 
                    email = user.Email, status = 1, message = "Registration Successful" });

            }
            else 
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }

            return BadRequest(new JsonResult(errorList));

        }


        // Login Method
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginDTO formdata) 
        {
            // Get the User from Database
            var user = await _userManager.FindByEmailAsync(formdata.Email);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authorization:LlaveSecreta"]));

            double tokenExpiryTime = Convert.ToDouble(_configuration["Authorization:ExpireTime"]);

            if (user != null &&  await _userManager.CheckPasswordAsync(user, formdata.Password))
            {
        
               
                // get user Role
                /*IdentityRole identityRole = new IdentityRole { Name = "Admin" };
                await _roleManager.CreateAsync(identityRole);
                var role = _roleManager.Roles.FirstOrDefault(x => x.Name == "Admin");
                await _userManager.AddToRoleAsync(user, role.Name);
                */

                var roles = await _userManager.GetRolesAsync(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, formdata.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                        new Claim("LoggedOn", DateTime.Now.ToString()),

                     }),

                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Authorization:Issuer"],
                    Audience = _configuration["Authorization:Audience"],
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime)
                };

                // Generate Token

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new {token = tokenHandler.WriteToken(token), expiration = token.ValidTo, username = user.UserName, userRole = roles.FirstOrDefault() });

            }

            // return error
            ModelState.AddModelError("", "Username/Password was not Found");
            return Unauthorized(new { LoginError = "Please Check the Login Credentials - Ivalid Username/Password was entered" });

        }

        



    }
}
