using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record CategoriaDto(int Id, string Nome, string? Descricao, decimal ValorDiariaBase);

public record CategoriaCreateDto
{
    [Required, MaxLength(60)]
    public string Nome { get; init; } = string.Empty;

    [MaxLength(255)]
    public string? Descricao { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor da diária base deve ser positivo.")]
    public decimal ValorDiariaBase { get; init; }
}
