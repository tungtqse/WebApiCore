using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataModel.Models;
using WebApiCore.DataTransferObject;
using WebApiCore.Utility;

namespace WebApiCore.ApplicationAPI.APIs.Authentication
{
    public class LoginApi
    {
        public class Query : IRequest<Result>
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class Result : IWebApiResponse
        {
            public Result()
            {
                Messages = new List<string>();
            }

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;

            public QueryHandler(IUnitOfWork unitOfWork, IMediator mediator)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var result = new Result();

                using (var unit = _unitOfWork)
                {
                    var user =
                        unit.Repository<UserCredential>()
                            .Where(w => w.StatusId == true && w.UserName.Equals(message.UserName, StringComparison.OrdinalIgnoreCase))
                            .FirstOrDefault();

                    if(user != null)
                    {
                        var isValid = PasswordHelper.ComparePassword(message.Password, user.Password);

                        if (!isValid)
                        {
                            result.Messages.Add("Password is incorrect");
                            //await _mediator.Send(new LockUserApi.Command() { UserId = user.Id });
                        }

                        result.IsSuccessful = isValid;
                    }
                    else
                    {
                        result.Messages.Add("User not found");
                    }                    

                    return await Task.FromResult(result);
                }
            }
        }
    }
}
