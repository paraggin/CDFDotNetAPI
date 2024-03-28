using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(object Id);
        T GetByIdAsNoTracking(object Id);
        Task<T> GetByIdAsync(object Id);
        T Insert(T entity);
        T Update(T entity);
        List<T> UpdateRange(List<T> entities);
        Task<T> UpdateAsync(T entity);
        void Delete(object Id);
        int SaveChanged();
        public IQueryable<T> GetWithIncludes(params Expression<Func<T, object>>[] includes);
        List<T> InsertRange(List<T> entities);
        void DeleteRange(IEnumerable<T> entities);
    }
}
