﻿using Lab.DataAccess.Repository;
using Lab.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab.Business.Services;

namespace Lab.Business
{

    public static class BusinessModule
    {
        public static void RegisterBusinessModule(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserToDoService, UserToDoService>();
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
        }
    }
}
