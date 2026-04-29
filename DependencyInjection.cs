using FluentValidation;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Persistence;
using SurveyBasket.Services;
using System.Reflection;

namespace SurveyBasket
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
             throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(connectionString));
            services.AddSwaggerServices();
            services.AddMapsterConf();
            services.AddFluentValidationConf();
            services.AddScoped<IPollService, PollService>();
            return services;
        }
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            return services;
        }
        public static IServiceCollection AddMapsterConf(this IServiceCollection services)
        {
            var mappingconfig = TypeAdapterConfig.GlobalSettings;
            mappingconfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mappingconfig));
            return services;
        }
        public static IServiceCollection AddFluentValidationConf(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
            return services;
        }
    }
}