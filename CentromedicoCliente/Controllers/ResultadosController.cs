using Amazon.S3;
using Centromedico.Database.Context;
using Amazon.S3.Model;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.DbModels;
using Cliente.DTO;
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

namespace CentromedicoCliente.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Client")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {

        private readonly MyDbContext _db;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IAmazonS3 _amazons3;

        public ResultadosController(IAmazonS3 amazonS3, IMapper mapper, UserManager<MyIdentityUser> userManager, MyDbContext db)
        {
            _amazons3 = amazonS3;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }


        /// <summary>
        /// Método que devuelve una lista de los resultados completados con los datos necesarios para ser mostrados.
        /// </summary>
        /// <returns>List of pruebaDTO.</returns>
        /// <response code="200">Existen resultados para este cliente.</response>  
        /// <response code="204">No existe ninguna resultado completado en la nube.</response>     
        [HttpGet]
        public async Task<ActionResult<List<pruebaDTO>>> GetAsync()
        {
            string userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MyIdentityUser user = await _userManager.FindByNameAsync(userName);

            List<pruebaDTO> pruebalst = await _db.pruebas
                .Where(x => x.estado == true && x.resultados.clientes.MyIdentityUserID == user.Id && x.resultados.estado == true)
                .ProjectTo<pruebaDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (!pruebalst.Any())
                return NoContent();
            else
                return pruebalst;
        }

        /// <summary>
        /// Método que sube un archivo a el servicio en la nube.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /resultados/upload
        ///     {
        ///        "file":"documento.pdf"
        ///     }
        ///
        /// </remarks>
        /// <param name="file"></param>
        /// <returns>PutObjectResponse</returns>
        /// <response code="200">El archivo se subió con exito a la nube.</response>  
        /// <response code="500">No se pudo subir el archivo a la nube.</response>  
        [HttpPost("[action]")]
        public async Task<ActionResult> UploadAsync([FromForm] IFormFile file)
        {
            string BucketName = "centromedico-assets";

            try
            {
                /*  var transferUtility = new TransferUtility(_amazons3);

                  var request = new TransferUtilityUploadRequest()
                  {
                      BucketName = "CentromedicoCliente",
                      Key = file.FileName,
                      InputStream = file.OpenReadStream(),
                      ContentType = file.ContentType,
                  };
                  // await transferUtility.UploadAsync(request);*/

                var putRequest = new PutObjectRequest()
                {
                    BucketName = BucketName,
                    Key = file.FileName,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead,

                };

                // Create a CopyObject request
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
                {
                    BucketName = "centromedico-assets",
                    Key = file.FileName,
                };
                  
                // Get path for request
                var result = await _amazons3.PutObjectAsync(putRequest);
                var url  = "https://"+ BucketName+".s3."+ _amazons3.Config.RegionEndpoint.SystemName + ".amazonaws.com/" + file.FileName;
                return Ok(url);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                ||
                amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }

                throw amazonS3Exception;
            }

        }

        /// <summary>
        /// Método que devuelve los archivos solicitados en la nube por parte del cliente.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /resultados/download
        ///     {
        ///        "pruebaUrl":"prueba_ayuna"
        ///     }
        ///
        /// </remarks>
        /// <param name="pruebaUrl"></param>
        /// <returns>Stream</returns>
        /// <response code="500">El nombre del documento solicitado no es válido.</response>  
        [HttpGet("[action]")]
        public async Task<Stream> DownloadAsync([FromQuery] string pruebaUrl)
        {
            try
            {
                var request = new GetObjectRequest()
                {
                    BucketName = "CentromedicoCliente",
                    Key = pruebaUrl
                };

                //await response.WriteResponseStreamToFileAsync("./", false, CancellationToken.None);

                GetObjectResponse response = await _amazons3.GetObjectAsync(request);
                return response.ResponseStream;

            }
            catch (Exception e)
            {
                throw new Exception("Ha ocurrido un problema al intentar descargar el documento, verifique el nombre clave del archivo. Error message: " + e.Message);
            }

        }

    }
}
