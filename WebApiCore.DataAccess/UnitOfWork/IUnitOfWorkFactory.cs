using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataAccess.UnitOfWork
{
    public interface IUnitOfWorkFactory<out TUnitOfWork> where TUnitOfWork : IUnitOfWork
    {
        TUnitOfWork Create();
    }
}
