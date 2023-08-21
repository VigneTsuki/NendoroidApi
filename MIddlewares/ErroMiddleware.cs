using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using NendoroidApi.Response.Base;
using Newtonsoft.Json;

namespace NendoroidApi.MIddlewares
{
    public class ErroMiddleware
    {
        private readonly RequestDelegate next;

        public ErroMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            ResponseBase responseBase;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                responseBase = new ResponseBase(false, $"{ex.Message} {ex?.InnerException?.Message}");
            else
                responseBase = new ResponseBase(false, "Ocorreu um erro no processamento da sua requisição. Por favor tente novamente mais tarde.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(responseBase);
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(result);
        }
    }
}
