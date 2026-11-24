using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationContext _context;

    public CategoriasController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll(
        [FromQuery] string? nome = null,
        [FromQuery] decimal? diariaMin = null,
        [FromQuery] decimal? diariaMax = null)
    {
        if (diariaMin < 0)
            ModelState.AddModelError(nameof(diariaMin), "A diária mínima não pode ser negativa.");
        if (diariaMax < 0)
            ModelState.AddModelError(nameof(diariaMax), "A diária máxima não pode ser negativa.");
        if (diariaMin.HasValue && diariaMax.HasValue && diariaMin > diariaMax)
            ModelState.AddModelError(nameof(diariaMax), "A diária máxima deve ser maior ou igual à mínima.");
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var query = _context.Categorias.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            var termo = nome.Trim();
            query = query.Where(c => c.Nome.Contains(termo));
        }
        if (diariaMin.HasValue)
            query = query.Where(c => c.ValorDiariaBase >= diariaMin.Value);
        if (diariaMax.HasValue)
            query = query.Where(c => c.ValorDiariaBase <= diariaMax.Value);

        var categorias = await query
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaDto(c.Id, c.Nome, c.Descricao, c.ValorDiariaBase))
            .ToListAsync();

        return Ok(categorias);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoriaDto>> GetById(int id)
    {
        var categoria = await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoriaDto(c.Id, c.Nome, c.Descricao, c.ValorDiariaBase))
            .FirstOrDefaultAsync();

        if (categoria is null)
            return NotFound(new { mensagem = $"Categoria com Id {id} não encontrada." });

        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> Create(CategoriaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var categoria = new Categoria
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao?.Trim(),
            ValorDiariaBase = dto.ValorDiariaBase
        };

        _context.Categorias.Add(categoria);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = $"Já existe uma categoria chamada '{categoria.Nome}'." });
        }

        var resultado = new CategoriaDto(categoria.Id, categoria.Nome, categoria.Descricao, categoria.ValorDiariaBase);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CategoriaCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null)
            return NotFound(new { mensagem = $"Categoria com Id {id} não encontrada." });

        categoria.Nome = dto.Nome.Trim();
        categoria.Descricao = dto.Descricao?.Trim();
        categoria.ValorDiariaBase = dto.ValorDiariaBase;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = $"Já existe uma categoria chamada '{categoria.Nome}'." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria is null)
            return NotFound(new { mensagem = $"Categoria com Id {id} não encontrada." });

        _context.Categorias.Remove(categoria);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: existem veículos vinculados a esta categoria." });
        }

        return NoContent();
    }
}
