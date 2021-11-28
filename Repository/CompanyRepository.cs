using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
            FindAll(trackChanges)
                .OrderBy(c => c)
                .ToList();

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                 .OrderBy(c => c.Name)
                 .ToListAsync();

        public Company GetById(Guid id, bool trackChanges) =>
            FindByCondition(c => c.Id == id, trackChanges)
            .SingleOrDefault();

        public async Task<Company> GetByIdAsync(Guid id, bool trackChanges) =>
            await FindByCondition(c => c.Id == id, trackChanges)
                  .SingleOrDefaultAsync();

        public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges) =>
            FindByCondition(c => ids.Contains(c.Id), trackChanges);

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
            await FindByCondition(c => ids.Contains(c.Id), trackChanges)
                  .OrderBy(c => c.Name)
                  .ToListAsync();
    }
}
