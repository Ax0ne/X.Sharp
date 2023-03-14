// Copyright (c) Ax0ne.  All Rights Reserved

namespace X.Sharp.Web.Middleware
{
    public class CustomMiddleware
    {
        private RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var cluture = context.Request.Query["cluture"];

            await _next(context);
        }
    }
}
