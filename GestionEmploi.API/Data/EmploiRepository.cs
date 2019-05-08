using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionEmploi.API.Helpers;
using GestionEmploi.API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace GestionEmploi.API.Data
{
    public class EmploiRepository : IEmploiRepository
    {
        private readonly DataContext _context;
        public EmploiRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        //--> Retourner Photos Principale user
        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
           return await _context.Photos.Where(u=>u.UserId==userId).FirstOrDefaultAsync(p=>p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo=await _context.Photos.FirstOrDefaultAsync(p=>p.Id==id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user=await _context.Users.Include(u=>u.Photos).FirstOrDefaultAsync(u=>u.Id==id);
            return user;

        }

        //--> Retourn la liste des users selon les paramètres définie dans userParams
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users=  _context.Users.Include(u=>u.Photos).OrderByDescending(u=>u.LastActive).AsQueryable();
            users=users.Where(u=>u.Id!=userParams.UserId); //--> élémener user courant de la liste

            users=users.Where(u=>u.Geder==userParams.Gender); //--> choisir juste les genre inverse(homme/femme)

            //--> Filter selon l'age
            if(userParams.MinAge!=18 || userParams.MaxAge!=99)
            {
                var minDob= DateTime.Today.AddYears(-userParams.MaxAge-1);
                var maxDob= DateTime.Today.AddYears(-userParams.MinAge);
                users=users.Where(u=>u.DateOfBirth>=minDob && u.DateOfBirth<=maxDob);
            }

            //--> Selon l'order 
            if(!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy)
                {
                    case "created":
                    users=users.OrderByDescending(u=>u.Created);
                    break;
                    
                    default:
                    users=users.OrderByDescending(u=>u.LastActive);
                    break;
                }
            }


            return await PagedList<User>.CreateAsync(users,userParams.PageNumber,userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync()>0;
            //--> s'il y a une sauvgarde donc return >0 sinon 0<
        }
    }
}