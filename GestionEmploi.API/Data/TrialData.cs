using Newtonsoft.Json;
using GestionEmploi.API.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace GestionEmploi.API.Data
{
    public class TrialData
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public TrialData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //--> Méthode permet d'importer donnée de test de fichier JSON
        public void TrialUsers()
        {
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserTrialData.json");

                var users = JsonConvert.DeserializeObject<List<User>>(userData);//--> convertir de fichier Json vers fichier .NET
                //--> Création des Roles
                var roles=new List<Role>{
                    new Role{Name="Admin"},
                    new Role{Name="Moderator"},
                    new Role{Name="Member"},
                    new Role{Name="VIP"}
                };
                //-->Ajout role à la table ROLE
                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
                //-->Ajout User
                foreach (var user in users)
                {
                    // user.Photos.ToList().foreach(p=>p.IsApproved=true);
                    _userManager.CreateAsync(user, "password").Wait();
                    //-->Ajouter pour chaque user un role "Member"
                    _userManager.AddToRoleAsync(user,"Member").Wait();
                }

                //-->Ajouter User Administrateur
                var adminUser=new User{
                    UserName = "Admin"
                };

                IdentityResult result=_userManager.CreateAsync(adminUser,"password").Result;
                var admin = _userManager.FindByNameAsync("Admin").Result;
                //--> Ajouter Role pour User Admin
                _userManager.AddToRolesAsync(admin,new[]{"Admin","Moderator"}).Wait();



            }
        }

    }
}