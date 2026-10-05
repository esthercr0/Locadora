# Relatório de Testes da API — Etapa 3

## 1. Objetivo

Este relatório apresenta os testes manuais realizados na API REST do Sistema de Locadora durante a Etapa 3 do trabalho.

Os testes foram executados por meio do **Swagger / OpenAPI**, com o objetivo de validar:

- funcionamento dos endpoints;
- operações CRUD;
- códigos HTTP;
- regras de negócio;
- filtros e consultas;
- tratamento de erros;
- registro de devolução;
- integração entre API, Entity Framework Core e SQL Server Express.

---

## 2. Ambiente de Testes

| Item | Tecnologia |
|---|---|
| Backend | ASP.NET Core / C# |
| ORM | Entity Framework Core |
| Banco de Dados | SQL Server Express |
| Testes da API | Swagger / OpenAPI |
| Ambiente | Localhost |
| Endpoint Swagger | `/swagger/index.html` |

---

## 3. Resumo dos Resultados

Durante os testes foram obtidos os seguintes códigos HTTP:

| Código | Significado | Resultado |
|:---:|---|:---:|
| `200` | Consulta realizada com sucesso | ✅ |
| `201` | Recurso criado com sucesso | ✅ |
| `204` | Atualização/exclusão realizada | ✅ |
| `400` | Requisição inválida | ✅ |
| `404` | Recurso não encontrado | ✅ |
| `409` | Conflito/regra de negócio | ✅ |

---

# 4. Testes do Swagger

## T01 — Integração com Swagger

**Objetivo:** verificar se a documentação interativa da API foi carregada corretamente.

**Procedimento:** acessar `/swagger/index.html`.

**Resultado esperado:** exibição dos controllers e endpoints da API.

**Resultado obtido:** Swagger carregado corretamente.

**Status:** ✅ APROVADO

**Evidência:** `01-swagger-integrado.png`

---

# 5. Testes de Fabricantes

## T02 — Cadastro de fabricante

**Método:** `POST`

**Endpoint:** `/api/Fabricantes`

**Dados utilizados:**

```json
{
  "nome": "Subaru",
  "paisOrigem": "Japão"
}
```

**Resultado esperado:** `201 Created`

**Resultado obtido:** `201 Created`

**Status:** ✅ APROVADO

**Evidência:** `02-fabricante-post-201.png`

---

## T03 — Consulta de fabricante por ID

**Método:** `GET`

**Endpoint:** `/api/Fabricantes/{id}`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `03-fabricante-get-id-200.png`

---

## T04 — Atualização de fabricante

**Método:** `PUT`

**Endpoint:** `/api/Fabricantes/{id}`

**Resultado esperado:** `204 No Content`

**Resultado obtido:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `04-fabricante-put-204.png`

---

## T05 — Fabricante duplicado

Foi realizada uma tentativa de cadastro utilizando um fabricante já existente.

**Resultado esperado:** `409 Conflict`

**Resultado obtido:** `409 Conflict`

**Status:** ✅ APROVADO

**Evidência:** `05-fabricante-conflito-409.png`

---

# 6. Testes de Categorias

## T06 — Cadastro de categoria

**Método:** `POST`

**Endpoint:** `/api/Categorias`

**Resultado esperado:** `201 Created`

**Resultado obtido:** `201 Created`

**Status:** ✅ APROVADO

**Evidência:** `06-categoria-post-201.png`

---

## T07 — Consulta de categoria

**Método:** `GET`

**Endpoint:** `/api/Categorias/{id}`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `07-categoria-get-200.png`

---

## T08 — Atualização de categoria

**Método:** `PUT`

**Endpoint:** `/api/Categorias/{id}`

**Resultado esperado:** `204 No Content`

**Resultado obtido:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `08-categoria-put-204.png`

---

# 7. Testes de Clientes

## T09 — Cadastro de cliente

**Método:** `POST`

**Endpoint:** `/api/Clientes`

**Resultado esperado:** `201 Created`

**Resultado obtido:** `201 Created`

**Status:** ✅ APROVADO

**Evidência:** `09-cliente-post-201.png`

---

## T10 — Consulta de cliente

**Método:** `GET`

**Endpoint:** `/api/Clientes/{id}`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `10-cliente-get-200.png`

---

## T11 — Atualização de cliente

**Método:** `PUT`

**Endpoint:** `/api/Clientes/{id}`

**Resultado esperado:** `204 No Content`

**Resultado obtido:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `11-cliente-put-204.png`

---

# 8. Testes de Veículos

## T12 — Cadastro de veículo

**Método:** `POST`

**Endpoint:** `/api/Veiculos`

**Resultado esperado:** `201 Created`

**Resultado obtido:** `201 Created`

**Status:** ✅ APROVADO

**Evidência:** `12-veiculo-post-201.png`

---

## T13 — Consulta de veículo

**Método:** `GET`

**Endpoint:** `/api/Veiculos/{id}`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `13-veiculo-get-200.png`

---

## T14 — Atualização de veículo

**Método:** `PUT`

**Endpoint:** `/api/Veiculos/{id}`

**Resultado esperado:** `204 No Content`

**Resultado obtido:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `14-veiculo-put-204.png`

---

# 9. Testes de Aluguéis

## T15 — Cadastro de aluguel

**Método:** `POST`

**Endpoint:** `/api/Alugueis`

**Resultado esperado:** `201 Created`

**Resultado obtido:** `201 Created`

**Status:** ✅ APROVADO

**Evidência:** `15-aluguel-post-201.png`

---

## T16 — Consulta de aluguel

**Método:** `GET`

**Endpoint:** `/api/Alugueis/{id}`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `16-aluguel-get-200.png`

---

## T17 — Atualização de aluguel

**Método:** `PUT`

**Endpoint:** `/api/Alugueis/{id}`

**Resultado esperado:** `204 No Content`

**Resultado obtido:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `17-aluguel-put-204.png`

---

## T18 — Registro de devolução

**Método:** `PUT`

**Endpoint:** `/api/Alugueis/{id}/devolucao`

**Resultado esperado:** `204 No Content`

**Resultado obtido:** `204 No Content`

**Status:** ✅ APROVADO

A operação registrou a devolução e atualizou as informações relacionadas ao aluguel e à quilometragem do veículo.

**Evidência:** `18-aluguel-devolucao-204.png`

---

# 10. Testes dos Filtros

## T19 — Veículos por fabricante

**Endpoint:** `GET /api/Veiculos/por-fabricante/{fabricanteId}`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `19-filtro-veiculos-fabricante-200.png`

---

## T20 — Veículos disponíveis

**Endpoint:** `GET /api/Veiculos/disponiveis`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `20-filtro-veiculos-disponiveis-200.png`

---

## T21 — Clientes sem aluguel ativo

**Endpoint:** `GET /api/Clientes/sem-aluguel-ativo`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `21-filtro-clientes-sem-aluguel-200.png`

---

## T22 — Aluguéis por período

**Endpoint:** `GET /api/Alugueis/por-periodo`

Período utilizado:

```text
inicio = 2026-10-01T00:00:00
fim    = 2026-10-31T23:59:59
```

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

**Evidência:** `22-filtro-alugueis-periodo-200.png`

---

## T23 — Faturamento por categoria

**Endpoint:** `GET /api/Alugueis/faturamento-por-categoria`

**Resultado esperado:** `200 OK`

**Resultado obtido:** `200 OK`

**Status:** ✅ APROVADO

A consulta utiliza `INNER JOIN` entre Aluguel, Veículo e Categoria e agrupa os dados para cálculo do faturamento.

**Evidência:** `23-filtro-faturamento-categoria-200.png`

---

# 11. Testes de Tratamento de Erros

## T24 — Período inválido

Foi informado um período no qual a data final era anterior à data inicial.

**Endpoint:** `GET /api/Alugueis/por-periodo`

**Resultado esperado:** `400 Bad Request`

**Resultado obtido:** `400 Bad Request`

**Status:** ✅ APROVADO

**Evidência:** `24-validacao-periodo-invalido-400.png`

---

## T25 — Recurso inexistente

Foi realizada uma consulta utilizando:

```text
id = 99999
```

**Endpoint:** `GET /api/Alugueis/99999`

**Resultado esperado:** `404 Not Found`

**Resultado obtido:** `404 Not Found`

**Status:** ✅ APROVADO

**Evidência:** `25-recurso-inexistente-404.png`

---

# 12. Testes de Exclusão

Os registros de teste foram removidos respeitando a ordem dos relacionamentos existentes no banco de dados.

## T26 — Exclusão de aluguel

**Resultado:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `26-aluguel-delete-204.png`

---

## T27 — Exclusão de veículo

**Resultado:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `27-veiculo-delete-204.png`

---

## T28 — Exclusão de cliente

**Resultado:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `28-cliente-delete-204.png`

---

## T29 — Exclusão de categoria

**Resultado:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `29-categoria-delete-204.png`

---

## T30 — Exclusão de fabricante

**Resultado:** `204 No Content`

**Status:** ✅ APROVADO

**Evidência:** `30-fabricante-delete-204.png`

---

# 13. Resultado Geral

| Grupo de testes | Resultado |
|---|:---:|
| Integração Swagger | ✅ Aprovado |
| Fabricantes | ✅ Aprovado |
| Categorias | ✅ Aprovado |
| Clientes | ✅ Aprovado |
| Veículos | ✅ Aprovado |
| Aluguéis | ✅ Aprovado |
| Devolução | ✅ Aprovado |
| 5 filtros obrigatórios | ✅ Aprovado |
| Validação 400 | ✅ Aprovado |
| Tratamento 404 | ✅ Aprovado |
| Tratamento 409 | ✅ Aprovado |
| Exclusões | ✅ Aprovado |

---

# 14. Conclusão

Os testes manuais realizados através do Swagger demonstraram que os principais recursos da API estão funcionando conforme esperado.

Foram validadas as operações CRUD, o processo de aluguel e devolução, as cinco consultas específicas, os relacionamentos entre as entidades e o tratamento de diferentes códigos HTTP.

Durante os testes do endpoint de faturamento por categoria foi identificada uma incompatibilidade de tradução de uma consulta `GroupBy` pelo Entity Framework Core. A consulta foi ajustada mantendo os `INNER JOINs` entre Aluguel, Veículo e Categoria e realizando o agrupamento após a materialização dos dados.

Após a correção, o endpoint foi novamente testado e retornou `200 OK`, concluindo com sucesso a bateria de testes da Etapa 3.
