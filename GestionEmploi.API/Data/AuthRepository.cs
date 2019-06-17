using System;
using System.Threading.Tasks;
using GestionEmploi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionEmploi.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            //chercher l'existance de l'utilisateur
           var user=await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x=>x.UserName==username); 
           if (user==null) return null; //si username n'existe pas retourne null

           return user;

        }

        //Méthode qui permet de vérifiet le mot de passe utilisateur
        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
             using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                   var ComputedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                   for (int i = 0; i < ComputedHash.Length; i++)
                   {
                        if (ComputedHash[i]!=passwordHash[i]){
                            return false;
                        }   
                   }
                   return true;
            }

        }

        public async Task<User> register(User user, string password)
        {
            byte[] passwordHash,passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt); //méthode permet de créer passwordHash et passwordSalt
            
            await _context.Users.AddAsync(user);//ajout user
            await _context.SaveChangesAsync(); //sauvgarder l'ajout

            return user;
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

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x=>x.UserName==username)) return true;
            return false;

        }
    }
}