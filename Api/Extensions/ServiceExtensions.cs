using TouRest.Application.Common.Helpers;
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
            //Add repositories to the DI container, count = 10
            services.AddScoped<IItineraryStopRepository, ItineraryStopRepository>();
            services.AddScoped<IItineraryActivityRepository, ItineraryActivityRepository>();
            services.AddScoped<IItineraryRepository, ItineraryRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            //Add services to the DI container, count = 11
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IItineraryService, ItineraryService>();
            services.AddScoped<IItineraryStopService, ItineraryStopService>();
            services.AddScoped<IItineraryActivityService, ItineraryActivityService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWishListService, WishListService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IRouteOptimizerService, RouteOptimizerService>();
            return services;
        }
    }
}
