﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Cliente.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cliente.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Cliente.Repository.Repositories
{
    public class CitaRepository : ICitaRepository
    {

        private readonly IMapper _mapper;
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly MyDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CitaRepository(IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager,
            IConfiguration configuration, UserManager<MyIdentityUser> userManager, MyDbContext db, IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }



        public List<citaDTO> getCitasListByCv(string codVerificacion)
        {

            List<citaDTO> citaslst = _db.citas
                .Where(p => p.cod_verificacionID == codVerificacion && p.estado == true)
                .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToList();

            return citaslst;

        }

        public async Task<List<citaDTO>> getCitasListByUserAsync()
        {

            try
            {
                MyIdentityUser user = await _userManager
                    .FindByNameAsync(_httpContextAccessor.HttpContext.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value);

                List<citaDTO> citaslst = _db.citas.Include(x => x.medicos)
                    .Where(p => p.pacientes.MyIdentityUsers == user && p.estado == true)
                    
                    .ProjectTo<citaDTO>(_mapper.ConfigurationProvider).ToList();

                return citaslst;
            }
            catch (Exception e)
            {
                throw new Exception("Error al intentar acceder a la información, por favor intente más tarde." + e.StackTrace);
            }
        }


        public string getCV(string docIdentidad)
        {
            cod_verificacion codV = _db.cod_verificacion
                .Include("citas")
                .Where(x => x.citas.pacientes.MyIdentityUsers.doc_identidad == docIdentidad 
                            && x.citas.estado == true)
                .FirstOrDefault();

            return codV?.value;
        }

        public void Add(citas entity)
        {
            _db.citas.Add(entity);
        }
        public bool ExistByUser(medicos medico, MyIdentityUser user)
        {
            try
            {
                citas result = _db.citas.FirstOrDefault(x => x.medicos == medico
                  && x.pacientes.MyIdentityUsers == user && x.estado == true);

                if (result != null)
                    return true;

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool ExistByDocIdentidadAndMedico(medicos medico, string docIdentidad)
        {
            try
            {
                citas result = _db.citas.FirstOrDefault(x => x.medicos == medico
                  && x.pacientes.doc_identidad == docIdentidad && x.estado == true && x.pacientes.confirm_doc_identidad);

                if (result != null)
                    return true;

                return false;

            }
            catch (Exception)
            {

                throw;
            }
        }



        public bool ExistByDocIdentidad(string docIdentidad)
        {

            try
            {
                if (_db.citas.FirstOrDefault(x => x.pacientes.doc_identidad == docIdentidad 
                                                  && x.estado == true) != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
