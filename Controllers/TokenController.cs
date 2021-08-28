using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using HospitalSalvador.Context;
using Microsoft.AspNetCore.Identity;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using HospitalSalvador.Services;

namespace HospitalSalvador.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]

    public class TokenController : Controller
    {
        // jwt and refresh token 
        private readonly token _token;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;


        public TokenController(RoleManager<IdentityRole> roleManager,
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


        /// <summary>
        /// Recibe un TokenRequestDTO, comprueba el GrantType (password o refreshToken) y llama al método pertinente.
        /// Se devuelve un tokenResponseDTO con el token, resfresh token y las credenciales del usuario (en caso de logging).
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Auth
        ///     {
        ///        "UserCredential":"jose@gmail.com", 
        ///        "password":"12345", 
        ///        "granttype":"password" 
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <returns>TokenResponseDTO</returns>
        /// <response code="400">Si las credenciales no son validas.</response>  
        /// <response code="500">Se recibió un payload invalido.</response>  
        [HttpPost("[action]")]
        public async Task<IActionResult> Auth([FromBody] TokenRequestDTO model) // granttype = "refresh_token"
        {
            // I will return Generic 500 HTTP Server Status Error
            // If it receives an invalid payload
            if (model == null)
            {
                return new StatusCodeResult(500);
            }

            switch (model.GrantType)
            {
                case "password":
                    return await GenerateNewToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                case "password_mobile":
                    return await GenerateNewToken(model, mobile: true);
                case "refresh_token_mobile":
                    return await RefreshToken(model, mobile: true);
                default:
                    return new BadRequestResult();
            }

        }

        /// <summary>
        /// Método que crea un New JWT y refresca el token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Auth
        ///     {
        ///        "UserCredential":"jose@gmail.com", 
        ///        "password":"12345", 
        ///        "granttype":"password" 
        ///     }
        ///
        /// </remarks>
        /// <param name="model"></param>
        /// <param name="mobile"></param>
        /// <response code="200">Operación exitosa, devuelve un TokenResponseDTO con el Token y refresh token incluido.</response>
        /// <response code="400">Si las credenciales no son validas.</response>  
        private async Task<IActionResult> GenerateNewToken(TokenRequestDTO model, bool mobile = false)
        {
            // check if there's an user with the given username
            var user = await _userManager.FindByNameAsync(model.UserCredential) ?? await _userManager.FindByEmailAsync(model.UserCredential);

            // Validate credentials
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {

                // username & password matches: create the refresh token
                var newRtoken = CreateRefreshToken(_configuration["Authorization:ClientId"], user.Id, mobile);

                // first we delete any existing old refreshtoken
                var oldrtoken = _db.token.Where(rt => rt.UserId == user.Id);

                if (oldrtoken != null)
                {
                    foreach (var oldrt in oldrtoken)
                    {
                        _db.token.Remove(oldrt);
                    }
                }

                // Add new refresh token to Database
                _db.token.Add(newRtoken);

                await _db.SaveChangesAsync();

                // Create & Return the access token which contains JWT and Refresh Token

                var accessToken = await CreateAccessToken(user, newRtoken.Value);

                return Ok(new { authToken = accessToken });
            }

            return BadRequest("El usuario o ontraseña son invalidos, por favor verifique sus credeniales.");
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
        /// <response code="400">Los datos suministrados son invalidos. Devuelve un string con un mesaje sobre el error producido.</response>
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO formdata)
        {
            // Will hold all the errors related to registration
            string _error = "";
            IdentityRole identityRole;
            var user = _mapper.Map<MyIdentityUser>(formdata);

            user.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.CreateAsync(user, formdata.Password);

            if (result.Succeeded)
            {

                // Get user Role
                identityRole = new IdentityRole { Name = "Patient" };
                await _roleManager.CreateAsync(identityRole);
                await _userManager.AddToRoleAsync(user, "Patient");

                //Now login in
               return await GenerateNewToken(new TokenRequestDTO
                {
                    UserCredential = formdata.Email,
                    Password = formdata.Password
                });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _error = IdentityErrorService.getDescription(error.Code);

                    break;
                }
            }

            return BadRequest(_error);

        }

        // Create access Token
        private async Task<TokenResponseDTO> CreateAccessToken(MyIdentityUser user, string refreshToken)
        {

            double tokenExpiryTime = Convert.ToDouble(_configuration["Authorization:ExpireTime"]);

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authorization:LlaveSecreta"]));

            // get user Role
            /* IdentityRole identityRole = new IdentityRole { Name = "Admin" };
             await _roleManager.CreateAsync(identityRole);
             var role = _roleManager.Roles.FirstOrDefault(x => x.Name == "Admin");
             await _userManager.AddToRoleAsync(user, role.Name);*/

            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
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

            // Generate token

            var newtoken = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(newtoken);

            return new TokenResponseDTO()
            {
                token = encodedToken,
                expiration = newtoken.ValidTo,
                refresh_token = refreshToken,
                roles = roles.FirstOrDefault(),
                username = user.UserName
            };
        }


        private token CreateRefreshToken(string clientId, string userId, bool mobile = false)
        {
            if (mobile)
            {
                return new token()
                {
                    ClientId = clientId,
                    UserId = userId,
                    Value = Guid.NewGuid().ToString("N"),
                    CreatedDate = DateTime.UtcNow,
                    ExpiryTime = DateTime.UtcNow.AddYears(1)
                };
            }

            return new token()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddHours(1)
            };
        }



        // Method to Refresh JWT and Refresh Token
        private async Task<IActionResult> RefreshToken(TokenRequestDTO model, bool mobile = false, bool CodVerificacionUser = false)
        {
            try
            {
                // check if the received refreshToken exists for the given clientId
                var rt = _db.token
                    .FirstOrDefault(t =>
                    t.ClientId == _configuration["Authorization:ClientId"]
                    && t.Value == model.RefreshToken.ToString());

                if (rt == null)
                {
                    // refresh token not found or invalid (or invalid clientId)
                    return new UnauthorizedResult();
                }

                // check if refresh token is expired
                if (rt.ExpiryTime < DateTime.UtcNow)
                {
                    return new UnauthorizedResult();
                }

                // check if there's an user with the refresh token's userId
                var user = await _userManager.FindByIdAsync(rt.UserId);


                if (user == null)
                {
                    // UserId not found or invalid
                    return new UnauthorizedResult();
                }

                // generate a new refresh token 

                var rtNew = CreateRefreshToken(rt.ClientId, rt.UserId, mobile);

                // invalidate the old refresh token (by deleting it)
                _db.token.Remove(rt);

                // add the new refresh token
                _db.token.Add(rtNew);

                // persist changes in the DB
                _db.SaveChanges();


                var response = await CreateAccessToken(user, rtNew.Value);

                return Ok(new { authToken = response });

            }
#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {

                return new UnauthorizedResult();
            }
        }



        /// <summary>
        /// Método que cierra todas las sessiones de un usuario en diferentes dispositivos.
        /// </summary>
        /// <remarks>
        /// ¡Aun no implementado!
        /// </remarks>
        /// <param name="model"></param>
        /// <response code="204">La operación se completó y no hay refresh token registrado a este usuario.</response>
        /// <response code="401">Refresh token not found or invalid (or invalid clientId)</response>  
        /// <response code="401">UserId not found or invalid</response>  
        /// <response code="401">Token refresh is expired</response>  
        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke(TokenRequestDTO model)
        {
            try
            {
                // check if the received refreshToken exists for the given clientId
                var rt = _db.token
                    .FirstOrDefault(t =>
                    t.ClientId == _configuration["Authorization:ClientId"]
                    && t.Value == model.RefreshToken.ToString());


                if (rt == null)
                {
                    // refresh token not found or invalid (or invalid clientId)
                    return new UnauthorizedResult();
                }

                // check if refresh token is expired
                if (rt.ExpiryTime < DateTime.UtcNow)
                {
                    return new UnauthorizedResult();
                }

                // check if there's an user with the refresh token's userId
                var user = _db.MyIdentityUsers.SingleOrDefault(u => u.UserName == rt.User.UserName);

                if (user == null)
                {
                    // UserId not found or invalid
                    return new UnauthorizedResult();
                }

                rt.Value = null;
                _db.SaveChanges();

            }

#pragma warning disable CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            catch (Exception ex)
#pragma warning restore CS0168 // La variable 'ex' se ha declarado pero nunca se usa
            {
                return BadRequest();
            }

            return NoContent();

        }
    }
}