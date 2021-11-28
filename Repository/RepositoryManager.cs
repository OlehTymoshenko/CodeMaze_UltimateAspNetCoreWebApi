using Contracts;
using Entities;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;

        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;


        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }


        public ICompanyRepository Companies => 
            _companyRepository ??= new CompanyRepository(_repositoryContext);

        public IEmployeeRepository Employees => 
            _employeeRepository ??= new EmployeeRepository(_repositoryContext);


        public void Save() => _repositoryContext.SaveChanges();

        public Task SaveAsync() =>
            _repositoryContext.SaveChangesAsync();
    }
}
