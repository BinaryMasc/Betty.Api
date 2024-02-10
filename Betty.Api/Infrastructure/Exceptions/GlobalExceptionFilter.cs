using Betty.Api.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Betty.Api.Infrastructure.Exceptions;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var statusCode = 500; // Internal Server Error

        // You can handle different types of exceptions differently
        if (exception is UnauthorizedAccessException)
        {
            statusCode = 401; // Unauthorized
        }
        else if (exception is ArgumentException || exception is ArgumentNullException || exception is ValidationException || exception is InvalidRequestException || exception is ItemNotFoundException)
        {
            statusCode = 400; // Bad Request
        }
        else if (exception is MySqlException || exception is DbException)
        {
            statusCode = 409; // Conflict
        }
        else _logger.LogError(exception, ("Error: " + exception.Message + "\nStackTrace: " + exception.StackTrace));


        // Set the response status code
        context.HttpContext.Response.StatusCode = statusCode;

        // Return a JSON response with an error message
        context.Result = new JsonResult(new
        {
            StatusCode = statusCode,
            Message = exception.Message
        });
    }
}