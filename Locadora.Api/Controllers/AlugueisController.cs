using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlugueisController : ControllerBase
{
    private readonly ApplicationContext _context;

    public AlugueisController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AluguelDto>>> GetAll()
    {
        var alugueis = await _context.Alugueis
            .AsNoTracking()
            .Include(a => a.Cliente)
            .Include(a => a.Veiculo)
            .OrderByDescending(a => a.DataRetirada)
            .Select(a => new AluguelDto(
                a.Id, a.ClienteId, a.Cliente.Nome, a.VeiculoId, a.Veiculo.Placa, a.Veiculo.Modelo,
                a.DataRetirada, a.DataPrevistaDevolucao, a.DataDevolucao, a.KmInicial, a.KmFinal,
                a.ValorDiaria, a.ValorTotal))
            .ToListAsync();

        return Ok(alugueis);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AluguelDto>> GetById(int id)
    {
        var aluguel = await _context.Alugueis
            .AsNoTracking()
            .Include(a => a.Cliente)
            .Include(a => a.Veiculo)
            .Where(a => a.Id == id)
            .Select(a => new AluguelDto(
                a.Id, a.ClienteId, a.Cliente.Nome, a.VeiculoId, a.Veiculo.Placa, a.Veiculo.Modelo,
                a.DataRetirada, a.DataPrevistaDevolucao, a.DataDevolucao, a.KmInicial, a.KmFinal,
                a.ValorDiaria, a.ValorTotal))
            .FirstOrDefaultAsync();

        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com Id {id} não encontrado." });

        return Ok(aluguel);
    }

    /// <summary>
    /// Filtro: aluguéis retirados dentro de um período (JOIN via navegação/Include
    /// entre Aluguel, Cliente e Veiculo).
    /// </summary>
    [HttpGet("por-periodo")]
    public async Task<ActionResult<IEnumerable<AluguelDto>>> GetByPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fim)
    {
        if (fim < inicio)
            return BadRequest(new { mensagem = "A data final deve ser maior ou igual à data inicial." });

        var alugueis = await _context.Alugueis
            .AsNoTracking()
            .Include(a => a.Cliente)
            .Include(a => a.Veiculo)
            .Where(a => a.DataRetirada >= inicio && a.DataRetirada <= fim)
            .OrderBy(a => a.DataRetirada)
            .Select(a => new AluguelDto(
                a.Id, a.ClienteId, a.Cliente.Nome, a.VeiculoId, a.Veiculo.Placa, a.Veiculo.Modelo,
                a.DataRetirada, a.DataPrevistaDevolucao, a.DataDevolucao, a.KmInicial, a.KmFinal,
                a.ValorDiaria, a.ValorTotal))
            .ToListAsync();

        return Ok(alugueis);
    }

    /// <summary>
    /// Filtro: faturamento agrupado por categoria de veículo (INNER JOIN explícito
    /// entre Aluguel, Veiculo e Categoria, com agregação/GroupBy).
    /// </summary>
    [HttpGet("faturamento-por-categoria")]
    public async Task<ActionResult<IEnumerable<FaturamentoCategoriaDto>>> GetFaturamentoPorCategoria()
    {
        var relatorio = await (
            from a in _context.Alugueis
            join v in _context.Veiculos on a.VeiculoId equals v.Id
            join c in _context.Categorias on v.CategoriaId equals c.Id
            where a.ValorTotal != null
            group a by new { c.Id, c.Nome } into g
            select new FaturamentoCategoriaDto(g.Key.Id, g.Key.Nome, g.Count(), g.Sum(x => x.ValorTotal ?? 0))
        ).AsNoTracking().OrderByDescending(r => r.FaturamentoTotal).ToListAsync();

        return Ok(relatorio);
    }

    [HttpPost]
    public async Task<ActionResult<AluguelDto>> Create(AluguelCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (dto.DataPrevistaDevolucao < dto.DataRetirada)
            return BadRequest(new { mensagem = "A data prevista de devolução deve ser maior ou igual à data de retirada." });

        var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
        if (cliente is null)
            return BadRequest(new { mensagem = $"Cliente com Id {dto.ClienteId} não existe." });

        var veiculo = await _context.Veiculos.FindAsync(dto.VeiculoId);
        if (veiculo is null)
            return BadRequest(new { mensagem = $"Veículo com Id {dto.VeiculoId} não existe." });

        var veiculoOcupado = await _context.Alugueis
            .AnyAsync(a => a.VeiculoId == dto.VeiculoId && a.DataDevolucao == null);
        if (veiculoOcupado)
            return Conflict(new { mensagem = "Este veículo já está alugado e ainda não foi devolvido." });

        var aluguel = new Aluguel
        {
            ClienteId = dto.ClienteId,
            VeiculoId = dto.VeiculoId,
            DataRetirada = dto.DataRetirada,
            DataPrevistaDevolucao = dto.DataPrevistaDevolucao,
            KmInicial = dto.KmInicial,
            ValorDiaria = dto.ValorDiaria
        };

        _context.Alugueis.Add(aluguel);
        await _context.SaveChangesAsync();

        var resultado = new AluguelDto(
            aluguel.Id, cliente.Id, cliente.Nome, veiculo.Id, veiculo.Placa, veiculo.Modelo,
            aluguel.DataRetirada, aluguel.DataPrevistaDevolucao, aluguel.DataDevolucao,
            aluguel.KmInicial, aluguel.KmFinal, aluguel.ValorDiaria, aluguel.ValorTotal);
        return CreatedAtAction(nameof(GetById), new { id = aluguel.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, AluguelUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var aluguel = await _context.Alugueis.FindAsync(id);
        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com Id {id} não encontrado." });

        if (aluguel.DataDevolucao != null)
            return Conflict(new { mensagem = "Não é possível alterar um aluguel que já foi finalizado." });

        if (dto.DataPrevistaDevolucao < aluguel.DataRetirada)
            return BadRequest(new { mensagem = "A data prevista de devolução deve ser maior ou igual à data de retirada." });

        aluguel.DataPrevistaDevolucao = dto.DataPrevistaDevolucao;
        aluguel.ValorDiaria = dto.ValorDiaria;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    /// Registra a devolução do veículo: fecha o aluguel, atualiza a quilometragem
    /// final e calcula o valor total cobrado (dias corridos x valor da diária).
    /// </summary>
    [HttpPut("{id:int}/devolucao")]
    public async Task<IActionResult> RegistrarDevolucao(int id, AluguelDevolucaoDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var aluguel = await _context.Alugueis.FindAsync(id);
        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com Id {id} não encontrado." });

        if (aluguel.DataDevolucao != null)
            return Conflict(new { mensagem = "Este aluguel já foi devolvido." });

        if (dto.KmFinal < aluguel.KmInicial)
            return BadRequest(new { mensagem = "A quilometragem final não pode ser menor que a inicial." });

        if (dto.DataDevolucao < aluguel.DataRetirada)
            return BadRequest(new { mensagem = "A data de devolução não pode ser anterior à data de retirada." });

        var dias = Math.Max(1, (int)Math.Ceiling((dto.DataDevolucao - aluguel.DataRetirada).TotalDays));

        aluguel.DataDevolucao = dto.DataDevolucao;
        aluguel.KmFinal = dto.KmFinal;
        aluguel.ValorTotal = dias * aluguel.ValorDiaria;

        var veiculo = await _context.Veiculos.FindAsync(aluguel.VeiculoId);
        if (veiculo is not null)
            veiculo.Quilometragem = dto.KmFinal;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var aluguel = await _context.Alugueis.FindAsync(id);
        if (aluguel is null)
            return NotFound(new { mensagem = $"Aluguel com Id {id} não encontrado." });

        _context.Alugueis.Remove(aluguel);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
