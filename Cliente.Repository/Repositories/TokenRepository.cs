using AutoMapper;
using Centromedico.Database.Context;
using Centromedico.Database.DbModels;
using Cliente.DTO;
using Cliente.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.Repository.Repositories
{
    public class TokenRepository : ITokenRepository
    {

        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;

        public TokenRepository(MyDbContext db, IMapper mapper, RoleManager<IdentityRole> roleManager,
            IHttpContextAccessor httpContextAccessor,
          UserManager<MyIdentityUser> userManager)
        {
            _db = db;
            _mapper = mapper;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }


        public async Task<IdentityResult> Add(RegisterDTO formdata)
        {


            IdentityRole identityRole;
            var user = _mapper.Map<MyIdentityUser>(formdata);
            user.SecurityStamp = Guid.NewGuid().ToString();

            var r = await _userManager.CreateAsync(user, formdata.Password);

            if (r.Succeeded)
            {
                // set user role
                identityRole = new IdentityRole { Name = "Secretary" }; // en caso de ser la primera vez
                await _roleManager.CreateAsync(identityRole);
                await _userManager.AddToRoleAsync(user, identityRole.Name);

            }
            return r;
        }

        public void Add(token newRtoken)
        {
            try
            {
                _db.token.Add(newRtoken);
            }
            catch (Exception)
            {
                throw;
            }


        }

        public IQueryable getAllByUserId(string id)
        {
            try
            {
                return _db.token.Where(rt => rt.UserId == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Remove(token oldrt)
        {

            try
            {
                _db.token.Remove(oldrt);
            }
            catch (Exception)
            {
                throw;
            }


        }
    }
}
