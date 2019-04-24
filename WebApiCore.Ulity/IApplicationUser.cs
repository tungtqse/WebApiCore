using System;

namespace WebApiCore.Utility
{
    public interface IApplicationUser
    {
        Guid GetUserId();
        string GetUserName();
        string GetUserEmail();
    }
}