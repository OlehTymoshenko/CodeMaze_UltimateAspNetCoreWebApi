using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository : IRepositoryBase<Employee> 
    {
        public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
        public Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);
        public Employee GetById(Guid companyId, Guid id, bool trackChanges);
        public Task<Employee> GetByIdAsync(Guid companyId, Guid id, bool trackChanges);
    }
}
