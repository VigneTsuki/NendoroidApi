using Microsoft.AspNetCore.Http;
using NendoroidApi.Response.Base;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace NendoroidApi.MIddlewares
{
    public class ForbiddenMiddleware
    {
        private readonly RequestDelegate _next;

        public ForbiddenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 403)
            {
                var result = JsonConvert.SerializeObject(new ResponseBase(false, "Não autorizado."));
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }
        }
    }
}
