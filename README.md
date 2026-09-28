# Locadora de Veículos

Sistema de aluguel de veículos desenvolvido em C# com Entity Framework, SQL Server Express e Swagger.

**Aluna:** Esther Caldeira Rivadalves
**Disciplina:** Tecnologias para Análise e Desenvolvimento de Sistemas – PUC Minas
**Trabalho individual — 30 pontos, entregue em 4 etapas**

## Tecnologias
- C# / .NET 10
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- SQL Server Express
- Swagger (a partir da Etapa 3)

## Estrutura do projeto
- `Locadora.Api/Model/`: classes de entidades
- `Locadora.Api/Data/ApplicationContext.cs`: contexto do Entity Framework
- `Locadora.Api/Migrations/`: migrations
- `Locadora.Api/Scripts/schema.sql`: script SQL gerado
- `Docs/`: diagramas, prints e relatórios de teste

## Como executar
1. Instale o SQL Server Express e ajuste a connection string em `Locadora.Api/appsettings.json`.
2. No Console do Gerenciador de Pacotes: `Update-Database`
3. *(a preencher...)*

---

## Status das etapas

| Etapa | Descrição | Status |
|---|---|---|
| 1 | Modelagem do banco de dados | Concluída |
| 2 | Implementação do backend (CRUD + filtros) | Pendente |
| 3 | Testes e documentação (Swagger) | Pendente |
| 4 | Vídeo apresentação (pitch) | Pendente |

---

## Etapa 1 — Modelagem do Banco de Dados

### Entidades

| Entidade | Chave primária | Chaves estrangeiras | Principais atributos |
|---|---|---|---|
| Fabricante | Id | – | Nome, PaisOrigem |
| Categoria | Id | – | Nome, Descricao, ValorDiariaBase |
| Veiculo | Id | FabricanteId, CategoriaId | Placa, Modelo, AnoFabricacao, Quilometragem |
| Cliente | Id | – | Nome, Cpf, Email, Telefone |
| Aluguel | Id | ClienteId, VeiculoId | DataRetirada, DataPrevistaDevolucao, DataDevolucao, KmInicial, KmFinal, ValorDiaria, ValorTotal |

### Relacionamentos
- Fabricante 1:N Veículo
- Categoria 1:N Veículo
- Cliente 1:N Aluguel
- Veículo 1:N Aluguel

### Restrições de integridade
- Índices únicos: Placa, Cpf, Email, Nome do fabricante, Nome da categoria
- Check constraints: ano ≥ 1900, quilometragem ≥ 0, período do aluguel válido, KmFinal ≥ KmInicial, diária > 0
- Exclusão restrita: não é possível excluir registros com dependentes
- DataDevolucao, KmFinal e ValorTotal são opcionais até a devolução do veículo

### Evidências
- `Docs/diagrama-conceitual.png`
- `Docs/ssms-tabelas.png`
- `Docs/ssms-constraints.png`
- `Docs/ssms-foreign-keys.png`
- `Docs/ssms-diagrama.png`

---

## Etapa 2 — Implementação do Backend
API REST em ASP.NET Core que acessa o SQL Server Express por meio do Entity Framework Core. Cada entidade possui um controller próprio, e a comunicação usa DTOs (`Dtos/`) em vez de expor as entidades diretamente.
 
### Endpoints CRUD
 
Todas as entidades possuem as cinco operações abaixo. A rota base segue o padrão `api/{recurso}`.
 
| Recurso | Rota base | GET (todos) | GET (por id) | POST | PUT | DELETE |
|---------|-----------|:-----------:|:------------:|:----:|:---:|:------:|
| Fabricante | `/api/fabricantes` | ✔ | ✔ | ✔ | ✔ | ✔ |
| Categoria | `/api/categorias` | ✔ | ✔ | ✔ | ✔ | ✔ |
| Cliente | `/api/clientes` | ✔ | ✔ | ✔ | ✔ | ✔ |
| Veículo | `/api/veiculos` | ✔ | ✔ | ✔ | ✔ | ✔ |
| Aluguel | `/api/alugueis` | ✔ | ✔ | ✔ | ✔ | ✔ |
 
### Filtros (consultas com joins)
 
| # | Rota | Descrição | Tipo de join |
|---|------|-----------|--------------|
| 1 | `GET /api/veiculos/por-fabricante/{fabricanteId}` | Veículos de um fabricante, com nome do fabricante e da categoria | INNER JOIN explícito (`join` do LINQ) entre Veiculo, Fabricante e Categoria |
| 2 | `GET /api/veiculos/disponiveis?categoriaId=` | Veículos sem aluguel em aberto, com filtro opcional por categoria | LEFT OUTER JOIN (`join ... into ... DefaultIfEmpty()`) entre Veiculo e Aluguel |
| 3 | `GET /api/clientes/sem-aluguel-ativo` | Clientes sem nenhum aluguel em aberto | LEFT OUTER JOIN entre Cliente e Aluguel |
| 4 | `GET /api/alugueis/por-periodo?inicio=&fim=` | Aluguéis retirados dentro de um período, com nome do cliente e dados do veículo | JOIN por navegação (`Include`) entre Aluguel, Cliente e Veiculo |
| 5 | `GET /api/alugueis/faturamento-por-categoria` | Quantidade de aluguéis finalizados e faturamento total por categoria | INNER JOIN explícito entre Aluguel, Veiculo e Categoria, com `GroupBy` |
 
Tipos de join utilizados: **INNER JOIN**, **LEFT OUTER JOIN** e **JOIN por navegação (Include)**.
 
### Regras de negócio
 
- Um veículo com aluguel em aberto (sem `DataDevolucao`) não pode ser alugado novamente (409).
- `PUT /api/alugueis/{id}` altera apenas `DataPrevistaDevolucao` e `ValorDiaria`, e somente em aluguéis ainda não finalizados.
- `PUT /api/alugueis/{id}/devolucao` registra a devolução: preenche `DataDevolucao` e `KmFinal`, calcula `ValorTotal` (dias corridos, mínimo de 1, multiplicados pela diária) e atualiza a quilometragem do veículo.
- `KmFinal` não pode ser menor que `KmInicial`, e a data de devolução não pode ser anterior à retirada.
- A placa é normalizada para maiúsculas antes de salvar.
### Validação de dados e tratamento de erros
 
| Mecanismo | Onde | Resultado |
|-----------|------|-----------|
| Data Annotations nos DTOs (`Required`, `MaxLength`, `Range`, `EmailAddress`, `StringLength`) | `Dtos/` | 400 com detalhamento dos campos inválidos (`ValidationProblemDetails`) |
| Verificação de existência de chaves estrangeiras antes de gravar | Controllers de Veículo e Aluguel | 400 com mensagem indicando o Id inexistente |
| Recurso não encontrado | Todos os controllers | 404 com mensagem |
| Violação de índice único (placa, CPF, e-mail, nomes) ou de exclusão restrita, capturada via `DbUpdateException` | Controllers | 409 com mensagem |
| Regras de negócio violadas (veículo ocupado, aluguel já devolvido, datas e quilometragens inconsistentes) | Controller de Aluguel | 400 ou 409 com mensagem |
| Exceções não previstas | `Program.cs` (`UseExceptionHandler`) | 500 em JSON (`ProblemDetails`), com detalhe da exceção apenas em ambiente de desenvolvimento |
 
### Códigos de resposta utilizados
 
`200 OK`, `201 Created`, `204 No Content`, `400 Bad Request`, `404 Not Found`, `409 Conflict`, `500 Internal Server Error`
 
### Exemplos de requisição
 
Ordem recomendada para popular o banco, por causa das chaves estrangeiras: Fabricante → Categoria → Veículo → Cliente → Aluguel.
 
```json
// POST /api/fabricantes
{ "nome": "Toyota", "paisOrigem": "Japão" }
 
// POST /api/categorias
{ "nome": "Econômico", "descricao": "Carros compactos e populares", "valorDiariaBase": 120.00 }
 
// POST /api/veiculos
{ "placa": "ABC1D23", "modelo": "Corolla", "anoFabricacao": 2023, "quilometragem": 15000, "fabricanteId": 1, "categoriaId": 1 }
 
// POST /api/clientes
{ "nome": "Maria Silva", "cpf": "12345678901", "email": "maria.silva@email.com", "telefone": "31999998888" }
 
// POST /api/alugueis
{ "clienteId": 1, "veiculoId": 1, "dataRetirada": "2026-09-28T09:00:00", "dataPrevistaDevolucao": "2026-10-02T09:00:00", "kmInicial": 15000, "valorDiaria": 150.00 }
 
// PUT /api/alugueis/1/devolucao
{ "dataDevolucao": "2026-10-01T09:00:00", "kmFinal": 15400 }
```
---

## Etapa 3 — Testes e Documentação
*(a preencher...)*
---

## Etapa 4 — Vídeo Apresentação
*(a preencher...)*
---
