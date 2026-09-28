using Locadora.Api.Data;
using Locadora.Api.Dtos;
using Locadora.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Locadora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ClientesController(ApplicationContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        var clientes = await _context.Clientes
            .AsNoTracking()
            .OrderBy(c => c.Nome)
            .Select(c => new ClienteDto(c.Id, c.Nome, c.Cpf, c.Email, c.Telefone))
            .ToListAsync();

        return Ok(clientes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        var cliente = await _context.Clientes
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new ClienteDto(c.Id, c.Nome, c.Cpf, c.Email, c.Telefone))
            .FirstOrDefaultAsync();

        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente com Id {id} não encontrado." });

        return Ok(cliente);
    }

    /// <summary>
    /// Filtro: clientes que não possuem nenhum aluguel em aberto (LEFT OUTER JOIN
    /// entre Cliente e Aluguel, mantendo apenas quem não tem correspondência).
    /// </summary>
    [HttpGet("sem-aluguel-ativo")]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetSemAluguelAtivo()
    {
        var clientes = await (
            from c in _context.Clientes
            join a in _context.Alugueis.Where(a => a.DataDevolucao == null)
                on c.Id equals a.ClienteId into alugueisAtivos
            from aluguelAtivo in alugueisAtivos.DefaultIfEmpty()
            where aluguelAtivo == null
            orderby c.Nome
            select new ClienteDto(c.Id, c.Nome, c.Cpf, c.Email, c.Telefone)
        ).AsNoTracking().ToListAsync();

        return Ok(clientes);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create(ClienteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var cliente = new Cliente
        {
            Nome = dto.Nome.Trim(),
            Cpf = dto.Cpf.Trim(),
            Email = dto.Email.Trim(),
            Telefone = dto.Telefone?.Trim()
        };

        _context.Clientes.Add(cliente);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um cliente cadastrado com este CPF ou e-mail." });
        }

        var resultado = new ClienteDto(cliente.Id, cliente.Nome, cliente.Cpf, cliente.Email, cliente.Telefone);
        return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, resultado);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ClienteCreateDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente com Id {id} não encontrado." });

        cliente.Nome = dto.Nome.Trim();
        cliente.Cpf = dto.Cpf.Trim();
        cliente.Email = dto.Email.Trim();
        cliente.Telefone = dto.Telefone?.Trim();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Já existe um cliente cadastrado com este CPF ou e-mail." });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound(new { mensagem = $"Cliente com Id {id} não encontrado." });

        _context.Clientes.Remove(cliente);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new { mensagem = "Não é possível excluir: existem aluguéis vinculados a este cliente." });
        }

        return NoContent();
    }
}
