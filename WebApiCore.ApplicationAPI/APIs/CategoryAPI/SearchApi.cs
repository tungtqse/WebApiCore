﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using EntityFramework.DbContextScope.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess;
using WebApiCore.DataTransferObject;

namespace WebApiCore.ApplicationAPI.APIs.CategoryAPI
{
    public class SearchApi 
    {
        public class Query : PagingModel, IRequest<Result>
        {
            public string Name { get; set; }
        }

        public class Result : ISearchResult<NestedModel.CategoryModel>, IWebApiResponse
        {
            public IList<NestedModel.CategoryModel> SearchResultItems { get; set; }
            public int Count { get; set; }
            public Result()
            {
                Messages = new List<string>();
            }

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }
        }

        // Mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<DataModel.Models.Category, NestedModel.CategoryModel>()
                    ;
            }
        }

        public class NestedModel
        {
            public class CategoryModel
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
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
                        context.Set<DataModel.Models.Category>()
                            .Where(w => w.StatusId == true);

                    if (!string.IsNullOrEmpty(message.Name))
                    {
                        query = query.Where(f => f.Name.Contains(message.Name));
                    }

                    var count = query.Count();
                    var items = query.OrderBy(s => s.Name).Skip(message.Skip).Take(message.Take).ProjectTo<NestedModel.CategoryModel>().ToList();                  

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
