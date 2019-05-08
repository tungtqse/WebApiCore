using AutoMapper;
using EntityFramework.DbContextScope.Interfaces;
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

namespace WebApiCore.ApplicationAPI.APIs.Movies
{
    public class CreateApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Title { get; set; }
            public string Code { get; set; }
            public double Duration { get; set; }
            public string Description { get; set; }
            public double Rate { get; set; }
            public DateTime? PublishDate { get; set; }
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
                RuleFor(f => f.Title).NotNull().NotEmpty()
                    .WithMessage("Title is null or empty");
                RuleFor(f => f.Code).NotNull().NotEmpty()
                    .WithMessage("Code is null or empty");
            }
        }

        // Mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, Movie>()
                    .ForMember(m=>m.Id, o=>o.MapFrom(f=>Guid.NewGuid()))
                    ;
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

                using (var scope = _scopeFactory.Create())
                {
                    var context = scope.DbContexts.Get<MainContext>();

                    isValid = context.Set<Movie>().Any(f =>f.Code.Equals(message.Code, StringComparison.OrdinalIgnoreCase));

                    if (!isValid)
                    {
                        var movie = Mapper.Map<Movie>(message);
                        context.Set<Movie>().Add(movie);
                        isValid = true;
                    }
                    else
                    {
                        result.Messages.Add("Code is existed");
                    }

                    scope.SaveChanges();
                }

                result.IsSuccessful = isValid;

                #endregion


                return Task.FromResult(result);
            }
        }

        #endregion
    }
}
