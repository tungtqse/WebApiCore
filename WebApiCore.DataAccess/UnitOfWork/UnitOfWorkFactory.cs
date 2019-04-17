using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataAccess.UnitOfWork
{
    public class UnitOfWorkFactory<TUnitOfWork, TDbContext> : IUnitOfWorkFactory<TUnitOfWork>
        where TUnitOfWork : UnitOfWork, IUnitOfWork
        where TDbContext : MainContext
    {
        readonly string connectionStringName;

        public UnitOfWorkFactory(string connectionStringName)
        {
            this.connectionStringName = connectionStringName;
        }

        public virtual TUnitOfWork Create()
        {
            var ctx = Activator.CreateInstance(typeof(TDbContext), connectionStringName) as TDbContext;
            return Activator.CreateInstance(typeof(TUnitOfWork), ctx) as TUnitOfWork;
        }
    }
}
