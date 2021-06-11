using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalSalvador.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserCredential { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string RoleName { get; set; } //this is only for development purposes
    }
}
