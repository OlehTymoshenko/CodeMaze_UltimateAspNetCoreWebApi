using Microsoft.AspNetCore.Builder;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Entities.ErrorModel;

namespace WebAPI_CodeMazeGuide.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder appBuilder, 
            ILoggerManager logger)
        {
            appBuilder.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if(exceptionFeature is not null)
                    {
                        logger.LogError($"Something went wrong: {exceptionFeature.Error}");

                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = StatusCodes.Status500InternalServerError,
                            Message = "Internal server error"
                        }.ToString());
                    }
                });
            });
        }
    }
}
