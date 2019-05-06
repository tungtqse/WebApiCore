using AutoMapper;
using EntityFramework.DbContextScope.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.DataAccess;
using WebApiCore.DataModel.Models;
using WebApiCore.DataTransferObject;

namespace WebApiCore.ApplicationAPI.APIs.UserProfiles
{
    public class UploadAvatarApi
    {
        public class Command : IRequest<CommandResponse>
        {
            public byte[] Data { get; set; }
            public string Name { get; set; }
            public int Size { get; set; }
            public string Type { get; set; }
            public Guid ParentId { get; set; }
            public string Extension { get; set; }
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
                    ;
            }
        }

        public class CommandHandler : IRequestHandler<Command, CommandResponse>
        {
            private readonly IDbContextScopeFactory _scopeFactory;

            public CommandHandler(IDbContextScopeFactory scopeFactory)
            {
                _scopeFactory = scopeFactory;
            }

            public async Task<CommandResponse> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = new CommandResponse();

                using (var scope = _scopeFactory.Create())
                {
                    var context = scope.DbContexts.Get<MainContext>();

                    var att = new AttachmentFile();
                    Mapper.Map(message, att);
                    att.Id = Guid.NewGuid();
                    att.ParentId = message.ParentId;
                    context.Set<AttachmentFile>().Add(att);

                    scope.SaveChanges();
                }

                return await Task.FromResult(result);
            }
        }
    }
}
