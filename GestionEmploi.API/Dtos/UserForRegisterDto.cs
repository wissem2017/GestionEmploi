using System.ComponentModel.DataAnnotations;

namespace GestionEmploi.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="Mot de passe minimum 4 charactères et maximum 8")]
        public string Password { get; set; }
    }
}