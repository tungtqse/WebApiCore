using System;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using WebApiCore.DataAccess;
using WebApiCore.DataAccess.DbScopeFactory;
using EntityFramework.DbContextScope;
using EntityFramework.DbContextScope.Interfaces;
using WebApiCore.DataTransferObject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using WebApiCore.Utility;
using AutoMapper;

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
            services.AddAutoMapper();

            // configure basic authentication 
            //services.AddAuthentication("BasicAuthentication")
            //    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            // register MediatR Handler
            MediatorConfig(services, typeof(ApplicationAPI.MediatorModule), "Handler");

            // register AutoMapper
            AutoMapperConfig(services, typeof(ApplicationAPI.AutoMapperInitializer));

            // configure JWT
            JWTConfig(services);

            // configure HttpContext
            services.AddHttpContextAccessor();
        }

        private void AutoMapperConfig(IServiceCollection services, Type type)
        {
            var assemblyToScan = Assembly.GetAssembly(type);
            var assemblies = new[] { assemblyToScan };

            var profiles = assemblies.SelectMany(x => x.GetExportedTypes()
                .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                .Where(y => y.IsClass && !y.IsAbstract && !y.IsGenericType && y.Name.EndsWith("MappingProfile")));          

            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
        }

        private void JWTConfig(IServiceCollection services)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));

            // secretKey contains a secret passphrase only your server knows
            var secretKey = Configuration.GetSection("JWTSettings:SecretKey").Value;
            var issuer = Configuration.GetSection("JWTSettings:Issuer").Value;
            var audience = Configuration.GetSection("JWTSettings:Audience").Value;
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            // configure JWT
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey
            };
        });
        }

        private void MediatorConfig(IServiceCollection services, Type type, string name)
        {
            //Get Get Assembly of service project
            //var assemblyToScan = Assembly.GetAssembly(type); //..or whatever assembly you need
            var assemblyToScan = Assembly.GetAssembly(type);
            var assemblies = new[] { assemblyToScan };

            var allPublicTypes = assemblies.SelectMany(x => x.GetExportedTypes()
                .Where(y => y.IsClass && !y.IsAbstract && !y.IsGenericType && y.Name.EndsWith(name)));

            //Config services injection
            foreach (var handler in allPublicTypes)
            {
                services.AddMediatR(handler.GetTypeInfo().Assembly);
            }
        }      

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                    //when authorization has failed, should retrun a json message to client
                    if (error != null && error.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            State = "Unauthorized",
                            Msg = "token expired"
                        }));
                    }
                    //when orther error, retrun a error message json to client
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            State = "Internal Server Error",
                            Msg = error.Error.Message
                        }));
                    }
                    //when no error, do next.
                    else await next();
                });
            });            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            Utility.AppContext.Configure(app.ApplicationServices
                      .GetRequiredService<IHttpContextAccessor>());

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }        
    }
}
