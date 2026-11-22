using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record CategoriaDto(int Id, string Nome, string? Descricao, decimal ValorDiariaBase);

public record CategoriaCreateDto
{
    [Required, MaxLength(60)]
    public string Nome { get; init; } = string.Empty;

    [MaxLength(255)]
    public string? Descricao { get; init; }

    [Range(typeof(decimal), "0.01", "99999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "A diária base deve estar entre 0,01 e 99.999.999,99.")]
    public decimal ValorDiariaBase { get; init; }
}
