using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Application.Common.Exceptions;
using ShoppingCart.Domain.Exceptions;

namespace ShoppingCart.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = MapException(exception);

        if (error.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Erro inesperado ao processar {Method} {Path}.",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Erro tratado ao processar {Method} {Path}.",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = error.StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = error.StatusCode,
            Title = error.Title,
            Detail = error.Detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["code"] = error.Code;
        problemDetails.Extensions["traceId"] =
            httpContext.TraceIdentifier;

        var problemWritten =
            await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails
                });

        if (!problemWritten)
        {
            httpContext.Response.ContentType =
                "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);
        }

        return true;
    }

    private static ErrorDetails MapException(
        Exception exception)
    {
        return exception switch
        {
            ValidationException => new ErrorDetails(
                StatusCodes.Status400BadRequest,
                "Dados de entrada inválidos.",
                exception.Message,
                "validation_error"),

            NotFoundException => new ErrorDetails(
                StatusCodes.Status404NotFound,
                "Recurso não encontrado.",
                exception.Message,
                "not_found"),

            DomainException => new ErrorDetails(
                StatusCodes.Status422UnprocessableEntity,
                "Regra de negócio inválida.",
                exception.Message,
                "business_rule_violation"),

            _ => new ErrorDetails(
                StatusCodes.Status500InternalServerError,
                "Erro interno do servidor.",
                "Ocorreu um erro inesperado ao processar a solicitação.",
                "internal_error")
        };
    }

    private sealed record ErrorDetails(
        int StatusCode,
        string Title,
        string Detail,
        string Code);
}