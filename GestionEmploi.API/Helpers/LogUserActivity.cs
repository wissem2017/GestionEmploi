using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GestionEmploi.API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace GestionEmploi.API.Helpers
{
    //--> Filter parmet d'exécuter une action aprés une instriction
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext= await next();
            var userId= int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var repo=resultContext.HttpContext.RequestServices.GetService<IEmploiRepository>();
            var user=await repo.GetUser(userId,true);
            user.LastActive=DateTime.Now;
            await repo.SaveAll();

        }
    }
}