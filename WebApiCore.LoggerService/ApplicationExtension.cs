using ElmahCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiCore.LoggerService
{
    public static class ApplicationExtension
    {
        public static void ConfigureElmah(this IApplicationBuilder app)
        {
            app.UseElmah();            
        }
    }
}
