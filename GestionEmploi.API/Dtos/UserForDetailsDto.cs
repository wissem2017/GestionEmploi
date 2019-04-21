using System;
using System.Collections.Generic;
using GestionEmploi.API.Models;

namespace GestionEmploi.API.Dtos
{
    public class UserForDetailsDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Geder { get; set; }//genre
        public int Age { get; set; }
        public string  KnowAs { get; set; } //Nom de profile
        public DateTime Created { get; set; } //Date de création Compte
        public DateTime LastActive { get; set; }//Dernière Accssé
        public string Intoduction { get; set; } //Présentation de chercheur
        public string LookingFor { get; set; } //Poste chercher
        public string City { get; set; }
        public string  Country { get; set; }
       public string PhotoUrl { get; set; }
       
        public ICollection<PhotoForDetailsDto> Photos { get; set; }
      
      

    }
}