﻿using AutoMapper;
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

namespace WebApiCore.ApplicationAPI.APIs.Albums
{
    public class SearchApi
    {
        public class Query : PagingModel, IRequest<Result>
        {
            public string Name { get; set; }
        }

        public class Result : ISearchResult<NestedModel.AlbumModel>, IWebApiResponse
        {
            public IList<NestedModel.AlbumModel> SearchResultItems { get; set; }
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
            public class AlbumModel
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
                public string Description { get; set; }
                public DateTime? PublishDate { get; set; }
            }
        }

        // Mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Album, NestedModel.AlbumModel>()
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
                        context.Set<Album>()
                            .Where(w => w.StatusId == true);

                    if (!string.IsNullOrEmpty(message.Name))
                    {
                        query = query.Where(f => f.Name.Contains(message.Name)
                                            || f.Description.Contains(message.Name));
                    }
                  
                    var count = query.Count();
                    var items = query.OrderBy(s => s.Name).Skip(message.Skip).Take(message.Take).ProjectTo<NestedModel.AlbumModel>().ToList();

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
