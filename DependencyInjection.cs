using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasket.Authentication;
using SurveyBasket.Authentication.Filters;
using SurveyBasket.Health;
using SurveyBasket.Settings;
using System.Reflection;
using System.Text;
namespace SurveyBasket
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllers();
            services.AddCors(options=>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            }); 
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
             throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddAuthConfig(configuration);
            services.AddDbContext<ApplicationDbContext>(options =>options.UseSqlServer(connectionString));
            services.AddSwaggerServices();
            services.AddMapsterConfig();
            services.AddBackgroundJobsConfig(configuration);
            services.AddFluentValidationConfig();
            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IResultService, ResultService>();   
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<INotifiactionService, NotificationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.AddHealthChecks().AddSqlServer(name:"database",connectionString:configuration.GetConnectionString("DefaultConnection")!)
                .AddHangfire(options => options.MinimumAvailableServers=1)
                .AddCheck<MailProviderHealthCheck>("Mail Services");
            return services;
        }
        private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            return services;
        }
        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            var mappingconfig = TypeAdapterConfig.GlobalSettings;
            mappingconfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(implementationInstance: new Mapper(mappingconfig));
            return services;
        }
        private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
            return services;
        }
        private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddOptions<JwtOptions>().BindConfiguration(JwtOptions.SectionName).ValidateDataAnnotations().ValidateOnStart();
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
            services.AddTransient<IAuthorizationHandler,PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider,PermissionAuthorizationPolicyProvider>();
            var jwtSettings=configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }
           ).AddJwtBearer(o =>
           {
               o.SaveToken  = true;
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                   ValidIssuer = jwtSettings!.Issuer,
                   ValidAudience = jwtSettings!.Audience,
               };
           });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
               options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            }
            );
            return services;
        }
        private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
            services.AddHangfireServer();
            return services;
        }
    }
}