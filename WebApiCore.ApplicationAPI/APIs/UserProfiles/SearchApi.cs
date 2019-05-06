using AutoMapper.QueryableExtensions;
using EntityFramework.DbContextScope.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess;
using WebApiCore.DataTransferObject;

namespace WebApiCore.ApplicationAPI.APIs.UserProfiles
{
    public class SearchApi
    {
        public class Query : PagingModel, IRequest<Result>
        {
            public string Name { get; set; }
        }

        public class Result : ISearchResult<NestedModel.UserProfileModel>, IWebApiResponse
        {
            public IList<NestedModel.UserProfileModel> SearchResultItems { get; set; }
            public int Count { get; set; }            

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }

            public Result()
            {
                Messages = new List<string>();
            }
        }

        public class NestedModel
        {
            public class UserProfileModel
            {
                public Guid Id { get; set; }
                public string FirstName { get; set; }
                public string LastName { get; set; }
                public string MiddleName { get; set; }
                public string Email { get; set; }
                public string Name
                {
                    get { return GetName(); }
                }

                private string GetName()
                {
                    var name = (!string.IsNullOrEmpty(FirstName)) ? FirstName + " " : string.Empty;
                    name += (!string.IsNullOrEmpty(MiddleName)) ? MiddleName + " " : string.Empty;
                    name += (!string.IsNullOrEmpty(LastName)) ? LastName : string.Empty;

                    return name.Trim();
                }
            }
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
                using (var scope = _scopeFactory.CreateReadOnly())
                {
                    var context = scope.DbContexts.Get<MainContext>();

                    var query =
                        context.Set<DataModel.Models.UserProfile>()
                            .Where(w => w.StatusId == true);

                    if (!string.IsNullOrEmpty(message.Name))
                    {
                        query = query.Where(f => f.Email.Contains(message.Name)
                                            || f.FirstName.Contains(message.Name)
                                            || f.MiddleName.Contains(message.Name)
                                            || f.LastName.Contains(message.Name));
                    }

                    var count = query.Count();
                    var items = query.OrderBy(s => s.Email).Skip(message.Skip).Take(message.Take).ProjectTo<NestedModel.UserProfileModel>().ToList();

                    var result = new Result()
                    {
                        Count = count,
                        SearchResultItems = items
                    };

                    return Task.FromResult(result);
                }
            }
        }
    }
}
