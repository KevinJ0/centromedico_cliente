using AutoMapper;
using AutoMapper.QueryableExtensions;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using CentromedicoCliente.Services.Interfaces;
using Cliente.DTO;
using Cliente.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services
{
    public class SeguroService : ISeguroService
    {


        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        private readonly ISeguroRepository _seguroRepo;

        public SeguroService(ISeguroRepository seguroRepo, MyDbContext db, IMapper mapper)
        {
            _mapper = mapper;
            _db = db;
            _seguroRepo = seguroRepo;
        }


        public List<seguroDTO> getAllByDoctorId(int medicoID)
        {
            try
            {

                List<seguros> seguroslst = _seguroRepo.getAllByDoctorId(medicoID);
                List<seguroDTO> segurosDtolst = _mapper.Map<List<seguroDTO>>(seguroslst);

                return segurosDtolst;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<seguroDTO> getSegurosByServicio(int medicoID, int servicioID)
        {
            try
            {
                List<seguros> seguroslst = _seguroRepo.getSegurosByServicio(medicoID, servicioID);

                List<seguroDTO> segurosDtolst = _mapper.Map < List<seguroDTO>>(seguroslst);

                return segurosDtolst;

            }
            catch (Exception )
            {
                throw ;
            }
        }

    }
}
