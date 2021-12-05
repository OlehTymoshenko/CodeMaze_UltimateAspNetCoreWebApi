using Entities.Models.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Net.Http;

namespace WebAPI_CodeMazeGuide.Controllers
{
    [Route("api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public RootController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HttpGet] 
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (!mediaType.Contains("application/vnd.olehtymoshenko.apiroot"))
            {
                return  NoContent();
            }

            List<Link> links = new()
            {
                new Link()
                {
                    Href = _linkGenerator.GetUriByAction(HttpContext, action: nameof(GetRoot)),
                    Rel = "self",
                    Method = HttpMethod.Get.Method
                },
                new Link()
                {
                    Href = _linkGenerator.GetUriByName(HttpContext, "GetCompanies", new { }),
                    Rel = "companies",
                    Method = HttpMethod.Get.Method
                },
                new Link()
                {
                    Href  = _linkGenerator.GetUriByName(HttpContext, "CreateCompany", new { }), 
                    Rel = "create_company",
                    Method = HttpMethod.Post.Method
                }
            };

            return Ok(links);
        }
    }
}
