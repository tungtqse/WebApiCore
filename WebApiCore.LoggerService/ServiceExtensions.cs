using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.LoggerService
{
    public static class ServiceExtensions
    {
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddElmah<SqlErrorLog>(options =>
            {
                options.ConnectionString = "Data Source=.;Initial Catalog=PetProject2;Integrated Security=True"; // DB structure see here: https://bitbucket.org/project-elmah/main/downloads/ELMAH-1.2-db-SQLServer.sql
            });
        }
    }    
}
