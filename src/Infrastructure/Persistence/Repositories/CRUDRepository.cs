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
    public class CRUDRepository<T> : CRUDRepositoryAsync<T>,  IRepository<T> where T : class ,ISoftDeletable
    {
        public CRUDRepository(ApplicationDbContext context) : base(context)
        {
        }

        public  IEnumerable<T> GetAll(bool withNoTracking = true, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);

            }
            if (withNoTracking == true)
                query.AsNoTracking();

            return query.ToList();
        }

        public T Add(T entity)
        {
             _context.Set<T>().Add(entity);
             _context.SaveChanges();
            return entity;
        }

        public  T Delete<IDType>(IDType id)
        {
            var foundEntity =  _context.Set<T>().Find(id);
            if (foundEntity == null) return null;

            _context.Set<T>().Remove(foundEntity);
             _context.SaveChanges();

            return foundEntity;
        }

        public  T Edit<O>(O id, T entity, Expression<Func<T, O>> keySelector)
        {
            var foundEntity =  _context.Set<T>().Find(id);
            if (foundEntity == null) return null;
            _context.Entry(entity).Property(keySelector).CurrentValue = id;
            _context.Entry(foundEntity).CurrentValues.SetValues(entity);
           var res =  _context.SaveChanges();
            return foundEntity;
        }



        public  T GetById<IDType>(IDType id, params Expression<Func<T, object>>[] includes)
        {
            var query = _context.Set<T>().AsQueryable();
            if (includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return  _context.Set<T>().Find(id);
        }

        public T SoftDelete<IDType>(IDType id)
        {
            var foundEntity = _context.Set<T>().Find(id);
            if (foundEntity == null)
                return null;
            else
                foundEntity.IsDeleted = true;
            _context.SaveChanges();

            return foundEntity;
        }

    }
}
