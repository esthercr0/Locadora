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
*(a preencher...)*
---

## Etapa 3 — Testes e Documentação
*(a preencher...)*
---

## Etapa 4 — Vídeo Apresentação
*(a preencher...)*
---
