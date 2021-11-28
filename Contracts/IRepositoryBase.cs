using System;
using System.Linq;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        public IQueryable<T> FindAll(bool trackChanges);
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression,
            bool trackChanges);
        public void Create(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
