using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using WebApiCore.DataAccess;
using WebApiCore.DataAccess.Repository;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataAccess.DbScopeFactory;
using EntityFramework.DbContextScope;
using EntityFramework.DbContextScope.Interfaces;

namespace WebApiCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddDbContext<MainContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), builder => builder.MigrationsAssembly(typeof(Startup).Assembly.FullName)));
            //services.AddTransient(typeof(IDbRepository<>), typeof(DbRepository<>));
            //services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            services.AddScoped(typeof(ApplicationDbContextOptions));
            services.AddTransient(typeof(IDbContextFactory),typeof(DbContextFactory));
            services.AddTransient(typeof(IDbContextScopeFactory), typeof(DbContextScopeFactory));

            // register MediatR Handler
            MediatorConfig(services, typeof(ApplicationAPI.APIs.CategoryAPI.SearchApi.QueryHandler));
        }

        private static void MediatorConfig(IServiceCollection services, Type type)
        {
            //Get Get Assembly of service project
            //var assemblyToScan = Assembly.GetAssembly(type); //..or whatever assembly you need
            var assemblyToScan = Assembly.GetAssembly(type);

            //Config services injection
            AddMediatRHandler(services, "Handler", assemblyToScan);
        }

        public static void AddMediatRHandler(IServiceCollection services, string endName, params Assembly[] assemblies)
        {
            var allPublicTypes = assemblies.SelectMany(x => x.GetExportedTypes()
                .Where(y => y.IsClass && !y.IsAbstract && !y.IsGenericType && y.Name.EndsWith("Handler")));

            foreach (var handler in allPublicTypes)
            {
                services.AddMediatR(handler.GetTypeInfo().Assembly);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
