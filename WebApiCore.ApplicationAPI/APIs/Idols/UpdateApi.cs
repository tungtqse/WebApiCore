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

namespace WebApiCore.ApplicationAPI.APIs.Idols
{
    public class UpdateApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MiddleName { get; set; }
            public int? Age { get; set; }
            public bool Sex { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string NickName { get; set; }
            public double Weight { get; set; }
            public double Height { get; set; }
            public string Star { get; set; }
            public string Habit { get; set; }
            public Guid? BloodId { get; set; }
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
                RuleFor(f => f.FirstName).NotNull().NotEmpty()
                    .WithMessage("Name is null or empty");
                RuleFor(f => f.NickName).NotNull().NotEmpty()
                    .WithMessage("Nick Name is null or empty");
            }
        }

        // Mapping
        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, Idol>()
                    .ForMember(m => m.Id, o => o.Ignore())
                    .ForMember(m => m.CreatedDate, o => o.Ignore())
                    .ForMember(m => m.ModifiedDate, o => o.Ignore())
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

                    isValid = context.Set<Idol>().Any(f => f.Id != message.Id && f.NickName.Equals(message.NickName, StringComparison.OrdinalIgnoreCase));

                    if (!isValid)
                    {
                        var idol = context.Set<Idol>().Where(f => f.Id == message.Id).FirstOrDefault();
                        Mapper.Map(message, idol);
                        isValid = true;
                        scope.SaveChanges();
                    }
                    else
                    {
                        result.Messages.Add("Name is existed");
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
