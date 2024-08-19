CREATE TABLE [dbo].[PVPrecios](
	[id] [int] NOT NULL,
	[idproducto] [int] NULL,
	[IdPresentacionVenta] [int] NULL,
	[ultimoCosto] [float] NULL,
	[costoponderado] [float] NULL,
	[idSucursal] [int] NULL,
	[General] [float] NULL,
	[Talleres] [float] NULL,
	[Distribuidores] [float] NULL,
	[GeneralAnterior] [float] NULL,
	[TalleresAnterior] [float] NULL,
	[DistribuidoresAnterior] [float] NULL,
	[UltimoCostoAnterior] [float] NULL,
	[Activo] [bit] NULL,
	[fechamodificado] [datetime] NULL,
	[idusuariomodificado] [int] NULL,
	[FechaModificadoCambio] [datetime] NULL
)