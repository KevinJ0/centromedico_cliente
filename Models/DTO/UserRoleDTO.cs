using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalSalvador.Models.DTO
{
    public class UserRoleDTO
    {
        public string UserId { get; set; }
        [DisplayName("Nombre")]
        [Required]
        public string UserName { get; set; }
        public bool IsSelected { get; set; }
    }
}

 