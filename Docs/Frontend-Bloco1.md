# Front-end — bloco 1: Fabricantes e Categorias

## Alterações

- Mantida a modelagem e as rotas CRUD da primeira parte.
- Acrescentados filtros opcionais à listagem de fabricantes (`nome`, `paisOrigem`) e categorias (`nome`, `diariaMin`, `diariaMax`).
- Faixas de diária inválidas retornam HTTP 400; a diária de cadastro respeita a precisão `decimal(10,2)` do banco.
- ASP.NET Core serve a pasta `frontend/` na raiz HTTP, preservando `/api` e `/swagger`.
- O publish inclui os arquivos de interface em `wwwroot/`, sem os testes e a documentação de desenvolvimento.
- HTTPS permanece obrigatório fora do ambiente de desenvolvimento. O perfil HTTP local permite executar a interface em `http://localhost:5215`.
- Front-end com listagem, pesquisa, detalhes, cadastro, edição e exclusão das duas entidades, consumindo a API real.

## Verificações e limites

- Compilação do projeto da API: aprovada.
- Testes JavaScript do contrato HTTP: 9 aprovados.
- Verificações C# dos filtros e validações: compiladas, mas não executadas. O Controle de Aplicativos do Windows bloqueou a carga de `Locadora.Checks.dll` (0x800711C7); nenhuma proteção do sistema foi alterada.
- Persistência real no SQL Server: pendente. A tentativa de conexão a `.\SQLEXPRESS` não respondeu neste ambiente; o gerenciador de serviços também não estava acessível pela execução restrita. Isso não comprova que a instalação da usuária esteja incorreta.
- Revisão visual no navegador: pendente para as telas integradas.
- A consulta de vulnerabilidades NuGet apresentou NU1900 por indisponibilidade de acesso ao serviço. As dependências da API foram restauradas do cache existente; a auditoria não foi desativada.

## Próximos blocos

Após revisar este bloco e validar o acesso ao banco: integrar Veículos e Clientes, depois Aluguéis e Devolução; integrar as cinco consultas especiais; concluir link público dos wireframes, evidências, relatório SBC em LaTeX, roteiro do pitch e ZIP final.
