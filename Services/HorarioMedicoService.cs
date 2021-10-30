using Centromedico.Database.Context;
using CentromedicoCliente.Exceptions;
using CentromedicoCliente.Services.Interfaces;
using Cliente.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentromedicoCliente.Services
{
    public class HorarioMedicoService : IHorarioMedicoService
    {
        private readonly IHorarioMedicoRepository _horarioMedicoRepo;

        private readonly MyDbContext _db;
        public HorarioMedicoService(IHorarioMedicoRepository horarioMedicoRepo, MyDbContext db)
        {
            _db = db;
            _horarioMedicoRepo = horarioMedicoRepo;

        }


        //Lista de horas disponibles
        public Dictionary<DateTime, int> getHoursList(DateTime fecha_hora, int medicoID)
        {
            //get the  appointment list schedule of this doctor
            var dateTimeDic = _horarioMedicoRepo.getAvailableHoursTurnDic(fecha_hora, medicoID);

            if (dateTimeDic == null)
                throw new EntityNotFoundException("Este médico no labora el día escogido: " + fecha_hora.Date.ToShortDateString());
            else if (!dateTimeDic.Any())
                throw new BadRequestException("El médico no cuenta con días hábiles en la fecha escogida: " + fecha_hora.Date.ToShortDateString());

            return dateTimeDic;
        }


    }
}
