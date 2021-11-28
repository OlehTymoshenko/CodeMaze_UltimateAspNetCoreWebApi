using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Contracts;
using Entities;

namespace Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext _repositoryContext;

        public RepositoryBase(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public void Create(T entity) =>
            _repositoryContext.Set<T>().Add(entity);

        public void Delete(T entity) =>
            _repositoryContext.Set<T>().Remove(entity);

        public void Update(T entity) =>
            _repositoryContext.Set<T>().Update(entity);

        public IQueryable<T> FindAll(bool trackChanges) =>
            trackChanges switch
            {
                true => _repositoryContext.Set<T>(),
                false => _repositoryContext.Set<T>()
                            .AsNoTracking()
            };

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
            trackChanges switch
            {
                true => _repositoryContext.Set<T>().
                            Where(expression),
                false => _repositoryContext.Set<T>()
                            .Where(expression)
                            .AsNoTracking()
            };

    }
}
