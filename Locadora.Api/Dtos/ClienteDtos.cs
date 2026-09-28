using System.ComponentModel.DataAnnotations;

namespace Locadora.Api.Dtos;

public record ClienteDto(int Id, string Nome, string Cpf, string Email, string? Telefone);

public record ClienteCreateDto
{
    [Required, MaxLength(150)]
    public string Nome { get; init; } = string.Empty;

    [Required, StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter exatamente 11 dígitos.")]
    public string Cpf { get; init; } = string.Empty;

    [Required, MaxLength(150), EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; init; } = string.Empty;

    [MaxLength(20)]
    public string? Telefone { get; init; }
}
