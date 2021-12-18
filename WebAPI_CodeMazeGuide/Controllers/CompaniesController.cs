using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Marvin.Cache.Headers;
using Marvin.Cache.Headers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCaching;
using WebAPI_CodeMazeGuide.ActionFilters;
using WebAPI_CodeMazeGuide.ModelBinders;

namespace WebAPI_CodeMazeGuide.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    // [ResponseCache(CacheProfileName = "120SecondsDuration")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IValidatorValueInvalidator validatorValueInvalidator;
        private readonly IStoreKeyAccessor storeKeyAccessor;

        public CompaniesController(ILoggerManager loggerManager,
            IRepositoryManager repositoryManager,
            IMapper mapper,
            IValidatorValueInvalidator validatorValueInvalidator,
            IStoreKeyAccessor storeKeyAccessor)
        {
            _logger = loggerManager;
            _repository = repositoryManager;
            _mapper = mapper;
            this.validatorValueInvalidator = validatorValueInvalidator;
            this.storeKeyAccessor = storeKeyAccessor;
            //

        }

        [HttpGet(Name ="GetCompanies")]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            var companies = await _repository.Companies.GetAllCompaniesAsync(false);

            var companiesDto = _mapper.Map<IEnumerable<CompanyDTO>>(companies);

            return Ok(companiesDto);
        }

        [HttpGet("{id}", Name = "CompanyById")]
        // [ResponseCache(Duration = 60)]
        [HttpCacheExpiration(MaxAge = 300, CacheLocation = CacheLocation.Public)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetComapanyByIdAsync(Guid id)
        {
            var company = await _repository.Companies.GetByIdAsync(id, false);

            if(company is null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var companyDto = _mapper.Map<CompanyDTO>(company);

            return Ok(companyDto);
        }

        [HttpGet("collection/({ids})")]
        public async Task<IActionResult> GetCollectionOfCompaniesAsync(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if(ids is null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }

            var companyEntities = await _repository.Companies.GetByIdsAsync(ids, false);

            if(companyEntities.Count() != ids.Count()) 
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }

            var companiesDto = _mapper.Map<IEnumerable<CompanyDTO>>(companyEntities);

            return Ok(companiesDto);
        }

        [HttpPost(Name = "CreateCompany")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompanyAsync(CompanyForCreationDTO companyCreationDto)    
        {
            var companyEntity = _mapper.Map<Company>(companyCreationDto);

            _repository.Companies.Create(companyEntity);
            await _repository.SaveAsync();

            var companyResultDto = _mapper.Map<CompanyDTO>(companyEntity);

            return CreatedAtRoute("CompanyById", new { id = companyEntity.Id }, 
                companyResultDto);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCollectionOfCompaniesAsync(
            IEnumerable<CompanyForCreationDTO> companiesCreationDto)
        {
            if (companiesCreationDto is null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }

            var companiesEntity = _mapper.Map<IEnumerable<Company>>(companiesCreationDto);

            foreach (var company in companiesEntity)
            {
                _repository.Companies.Create(company);
            }

            await _repository.SaveAsync();

            var companiesCollectionResultDto = _mapper.Map<IEnumerable<CompanyDTO>>(companiesEntity);

            return CreatedAtAction(nameof(GetCollectionOfCompaniesAsync), 
                new { ids = string.Join(',', companiesCollectionResultDto.Select(c => c.Id)) },
                companiesCollectionResultDto);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsActionFilterAttribute))]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var company = HttpContext.Items["company"] as Company 
                ?? throw new KeyNotFoundException("It's not found company key in HttpContext.Items dictionary");

            _repository.Companies.Delete(company);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute), Order = 1)]
        [ServiceFilter(typeof(ValidateCompanyExistsActionFilterAttribute), Order = 2)]
        public async Task<IActionResult> UpdateAsync(Guid id, CompanyForUpdatingDTO companyUpdateDTO)
        {
            var companyEntity = HttpContext.Items["company"] as Company
                ?? throw new KeyNotFoundException("It's not found \"company\" key in HttpContext.Items dictionary");

            _mapper.Map(companyUpdateDTO, companyEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "GET, POST, PUT, DELETE, OPTIONS");

            return Ok();
        }
    }
}
