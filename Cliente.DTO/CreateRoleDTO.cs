﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cliente.DTO
{
    public class CreateRoleDTO
    {
        [Required]
        [DisplayName("Nombre")]
        public string RoleName { get; set; }
    }
}
