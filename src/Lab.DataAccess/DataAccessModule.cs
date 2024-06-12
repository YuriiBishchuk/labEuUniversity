using Lab.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab.DataAccess
{
    public static class DataAccessModule
    {
        public static void RegisterDataAccessModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = Environment.GetEnvironmentVariable("AZURE_POSTGRESQL_CONNECTIONSTRING")
                          ?? configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
           
            options.UseNpgsql(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IToDoRepository, ToDoRepository>();
        }
    }
}
