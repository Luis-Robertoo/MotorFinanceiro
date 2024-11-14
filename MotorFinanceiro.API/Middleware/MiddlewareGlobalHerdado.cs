using System.Diagnostics;

namespace MotorFinanceiro.API.Middleware;

public class MiddlewareGlobalHerdado : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //ida
        var sw = Stopwatch.StartNew();

        await next(context);

        //volta
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);

    }
}
