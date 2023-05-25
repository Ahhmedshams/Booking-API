using Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public class CRUDRepositoryAsync<T> : IAsyncRepository<T> where T : class
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



        public async Task<T?> GetByIdAsync<IDType>(IDType id , params Expression<Func<T, object>>[] includes)
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
    }
}
