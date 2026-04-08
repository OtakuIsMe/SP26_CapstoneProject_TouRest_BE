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
            //Add repositories to the DI container, count = 14
            services.AddScoped<IBookingItineraryRepository, BookingItineraryRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IItineraryActivityRepository, ItineraryActivityRepository>();
            services.AddScoped<IItineraryRepository, ItineraryRepository>();
            services.AddScoped<IItineraryStopRepository, ItineraryStopRepository>();
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IPackageServiceRepository, PackageServiceRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<IWishListRepository, WishListRepository>();
            //Add services to the DI container, count = 15
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IBookingItineraryService, BookingItineraryService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IItineraryActivityService, ItineraryActivityService>();
            services.AddScoped<IItineraryService, ItineraryService>();
            services.AddScoped<IItineraryStopService, ItineraryStopService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IPackageServiceService, PackageServiceService>();
            services.AddScoped<IProviderService, ProviderService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWishListService, WishListService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IRouteOptimizerService, RouteOptimizerService>();
            return services;
        }
    }
}
