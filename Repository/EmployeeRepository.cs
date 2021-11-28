using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository.Extensions;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository 
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }


        public Employee GetById(Guid companyId, Guid id, bool trackChanges) =>
            FindByCondition(e => e.CompanyId == companyId && e.Id == id, trackChanges)
            .SingleOrDefault();

        public async Task<Employee> GetByIdAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId == companyId && e.Id == id, trackChanges)
                  .SingleOrDefaultAsync();

        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
            FindByCondition(e => e.CompanyId == companyId, trackChanges)
            .OrderBy(e => e.Name);

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
            EmployeeParameters employeeParameters, bool trackChanges)
        {
            var employees = await FindByCondition(e => e.CompanyId == companyId, trackChanges)
                                     .FilterByAge(employeeParameters.MinAge, employeeParameters.MaxAge)
                                     .SearchByName(employeeParameters.NameToSearch)
                                     .Sort(employeeParameters.OrderBy)
                                     .ToListAsync();

            return PagedList<Employee>.ToPagedList(employees, employeeParameters.PageSize, 
                employeeParameters.PageNumber);
        }
    }
}
