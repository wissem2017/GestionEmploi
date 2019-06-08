using System.Linq;
using AutoMapper;
using GestionEmploi.API.Dtos;
using GestionEmploi.API.Models;

namespace GestionEmploi.API.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            //--> Création MAP de User vers UserForListDto
            CreateMap<User,UserForListDto>()
            .ForMember(dest=>dest.PhotoUrl,opt=>{opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);})
            .ForMember(dest=>dest.Age,opt=>{opt.ResolveUsing(src=>src.DateOfBirth.CalculateAge());});

             //--> Création MAP de User vers UserForDetailsDto
            CreateMap<User,UserForDetailsDto>()
            .ForMember(dest=>dest.PhotoUrl,opt=>{opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);})
            .ForMember(dest=>dest.Age,opt=>{opt.ResolveUsing(src=>src.DateOfBirth.CalculateAge());});
            
            //--> Création MAP de Photo vers PhotoForDetailsDto
            CreateMap<Photo,PhotoForDetailsDto>();
             //--> Création MAP de FileUser vers FileUserForDetailsDto
           // CreateMap<FileUser,FileUserForDetailsDto>();
           
           CreateMap<UserForUpdateDto,User>();

           CreateMap<Photo,PhotoForReturnDto>();

           CreateMap<PhotoForCreateDto,Photo>();

           CreateMap<UserForRegisterDto,User>();
           
           CreateMap<MessageForCreationDto,Message>().ReverseMap();

            CreateMap<Message,MessageToReturnDto>().ForMember(dest=>dest.SenderPhotoUrl,opt=>{opt.MapFrom(src=>src.Sender.Photos.FirstOrDefault(p=>p.IsMain).Url);})
            .ForMember(dest=>dest.RecipientPhotoUrl,opt=>{opt.MapFrom(src=>src.Recipient.Photos.FirstOrDefault(p=>p.IsMain).Url);});



        }
    }
}