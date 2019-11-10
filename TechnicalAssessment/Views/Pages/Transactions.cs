using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TechnicalAssessment.Views.Pages
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class Transactions
    {
        private readonly RequestDelegate _next;

        public Transactions(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TransactionsExtensions
    {
        public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Transactions>();
        }
    }
}
