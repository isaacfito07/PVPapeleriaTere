-- Drop a table called 'PVProveedores' in schema 'dbo'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[PVProveedores]', 'U') IS NOT NULL
DROP TABLE [dbo].[PVProveedores]
GO

CREATE TABLE [dbo].[PVProveedores](
	[Id] [int] NOT NULL,
	[Clave] [varchar](50) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[RFC] [varchar](50) NULL,
	[Telefono] [varchar](30) NULL,
    [Activo] [BIT] NULL
);