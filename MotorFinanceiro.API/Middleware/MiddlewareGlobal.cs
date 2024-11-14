using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;

namespace MotorFinanceiro.API.Middleware;

public class MiddlewareGlobal
{
    private readonly RequestDelegate _next;

    public MiddlewareGlobal(RequestDelegate nextMiddleware)
    {
        _next = nextMiddleware;
    }

    public async Task Invoke(HttpContext context)
    {
        //ida
        var sw = Stopwatch.StartNew();
        string responseBody = string.Empty;
        Stream originalResponseBody = context.Response.Body;
        try
        {
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            Console.WriteLine($"Tamanho antes: {context.Response.Body.Length}");

            await _next(context);

            Console.WriteLine($"Tamanho depois: {context.Response.Body.Length}");

            memStream.Position = 0;
            responseBody = new StreamReader(memStream).ReadToEnd();

            memStream.Position = 0;
            await memStream.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;

            //volta
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Tempo Erro = {sw.ElapsedMilliseconds}");
            context.Response.Body = originalResponseBody;
            var problemDetails = new ProblemDetails
            {
                Title = "Ocorreu um erro ao processar a requisição.",
                Status = (int)HttpStatusCode.BadRequest,
                Type = ex.GetBaseException().GetType().Name,
                Detail = ex.Message,
                Instance = context.Request.Host.Host,

            };

            context.Response.StatusCode = (int)problemDetails.Status;
            context.Response.ContentType = @"application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        finally
        {
            sw.Stop();
            Console.WriteLine($"Tempo total = {sw.ElapsedMilliseconds}");
        }
    }
}
