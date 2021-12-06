using Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebAPI_CodeMazeGuide.Controllers
{
    [ApiVersion("2.0", Deprecated = true)]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repository;

        public CompaniesV2Controller(IRepositoryManager repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Companies.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
    }
}
