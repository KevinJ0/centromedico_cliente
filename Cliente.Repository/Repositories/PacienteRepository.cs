using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Cliente.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly MyDbContext _db;

        public PacienteRepository( MyDbContext db)
        {
            _db = db;
        }

        public void Add(pacientes entity) {

            _db.pacientes.Add(entity);
        }


        public pacientes getWithDocIdent(MyIdentityUser user)
        {
            var r = _db.pacientes
                     .FirstOrDefault(p => p.MyIdentityUsers == user && !String.IsNullOrWhiteSpace(p.doc_identidad)); //this is for add another row with the same ID tutor.
           
            return r;
        }

        public void Update(pacientes entity)
        {
            _db.pacientes.Update(entity);
        }
    }
}
