using Contracts;
using Entities;
using Entities.DTOs;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.DataShaping;
using System;
using System.Linq;
using System.Reflection;
using AspNetCoreRateLimit;
using WebAPI_CodeMazeGuide.ActionFilters;
using WebAPI_CodeMazeGuide.CustomInOutFormatters;
using System.Collections.Generic;

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
            services.AddScoped<ValidateMediaTypeAttribute>();
            return services;
        }

        public static IServiceCollection ConfigureVersioning(this IServiceCollection services) =>
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

        public static IServiceCollection ConfigureDataShaper(this IServiceCollection services) =>
            services.AddScoped<IDataShaper<EmployeeDTO>, DataShaper<EmployeeDTO>>();

        public static IMvcBuilder ConfigureOutputCSVFormatter(this IMvcBuilder mvcBuilder) =>
            mvcBuilder.AddMvcOptions(opt =>
            {
                opt.OutputFormatters.Add(new CsvOutputFormatter());
            });

        public static IServiceCollection ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();

        public static IServiceCollection ConfigureHtttpCachingHeaders(this IServiceCollection services) =>
            services.AddHttpCacheHeaders(
                expOpt =>
                {
                    expOpt.MaxAge = 69;
                    expOpt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
                },
                validOpt => 
                {
                    validOpt.MustRevalidate = true;
                });

        public static IServiceCollection ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                new ()
                {
                    Endpoint = "*",
                    Limit = 5,
                    Period = "5m"
                }
            };

            services.Configure<IpRateLimitOptions>(opt => 
            {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

            return services;
        }

        public static IMvcBuilder AddCustomMediaTypes(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddMvcOptions(opt => 
            {
                var newtonsoftJsonOutputFormatter = opt.OutputFormatters.
                    OfType<NewtonsoftJsonOutputFormatter>()
                    ?.FirstOrDefault();

                if(newtonsoftJsonOutputFormatter is not null)
                {
                    newtonsoftJsonOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.olehtymoshenko.hateoas+json");
                    newtonsoftJsonOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.olehtymoshenko.apiroot+json");
                }

                var xmlOutputFormatter = opt.OutputFormatters.
                    OfType<XmlDataContractSerializerOutputFormatter>()
                    ?.FirstOrDefault();

                if (xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.olehtymoshenko.hateoas+xml");
                    xmlOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.olehtymoshenko.apiroot+xml");
                }
            });

            return mvcBuilder;
        }

    }
}
