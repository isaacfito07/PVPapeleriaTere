CREATE TABLE [dbo].[PVTipoCliente](
	[Id] [int] NOT NULL,
	[TipoCliente] [varchar](50) NULL,
	[Porcentaje] [float] NULL,
	[Activo] [bit] NULL,
	[FechaAlta] [datetime] NULL,
	[IdUsuarioAlta] [int] NULL,
	[FechaModificado] [datetime] NULL,
	[IdUsuarioModificado] [nchar](10) NULL
)