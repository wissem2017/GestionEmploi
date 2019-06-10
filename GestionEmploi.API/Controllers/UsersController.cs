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
using Microsoft.Extensions.Options;
using Stripe;

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
        private readonly IOptions<StripeSettings> _stripeSettings;

        public UsersController(IEmploiRepository repo, IMapper mapper, IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); //--> retourner ID de User connecté
            var userFromRepo = await _repo.GetUser(currentUserId);//--> Retourner tous les infos de user connecté
            userParams.UserId = currentUserId;

            //--> Tester si genre exist dans user
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Geder == "homme" ? "femme" : "homme";
            }

            var users = await _repo.GetUsers(userParams);

            //--> Faire la laison avec DTO
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            //--> Faire la laison avec DTO
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDto, userFromRepo);

            //--> Vérifier si le sauvgarde à été
            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            //-->On cas d'erreur de connexion
            throw new Exception($"Il y a une erreur dans la mise à jour des données pour l'adhérant numéro {id}");
        }

        //--> Ajout Like
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);
            if (like != null)
                return BadRequest("Tu est admiré cet abonné avant");

            //-->Vérifier l'existance de recieientId
            if (await _repo.GetUser(recipientId) == null)
                return NotFound();

            //-->Ajouter Like
            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };
            _repo.Add<Like>(like); //--> Ajout dans la mémoire
            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Défaut d'admirer");

        }

        //--> Méthode pour faire le payment de user
        [HttpPost("{userId}/charge/{stripeToken}")]
        public async Task<IActionResult> Charge(int userId, string stripeToken)

        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var customers = new CustomerService();
            var charges = new ChargeService();
			
            // var options = new TokenCreateOptions
            // {
            // Card = new CreditCardOptions
            //     {
            //         // Number = "4242424242424242",
            //         // ExpYear = 2020,
            //         // ExpMonth = 3,
            //         // Cvc = "123"
            //     }
            // };

            // var service = new TokenService();
            // Token stripeToken = service.Create(options);

            var customer = customers.Create(new CustomerCreateOptions {
               // SourceToken = stripeToken
               Source = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions {
            Amount = 5000,
            Description = "Abonnement à vie ",
            Currency = "usd",
            CustomerId = customer.Id
            });

            var payment = new Payment{
                PaymentDate = DateTime.Now,
                Amount = charge.Amount/100,
                UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                ReceiptUrl = charge.ReceiptUrl,
                Description = charge.Description,
                Currency = charge.Currency,
                IsPaid = charge.Paid
            };

            _repo.Add<Payment>(payment);
            
            if(await _repo.SaveAll()){
           return Ok(new {IsPaid = charge.Paid } );
            }
            
            return BadRequest("Défaut de payer ");

        }

        [HttpGet("{userId}/payment")]
        public async Task<IActionResult> GetPaymentForUser(int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var payment=await _repo.GetPaymentForUser(userId);
                return Ok(payment);

        }



    }
}