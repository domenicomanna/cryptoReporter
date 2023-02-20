using System.Net;

namespace Api.Common.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode HttpStatusCode { get; set; }
    public string ErrorMessage { get; set; } = String.Empty;

    public ApiException() { }

    public ApiException(HttpStatusCode httpStatusCode)
    {
        HttpStatusCode = httpStatusCode;
    }

    public ApiException(HttpStatusCode httpStatusCode, string errorMessage)
    {
        HttpStatusCode = httpStatusCode;
        ErrorMessage = errorMessage;
    }
}
