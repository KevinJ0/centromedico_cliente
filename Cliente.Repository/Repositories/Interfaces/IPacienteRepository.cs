using Centromedico.Database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories.Interfaces
{
    public interface IPacienteRepository
    {
        public void Add(pacientes entity);
        public void Update(pacientes entity);
        pacientes getByDocIdent(string docIdentidad);
        pacientes getByUser(MyIdentityUser userPacienteDto);
    }
}
