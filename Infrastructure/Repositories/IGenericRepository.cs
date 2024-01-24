using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IGenericRepository<TEntity> : IDisposable where TEntity : class
    {
        TEntity Create(TEntity entity);
        Task<TEntity> CreateAsync(TEntity entity);
        IList<TEntity> CreateRannge(IList<TEntity> entity);
        Task<IList<TEntity>> CreateRanngeAsync(IList<TEntity> entity);
        Task<IList<TEntity>> GetAll();
        Task<TEntity> GetById(int id);
        Task<TEntity> GetByIdWithoutTracking(int id);
        IList<TEntity> Find(Func<TEntity, bool> predicate);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        IList<TEntity> UpdateRange(IList<TEntity> entities);
        Task<IList<TEntity>> UpdateRangeAsync(IList<TEntity> entities);
        void Delete(int id);
        Task DeleteAsync(TEntity entity);
        void DeleteRange(IList<TEntity> entities);
        Task DeleteRangeAsync(IList<TEntity> entities);
        IList<TEntity> SearchBy(string columName, string searchText);
        void Save();
        Task SaveAsync();
        TEntity UpdateExceptProperties(TEntity entity, params Expression<Func<TEntity, object>>[] excludedProperties);
    }
}
