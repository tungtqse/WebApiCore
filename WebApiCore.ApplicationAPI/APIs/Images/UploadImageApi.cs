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

namespace WebApiCore.ApplicationAPI.APIs.Images
{
    public class UploadImageApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public byte[] FileData { get; set; }
            public string FileName { get; set; }
            public int FileSize { get; set; }
            public string FileType { get; set; }
            public string FileExtension { get; set; }
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

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Command, AttachmentFile>()
                    .ForMember(m => m.Id, o => o.MapFrom(f => Guid.NewGuid()))
                    ;
            }
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

                using (var scope = _scopeFactory.Create())
                {
                    var context = scope.DbContexts.Get<MainContext>();

                    var attachmentId = Guid.NewGuid();

                    var image = new Image()
                    {
                        AltName = message.FileName,
                        Extension = message.FileExtension,
                        Size = message.FileSize,
                        Id = Guid.NewGuid(),
                        AttachmentFileId = attachmentId
                    };

                    var attchment = Mapper.Map<AttachmentFile>(message);
                    attchment.ParentId = image.Id;
                    attchment.Id = attachmentId;

                    image.AttachmentFile = attchment;

                    context.Set<Image>().Add(image);

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
