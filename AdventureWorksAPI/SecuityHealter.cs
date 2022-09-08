using Microsoft.Extensions.Primitives;
namespace AdventureWorksAPI
{
    public class SecuityHealter
    {
        private readonly RequestDelegate next;
        public SecuityHealter(RequestDelegate next)
        {
            this.next = next;
        }
        public Task Inovoke(HttpContext context)
        {
            ontext.Response.Headers.Add("super-secure", new StringValues("enable"));
            return next(context);

        }
    }
}
