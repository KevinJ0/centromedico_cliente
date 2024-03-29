﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Cliente.DTO
{
    public class TokenRequestDTO
    {
        public string GrantType { get; set; } // password or refresh_token
        public string ClientId { get; set; }
       
        public string UserCredential { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
    }
}
