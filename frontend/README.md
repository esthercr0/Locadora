# Front-end da locadora — primeiro bloco

HTML, CSS, JavaScript e o grid do Bootstrap 5.3.9, incluído localmente com sua licença MIT em `vendor/`.

## Implementado

- Fabricantes: cadastro, detalhes, edição, exclusão e filtros combinados por nome e país.
- Categorias: cadastro, detalhes, edição, exclusão e filtros combinados por nome e faixa de diária.
- Mensagens de validação por campo, conflito e serviço indisponível.
- Confirmação de exclusão, bloqueio de envios repetidos e cancelamento de pesquisas antigas.
- Dados obtidos da API; não há dados estáticos de demonstração nem armazenamento local dos cadastros.

Veículos, Clientes, Aluguéis, Devolução e as consultas especiais serão integrados nos próximos blocos, conforme os wireframes aprovados.

## Executar

Na raiz do repositório:

```powershell
dotnet run --project Locadora.Api --launch-profile http
```

Abra http://localhost:5215. O Swagger continua em http://localhost:5215/swagger. A página e os endpoints `/api` são servidos pela mesma aplicação; não é necessário um segundo servidor nem habilitar CORS.

O SQL Server Express precisa estar acessível. Confira `ConnectionStrings:DefaultConnection` em `Locadora.Api/appsettings.json`; a configuração original aponta para `localhost\SQLEXPRESS`, banco `LocadoraDb`, com autenticação do Windows. Use o banco da primeira parte para reaproveitar seus registros.

Se o banco ainda não existir, após conferir a conexão, execute o comando abaixo a partir de `Locadora.Api/` para aplicar as migrations existentes:

```powershell
dotnet ef database update
```

Abrir `index.html` com duplo clique não conecta o cadastro à API. Use o endereço HTTP da aplicação.

## Rotas utilizadas

| Tela | Operações | Parâmetros de pesquisa |
|---|---|---|
| Fabricantes | `GET /api/fabricantes`, `GET /api/fabricantes/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | `nome`, `paisOrigem` |
| Categorias | `GET /api/categorias`, `GET /api/categorias/{id}`, `POST`, `PUT /{id}`, `DELETE /{id}` | `nome`, `diariaMin`, `diariaMax` |

Exemplos:

```text
/api/fabricantes?nome=Toy&paisOrigem=Jap
/api/categorias?nome=Eco&diariaMin=100&diariaMax=200
```

Os filtros são opcionais, combinados com `AND` e executados no banco pelo EF Core. Os limites da faixa de diária são inclusivos. Os endpoints sem parâmetros mantêm a listagem anterior.

## Verificações

```powershell
node --test frontend/tests/api.test.js
dotnet run --project tests/Locadora.Checks
```

Os testes JavaScript exercitam o contrato HTTP com respostas controladas, incluindo erros e cancelamento. As verificações C# capturam as consultas reais dos controllers e conferem a geração de SQL, interrompendo antes da conexão. Esses testes não comprovam gravação nem consulta de dados em um SQL Server em execução.

## Conferência manual com o banco acessível

1. Confirme a listagem de alguns registros usados na primeira parte.
2. Cadastre um fabricante temporário; combine nome e país nos filtros; abra os detalhes e edite o país.
3. Atualize a página e confirme que o registro continua salvo; exclua o registro temporário.
4. Cadastre uma categoria temporária; filtre por nome e faixa de diária; edite e exclua.
5. Tente cadastrar um nome já existente e confira a mensagem de conflito.
6. Tente excluir um fabricante/categoria com veículo vinculado e confira o bloqueio sem perda dos registros.

## Estado da validação

Compilação e testes automatizados são registrados em `Docs/Frontend-Bloco1.md`. O teste com o SQL Server e a revisão visual das telas integradas ainda precisam ser realizados. Não apresentar o wireframe como evidência de integração.
