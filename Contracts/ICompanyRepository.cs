using Entities.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository : IRepositoryBase<Company> 
    {
        public IEnumerable<Company> GetAllCompanies(bool trackChanges);
        public Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
        public Company GetById(Guid id, bool trackChanges);
        public Task<Company> GetByIdAsync(Guid id, bool trackChanges); 
        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
        public Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
    }
}
