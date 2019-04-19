using EntityFramework.DbContextScope.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.DataAccess.DbScopeFactory
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly ApplicationDbContextOptions _options;

        public DbContextFactory(ApplicationDbContextOptions options)
        {
            _options = options;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : class, IDbContext
        {
            return new MainContext(_options) as TDbContext;
        }
    }
}
