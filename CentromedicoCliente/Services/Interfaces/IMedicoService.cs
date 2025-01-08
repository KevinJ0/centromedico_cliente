using Cliente.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces
{
    public interface IMedicoService
    {
        Task<medicoDTO> getFullByIdAsync(int id);
        ActionResult<List<medicoDirectorioDTO>> getAllMedical(string nombre, string especialidadID, string seguroID);
    }
}
