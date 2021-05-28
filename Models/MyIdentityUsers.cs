using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalSalvador.Models
{
    [Index(nameof(UserName), Name = "Unique_username", IsUnique = true)]
    public class MyIdentityUser : IdentityUser
    {
        [InverseProperty("MyIdentityUsers")]
        public virtual ICollection<secretarias> secretarias { get; set; }
        [InverseProperty("MyIdentityUsers")]
        public virtual ICollection<medicos> medicos { get; set; }
        [InverseProperty("MyIdentityUsers")]
        public virtual ICollection<clientes> clientes { get; set; }
        [InverseProperty("MyIdentityUsers")]
        public virtual ICollection<pacientes> pacientes { get; set; }

        public string Notes { get; set; }
        public int Type { get; set; }
        /*[Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
   */
        public string DisplayName { get; set; }

        public virtual List<token> tokens { get; set; }
    }

}
