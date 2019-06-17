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
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GestionEmploi.API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AuthController(IConfiguration config, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")] //--> On spécifier la méthode "register" pour le verbe POST
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            var userToCreate = _mapper.Map<User>(userForRegisterDto);
            var result =await _userManager.CreateAsync(userToCreate,userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailsDto>(userToCreate);

            if(result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { contrommer = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);            
        }


        [HttpPost("login")] //--> On spécifier la méthode "login" pour le verbe POST
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var user =await _userManager.FindByNameAsync(userForLoginDto.username);
            var result=await _signInManager.CheckPasswordSignInAsync(user,userForLoginDto.password,false);

            if(result.Succeeded){
                var appUser=await _userManager.Users.Include(p=>p.Photos).FirstOrDefaultAsync(
                    u=>u.NormalizedUserName==userForLoginDto.username.ToUpper()
                );
                  var userToReturn = _mapper.Map<UserForListDto>(appUser);
                 //--> Retourn Token
            return Ok(new
            {
                token = GenerateJwtToken(appUser).Result,
                user = userToReturn
            });
                
            }
            return Unauthorized();

        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var roles=await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }

            //2- Création de key de sécurité
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); //--> Utilisation algorithme de chiffrement

            //3- création de Tokin
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), //date de validation pondant 1 jour
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}