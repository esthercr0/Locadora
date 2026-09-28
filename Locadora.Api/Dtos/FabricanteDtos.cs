using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record FabricanteDto(int Id, string Nome, string? PaisOrigem);

public record FabricanteCreateDto
{
    [Required, MaxLength(100)]
    public string Nome { get; init; } = string.Empty;

    [MaxLength(60)]
    public string? PaisOrigem { get; init; }
}
