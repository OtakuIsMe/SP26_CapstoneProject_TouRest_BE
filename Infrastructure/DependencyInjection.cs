using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TouRest.Application.Interfaces;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Persistence;
using TouRest.Infrastructure.Repositories;
using TouRest.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace TouRest.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IPackageServiceRepository, PackageServiceRepository>();

            // Register Services
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
