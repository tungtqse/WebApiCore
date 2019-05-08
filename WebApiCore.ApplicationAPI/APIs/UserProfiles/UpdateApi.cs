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
using WebApiCore.DataTransferObject;

namespace WebApiCore.ApplicationAPI.APIs.UserProfiles
{
    public class UpdateApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string MiddleName { get; set; }
            public string Email { get; set; }
            public int? Age { get; set; }
            public bool Sex { get; set; }
            public DateTime? DateOfBirth { get; set; }
        }

        public class CommandResponse : IWebApiResponse
        {
            public int Code { get; set; }
            public bool IsSuccessful { get; set; }
            public List<string> Messages { get; set; }

            public CommandResponse()
            {
                Messages = new List<string>();
            }            
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(f => f.Email).NotNull().NotEmpty()
                    .WithMessage("Email is null or empty");
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command,DataModel.Models.UserProfile>()
                    .ForMember(m=>m.AvatarImage, o => o.Ignore())
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

                    isValid = context.Set<DataModel.Models.UserProfile>().Any(f => f.Id != message.Id && f.Email.Equals(message.Email, StringComparison.OrdinalIgnoreCase));

                    if (!isValid)
                    {
                        var item = context.Set<DataModel.Models.UserProfile>().Where(f => f.Id == message.Id).FirstOrDefault();
                        Mapper.Map(message,item);
                        isValid = true;
                        scope.SaveChanges();
                    }
                    else
                    {
                        result.Messages.Add("Email is existed");
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
