﻿using EntityFramework.DbContextScope.Interfaces;
using FluentValidation;
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
    public class CreateApi
    {
        public class Command : IRequest<CommandResponse>
        {
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

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(f => f.Name).NotNull().NotEmpty()
                    .WithMessage("Name is null or empty");
            }
        }

        #region CommandHandler

        public class CommandHandler : IRequestHandler<Command, CommandResponse>
        {
            private readonly IDbContextScopeFactory _scopeFactory;
            private readonly IValidatorFactory _validatorFactory;

            public CommandHandler(IDbContextScopeFactory scopeFactory, IValidatorFactory validatorFactory)
            {
                _scopeFactory = scopeFactory;
                _validatorFactory = validatorFactory;
            }

            public Task<CommandResponse> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = new CommandResponse();

                #region Validate

                var isValid = true;

                try
                {
                    using (var scope = _scopeFactory.Create())
                    {
                        var context = scope.DbContexts.Get<MainContext>();

                        var blood = new Blood()
                        {
                            Name = message.Name
                        };

                        context.Set<Blood>().Add(blood);

                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    isValid = false;
                    result.Messages.Add(ex.Message);
                }

                result.IsSuccessful = isValid;

                #endregion


                return Task.FromResult(result);
            }
        }

        #endregion
    }
}
