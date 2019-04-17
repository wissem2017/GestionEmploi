using Newtonsoft.Json;
using GestionEmploi.API.Models;
using System.Collections.Generic;

namespace GestionEmploi.API.Data
{
    public class TrialData
    {
        private readonly DataContext _context;
        public TrialData(DataContext context)
        {
            _context = context;
            
        }

        //--> Méthode permet d'importer donnée de test de fichier JSON
        public void TrialUsers(){
            var userData=System.IO.File.ReadAllText("Data/UserTrialData.json");

            var users=JsonConvert.DeserializeObject<List<User>>(userData);//--> convertir de fichier Json vers fichier .NET

            foreach (var user in users)
            {
                byte[] passwordHash,passwordSalt;
                CreatePasswordHash("password",out passwordHash, out passwordSalt); //--> Création passwordHash et passwordSalt

                user.PasswordHash=passwordHash;
                user.PasswordSalt=passwordSalt;
                user.Username=user.Username.ToLower();

                _context.Add(user);//--> Ajouter dans le context
            }

            _context.SaveChanges(); //--> fait l'Enregisterement

             

        }

         //méthode permet de créer passwordHash et passwordSalt
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //créer une instence pour utiliser algorithme pour créer passwordSalt et passwordHash
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
                    passwordSalt=hmac.Key; //avoir le clé
                    passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    //création passwordHash en modifiant password en type string en type byte
            }
             
        }
    }
}