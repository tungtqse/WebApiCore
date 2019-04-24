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

namespace WebApiCore.ApplicationAPI.APIs.CategoryAPI
{
    public class UpdateApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
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

        #region CommandHandler

        public class CommandHandler : IRequestHandler<Command, CommandResponse>
        {
            private readonly IDbContextScopeFactory _scopeFactory;

            public CommandHandler(IDbContextScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            public Task<CommandResponse> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = new CommandResponse();

                #region Validate

                var isValid = true;

                if (string.IsNullOrEmpty(message.Name))
                {
                    isValid = false;
                    result.Messages.Add("Name is required");
                }

                if (isValid)
                {
                    try
                    {
                        using (var scope = _scopeFactory.Create())
                        {
                            var context = scope.DbContexts.Get<MainContext>();

                            
                            isValid = context.Set<DataModel.Models.Category>().Any(f => f.Id != message.Id && f.Name.Equals(message.Name, StringComparison.OrdinalIgnoreCase));

                            if (!isValid)
                            {
                                var category = context.Set<DataModel.Models.Category>().Where(f => f.Id == message.Id).FirstOrDefault();
                                category.Name = message.Name;

                                context.SaveChanges();
                            }
                            else
                            {
                                result.Messages.Add("Name is existed");
                            }                            
                        }
                    }
                    catch (Exception ex)
                    {
                        isValid = false;
                        result.Messages.Add(ex.Message);
                    }
                }

                result.IsSuccessful = isValid;

                #endregion


                return Task.FromResult(result);
            }
        }

        #endregion
    }
}
