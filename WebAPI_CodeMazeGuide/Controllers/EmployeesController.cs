using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Contracts;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Threading.Tasks;
using WebAPI_CodeMazeGuide.ActionFilters;
using Entities.RequestFeatures;
using System.Text.Json;
using WebAPI_CodeMazeGuide.Utility;

namespace WebAPI_CodeMazeGuide.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly EmployeeLinks _employeeLinks;

        public EmployeesController(IRepositoryManager repository,
            ILoggerManager logger,
            IMapper mapper, 
            EmployeeLinks employeeLinks) => 
            (_repository, _logger, _mapper, _employeeLinks) = (repository, logger, mapper, employeeLinks);

        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployeesForCompanyAsync(Guid companyId, 
            [FromQuery] EmployeeParameters employeeParameters)
        {
            if(!employeeParameters.IsAgeRangeValid)
            {
                return BadRequest("Max age can't be less than min age.");
            }

            var company = await _repository.Companies.GetByIdAsync(companyId, false);

            if(company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employees = await _repository.Employees.GetEmployeesAsync(companyId, 
                employeeParameters, false);

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(employees.PaginationMetaData));

            var employeesDto = _mapper.Map<IEnumerable<EmployeeDTO>>(employees);

            var links = _employeeLinks.TryGenerateLinks(employeesDto, employeeParameters.Fields,
                companyId, HttpContext);



            return links.HasLinks ? Ok(links.LinkedEntities) : Ok(links.ShapedEntities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeByIdAsync(Guid companyId, Guid id)
        {
            var company = await _repository.Companies.GetByIdAsync(companyId, false);

            if (company is null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employee = await _repository.Employees.GetByIdAsync(companyId, id, false);

            if (employee is null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            var employeeDto = _mapper.Map<EmployeeDTO>(employee);

            return Ok(employeeDto);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateEmployeeAsync(Guid companyId, EmployeeForCreationDTO employeeCreationDTO)
        {
            if(await _repository.Companies.GetByIdAsync(companyId, false) is null) 
            {
                _logger.LogError($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeEntity = _mapper.Map<Employee>(employeeCreationDTO);
            employeeEntity.CompanyId = companyId;

            _repository.Employees.Create(employeeEntity);
            await _repository.SaveAsync();

            var employeeResultDto = _mapper.Map<EmployeeDTO>(employeeEntity);

            return CreatedAtAction(nameof(GetEmployeeByIdAsync), 
                new { companyId, employeeEntity.Id },
                employeeResultDto);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValitedateEmployeeForCompanyExistsActionFilterAttribute))]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid companyId, Guid id)
        {
            var employee = HttpContext.Items["employee"] as Employee 
                ?? throw new KeyNotFoundException("It's not found \"employee\" key in HttpContext.Items dictionary");

            _repository.Employees.Delete(employee);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValitedateEmployeeForCompanyExistsActionFilterAttribute))]
        public async Task<IActionResult> UpdateAsync(Guid companyId, Guid id, 
            EmployeeForUpdatingDTO employeeUpdatingDTO)
        {
            var employeeEntity = HttpContext.Items["employee"] as Employee
                ?? throw new KeyNotFoundException("It's not found \"employee\" key in HttpContext.Items dictionary");

            _mapper.Map(employeeUpdatingDTO, employeeEntity);
            
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        [ServiceFilter(typeof(ValitedateEmployeeForCompanyExistsActionFilterAttribute))]
        public async Task<IActionResult> PatchAsync(Guid companyId, Guid id, 
            JsonPatchDocument<EmployeeForUpdatingDTO> employeeJsonPatchDoc)
        {
            if (employeeJsonPatchDoc is null)
            {
                _logger.LogInfo("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null");
            }

            var employeeEntity = HttpContext.Items["employee"] as Employee
                ?? throw new KeyNotFoundException("It's not found \"employee\" key in HttpContext.Items dictionary");

            var employeeToPatch = _mapper.Map<EmployeeForUpdatingDTO>(employeeEntity);

            employeeJsonPatchDoc.ApplyTo(employeeToPatch, ModelState);

            TryValidateModel(employeeToPatch);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeEntity);

            await _repository.SaveAsync();

            return NoContent();
        }

    }
}
