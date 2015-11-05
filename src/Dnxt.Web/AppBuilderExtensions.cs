using System.Threading;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

namespace Dnxt.Web
{
    public static class AppBuilderExtensions
    {
        public static void Run(this IApplicationBuilder app, AsyncAction<HttpContext> action, CancellationToken token)
        {
            app.Run(ctx => action(ctx, token));
        }
    }
}