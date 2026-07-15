using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using ShoppingCart.Api;
using ShoppingCart.Api.ExceptionHandling;
using ShoppingCart.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

const string frontendCorsPolicy = "Frontend";

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails =
                new ValidationProblemDetails(
                    context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Dados de entrada inválidos.",
                    Detail =
                        "Corrija os campos informados e tente novamente.",
                    Instance =
                        context.HttpContext.Request.Path
                };

            problemDetails.Extensions["code"] =
                "validation_error";

            problemDetails.Extensions["traceId"] =
                context.HttpContext.TraceIdentifier;

            var result =
                new BadRequestObjectResult(problemDetails);

            result.ContentTypes.Add(
                "application/problem+json");

            return result;
        };
    });

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd(
            "traceId",
            context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

builder.Services.AddApplicationUseCases();

builder.Services.AddInfrastructure(
    builder.Configuration);

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? new[] { "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        frontendCorsPolicy,
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Shopping Cart API",
            Version = "v1",
            Description =
                "API REST para gerenciamento de carrinho de compras."
        });
});

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Shopping Cart API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors(frontendCorsPolicy);

app.MapControllers();

app.Run();

public partial class Program;