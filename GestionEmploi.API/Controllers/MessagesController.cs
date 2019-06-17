
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GestionEmploi.API.Data;
using GestionEmploi.API.Dtos;
using GestionEmploi.API.Helpers;
using GestionEmploi.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionEmploi.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
  
    public class MessagesController : ControllerBase
    {
         private readonly IEmploiRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IEmploiRepository repo, IMapper mapper)
        {
             _mapper = mapper;
            _repo = repo;
        }

        //--> Méthode permet de retourner un message
        [HttpGet("{id}",Name="GetMessage")]
        public async Task<IActionResult> GetMessage(int userId,int id)
        {
            //--> Vérifier l'authorisation de user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messageFromRepo=await _repo.GetMessage(id);
            if(messageFromRepo==null)
            return NotFound();

            return Ok(messageFromRepo);

        }

        //-->Retouner tous les message
        [HttpGet]
        public async Task<IActionResult> GetMessageForUser(int userId, [FromQuery]MessageParams messageParams)
        {
             //--> Vérifier l'authorisation de user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageParams.UserId=userId;

            //--> Retourner la liste des messages
            var MessagesFromRepo= await _repo.GetMessagesforUser(messageParams);

            //-->Modifier format de retour
            var messages=_mapper.Map<IEnumerable<MessageToReturnDto>>(MessagesFromRepo);

            //--> Retour avec pagination
            Response.AddPagination(MessagesFromRepo.CurrentPage,MessagesFromRepo.PageSize,MessagesFromRepo.TotalCount,MessagesFromRepo.TotalPages );

            return Ok(messages);
        }



        //--> Méthode permet d'ajouter un message
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId,MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId,true);
             
             //--> Vérifier l'authorisation de user
            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            messageForCreationDto.SenderId=userId;
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId,false);
            if(recipient==null)
            return BadRequest("Le destinataire n'a pas été atteint");

            //--> convertir de messageForCreationDto vers Message
            var message=_mapper.Map<Message>(messageForCreationDto);

            _repo.Add(message);//--> Ajout à la mémoire DbSet
            
            if(await _repo.SaveAll()){
                var messageToReturn=_mapper.Map<MessageToReturnDto>(message);

                 return CreatedAtRoute("GetMessage",new {id=message.Id},messageToReturn);
            }
           

            throw new Exception("Un problème est survenu lors de l'enregistrement du nouveau message.");
        }

        //-->Retourner les message entre deux users
        [HttpGet("chat/{recipientId}")]
        public async Task<IActionResult> GetConversation(int userId, int recipientId)
        {
              //--> Vérifier l'authorisation de user
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var messagesFromRepo= await _repo.GetConversation(userId,recipientId);

            var messageToReturn=_mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

            return Ok(messageToReturn);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadMessagesForUser(int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var count = await _repo.GetUnreadMessagesForUser( int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            
            return Ok(count);
        }

        [HttpPost("read/{id}")]
        public async Task<IActionResult> MarkMessageAsRead(int userId,int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var message = await _repo.GetMessage(id);

             if(message.RecipientId != userId)
                 return Unauthorized();
                 
            message.IsRead = true;
            message.DateRead=DateTime.Now;
            await _repo.SaveAll();
            return NoContent();
       }	


       [HttpPost("{id}")]
       public async Task<IActionResult> DeleteMessage(int id, int userId)
       {
           if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

             var message = await _repo.GetMessage(id);

             if(message.RecipientId == userId)
                 message.RecipientDeleted=true;

            if(message.SenderId==userId)
                message.SenderDeleted=true;

            if(message.SenderDeleted && message.RecipientDeleted)
                _repo.Delete(message);
            
            if(await _repo.SaveAll())
            return NoContent();

            throw new Exception("Il y à une erreur lors de suppression message");

                

       }

    }
}