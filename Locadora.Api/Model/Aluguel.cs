using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Locadora.Api.Model;

public class Aluguel
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Cliente))]
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    [ForeignKey(nameof(Veiculo))]
    public int VeiculoId { get; set; }
    public Veiculo Veiculo { get; set; } = null!;

    public DateTime DataRetirada { get; set; }
    public DateTime DataPrevistaDevolucao { get; set; }
    public DateTime? DataDevolucao { get; set; }

    public int KmInicial { get; set; }
    public int? KmFinal { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorDiaria { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal? ValorTotal { get; set; }
}