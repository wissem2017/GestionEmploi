using System.Collections.Generic;
using System.Threading.Tasks;
using GestionEmploi.API.Helpers;
using GestionEmploi.API.Models;

namespace GestionEmploi.API.Data
{
    public interface IEmploiRepository
    {
         void Add<T>(T entity) where T:class; //--> Méthode qui sera appler à tous les Tables
         
         void Delete<T>(T entity) where T:class;

         Task<bool> SaveAll();

         Task<PagedList<User>> GetUsers(UserParams userParams); //--> Méthode permet de retouner tous selon Pagination

         Task<User> GetUser(int id);

         Task<Photo> GetPhoto(int id);//--> Permet de retourner photo selon id

         Task<Photo> GetMainPhotoForUser( int userId); //--> Retourner la photo principale

         Task<Like> GetLike(int userId,int recipientId)     ;//-->faire la laison entre les likers

        Task<Message> GetMessage(int id); //--> Permert de retourner le contenu d'un message

        Task<PagedList<Message>> GetMessagesforUser(MessageParams messageParams);//--> Retourner la liste des messages pour un user

        Task<IEnumerable<Message>> GetConversation(int userId, int recipientId); //--> Retourner la liste des message entre deux Users

        Task<int> GetUnreadMessagesForUser(int userId);//-->Retourne le nbre des messages non lues

    }
}