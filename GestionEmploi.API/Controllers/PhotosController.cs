using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GestionEmploi.API.Data;
using GestionEmploi.API.Dtos;
using GestionEmploi.API.Helpers;
using GestionEmploi.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GestionEmploi.API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IEmploiRepository _repo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IMapper _mapper;
        private Cloudinary _cloudinary;

        public PhotosController(IEmploiRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper)
        {
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            _repo = repo;

            //--> on créer un account pour photos dans Cloudinary
            Account acc=new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}",Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            var photofromRepository=await _repo.GetPhoto(id);
            var photo=_mapper.Map<PhotoForReturnDto>(photofromRepository);

            return Ok(photo);
        }



        //--> Méthode permet d'ajouter  photo
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,[FromForm]PhotoForCreateDto photoForCreateDto )
        {
            //--> Vérifier id de user avec id dans token
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            //--> Récupérer les donner de user
            var userFromRepo=await _repo.GetUser(userId,true);

            var file=photoForCreateDto.File;
            var uploadResult= new ImageUploadResult();
            //--> Télécharger photo
            if(file!=null && file.Length>0){
                //--> Paramétrage de photo
                using(var stream=file.OpenReadStream()){
                    var uploadParams=new ImageUploadParams(){
                        File=new FileDescription(file.Name,stream),
                        Transformation=new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    //--> téléchargement de photo chez Cloudinary
                    uploadResult=_cloudinary.Upload(uploadParams);
                }
            }
            //--> Récupérer les données de photo 
            photoForCreateDto.Url=uploadResult.Uri.ToString();
            photoForCreateDto.publicId=uploadResult.PublicId;
            var photo=_mapper.Map<Photo>(photoForCreateDto);

            //--> tester s'il y a une photo principale de user
            if(!userFromRepo.Photos.Any(p=>p.IsMain))
            photo.IsMain=true;

            //--> Ajouter photo à la BD
            userFromRepo.Photos.Add(photo);
            if(await _repo.SaveAll())
            {
                var PhotoToReturn=_mapper.Map<PhotoForReturnDto>(photo);
                 return CreatedAtRoute("GetPhoto",new {id=photo.Id},PhotoToReturn);
            }
           

            return BadRequest("Erreur dans l'ajout de photo");

        }

        //-->Méthode permet de modifier image principale
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
             //--> Vérifier id de user avec id dans token
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            //--> Récupérer les donner de user
            var userFromRepo=await _repo.GetUser(userId,true);

            //--> Vérifier que photo appartient aux photos user
            if(!userFromRepo.Photos.Any(p=>p.Id==id))
            return Unauthorized();

            var DesiredMainPhoto=await _repo.GetPhoto(id);
            if(DesiredMainPhoto.IsMain)
            return BadRequest("Ceci est la photo principale");

            //-->retourner photo principale
            var CurrentMainPhoto=await _repo.GetMainPhotoForUser(userId);

            CurrentMainPhoto.IsMain=false;

            DesiredMainPhoto.IsMain=true;
            //-->Sauvgarder la modifécation
            if(await _repo.SaveAll())
            return NoContent();

            //--> On Cas d'erreur
            return BadRequest("On ne peut pas modifier photo principale");
        }

        //--> Méthode permet de supprimer photo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId,int id)
        {
             //--> Vérifier id de user avec id dans token
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            //--> Récupérer les donner de user
            var userFromRepo=await _repo.GetUser(userId,true);

             //--> Vérifier que photo appartient aux photos user
            if(!userFromRepo.Photos.Any(p=>p.Id==id))
            return Unauthorized();

            var Photo=await _repo.GetPhoto(id);
            if(Photo.IsMain)
            return BadRequest("On ne peut pas supprimer photo principale");

            if(Photo.PublicId!=null)
            {
                var deleteParams=new DeletionParams(Photo.PublicId);
                var result=this._cloudinary.Destroy(deleteParams);
                if(result.Result=="ok"){
                    _repo.Delete(Photo);
                }
            }
            if(Photo.PublicId==null){
                _repo.Delete(Photo);
            }

            if(await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Échec de la suppression de l'image");

        }


    }
}