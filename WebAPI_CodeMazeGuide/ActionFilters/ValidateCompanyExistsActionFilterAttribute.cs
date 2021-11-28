using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI_CodeMazeGuide.ActionFilters
{
    public class ValidateCompanyExistsActionFilterAttribute : IAsyncActionFilter
    {
        readonly ILoggerManager _logger;
        readonly IRepositoryManager _repositoryManager;

        public ValidateCompanyExistsActionFilterAttribute(ILoggerManager logger, IRepositoryManager repositoryManager)
        {
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                _logger.LogError("Invalid model state. Controller: { controller}, " +
                                 "action: { action}");
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
                return;
            }

            // if request is PUT, than we should track changes in EF
            var trackChanges = context.HttpContext.Request.Method
                .Equals("PUT", StringComparison.InvariantCultureIgnoreCase);
            
            var id =  (Guid) context.ActionArguments["id"];

            var company = await _repositoryManager.Companies.GetByIdAsync(id, trackChanges);

            if(company is null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("company", company);
                await next();
            }
        }
    }
}
