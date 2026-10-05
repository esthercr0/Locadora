# Documentação da API — Sistema de Locadora

## 1. Introdução

Esta documentação apresenta os endpoints REST desenvolvidos para o sistema de aluguel de veículos.

A API foi desenvolvida utilizando:

- C# / ASP.NET Core
- Entity Framework Core
- SQL Server Express
- Swagger / OpenAPI

A API permite o gerenciamento de fabricantes, categorias, clientes, veículos e aluguéis, além de disponibilizar consultas específicas para atender às regras de negócio do sistema.

---

# 2. Códigos HTTP utilizados

| Código | Significado | Utilização |
|---|---|---|
| 200 | OK | Consulta realizada com sucesso |
| 201 | Created | Registro criado com sucesso |
| 204 | No Content | Atualização ou exclusão realizada com sucesso |
| 400 | Bad Request | Dados enviados são inválidos |
| 404 | Not Found | Registro não encontrado |
| 409 | Conflict | Conflito com uma regra de negócio |
| 500 | Internal Server Error | Erro inesperado no servidor |

---

# 3. Fabricantes

Endpoint base:

`/api/Fabricantes`

## GET /api/Fabricantes

Retorna todos os fabricantes cadastrados.

**Resposta de sucesso:** `200 OK`

---

## GET /api/Fabricantes/{id}

Retorna um fabricante específico.

### Parâmetro

| Parâmetro | Tipo | Local | Descrição |
|---|---|---|---|
| id | int | rota | Identificador do fabricante |

### Possíveis respostas

- `200 OK` — fabricante encontrado.
- `404 Not Found` — fabricante não encontrado.

---

## POST /api/Fabricantes

Cadastra um novo fabricante.

### Exemplo de requisição

```json
{
  "nome": "Subaru",
  "paisOrigem": "Japão"
}
```

### Possíveis respostas

- `201 Created` — fabricante criado.
- `400 Bad Request` — dados inválidos.
- `409 Conflict` — fabricante já cadastrado.

---

## PUT /api/Fabricantes/{id}

Atualiza um fabricante existente.

### Possíveis respostas

- `204 No Content` — fabricante atualizado.
- `400 Bad Request` — dados inválidos.
- `404 Not Found` — fabricante não encontrado.
- `409 Conflict` — conflito com outro registro.

---

## DELETE /api/Fabricantes/{id}

Exclui um fabricante.

### Possíveis respostas

- `204 No Content` — fabricante excluído.
- `404 Not Found` — fabricante não encontrado.
- `409 Conflict` — fabricante possui registros relacionados.

---

# 4. Categorias

Endpoint base:

`/api/Categorias`

## GET /api/Categorias

Retorna todas as categorias cadastradas.

**Resposta:** `200 OK`

---

## GET /api/Categorias/{id}

Retorna uma categoria pelo seu identificador.

### Possíveis respostas

- `200 OK`
- `404 Not Found`

---

## POST /api/Categorias

Cadastra uma categoria de veículo.

### Exemplo

```json
{
  "nome": "SUV",
  "descricao": "Veículos utilitários esportivos",
  "valorDiariaBase": 180.00
}
```

### Possíveis respostas

- `201 Created`
- `400 Bad Request`
- `409 Conflict`

---

## PUT /api/Categorias/{id}

Atualiza uma categoria.

### Possíveis respostas

- `204 No Content`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

---

## DELETE /api/Categorias/{id}

Exclui uma categoria.

### Possíveis respostas

- `204 No Content`
- `404 Not Found`
- `409 Conflict`

---

# 5. Clientes

Endpoint base:

`/api/Clientes`

## GET /api/Clientes

Retorna todos os clientes.

**Resposta:** `200 OK`

---

## GET /api/Clientes/{id}

Consulta um cliente pelo identificador.

### Possíveis respostas

- `200 OK`
- `404 Not Found`

---

## POST /api/Clientes

Cadastra um cliente.

### Exemplo

```json
{
  "nome": "Beatriz Almeida",
  "cpf": "98765432100",
  "email": "beatriz.almeida.teste@email.com",
  "telefone": "31991234567"
}
```

### Possíveis respostas

- `201 Created`
- `400 Bad Request`
- `409 Conflict`

O código `409 Conflict` pode ocorrer, por exemplo, quando CPF ou e-mail já estão cadastrados.

---

## PUT /api/Clientes/{id}

Atualiza os dados de um cliente.

### Possíveis respostas

- `204 No Content`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

---

## DELETE /api/Clientes/{id}

Exclui um cliente.

### Possíveis respostas

- `204 No Content`
- `404 Not Found`
- `409 Conflict`

---

## GET /api/Clientes/sem-aluguel-ativo

Retorna clientes que não possuem aluguel ativo.

Este endpoint é utilizado para identificar clientes disponíveis para iniciar um novo aluguel.

### Resposta

`200 OK`

---

# 6. Veículos

Endpoint base:

`/api/Veiculos`

## GET /api/Veiculos

Retorna todos os veículos.

**Resposta:** `200 OK`

---

## GET /api/Veiculos/{id}

Consulta um veículo pelo identificador.

### Possíveis respostas

- `200 OK`
- `404 Not Found`

---

## POST /api/Veiculos

Cadastra um veículo.

### Exemplo

```json
{
  "placa": "SUB4A26",
  "modelo": "Forester",
  "anoFabricacao": 2024,
  "quilometragem": 18500,
  "fabricanteId": 4,
  "categoriaId": 4
}
```

Os valores de `fabricanteId` e `categoriaId` devem corresponder a registros existentes.

### Possíveis respostas

- `201 Created`
- `400 Bad Request`
- `409 Conflict`

---

## PUT /api/Veiculos/{id}

Atualiza os dados de um veículo.

### Possíveis respostas

- `204 No Content`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

---

## DELETE /api/Veiculos/{id}

Exclui um veículo.

### Possíveis respostas

- `204 No Content`
- `404 Not Found`
- `409 Conflict`

---

## GET /api/Veiculos/por-fabricante/{fabricanteId}

Retorna os veículos pertencentes a determinado fabricante.

### Parâmetro

| Parâmetro | Tipo | Descrição |
|---|---|---|
| fabricanteId | int | Identificador do fabricante |

### Resposta

`200 OK`

A consulta relaciona os dados de veículo com fabricante e demais informações necessárias.

---

## GET /api/Veiculos/disponiveis

Retorna veículos que não possuem aluguel ativo.

### Resposta

`200 OK`

O endpoint utiliza uma consulta baseada em `LEFT OUTER JOIN` para identificar veículos sem um aluguel ativo associado.

---

# 7. Aluguéis

Endpoint base:

`/api/Alugueis`

## GET /api/Alugueis

Retorna todos os aluguéis cadastrados.

Os dados retornados incluem informações relacionadas ao cliente e ao veículo.

**Resposta:** `200 OK`

---

## GET /api/Alugueis/{id}

Consulta um aluguel pelo identificador.

### Possíveis respostas

- `200 OK`
- `404 Not Found`

Exemplo de erro:

```json
{
  "mensagem": "Aluguel com Id 99999 não encontrado."
}
```

---

## POST /api/Alugueis

Registra um novo aluguel.

### Exemplo

```json
{
  "clienteId": 4,
  "veiculoId": 4,
  "dataRetirada": "2026-10-05T09:00:00",
  "dataPrevistaDevolucao": "2026-10-10T09:00:00",
  "kmInicial": 19000,
  "valorDiaria": 190.00
}
```

### Regras

- O cliente deve existir.
- O veículo deve existir.
- A data prevista de devolução não pode ser anterior à retirada.
- Um veículo com aluguel ativo não pode ser alugado novamente.

### Possíveis respostas

- `201 Created`
- `400 Bad Request`
- `409 Conflict`

---

## PUT /api/Alugueis/{id}

Atualiza informações de um aluguel ativo.

Podem ser atualizados:

- Data prevista de devolução.
- Valor da diária.

### Exemplo

```json
{
  "dataPrevistaDevolucao": "2026-10-12T09:00:00",
  "valorDiaria": 200.00
}
```

### Possíveis respostas

- `204 No Content`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

Um aluguel já finalizado não pode ser alterado.

---

## PUT /api/Alugueis/{id}/devolucao

Registra a devolução do veículo.

### Exemplo

```json
{
  "dataDevolucao": "2026-10-12T14:00:00",
  "kmFinal": 19350
}
```

### Funcionamento

Ao registrar a devolução:

1. A data de devolução é registrada.
2. A quilometragem final é armazenada.
3. O número de dias do aluguel é calculado.
4. O valor total do aluguel é calculado.
5. A quilometragem atual do veículo é atualizada.
6. O veículo volta a ser considerado disponível.

### Validações

- A quilometragem final não pode ser menor que a inicial.
- A data da devolução não pode ser anterior à retirada.
- Um aluguel já devolvido não pode ser devolvido novamente.

### Possíveis respostas

- `204 No Content`
- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`

---

## DELETE /api/Alugueis/{id}

Exclui um aluguel.

### Possíveis respostas

- `204 No Content`
- `404 Not Found`

---

# 8. Consultas e filtros

Foram implementadas cinco consultas específicas.

## 8.1 Veículos por fabricante

`GET /api/Veiculos/por-fabricante/{fabricanteId}`

Permite consultar veículos relacionados a determinado fabricante.

**Resposta:** `200 OK`

---

## 8.2 Veículos disponíveis

`GET /api/Veiculos/disponiveis`

Retorna veículos sem aluguel ativo.

**Resposta:** `200 OK`

---

## 8.3 Clientes sem aluguel ativo

`GET /api/Clientes/sem-aluguel-ativo`

Retorna clientes que não possuem aluguel em aberto.

**Resposta:** `200 OK`

---

## 8.4 Aluguéis por período

`GET /api/Alugueis/por-periodo?inicio={inicio}&fim={fim}`

Retorna os aluguéis cuja retirada ocorreu dentro do período informado.

### Parâmetros

| Parâmetro | Tipo | Descrição |
|---|---|---|
| inicio | DateTime | Início do período |
| fim | DateTime | Final do período |

### Validação

Caso `fim` seja anterior a `inicio`, a API retorna:

`400 Bad Request`

Exemplo:

```json
{
  "mensagem": "A data final deve ser maior ou igual à data inicial."
}
```

---

## 8.5 Faturamento por categoria

`GET /api/Alugueis/faturamento-por-categoria`

Agrupa os aluguéis finalizados por categoria e apresenta a quantidade de aluguéis e o faturamento correspondente.

**Resposta:** `200 OK`

---

# 9. JOINs utilizados

As consultas da API utilizam relacionamentos entre diferentes entidades.

## 9.1 INNER JOIN

O relatório:

`GET /api/Alugueis/faturamento-por-categoria`

utiliza `INNER JOIN` explícito entre:

`Aluguel → Veículo → Categoria`

Exemplo da implementação:

```csharp
from a in _context.Alugueis
join v in _context.Veiculos
    on a.VeiculoId equals v.Id
join c in _context.Categorias
    on v.CategoriaId equals c.Id
```

Esse relacionamento permite descobrir a categoria de cada veículo associado aos aluguéis e calcular o faturamento agrupado.

O agrupamento é realizado após a materialização dos dados para evitar incompatibilidades de tradução de `GroupBy` pelo Entity Framework.

---

## 9.2 LEFT OUTER JOIN

A consulta:

`GET /api/Veiculos/disponiveis`

utiliza a lógica de `LEFT OUTER JOIN`.

O objetivo é encontrar veículos que não possuem aluguel ativo.

A utilização desse tipo de JOIN é adequada porque permite manter os veículos no resultado mesmo quando não existe um aluguel ativo correspondente.

---

# 10. Tratamento de erros

A API possui tratamento para diferentes situações.

### 400 — Bad Request

Utilizado quando os dados enviados são inválidos ou violam uma regra de validação.

Exemplo:

```json
{
  "mensagem": "A data final deve ser maior ou igual à data inicial."
}
```

### 404 — Not Found

Utilizado quando um registro solicitado não existe.

Exemplo:

```json
{
  "mensagem": "Aluguel com Id 99999 não encontrado."
}
```

### 409 — Conflict

Utilizado quando uma operação entra em conflito com os dados ou regras existentes.

Exemplos:

- Fabricante duplicado.
- CPF ou e-mail já cadastrado.
- Tentativa de alugar veículo que já possui aluguel ativo.
- Tentativa de alterar aluguel já finalizado.

### 500 — Internal Server Error

Erros inesperados são tratados pela aplicação e retornados no formato `ProblemDetails`.

---

# 11. Swagger / OpenAPI

A API possui integração com Swagger para documentação e testes manuais.

Durante o desenvolvimento, a interface pode ser acessada em:

`/swagger`

ou:

`/swagger/index.html`

O Swagger permite:

- Visualizar todos os endpoints.
- Consultar os métodos HTTP.
- Visualizar os parâmetros.
- Enviar requisições.
- Informar JSON no corpo das requisições.
- Visualizar os códigos HTTP.
- Conferir os dados retornados pela API.

---

# 12. Conclusão

A API da Locadora implementa operações REST para gerenciamento das principais entidades do sistema e consultas específicas relacionadas às regras de negócio.

Foram implementadas operações CRUD, filtros, relacionamentos entre entidades, consultas utilizando diferentes tipos de JOIN, tratamento de erros e documentação/testes por meio do Swagger.

A integração entre ASP.NET Core, Entity Framework Core e SQL Server Express permite persistência e consulta dos dados de forma estruturada, enquanto o Swagger facilita a validação e documentação dos endpoints.
