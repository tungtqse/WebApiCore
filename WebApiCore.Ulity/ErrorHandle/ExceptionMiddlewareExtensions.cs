using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using WebApiCore.DataTransferObject;
using WebApiCore.LoggerService;

namespace WebApiCore.Utility.ErrorHandle
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async (context) =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                    if(error != null && error.Error != null)
                    {
                        logger.LogError($"Something went wrong: {error.Error}");
                        //when authorization has failed, should retrun a json message to client
                        if (error.Error is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";                            

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorDetails
                            {
                                Code = context.Response.StatusCode,
                                State = "Unauthorized",
                                Messages = new List<string>() { "Token Expired" }
                            }));
                        }
                        //when orther error, retrun a error message json to client
                        else 
                        {
                            context.Response.StatusCode = 500;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorDetails
                            {
                                Code = context.Response.StatusCode,
                                State = "Internal Server Error",
                                Messages = new List<string>() { error.Error.Message }
                            }));
                        }
                    }                    
                });
            });
        }
    }
}
