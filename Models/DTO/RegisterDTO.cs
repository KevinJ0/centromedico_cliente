﻿using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalSalvador.Models.DTO
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string RoleName { get; set; } //this is only for develop purpose
    }
}
