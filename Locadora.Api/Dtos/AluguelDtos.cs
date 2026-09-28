using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record AluguelDto(
    int Id,
    int ClienteId,
    string ClienteNome,
    int VeiculoId,
    string VeiculoPlaca,
    string VeiculoModelo,
    DateTime DataRetirada,
    DateTime DataPrevistaDevolucao,
    DateTime? DataDevolucao,
    int KmInicial,
    int? KmFinal,
    decimal ValorDiaria,
    decimal? ValorTotal);

public record AluguelCreateDto
{
    public int ClienteId { get; init; }

    public int VeiculoId { get; init; }

    [Required]
    public DateTime DataRetirada { get; init; }

    [Required]
    public DateTime DataPrevistaDevolucao { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem inicial não pode ser negativa.")]
    public int KmInicial { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor da diária deve ser positivo.")]
    public decimal ValorDiaria { get; init; }
}

public record AluguelUpdateDto
{
    [Required]
    public DateTime DataPrevistaDevolucao { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O valor da diária deve ser positivo.")]
    public decimal ValorDiaria { get; init; }
}

public record AluguelDevolucaoDto
{
    [Required]
    public DateTime DataDevolucao { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "A quilometragem final não pode ser negativa.")]
    public int KmFinal { get; init; }
}

public record FaturamentoCategoriaDto(
    int CategoriaId,
    string CategoriaNome,
    int QuantidadeAlugueis,
    decimal FaturamentoTotal);
