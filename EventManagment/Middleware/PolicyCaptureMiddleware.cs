using Microsoft.AspNetCore.Authorization;

namespace EventManagment.Middleware
{
    public class PolicyCaptureMiddleware
    {
        private readonly RequestDelegate _next;

        public PolicyCaptureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var authorizeAttributes = endpoint?.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

            if (authorizeAttributes != null)
            {
                foreach (var authorizeAttribute in authorizeAttributes)
                {
                    if (authorizeAttribute.Policy != null)
                    {
                        context.Items["Policy"] = authorizeAttribute.Policy;
                        break;
                    }
                }
            }

            await _next(context);
        }
    }
}
