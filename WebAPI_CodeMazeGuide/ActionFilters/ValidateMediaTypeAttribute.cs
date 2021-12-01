using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace WebAPI_CodeMazeGuide.ActionFilters
{
    public class ValidateMediaTypeAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue("Accept", out var acceptHeader))
            {
                context.Result = new BadRequestObjectResult("Accept header is missing");
                return;
            }

            if(!MediaTypeHeaderValue.TryParse(acceptHeader.ToString(), out var parsedMediaTypeHeader))
            {
                context.Result = new BadRequestObjectResult($"Media type not present. " +
                    "Please add Accept header with the required media type.");
                return;
            }

            context.HttpContext.Items.Add("AcceptHeaderMediaType", parsedMediaTypeHeader);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
