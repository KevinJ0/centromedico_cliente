using Cliente.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces
{
    public interface ITokenService
    {
        Task<bool> RegisterAsync(RegisterDTO formdata);
        Task<IActionResult> RefreshToken(TokenRequestDTO model, bool mobile);
        Task<IActionResult> GenerateNewToken(TokenRequestDTO model, bool mobile);
    }
}
