﻿using Domain.Common;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T?> GetByIdAsync<IDType>(IDType id , params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync(bool withNoTracking = true, params Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task<T> DeleteAsync<IDType>(IDType id);
        Task<T> EditAsync<IDType>(IDType id, T entity, Expression<Func<T, IDType>> keySelector);
        Task<T?> GetByIdAsync<IDType1, IDType2>(IDType1 id1, IDType2 id2, params Expression<Func<T, object>>[] includes);
        Task<bool> SoftDeleteAsync(int id);
        ISoftDeletable SoftDelete(ISoftDeletable Entity);
        IEnumerable<ISoftDeletable> SoftDelete(IEnumerable<ISoftDeletable> Entities);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<int> SaveChangesAsync();
    }
}




