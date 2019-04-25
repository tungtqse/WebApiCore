using AutoMapper;
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

namespace WebApiCore.ApplicationAPI.APIs.BloodAPI
{
    public class GetApi
    {
        public class Query : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result : IWebApiResponse
        {
            public NestedModel.BloodModel Data { get; set; }
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
            public class BloodModel
            {
                public Guid Id { get; set; }
                public string Name { get; set; }
            }
        }

        // Mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Blood, NestedModel.BloodModel>()
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
                    var result = new Result();

                    var context = scope.DbContexts.Get<MainContext>();

                    var item =
                        context.Set<Blood>()
                            .Where(w => w.StatusId == true && w.Id == message.Id).FirstOrDefault();


                    if (item != null)
                    {
                        result.IsSuccessful = true;
                        result.Data = Mapper.Map<NestedModel.BloodModel>(item);
                    }
                    else
                    {
                        result.Messages.Add("Item not found");
                    }

                    return Task.FromResult(result);
                }
            }
        }
    }
}
