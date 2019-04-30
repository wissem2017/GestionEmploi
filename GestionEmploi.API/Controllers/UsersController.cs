using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GestionEmploi.API.Data;
using GestionEmploi.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; 
using GestionEmploi.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using GestionEmploi.API.Helpers;

namespace GestionEmploi.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    
    public class UsersController : ControllerBase
    {
        private readonly IEmploiRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IEmploiRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            //--> Faire la laison avec DTO
            var usersToReturn=_mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}",Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            //--> Faire la laison avec DTO
            var userToReturn=_mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,UserForUpdateDto userForUpdateDto ){
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userFromRepo=await _repo.GetUser(id);
           
            _mapper.Map(userForUpdateDto, userFromRepo);

            //--> Vérifier si le sauvgarde à été
            if(await _repo.SaveAll()){
                return NoContent();
            }

            //-->On cas d'erreur de connexion
            throw new Exception($"Il y a une erreur dans la mise à jour des données pour l'adhérant numéro {id}");



        }



    }
}