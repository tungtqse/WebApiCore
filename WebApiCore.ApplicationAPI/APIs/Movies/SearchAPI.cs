using AutoMapper;
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
using WebApiCore.DataModel.Models;
using WebApiCore.DataTransferObject;

namespace WebApiCore.ApplicationAPI.APIs.Movies
{
    public class SearchApi
    {
        public class Query : PagingModel, IRequest<Result>
        {
            public string Title { get; set; }
            public string Code { get; set; }
        }

        public class Result : ISearchResult<NestedModel.MovieModel>, IWebApiResponse
        {
            public IList<NestedModel.MovieModel> SearchResultItems { get; set; }
            public int Count { get; set; }
            public Result()
            {
                Messages = new List<string>();
            }

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }
        }

        public class NestedModel
        {
            public class MovieModel
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public string Code { get; set; }
            }
        }

        // Mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Movie, NestedModel.MovieModel>()
                    ;
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
                        context.Set<Movie>()
                            .Where(w => w.StatusId == true);

                    if (!string.IsNullOrEmpty(message.Title))
                    {
                        query = query.Where(f => f.Title.Contains(message.Title));
                    }

                    if (!string.IsNullOrEmpty(message.Code))
                    {
                        query = query.Where(f => f.Code.Contains(message.Code));
                    }

                    var count = query.Count();
                    var items = query.OrderBy(s => s.Title).Skip(message.Skip).Take(message.Take).ProjectTo<NestedModel.MovieModel>().ToList();

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
