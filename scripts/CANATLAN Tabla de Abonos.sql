CREATE TABLE [dbo].[PVAbono]
(
    [Id] INT IDENTITY PRIMARY KEY,
    [Monto] NUMERIC(18,2) NOT NULL,
    [idCliente] INT NOT NULL,
    [Activo] BIT NOT NULL,
    [FechaAlta] [datetime] NOT NULL,
	[IdUsuarioAlta] [int] NOT NULL,
)