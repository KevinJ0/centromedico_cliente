using Centromedico.Database.DbModels;
using Cliente.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories.Interfaces
{
    public interface IMedicoRepository
    {
        public medicos getById(int medicoID);
        public medicos getMedicoWithExtras(int medicoID);
        public medicoDTO getMedicoDtoWithExtras(int medicoID);


    }
}
