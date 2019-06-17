using System.Threading.Tasks;
using GestionEmploi.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GestionEmploi.API.Dtos;
using Microsoft.AspNetCore.Identity;
using GestionEmploi.API.Models;
using Microsoft.Extensions.Options;
using GestionEmploi.API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace GestionEmploi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public AdminController(DataContext context, UserManager<User> userManager, IOptions<CloudinarySettings> cloudinaryConfig)
        {
           _cloudinaryConfig = cloudinaryConfig;
            _userManager = userManager;
            _context = context;

             //--> on créer un account pour photos dans Cloudinary
            Account acc=new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<IActionResult> GetUserWithRoles()
        {
            var userList = await (from user in _context.Users
                                  orderby user.UserName
                                  select new
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      Roles = (from userRole in user.UserRoles
                                               join role in _context.Roles
                                               on userRole.RoleId equals role.Id
                                               select role.Name).ToList()
                                  }).ToListAsync();
            return Ok(userList);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<IActionResult> GetPhotosForModeration()
        {
            var photos = await _context.Photos
                .Include(u => u.User)
                .IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(u => new
                {
                    Id = u.Id,
                    UserName = u.User.UserName,
                    KnownAs = u.User.KnowAs,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                }).ToListAsync();

            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<IActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _context.Photos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == photoId);

            photo.IsApproved = true;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{photoId}")]
        public async Task<IActionResult> RejectPhoto(int photoId)
        {
            var photo = await _context.Photos
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo.IsMain)
                return BadRequest("Vous pouvez rejeter l'image de base");

            if (photo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _context.Photos.Remove(photo);
                }
            }

            if (photo.PublicId == null)
            {
                _context.Photos.Remove(photo);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

			

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editroles/{userName}")]
        public async Task<IActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles)); //Ajouter rôle sans repeter l'ancien
            if (!result.Succeeded)
                return BadRequest("Erreur lors d'ajout de rôle");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));//Supprimer les rôles non séléctionner
            if (!result.Succeeded)
                return BadRequest("Erreur lors de supprission de rôle");

            return Ok(await _userManager.GetRolesAsync(user));


        }

    }
}