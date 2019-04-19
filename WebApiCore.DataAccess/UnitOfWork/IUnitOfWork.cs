using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WebApiCore.DataAccess.Repository;

namespace WebApiCore.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangeAsyncs();
        IDbRepository<T> Repository<T>() where T : class;
        UnitOfWork Create();
    }
}
