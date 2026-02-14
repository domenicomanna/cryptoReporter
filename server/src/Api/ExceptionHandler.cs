using System.Net;
using System.Text.Json;
using Api.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ExceptionHandler
{
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandler(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        Exception? exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is null)
            return;
        ProblemDetails problemDetails = GetProblemDetails(exception);
        httpContext.Response.ContentType = "application/problem+json";
        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        Stream stream = httpContext.Response.Body;
        await JsonSerializer.SerializeAsync(stream, problemDetails);
    }

    public ProblemDetails GetProblemDetails(Exception exception)
    {
        if (exception is ApiException)
        {
            ApiException? apiException = exception as ApiException;
            return new ProblemDetails
            {
                Status = (int)(apiException?.HttpStatusCode ?? HttpStatusCode.InternalServerError),
                Title = apiException?.ErrorMessage ?? "",
            };
        }

        if (_environment.IsDevelopment())
        {
            return new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = $"An excpetion occurred: {exception.Message}",
                Detail = exception.ToString(),
            };
        }

        return new ProblemDetails { Status = (int)HttpStatusCode.InternalServerError, Title = "An error occurred" };
    }
}
