using System.ComponentModel.DataAnnotations;
using System;

namespace GestionEmploi.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="Mot de passe minimum 4 charactères et maximum 8")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnowAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created=DateTime.Now;
            LastActive=DateTime.Now;

        }

    }
}