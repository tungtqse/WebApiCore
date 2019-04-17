using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess.UnitOfWork;
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

        public class NestedModel
        {
            public class CategoryModel
            {
                public string Name { get; set; }
            }
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IUnitOfWork _unitOfWork;

            public QueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }           

            public Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                using (var unit = _unitOfWork)
                {
                    var query =
                        unit.Repository<DataModel.Models.Category>()
                            .Where(w => w.StatusId == true);

                    if (!string.IsNullOrEmpty(message.Name))
                    {
                        query = query.Where(f => f.Name.Contains(message.Name));
                    }

                    var count = query.Count();
                    var items = query.OrderBy(s => s.Name).Select(s => new NestedModel.CategoryModel()
                    {
                        Name = s.Name
                    }).Skip(message.Skip).Take(message.Take).ToList();
                  

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
