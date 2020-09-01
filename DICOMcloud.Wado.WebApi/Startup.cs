namespace DICOMcloud.Wado.WebApi
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using DICOMcloud.Wado.WebApi.Extensions;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;

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
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureMvc(false);
            services.ConfigureDbContext(this.Configuration);
            services.ConfigureAutoMapper();
            // services.ConfigureAuthentication(this.Configuration);
            // this.ConfigureAuthorization(services);

            services.AddSwaggerDocumentation(Configuration)
                .AddHttpContextAccessor()
                .AddResponseCompression()
                .AddOptions();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<UrlOptions>(Configuration.GetSection("Urls"));

            // services.AddTransient<ITradingMarketService, TradingMarketService>();

            // var settings = this.Configuration.GetSection("Strategy").Get<Setting>();
            // services.Configure<Setting>(options => Configuration.GetSection("Strategy").Bind(options));
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseExceptionHandler("/error");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseDefaultCors();
            // app.UseAuthentication();
            // app.UseAuthorization();
            app.UseResponseCompression();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(options =>
                {
                    options.DisplayOperationId();
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                    options.RoutePrefix = string.Empty;
                }
            );

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

    }
}