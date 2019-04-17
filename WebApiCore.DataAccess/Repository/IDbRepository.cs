using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiCore.DataAccess.Repository
{
    public interface IDbRepository
    {
    }

    //IQueryable<TEntity>, IEnumerable<TEntity>, IEnumerable, IQueryable, IAsyncEnumerableAccessor<TEntity>, IInfrastructure<IServiceProvider>, IListSource where TEntity : class

    public interface IDbRepository<T> : IDbRepository, IQueryable<T>, IEnumerable<T>, IEnumerable, IQueryable where T : class
    {
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        T Find(params object[] keyValues);
        Task<T> FindAsync(params object[] keyValues);
        
    }
}
