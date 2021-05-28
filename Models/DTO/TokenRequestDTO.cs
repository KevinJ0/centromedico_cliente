using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalSalvador.Models.DTO
{
    public class TokenRequestDTO
    {
        public string GrantType { get; set; } // password or refresh_token
        public string ClientId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string Password { get; set; }
    }
}
