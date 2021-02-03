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
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210131222346_InitialCreate')
BEGIN
    CREATE TABLE [Movies] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [Language] nvarchar(max) NULL,
        [Duration] nvarchar(max) NULL,
        [PlayingDate] datetime2 NOT NULL,
        [PlayingTime] datetime2 NOT NULL,
        [TicketPrice] int NOT NULL,
        [Rating] int NOT NULL,
        [Genre] nvarchar(max) NULL,
        [TrailorUrl] nvarchar(max) NULL,
        [ImageUrl] nvarchar(max) NULL,
        CONSTRAINT [PK_Movies] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210131222346_InitialCreate')
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [Password] nvarchar(max) NULL,
        [Role] nvarchar(max) NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210131222346_InitialCreate')
BEGIN
    CREATE TABLE [Reservations] (
        [Id] int NOT NULL IDENTITY,
        [Qty] int NOT NULL,
        [Price] float NOT NULL,
        [Phone] nvarchar(max) NULL,
        [ReservationTime] datetime2 NOT NULL,
        [MovieId] int NOT NULL,
        [UserId] int NOT NULL,
        CONSTRAINT [PK_Reservations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reservations_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reservations_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210131222346_InitialCreate')
BEGIN
    CREATE INDEX [IX_Reservations_MovieId] ON [Reservations] ([MovieId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210131222346_InitialCreate')
BEGIN
    CREATE INDEX [IX_Reservations_UserId] ON [Reservations] ([UserId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210131222346_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210131222346_InitialCreate', N'5.0.2');
END;
GO

COMMIT;
GO

