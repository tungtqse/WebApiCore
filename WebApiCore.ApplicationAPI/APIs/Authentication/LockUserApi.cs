using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiCore.Common;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataModel.Models;

namespace WebApiCore.ApplicationAPI.APIs.Authentication
{
    public class LockUserApi
    {
        public class Command : IRequest
        {
            public Guid UserId { get; set; }
        }

        #region CommandHandler

        public class CommandHandler : IRequestHandler<Command>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Task<Unit> Handle(Command message, CancellationToken cancellationToken)
            {
                var result = new Unit();

                using (var unit = _unitOfWork)
                {
                    var user = unit.Repository<UserCredential>().Where(f => f.Id == message.UserId).FirstOrDefault();

                    user.AccessFailedCount += 1;

                    user.IsLock = user.AccessFailedCount == Constant.AccessFailedCount;

                    unit.SaveChanges();
                }

                return Task.FromResult(result);
            }
        }

        #endregion
    }
}
