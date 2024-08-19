--Creación de la tabla temporal
DECLARE @TempVentaPago TABLE
(
	[Id] [int] NOT NULL,
	[FolioVenta] [varchar](100) NOT NULL,
	[MontoRecibido] [float] NOT NULL,
	[Cambio] [float] NOT NULL,
	[MontoEfectivo] [float] NOT NULL,
	[MontoTarjeta] [float] NOT NULL,
	[MontoTransferencia] [float] NOT NULL,
	[MontoMonedero] [float] NOT NULL,
	[MontoVales] [float] NOT NULL,
	[TipoTarjeta] [varchar](80) NULL,
	[AutorizacionTarjeta] [varchar](200) NULL,
	[Respuesta] [varchar](200) NULL,
	[FolioTransferencia] [varchar](150) NULL,
	[MontoDescuento] [float] NULL,
	[FechaAlta] [datetime] NOT NULL,
	[IdUsuarioAlta] [int] NOT NULL,
	[DisparadoNube] [bit] NOT NULL,
	[FechaDisparo] [datetime] NULL,
	[IdUsuarioDisparo] [int] NULL
)

--Pasar los datos a la tabla temporal
INSERT INTO @TempVentaPago
SELECT
    Id,
	FolioVenta,
	MontoRecibido,
	Cambio,
	MontoEfectivo,
	MontoTarjeta,
	MontoTransferencia,
	MontoMonedero,
	MontoVales,
	TipoTarjeta,
	AutorizacionTarjeta,
	Respuesta,
	FolioTransferencia,
	MontoDescuento,
	FechaAlta,
	IdUsuarioAlta,
	DisparadoNube,
	FechaDisparo,
	IdUsuarioDisparo
FROM PVVentaPago ORDER BY id


-- Drop a table called 'PVVentaPago' in schema 'dbo'
-- Drop the table if it already exists
IF OBJECT_ID('[dbo].[PVVentaPago]', 'U') IS NOT NULL
DROP TABLE [dbo].[PVVentaPago]


-- Creacion de la tabla nuevamente con el nuevo campo
CREATE TABLE [dbo].[PVVentaPago]
(
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


--Se insertan los datos de nuevo
INSERT INTO PVVentaPago
SELECT
	FolioVenta,
	MontoRecibido,
	Cambio,
	MontoEfectivo,
	MontoTarjeta,
    0, --MontoTarjetaCredito
	MontoTransferencia,
	MontoMonedero,
	MontoVales,
	TipoTarjeta,
	AutorizacionTarjeta,
	Respuesta,
	FolioTransferencia,
	MontoDescuento,
	FechaAlta,
	IdUsuarioAlta,
	DisparadoNube,
	FechaDisparo,
	IdUsuarioDisparo
FROM @TempVentaPago ORDER BY id
