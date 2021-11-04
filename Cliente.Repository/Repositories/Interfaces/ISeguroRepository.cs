﻿using Centromedico.Database.DbModels;
using Cliente.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories.Interfaces
{
    public interface ISeguroRepository
    {
        List<seguros> getAllByDoctorId(int medicoID);
        Task<seguros> getByIdAsync(int? segurosID);
        List<seguros> getSegurosByServicio(int medicoID, int servicioID);
    }
}
