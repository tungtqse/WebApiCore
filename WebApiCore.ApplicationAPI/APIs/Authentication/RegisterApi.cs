using EntityFramework.DbContextScope.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataModel.Models;
using WebApiCore.DataTransferObject;
using WebApiCore.Utility;

namespace WebApiCore.ApplicationAPI.APIs.Authentication
{
    public class RegisterApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        public class CommandResponse : IWebApiResponse
        {
            public CommandResponse()
            {
                Messages = new List<string>();
            }

            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }
        }

        public class NestedModel
        {

        }

        #region CommandHandler

        public class CommandHandler : IRequestHandler<Command, CommandResponse>
        {
            private readonly IDbContextScopeFactory _scopeFactory;
            private readonly IMediator _mediator;

            public CommandHandler(IDbContextScopeFactory scopeFactory, IMediator mediator)
            {
                _scopeFactory = scopeFactory;
                _mediator = mediator;
            }

            public async Task<CommandResponse> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = new CommandResponse();

                #region Validate

                var isValid = true;

                if (string.IsNullOrEmpty(message.UserName))
                {
                    isValid = false;
                    result.Messages.Add("Username is required");
                }

                if (string.IsNullOrEmpty(message.Password))
                {
                    isValid = false;
                    result.Messages.Add("Password is required");
                }

                if (isValid)
                {
                    if ((await _mediator.Send(new CheckUserApi.Query() { UserName = message.UserName, Email = message.Email })).IsExist)
                    {
                        isValid = false;
                        result.Messages.Add("Username or email was existed");
                    }
                    else
                    {
                        try
                        {
                            using (var scope = _scopeFactory.Create())
                            {
                                var context = scope.DbContexts.Get<MainContext>();

                                var profile = new UserProfile()
                                {
                                    Id = Guid.NewGuid(),
                                    Email = message.Email
                                };

                                var user = new UserCredential()
                                {
                                    Id = Guid.NewGuid(),
                                    UserName = message.UserName,
                                    Password = PasswordHelper.DecodePassword(message.Password),
                                    UserProfileId = profile.Id
                                };

                                context.Set<UserProfile>().Add(profile);
                                context.Set<UserCredential>().Add(user);

                                scope.SaveChanges();
                            }
                        }
                        catch (Exception ex)
                        {
                            isValid = false;
                            result.Messages.Add(ex.Message);
                        }
                    }
                }

                result.IsSuccessful = isValid;

                #endregion


                return await Task.FromResult(result);
            }
        }

        #endregion

    }


}
