using Cliente.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services.Interfaces
{
    public interface ISeguroService
    {
        List<seguroDTO> getAllByDoctorId(int medicoID);
        List<seguroDTO> getSegurosByServicio(int medicoID, int servicioID);
    }
}
