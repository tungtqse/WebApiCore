using AutoMapper;
using EntityFramework.DbContextScope.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess;
using WebApiCore.DataModel.Models;
using WebApiCore.DataTransferObject;

namespace WebApiCore.DomainService.Features.Attachments
{
    public class UploadFileService
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid ParentId { get; set; }
            public string FileName { get; set; }
            public string FileType { get; set; }
            public string FileExtension { get; set; }
            public int FileSize { get; set; }
            public byte[] FileData { get; set; }
            public string Remarks { get; set; }
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
                RuleFor(f => f.ParentId).NotNull().NotEmpty().NotEqual(Guid.Empty)
                    .WithMessage("ParentId is null or empty");
                RuleFor(f => f.FileData).NotNull().NotEmpty()
                    .WithMessage("FileData is null or empty");
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, AttachmentFile>()
                    .ForMember(m=>m.Id, o=>o.MapFrom(f=>Guid.NewGuid()))
                    .ForMember(m => m.ParentId, o => o.MapFrom(f => f.ParentId))
                    ;
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResponse>
        {
            private readonly IDbContextScopeFactory _scopeFactory;
            private readonly IValidatorFactory _validatorFactory;

            public CommandHandler(IDbContextScopeFactory scopeFactory, IValidatorFactory validatorFactory)
            {
                _scopeFactory = scopeFactory;
                _validatorFactory = validatorFactory;
            }

            public async Task<CommandResponse> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = new CommandResponse();

                using (var scope = _scopeFactory.Create())
                {
                    try
                    {
                        var context = scope.DbContexts.Get<MainContext>();

                        var att = Mapper.Map<AttachmentFile>(message);

                        context.Set<AttachmentFile>().Add(att);

                        result.IsSuccessful = true;
                    }
                    catch (Exception ex)
                    {
                        result.Messages.Add(ex.Message);
                    }

                    scope.SaveChanges();

                }

                return await Task.FromResult(result);
            }
        }
    }
}
