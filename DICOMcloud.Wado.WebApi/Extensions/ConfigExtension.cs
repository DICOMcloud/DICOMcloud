namespace DICOMcloud.Wado.WebApi.Extensions
{
    #region Usings

    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    #endregion

    /// <summary>
    ///     The ConfigExtension.
    /// </summary>
    public static class ConfigExtension
    {
        #region Constructors and Destructors

        #endregion

        #region Public Properties


        #endregion

        #region Public Methods And Operators

        public static void ConfigureMvc(this IServiceCollection services, bool addAuth)
        {
            services.AddControllers(options =>
            {
                options.EnableEndpointRouting = false;
                if (addAuth)
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                }
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy()));
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
            });
        }

        public static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // services.AddDbContext<TestDbContext>(
            //     options =>
            //         options.UseNpgsql(
            //             configuration.GetConnectionString("defaultConnection"),
            //             a => a.MigrationsAssembly(migrationsAssembly)));
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Enable this to debug PII errors.
            IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    configuration.Bind("Authentication", options);
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            //Log failed authentications
                            Console.WriteLine(context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            //Log successful authentications
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Default",
                    builder => builder.WithOrigins("http://localhost:4201")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            return services;
        }
        public static void UseDefaultCors(this IApplicationBuilder app)
        {
            app.UseCors("Default");
        }


        #endregion

        #region Other Methods

        #endregion
    }
}