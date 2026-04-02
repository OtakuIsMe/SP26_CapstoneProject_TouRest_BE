using TouRest.Application.Interfaces;
using TouRest.Application.Services;
using TouRest.Domain.Interfaces;
using TouRest.Infrastructure.Repositories;

namespace TouRest.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            //Add repositories to the DI container, count = 9
            services.AddScoped<IItineraryStopRepository, ItineraryStopRepository>();
            services.AddScoped<IItineraryActivityRepository, ItineraryActivityRepository>();
            services.AddScoped<IItineraryRepository, ItineraryRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            //Add services to the DI container, count = 9
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IItineraryService, ItineraryService>();
            services.AddScoped<IItineraryStopService, ItineraryStopService>();
            services.AddScoped<IItineraryActivityService, ItineraryActivityService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWishListService, WishListService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            return services;
        }
    }
}
