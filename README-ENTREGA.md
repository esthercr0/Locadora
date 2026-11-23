# Locadora · Entrega do front-end

O relatório em LaTeX e seu PDF ficam em **Docs/Relatorio/**. Os wireframes estão em **Docs/Wireframes/index.html**. O roteiro de gravação está em **Docs/Roteiro-Pitch.md**. O código da interface está em **frontend/** e os artefatos anteriores em **Docs/** e **Locadora.Api/**.

Com .NET 10 e o SQL Server Express da primeira parte disponíveis, execute na raiz do projeto:

    dotnet run --project Locadora.Api/Locadora.Api.csproj --launch-profile http

Abra http://localhost:5215. O Swagger fica em http://localhost:5215/swagger. A API usa o banco LocadoraDb, configurado no appsettings.json. Se estiver usando o banco original, preserve-o para reaproveitar os registros. Para um banco novo, aplique as migrations existentes.

A interface inclui CRUD e filtros em duas ou mais colunas para todas as entidades, devolução de veículo e cinco consultas na aba Consultas e relatórios. O vídeo de 3 a 5 minutos deve ser gravado e enviado pela aluna no Canvas.
