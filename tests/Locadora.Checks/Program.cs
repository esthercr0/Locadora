using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq.Expressions;
using Locadora.Api.Controllers;
using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

// Verifica as consultas reais dos controllers, interrompendo antes de abrir a conexão.
// Não substitui os testes de persistência e HTTP contra o SQL Server.
var capture = new CaptureQuery();
var options = new DbContextOptionsBuilder<ApplicationContext>()
    .UseSqlServer("Server=unused;Database=LocadoraChecks;Integrated Security=true;TrustServerCertificate=true")
    .AddInterceptors(capture, new StopConnection()).Options;
using var context = new ApplicationContext(options);
var passed = 0;

async Task<string> SqlFor(Func<Task> action)
{
    capture.Last = null;
    try { await action(); throw new Exception("A consulta deveria parar antes de abrir a conexão."); }
    catch (ConnectionStopped) { }
    if (capture.Last is null) throw new Exception("A consulta não foi capturada.");
    return context.Fabricantes.AsQueryable().Provider.CreateQuery(capture.Last).ToQueryString();
}
void Check(bool result, string label)
{
    if (!result) throw new Exception($"FALHOU: {label}");
    passed++; Console.WriteLine($"OK: {label}");
}
CategoriasController CategoryController() => new(context)
{
    ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
    ProblemDetailsFactory = new CheckProblemDetailsFactory()
};

var fabricantes = new FabricantesController(context);
var sql = await SqlFor(async () => { await fabricantes.GetAll(" Toyota ", " Japão "); });
Check(sql.Contains("WHERE") && sql.Contains("[Nome] LIKE") && sql.Contains("[PaisOrigem] LIKE") && sql.Contains(" AND "), "Fabricantes: nome e país são combinados em SQL");
Check(sql.Contains("%Toyota%") && sql.Contains("%Japão%") && !sql.Contains("% Toyota %"), "Fabricantes: filtros parciais removem espaços externos");
sql = await SqlFor(async () => { await fabricantes.GetAll(); });
Check(!sql.Contains("WHERE"), "Fabricantes: chamada original sem filtros continua válida");

sql = await SqlFor(async () => { await CategoryController().GetAll("Eco", 100m, 200m); });
Check(sql.Contains("[Nome] LIKE") && sql.Contains("[ValorDiariaBase] >=") && sql.Contains("[ValorDiariaBase] <="), "Categorias: nome e limites inclusivos são aplicados no banco");
sql = await SqlFor(async () => { await CategoryController().GetAll(); });
Check(!sql.Contains("WHERE"), "Categorias: chamada original sem filtros continua válida");

foreach (var range in new[] { (-1m, 100m), (0m, -1m), (200m, 100m) })
{
    var response = await CategoryController().GetAll(diariaMin: range.Item1, diariaMax: range.Item2);
    Check(response.Result is ObjectResult { StatusCode: 400 }, $"Categorias: faixa inválida {range} retorna 400 sem acessar o banco");
}

bool Valid(decimal price)
{
    var dto = new CategoriaCreateDto { Nome = "Categoria", ValorDiariaBase = price };
    return Validator.TryValidateObject(dto, new ValidationContext(dto), new List<ValidationResult>(), true);
}
System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("pt-BR");
Check(Valid(0.01m) && Valid(99999999.99m) && !Valid(0m) && !Valid(100000000m), "Diária respeita os limites de decimal(10,2), inclusive em pt-BR");
Console.WriteLine($"{passed} verificações passaram. Nenhuma conexão de banco foi aberta.");

sealed class ConnectionStopped : Exception { }
sealed class CaptureQuery : IQueryExpressionInterceptor
{
    public Expression? Last { get; set; }
    public Expression QueryCompilationStarting(Expression expression, QueryExpressionEventData eventData) { Last = expression; return expression; }
}
sealed class StopConnection : DbConnectionInterceptor
{
    public override ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default) => throw new ConnectionStopped();
}
sealed class CheckProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null) => new() { Status = statusCode ?? 500, Title = title, Detail = detail, Type = type, Instance = instance };
    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null) => new(modelStateDictionary) { Status = statusCode ?? 400, Title = title, Detail = detail, Type = type, Instance = instance };
}
