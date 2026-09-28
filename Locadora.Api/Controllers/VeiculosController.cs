using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeiculosController : ControllerBase
{
    private readonly ApplicationContext _context;

    public VeiculosController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> GetAll()
    {
        var veiculos = await _context.Veiculos
            .AsNoTracking()
            .Include(v => v.Fabricante)
            .Include(v => v.Categoria)
            .OrderBy(v => v.Modelo)
            .Select(v => new VeiculoDto(
                v.Id, v.Placa, v.Modelo, v.AnoFabricacao, v.Quilometragem,
                v.FabricanteId, v.Fabricante.Nome, v.CategoriaId, v.Categoria.Nome))
            .ToListAsync();

        return Ok(veiculos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VeiculoDto>> GetById(int id)
    {
        var veiculo = await _context.Veiculos
            .AsNoTracking()
            .Include(v => v.Fabricante)
            .Include(v => v.Categoria)
            .Where(v => v.Id == id)
            .Select(v => new VeiculoDto(
                v.Id, v.Placa, v.Modelo, v.AnoFabricacao, v.Quilometragem,
                v.FabricanteId, v.Fabricante.Nome, v.CategoriaId, v.Categoria.Nome))
            .FirstOrDefaultAsync();

        if (veiculo is null)
            return NotFound(new { mensagem = $"Veículo com Id {id} não encontrado." });

        return Ok(veiculo);
    }

    /// <summary>
    /// Filtro: veículos de um fabricante (INNER JOIN explícito entre Veiculo, Fabricante e Categoria).
    /// </summary>
    [HttpGet("por-fabricante/{fabricanteId:int}")]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> GetByFabricante(int fabricanteId)
    {
        var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.Id == fabricanteId);
        if (!fabricanteExiste)
            return NotFound(new { mensagem = $"Fabricante com Id {fabricanteId} não encontrado." });

        var veiculos = await (
            from v in _context.Veiculos
            join f in _context.Fabricantes on v.FabricanteId equals f.Id
            join c in _context.Categorias on v.CategoriaId equals c.Id
            where f.Id == fabricanteId
            orderby v.Modelo
            select new VeiculoDto(v.Id, v.Placa, v.Modelo, v.AnoFabricacao, v.Quilometragem, f.Id, f.Nome, c.Id, c.Nome)
        ).AsNoTracking().ToListAsync();

        return Ok(veiculos);
    }

    /// <summary>
    /// Filtro: veículos disponíveis (sem aluguel em aberto), com filtro opcional por categoria.
    /// Usa LEFT OUTER JOIN entre Veiculo e Aluguel para excluir os que estão alugados no momento.
    /// </summary>
    [HttpGet("disponiveis")]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> GetDisponiveis([FromQuery] int? categoriaId)
    {
        var query =
            from v in _context.Veiculos
            join a in _context.Alugueis.Where(a => a.DataDevolucao == null)
                on v.Id equals a.VeiculoId into alugueisAtivos
            from aluguelAtivo in alugueisAtivos.DefaultIfEmpty()
            where aluguelAtivo == null
            select v;

        if (categoriaId.HasValue)
            query = query.Where(v => v.CategoriaId == categoriaId.Value);

        var veiculos = await query
            .Include(v => v.Fabricante)
            .Include(v => v.Categoria)
            .AsNoTracking()
            .OrderBy(v => v.Modelo)
            .Select(v => new VeiculoDto(
                v.Id, v.Placa, v.Modelo, v.AnoFabricacao, v.Quilometragem,
                v.FabricanteId, v.Fabricante.Nome, v.CategoriaId, v.Categoria.Nome))
            .ToListAsync();

        return Ok(veiculos);
    }

    [HttpPost]
    public async Task<ActionResult<VeiculoDto>> Create(VeiculoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var fabricante = await _context.Fabricantes.FindAsync(dto.FabricanteId);
        if (fabricante is null)
            return BadRequest(new { mensagem = $"Fabricante com Id {dto.FabricanteId} não existe." });

        var categoria = await _context.Categorias.FindAsync(dto.CategoriaId);
        if (categoria is null)
            return BadRequest(new { mensagem = $"Categoria com Id {dto.CategoriaId} não existe." });

        var veiculo = new Veiculo
        {
            Placa = dto.Placa.Trim().ToUpperInvariant(),
            Modelo = dto.Modelo.Trim(),
            AnoFabricacao = dto.AnoFabricacao,
            Quilometragem = dto.Quilometragem,
            FabricanteId = dto.FabricanteId,
            CategoriaId = dto.CategoriaId
        };

        _context.Veiculos.Add(veiculo);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = $"Já existe um veículo com a placa '{veiculo.Placa}'." });
        }

        var resultado = new VeiculoDto(
            veiculo.Id, veiculo.Placa, veiculo.Modelo, veiculo.AnoFabricacao, veiculo.Quilometragem,
            fabricante.Id, fabricante.Nome, categoria.Id, categoria.Nome);
        return CreatedAtAction(nameof(GetById), new { id = veiculo.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VeiculoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo is null)
            return NotFound(new { mensagem = $"Veículo com Id {id} não encontrado." });

        var fabricanteExiste = await _context.Fabricantes.AnyAsync(f => f.Id == dto.FabricanteId);
        if (!fabricanteExiste)
            return BadRequest(new { mensagem = $"Fabricante com Id {dto.FabricanteId} não existe." });

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExiste)
            return BadRequest(new { mensagem = $"Categoria com Id {dto.CategoriaId} não existe." });

        veiculo.Placa = dto.Placa.Trim().ToUpperInvariant();
        veiculo.Modelo = dto.Modelo.Trim();
        veiculo.AnoFabricacao = dto.AnoFabricacao;
        veiculo.Quilometragem = dto.Quilometragem;
        veiculo.FabricanteId = dto.FabricanteId;
        veiculo.CategoriaId = dto.CategoriaId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = $"Já existe um veículo com a placa '{veiculo.Placa}'." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var veiculo = await _context.Veiculos.FindAsync(id);
        if (veiculo is null)
            return NotFound(new { mensagem = $"Veículo com Id {id} não encontrado." });

        _context.Veiculos.Remove(veiculo);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: existem aluguéis vinculados a este veículo." });
        }

        return NoContent();
    }
}
