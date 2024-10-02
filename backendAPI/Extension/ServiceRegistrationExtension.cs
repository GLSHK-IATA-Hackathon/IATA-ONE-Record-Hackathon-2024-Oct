using Microsoft.AspNetCore.Mvc;
using WebAPITemplate.Models.RestClient;

namespace WebAPITemplate.Extension
{
    public static class ServiceRegistrationExtension
    {
        /// <summary>
        /// Add DI services here
        /// </summary>
        /// <param name="services"></param>
        public static void AddDIServices(this IServiceCollection services)
        {
            _ = services.AddCors(options =>
            {
                options.AddPolicy("AllowWithOrigin",
                    builder => builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
                options.AddPolicy("AllowAllOrigin",
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            _ = services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

            // Register Logger DI

            //add Restclient DI
            _ = services.AddSingleton<ApiRestClient>();

            // Register HttpContext DI
            _ = services.AddHttpContextAccessor();

            //register others object if necessary

        }

        public static void AddValidationServices(this IServiceCollection services)
        {
        }

        /// <summary>
        /// add configure file here
        /// </summary>
        /// <param name="Host"></param>
        public static void AddHostConfigure(this ConfigureHostBuilder Host)
        {
           
        }
    }
}
