using Application.Common.Interfaces.Repositories;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class CRUDRepositoryAsync<T> : IAsyncRepository<T> where T : class ,ISoftDeletable
    {
        protected readonly ApplicationDbContext _context;

        public CRUDRepositoryAsync(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync(bool withNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);

            }
            if (withNoTracking == true) 
                query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> DeleteAsync<IDType>(IDType id)
        {
            var foundEntity = await _context.Set<T>().FindAsync(id);
            if (foundEntity == null) return null;

            _context.Set<T>().Remove(foundEntity);
            await _context.SaveChangesAsync();

            return foundEntity;
        }

        public async Task<T> EditAsync<O>(O id, T entity, Expression<Func<T, O>> keySelector)
        {
            var foundEntity = await _context.Set<T>().FindAsync(id);
            if (foundEntity == null) return null;
            _context.Entry(entity).Property(keySelector).CurrentValue = id;
            _context.Entry(foundEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return foundEntity;
        }



        public async Task<T?> GetByIdAsync<IDType>(IDType id, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await _context.Set<T>().FindAsync(id);
        }


        public async Task<T?> GetByIdAsync<IDType1, IDType2>(IDType1 id1, IDType2 id2, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await _context.Set<T>().FindAsync(id1, id2);
        }




        public virtual async Task<bool> SoftDeleteAsync(int id)
        {
            var foundEntity = _context.Set<T>().Find(id);
            if (foundEntity == null)
                return false;
            else
                foundEntity.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public ISoftDeletable SoftDelete(ISoftDeletable Entity)
        {
            Entity.IsDeleted = true;
            return Entity;
        }

        public IEnumerable<ISoftDeletable> SoftDelete(IEnumerable<ISoftDeletable> Entities)
        {
            foreach (var Entity in Entities)
                Entity.IsDeleted = true;

            return Entities;
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
