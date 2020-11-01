namespace DICOMcloud.Wado.WebApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using DICOMcloud.Wado.WebApi.Extensions;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;
    using DICOMcloud.Wado.Configs;
    using System;
    using Microsoft.Extensions.Options;

    public class Startup
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        private IHostEnvironment _environment { get; }

        public Startup(
            IConfiguration configuration,
            IHostEnvironment enviroment)
        {
            this.Configuration = configuration;
            this._environment = enviroment;

            Console.WriteLine($"Config:{this.Configuration.GetConnectionString("pacsDataArchieve")}");
            Console.WriteLine($"PacsDataArchieve:{this.Configuration.GetValue<string>("Urls:WadoRsUrl")}");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureMvc(false);
            services.ConfigureDbContext(this.Configuration);
            services.ConfigureAutoMapper();
            // services.ConfigureAuthentication(this.Configuration);
            // this.ConfigureAuthorization(services);

            services.AddSwaggerDocumentation(Configuration, false)
                .AddHttpContextAccessor()
                .AddResponseCompression()
                .AddOptions();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<UrlOptions>(Configuration.GetSection("Urls"));
            services.Configure<QidoOptions>(Configuration.GetSection("Qido"));

            var urlOptions = services.BuildServiceProvider().GetRequiredService<IOptions<UrlOptions>>();
            DicomExtensions dicomExtensions = new DicomExtensions(this.Configuration, services, urlOptions);
            dicomExtensions.Build();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
                {
                    options.DisplayOperationId();
                    // build a swagger endpoint for each discovered API version
                    options.RoutePrefix = string.Empty;
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                }
            );

            app.UseRouting();
            app.UseDefaultCors();
            // app.UseAuthentication();
            // app.UseAuthorization();
            app.UseResponseCompression();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                // {
                //     AllowCachingResponses = false
                // });
            });
        }

        void ConfigureAuthorization(IServiceCollection services)
        {
            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("Trading", policy => policy.RequireClaim("trade_access", "1"));
            // });
        }
        
        private void ConfigureDicom()
        {
            // GlobalConfiguration.Configure(WebApiConfig.Register);

        }

    }
}