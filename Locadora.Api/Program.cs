using Locadora.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro de validação nos dados enviados."
            };
            return new BadRequestObjectResult(problemDetails);
        };
    });

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ocorreu um erro inesperado ao processar a requisição.",
            Detail = app.Environment.IsDevelopment() ? feature?.Error.Message : null
        };

        context.Response.StatusCode = problem.Status.Value;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
