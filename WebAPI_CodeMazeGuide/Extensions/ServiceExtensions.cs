using Contracts;
using Entities;
using Entities.DTOs;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.DataShaping;
using System;
using System.Reflection;
using WebAPI_CodeMazeGuide.ActionFilters;
using WebAPI_CodeMazeGuide.CustomInOutFormatters;

namespace WebAPI_CodeMazeGuide.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            return services;
        }

        public static IServiceCollection ConfigureIIS(this IServiceCollection services) =>
            services.Configure<IISOptions>(opt => { });

        public static IServiceCollection ConfigureLoggingService(this IServiceCollection services) =>
            services.AddScoped<ILoggerManager, LoggerManager>();

        public static IServiceCollection ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(builder =>
                builder.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                    opt => opt.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name)));

            return services;
        }

        public static IServiceCollection ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        
        public static IServiceCollection ConfigureCustomActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateCompanyExistsActionFilterAttribute>();
            services.AddScoped<ValitedateEmployeeForCompanyExistsActionFilterAttribute>();
            return services;
        }

        public static IServiceCollection ConfigureDataShaper(this IServiceCollection services) =>
            services.AddScoped<IDataShaper<EmployeeDTO>, DataShaper<EmployeeDTO>>();

        public static IMvcBuilder ConfigureOutputCSVFormatter(this IMvcBuilder mvcBuilder) =>
            mvcBuilder.AddMvcOptions(opt =>
            {
                opt.OutputFormatters.Add(new CsvOutputFormatter());
            });


    }
}
