# Roteiro do vídeo da Locadora (Publicado para orientação na avaliação)

Duração sugerida: 3min40s. Grave a tela e narre com sua voz. Use dados fictícios cadastrados no banco e mostre os resultados reais da aplicação.

- **0:00–0:30 · Problema.** “A locadora precisa controlar a frota, os clientes e cada aluguel, desde a retirada até a devolução.”
- **0:30–1:10 · Modelagem.** Mostre o diagrama. Explique que fabricante e categoria agrupam veículos, e cada aluguel liga um cliente a um veículo. Mostre o painel e explique o fluxo navegador, API ASP.NET Core, EF Core e SQL Server.
- **1:10–2:00 · Consulta 1.** Abra Consultas e relatórios e selecione Veículos disponíveis. Escolha uma categoria com resultados. Mostre a rota GET /api/veiculos/disponiveis no Swagger e explique que o banco retorna os veículos sem aluguel em aberto. Volte à tela e mostre as placas retornadas.
- **2:00–3:10 · Consulta 2.** Abra Faturamento por categoria e mostre o resultado. Indique no Swagger GET /api/alugueis/faturamento-por-categoria. Explique que a API relaciona aluguéis finalizados, veículos e categorias, e apresenta a quantidade e a soma dos valores. Se a lista estiver vazia, finalize um aluguel antes de gravar.
- **3:10–3:40 · Encerramento.** Mostre um aluguel finalizado e o total calculado. Resuma como os cadastros, filtros e consultas cobrem a operação da locadora.

Antes de gravar, confirme que as duas consultas têm resultados e que o áudio está legível. Submeta o vídeo na atividade correspondente do Canvas e confira o ZIP e o link do GitHub.
