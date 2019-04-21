using System;

namespace GestionEmploi.API.Dtos
{
    public class UserForListDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Geder { get; set; }//genre
        public int Age { get; set; }
        public string  KnowAs { get; set; } //Nom de profile
        public DateTime Created { get; set; } //Date de création Compte
        public DateTime LastActive { get; set; }//Dernière Accssé
        public string City { get; set; }
        public string  Country { get; set; }
        public string PhotoUrl { get; set; }
       
      
    }
}