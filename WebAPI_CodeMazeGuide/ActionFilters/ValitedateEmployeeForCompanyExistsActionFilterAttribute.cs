using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebAPI_CodeMazeGuide.ActionFilters
{
    public class ValitedateEmployeeForCompanyExistsActionFilterAttribute : IAsyncActionFilter
    {
        readonly IRepositoryManager _repositoryManager;
        readonly ILoggerManager _logger;

        public ValitedateEmployeeForCompanyExistsActionFilterAttribute(
            IRepositoryManager repositoryManager, 
            ILoggerManager logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
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

            var trackChanges = new[] { HttpMethod.Put.Method, HttpMethod.Patch.Method }
                                        .Contains(context.HttpContext.Request.Method, 
                                            StringComparer.InvariantCultureIgnoreCase);

            var companyId = (Guid) context.ActionArguments["companyId"];
            var id = (Guid) context.ActionArguments["id"];

            var company = await _repositoryManager.Companies.GetByIdAsync(companyId, false);

            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var employee = await _repositoryManager.Employees.GetByIdAsync(companyId, id, trackChanges);

            if (employee is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            context.HttpContext.Items.Add("employee", employee);
            await next();
        }
    }
}
