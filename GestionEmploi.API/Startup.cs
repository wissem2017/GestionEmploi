﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GestionEmploi.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using GestionEmploi.API.Helpers;
using AutoMapper;

namespace GestionEmploi.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x=>x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(option =>{
                option.SerializerSettings.ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            
            services.AddCors(); //--> Pour avoir l'autorisation de angular d'utiliser service API

            services.AddAutoMapper();

            services.AddTransient<TrialData>();//--> pour faire l'ajout légère des donnée de test

            services.AddScoped<IAuthRepository,AuthRepository>();
            
            services.AddScoped<IEmploiRepository,EmploiRepository>(); //Ajouter l'exécution de service pour EmploiRepository
           
            //--> Ajout service d'autorisation  MiddleWare
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(Options=>{
                Options.TokenValidationParameters=new TokenValidationParameters{
                    ValidateIssuerSigningKey=true,
                    IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer=false,
                    ValidateAudience= false
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,TrialData trialData )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               app.UseExceptionHandler(BuilderExtension =>
               {
                   BuilderExtension.Run(async context =>
                   {
                       context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                       var error = context.Features.Get<IExceptionHandlerFeature>();
                       if(error != null)
                       {
                           context.Response.AddApplicationError(error.Error.Message);
                           await context.Response.WriteAsync(error.Error.Message);
                       }
                   });
               });
            }

            // app.UseHttpsRedirection();
            // trialData.TrialUsers(); //--> Ajout données de test
            app.UseCors(x=>x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); //--> Avoir une autorisation pour tous le monde
            
            app.UseAuthentication();//--> Tester l'autorisation
            app.UseMvc();
        }
    }
}
