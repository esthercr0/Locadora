using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record VeiculoDto(
    int Id,
    string Placa,
    string Modelo,
    int AnoFabricacao,
    int Quilometragem,
    int FabricanteId,
    string FabricanteNome,
    int CategoriaId,
    string CategoriaNome);

public record VeiculoCreateDto
{
    [Required, MaxLength(8)]
    public string Placa { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string Modelo { get; init; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Ano de fabricação inválido.")]
    public int AnoFabricacao { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem não pode ser negativa.")]
    public int Quilometragem { get; init; }

    public int FabricanteId { get; init; }

    public int CategoriaId { get; init; }
}
