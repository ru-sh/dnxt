using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.AspNet.Http;

namespace Dnxt.Web
{
    public class HttpRouter : Router<HttpContext>
    {
        public HttpRouter([NotNull] IReadOnlyCollection<IRouter<HttpContext>> routers) : base(routers)
        {
        }

        public HttpRouter([NotNull] Predicate<HttpContext> predicate, [NotNull] AsyncAction<HttpContext> action) : base(predicate, action)
        {
        }

        public HttpRouter([NotNull] Predicate<HttpContext> predicate, [NotNull] AsyncAction action) : base(predicate, action)
        {
        }
    }
}