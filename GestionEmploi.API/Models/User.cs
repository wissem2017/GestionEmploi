using System;
using System.Collections.Generic;

namespace GestionEmploi.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Geder { get; set; }//genre
        public DateTime DateOfBirth { get; set; }
        public string  KnowAs { get; set; } //Nom de profile
        public DateTime Created { get; set; } //Date de création Compte
        public DateTime LastActive { get; set; }//Dernière Accssé
        public string Intoduction { get; set; } //Présentation de chercheur
        public string LookingFor { get; set; } //Poste chercher
        public string interests { get; set; }
        public string City { get; set; }
        public string  Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<FileUser> FilesUser { get; set; }
        public ICollection<Like> Likers { get; set; }
        public ICollection<Like> Likees { get; set; }
        public ICollection<Message> MessageSent { get; set; }
         public ICollection<Message> MessageReceived { get; set; }




    }
}