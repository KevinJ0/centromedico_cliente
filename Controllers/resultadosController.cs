using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HospitalSalvador.Context;
using HospitalSalvador.Models;
using HospitalSalvador.Models.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HospitalSalvador.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Client")]
    [ApiController]
    public class resultadosController : ControllerBase
    {

        private readonly MyDbContext _db;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _amazons3;

        public resultadosController(IAmazonS3 amazonS3, IMapper mapper, UserManager<MyIdentityUser> userManager, MyDbContext db)
        {
            _amazons3 = amazonS3;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }


        // GET: api/<resultadosController>
        [HttpGet]
        public async Task<ActionResult<List<pruebaDTO>>> GetAsync()
        {
            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyIdentityUser user = await _userManager.FindByNameAsync(userName);

            List<pruebaDTO> pruebalst = await _db.pruebas
                .Where(x => x.estado == true && x.resultados.clientes.MyIdentityUserID == user.Id && x.resultados.estado == true)
                .ProjectTo<pruebaDTO>(_mapper.ConfigurationProvider).ToListAsync();

            if (!pruebalst.Any())
                return NoContent();
            else
                return pruebalst;
        }



        // POST api/<resultadosController>
        [HttpPost("[action]")]
        public async Task<ActionResult> UploadAsync([FromForm] IFormFile file)
        {

            try
            {
              /*  var transferUtility = new TransferUtility(_amazons3);

                var request = new TransferUtilityUploadRequest()
                {
                    BucketName = "hospitalsalvador",
                    Key = file.FileName,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType,
                };
                // await transferUtility.UploadAsync(request);*/

                var putRequest = new PutObjectRequest()
                {
                    BucketName = "hospitalsalvador",
                    Key = file.FileName,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType
                };

                var result = await _amazons3.PutObjectAsync(putRequest);
                return Ok(result);
            }
            catch (AmazonS3Exception s3Exception)
            {
                throw new Exception(s3Exception.Message);
            }

        }

        [HttpGet("[action]")]
        public async Task<Stream> DownloadAsync([FromQuery] string pruebaUrl)
        {

            var request = new GetObjectRequest()
            {
                BucketName = "hospitalsalvador",
                Key = pruebaUrl
            };

            //await response.WriteResponseStreamToFileAsync("./", false, CancellationToken.None);

            GetObjectResponse response = await _amazons3.GetObjectAsync(request);
            return response.ResponseStream;


        }

        // DELETE api/<resultadosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
