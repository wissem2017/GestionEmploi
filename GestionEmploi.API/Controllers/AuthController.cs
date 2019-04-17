using System.Threading.Tasks;
using GestionEmploi.API.Data;
using Microsoft.AspNetCore.Mvc;
using GestionEmploi.API.Models;
using GestionEmploi.API.Dtos;
using System.Security.Claims; 
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace GestionEmploi.API.Controllers
{
  [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo,IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")] //--> On spécifier la méthode "register" pour le verbe POST
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto){
            
             userForRegisterDto.Username=userForRegisterDto.Username.ToLower(); //--> Convertir en miniscule
             if (await _repo.UserExists(userForRegisterDto.Username)) return BadRequest("Cet utilisateur exist déjà");

             var userToCreate = new User{Username=userForRegisterDto.Username};
             var CreatedUser= await _repo.register(userToCreate,userForRegisterDto.Password);

            return StatusCode(201);
        }


        [HttpPost("login")] //--> On spécifier la méthode "login" pour le verbe POST
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto){
          
            var userFromRepo=await _repo.Login(userForLoginDto.username.ToLower(),userForLoginDto.password);

            if (userFromRepo==null) return Unauthorized(); //méthode qui indique qu'il n'est pas autoriser

            //--> création JWT TOKIN
            //--1- Création les données demander par le server
            var claims=new[]{
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };

            //2- Création de key de sécurité
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512); //--> Utilisation algorithme de chiffrement
            
            //3- création de Tokin
            var tokenDescriptor=new SecurityTokenDescriptor{
                Subject= new ClaimsIdentity(claims),
                Expires=DateTime.Now.AddDays(1), //date de validation pondant 1 jour
                SigningCredentials=creds
            };

            var tokenHandler= new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);

            //--> Retourn Token
            return Ok(new {
                token=tokenHandler.WriteToken(token)
                });          
        }
    }
}