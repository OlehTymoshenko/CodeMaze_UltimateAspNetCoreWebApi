using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_CodeMazeGuide.ActionFilters
{
    public class ValidationFilterAttribute : IAsyncActionFilter
    {
        private readonly ILoggerManager _logger;

        public ValidationFilterAttribute(ILoggerManager logger)
        {
            _logger = logger;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before action executing
            var result = BeforeActionExecuting(context);
            if (result is not null)
            {
                context.Result = result;
                return;
            }

            await next();

            // after action executing
        }

        private IActionResult BeforeActionExecuting(ActionExecutingContext context)
        {
            IActionResult methodResult = null;

            var action = context.ActionDescriptor.RouteValues["action"];
            var controller = context.ActionDescriptor.RouteValues["controller"];

            var param = context.ActionArguments
                               .FirstOrDefault(p => p.Value.GetType().Name
                                    .Contains("dto", StringComparison.InvariantCultureIgnoreCase))
                               .Value;

            if (param is null)
            {
                _logger.LogError($"Object sent from client is null. Controller: {controller}, " +
                                 $"action: { action}");
                methodResult = new BadRequestObjectResult($"Object is null. Controller: { controller }, action: { action}");
            }
            else if (!context.ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the object. Controller: { controller}, " +
                                 "action: { action}");
                methodResult = new UnprocessableEntityObjectResult(context.ModelState);
            }

            return methodResult;
        }
    }
}
