using System.Threading.Tasks;
using GestionEmploi.API.Models;

namespace GestionEmploi.API.Data
{
    public interface IAuthRepository
    {
         Task<User> register(User user, string password);
         Task<User> Login(string username, string password);
         Task<bool> UserExists(string username);
         
    }
}