using System.Threading.Tasks;

namespace Contracts
{
    public interface IRepositoryManager
    {
        public ICompanyRepository Companies { get; }
        public IEmployeeRepository Employees { get; }

        public void Save();
        public Task SaveAsync();

    }
}
