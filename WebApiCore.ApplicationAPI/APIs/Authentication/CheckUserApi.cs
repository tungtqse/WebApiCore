using EntityFramework.DbContextScope.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataModel.Models;

namespace WebApiCore.ApplicationAPI.APIs.Authentication
{
    public class CheckUserApi
    {
        public class Query : IRequest<Result>
        {
            public string UserName { get; set; }
            public string Email { get; set; }
        }

        public class Result 
        {
            public bool IsExist { get; set; }
        }        

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IDbContextScopeFactory _scopeFactory;

            public QueryHandler(IDbContextScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            public Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = new Result();

                using (var scope = _scopeFactory.CreateReadOnly())
                {
                    var context = scope.DbContexts.Get<MainContext>();

                    var query = (from user in context.Set<UserCredential>()
                                 join profile in context.Set<UserProfile>()
                                 on user.UserProfileId equals profile.Id
                                 where user.StatusId == true
                                 && profile.StatusId == true
                                 && user.UserName.Equals(message.UserName, StringComparison.OrdinalIgnoreCase)
                                 && profile.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase)
                                 select user.Id);

                    result.IsExist = query.Any();
                }
                
                return Task.FromResult(result);
            }
        }
    }
}
