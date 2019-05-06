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
using WebApiCore.DataTransferObject;

namespace WebApiCore.ApplicationAPI.APIs.UserProfiles
{
    public class GetApi
    {
        public class Query : IRequest<Result>
        {
            public Guid Id { get; set; }
        }

        public class Result : IWebApiResponse
        {
            public NestedModel.UserProfileModel Data { get; set; }
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
                public int Age { get; set; }
                public bool Sex { get; set; }
                public DateTime DateOfBirth { get; set; }
                public Guid? AvatarImage { get; set; }
                public string Name
                {
                    get { return GetName(); }
                }

                private string GetName()
                {
                    var name = (!string.IsNullOrEmpty(FirstName)) ? FirstName + " " : string.Empty;
                    name += (!string.IsNullOrEmpty(MiddleName)) ? MiddleName + " " : string.Empty;
                    name += (!string.IsNullOrEmpty(LastName)) ? LastName: string.Empty;

                    return name.Trim();
                }
            }            
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<DataModel.Models.UserProfile, NestedModel.UserProfileModel>()
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
                        context.Set<DataModel.Models.UserProfile>()
                            .Where(w => w.StatusId == true && w.Id == message.Id).FirstOrDefault();


                    if (item != null)
                    {
                        result.IsSuccessful = true;
                        result.Data = Mapper.Map<NestedModel.UserProfileModel>(item);
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
