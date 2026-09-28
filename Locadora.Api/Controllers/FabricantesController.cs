using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FabricantesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public FabricantesController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FabricanteDto>>> GetAll()
    {
        var fabricantes = await _context.Fabricantes
            .AsNoTracking()
            .OrderBy(f => f.Nome)
            .Select(f => new FabricanteDto(f.Id, f.Nome, f.PaisOrigem))
            .ToListAsync();

        return Ok(fabricantes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FabricanteDto>> GetById(int id)
    {
        var fabricante = await _context.Fabricantes
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new FabricanteDto(f.Id, f.Nome, f.PaisOrigem))
            .FirstOrDefaultAsync();

        if (fabricante is null)
            return NotFound(new { mensagem = $"Fabricante com Id {id} não encontrado." });

        return Ok(fabricante);
    }

    [HttpPost]
    public async Task<ActionResult<FabricanteDto>> Create(FabricanteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var fabricante = new Fabricante
        {
            Nome = dto.Nome.Trim(),
            PaisOrigem = dto.PaisOrigem?.Trim()
        };

        _context.Fabricantes.Add(fabricante);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = $"Já existe um fabricante chamado '{fabricante.Nome}'." });
        }

        var resultado = new FabricanteDto(fabricante.Id, fabricante.Nome, fabricante.PaisOrigem);
        return CreatedAtAction(nameof(GetById), new { id = fabricante.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FabricanteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var fabricante = await _context.Fabricantes.FindAsync(id);
        if (fabricante is null)
            return NotFound(new { mensagem = $"Fabricante com Id {id} não encontrado." });

        fabricante.Nome = dto.Nome.Trim();
        fabricante.PaisOrigem = dto.PaisOrigem?.Trim();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = $"Já existe um fabricante chamado '{fabricante.Nome}'." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fabricante = await _context.Fabricantes.FindAsync(id);
        if (fabricante is null)
            return NotFound(new { mensagem = $"Fabricante com Id {id} não encontrado." });

        _context.Fabricantes.Remove(fabricante);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: existem veículos vinculados a este fabricante." });
        }

        return NoContent();
    }
}
