using Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System.IO;
using WebAPI_CodeMazeGuide.ActionFilters;
using WebAPI_CodeMazeGuide.Extensions;

namespace WebAPI_CodeMazeGuide
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(Path.Combine(Directory.GetCurrentDirectory(),
                "nlog.config"));

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureCors();
            services.ConfigureIIS();
            services.ConfigureLoggingService();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureRepositoryManager();
            services.ConfigureCustomActionFilters();
            services.AddAutoMapper(typeof(Startup));

            services.AddControllers(opt =>
            {
                opt.RespectBrowserAcceptHeader = true;
                opt.ReturnHttpNotAcceptable = true;
                opt.SuppressAsyncSuffixInActionNames = false;
            }).AddNewtonsoftJson()
              .AddXmlDataContractSerializerFormatters()
              .ConfigureOutputCSVFormatter()
              .ConfigureApiBehaviorOptions(opt =>
              {
                  opt.SuppressModelStateInvalidFilter = true;
              });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            });


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
