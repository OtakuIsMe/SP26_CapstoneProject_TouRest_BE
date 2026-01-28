using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;
using TouRest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace TouRest.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
            configuration["DATABASE_CONNECTION"]; // KHÔNG throw
            Console.WriteLine("Connection String: " + connectionString);
            // Register DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    connectionString
                ));

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
