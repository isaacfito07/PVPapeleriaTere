-- Drop a table called 'PVVentaPago' in schema 'dbo'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[PVVentaPago]', 'U') IS NOT NULL
DROP TABLE [dbo].[PVVentaPago]


CREATE TABLE [dbo].[PVVentaPago](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FolioVenta] [varchar](100) NOT NULL,
	[MontoRecibido] [float] NOT NULL,
	[Cambio] [float] NOT NULL,
	[MontoEfectivo] [float] NOT NULL,
	[MontoTarjeta] [float] NOT NULL,
	[MontoTarjetaCredito] [float] NOT NULL,
	[MontoTransferencia] [float] NOT NULL,
	[MontoMonedero] [float] NOT NULL,
	[MontoVales] [float] NOT NULL,
    [MontoCredito] [float] NOT NULL,
	[TipoTarjeta] [varchar](80) NULL,
	[AutorizacionTarjeta] [varchar](200) NULL,
	[Respuesta] [varchar](200) NULL,
	[FolioTransferencia] [varchar](150) NULL,
	[MontoDescuento] [float] NULL,
	[FechaAlta] [datetime] NOT NULL,
	[IdUsuarioAlta] [int] NOT NULL,
	[DisparadoNube] [bit] NULL,
	[FechaDisparo] [datetime] NULL,
	[IdUsuarioDisparo] [int] NULL
)