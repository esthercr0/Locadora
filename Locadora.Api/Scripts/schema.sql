IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Categorias] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(60) NOT NULL,
    [Descricao] nvarchar(255) NULL,
    [ValorDiariaBase] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
);

CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(150) NOT NULL,
    [Cpf] nvarchar(11) NOT NULL,
    [Email] nvarchar(150) NOT NULL,
    [Telefone] nvarchar(20) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);

CREATE TABLE [Fabricantes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(100) NOT NULL,
    [PaisOrigem] nvarchar(60) NULL,
    CONSTRAINT [PK_Fabricantes] PRIMARY KEY ([Id])
);

CREATE TABLE [Veiculos] (
    [Id] int NOT NULL IDENTITY,
    [Placa] nvarchar(8) NOT NULL,
    [Modelo] nvarchar(100) NOT NULL,
    [AnoFabricacao] int NOT NULL,
    [Quilometragem] int NOT NULL,
    [FabricanteId] int NOT NULL,
    [CategoriaId] int NOT NULL,
    CONSTRAINT [PK_Veiculos] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Veiculo_Ano] CHECK ([AnoFabricacao] >= 1900),
    CONSTRAINT [CK_Veiculo_Km] CHECK ([Quilometragem] >= 0),
    CONSTRAINT [FK_Veiculos_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Veiculos_Fabricantes_FabricanteId] FOREIGN KEY ([FabricanteId]) REFERENCES [Fabricantes] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Alugueis] (
    [Id] int NOT NULL IDENTITY,
    [ClienteId] int NOT NULL,
    [VeiculoId] int NOT NULL,
    [DataRetirada] datetime2 NOT NULL,
    [DataPrevistaDevolucao] datetime2 NOT NULL,
    [DataDevolucao] datetime2 NULL,
    [KmInicial] int NOT NULL,
    [KmFinal] int NULL,
    [ValorDiaria] decimal(10,2) NOT NULL,
    [ValorTotal] decimal(10,2) NULL,
    CONSTRAINT [PK_Alugueis] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Aluguel_KmFinal] CHECK ([KmFinal] IS NULL OR [KmFinal] >= [KmInicial]),
    CONSTRAINT [CK_Aluguel_Periodo] CHECK ([DataPrevistaDevolucao] >= [DataRetirada]),
    CONSTRAINT [CK_Aluguel_ValorDiaria] CHECK ([ValorDiaria] > 0),
    CONSTRAINT [FK_Alugueis_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Alugueis_Veiculos_VeiculoId] FOREIGN KEY ([VeiculoId]) REFERENCES [Veiculos] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Alugueis_ClienteId] ON [Alugueis] ([ClienteId]);

CREATE INDEX [IX_Alugueis_VeiculoId] ON [Alugueis] ([VeiculoId]);

CREATE UNIQUE INDEX [IX_Categorias_Nome] ON [Categorias] ([Nome]);

CREATE UNIQUE INDEX [IX_Clientes_Cpf] ON [Clientes] ([Cpf]);

CREATE UNIQUE INDEX [IX_Clientes_Email] ON [Clientes] ([Email]);

CREATE UNIQUE INDEX [IX_Fabricantes_Nome] ON [Fabricantes] ([Nome]);

CREATE INDEX [IX_Veiculos_CategoriaId] ON [Veiculos] ([CategoriaId]);

CREATE INDEX [IX_Veiculos_FabricanteId] ON [Veiculos] ([FabricanteId]);

CREATE UNIQUE INDEX [IX_Veiculos_Placa] ON [Veiculos] ([Placa]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260921213048_InitialCreate', N'10.0.12');

COMMIT;
GO

