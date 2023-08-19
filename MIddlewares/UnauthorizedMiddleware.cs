using Microsoft.AspNetCore.Http;
using NendoroidApi.Response.Base;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NendoroidApi.MIddlewares
{
    public class UnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public UnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 401)
            {
                var result = JsonConvert.SerializeObject(new ResponseBase("Não autenticado."));
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
        }
    }
}
