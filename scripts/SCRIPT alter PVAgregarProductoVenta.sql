ALTER PROCEDURE [dbo].[PVAgregarProductoVenta] 
( 
    -- Add the parameters for the stored procedure here 
    @FolioVenta VARCHAR(100),  
	@IdProducto INT,  
	@Cantidad FLOAT, 
	@Precio FLOAT, 
	@MontoDescuento FLOAT, 
	@IVA FLOAT, 
	@IEPS FLOAT, 
	@EsCaja BIT, 
	@Uom FLOAT, 
	@FechaAlta DATETIME, 
	@IdUsuarioAlta INT, 
	@IdPresentacionProducto INT, 
	@IdMarca INT, 
	@IdLinea INT, 
	@NumeroTelefonico VARCHAR(45), 
	@MontoComision FLOAT,
    @PrecioSinImpuesto FLOAT = 2500 
) 
AS 
BEGIN 
    -- SET NOCOUNT ON added to prevent extra result sets from 
    -- interfering with SELECT statements. 
    SET NOCOUNT ON 
 
    -- Insert statements for procedure here 
 
	--Revisa si el producto es en presentación indivudual y toma su nuevo ID 
	DECLARE @IdProductoIndividual INT; 
	SELECT @IdProductoIndividual = ISNULL(IdProductoIndividual,0) FROM PVPresentacionesVentaProd WHERE Id=@IdPresentacionProducto; 
 
	IF(@IdProductoIndividual > 0) 
		BEGIN 
			SELECT @IdProducto=@IdProductoIndividual; 
		END 
 
	--EXISTE EL PRODUCTO EN LA VENTA? 
	DECLARE @existe INT; 
	SELECT  @existe = COUNT(IdPresentacionProducto) FROM PVVentasDetalle  
		WHERE FolioVenta = @FolioVenta AND IdProducto = @IdProducto AND EsCaja = @EsCaja AND Uom = @Uom AND IdPresentacionProducto=@IdPresentacionProducto  
		AND ISNULL(NumeroTelefonico,'')=@NumeroTelefonico; 
 
 
	IF @existe = 0 OR (@existe = 1) 
	BEGIN 
		INSERT INTO PVVentasDetalle 
		(FolioVenta, IdProducto, Cantidad, Precio, MontoDescuento, IVA, IEPS, EsCaja, Uom, FechaAlta, IdUsuarioAlta, IdPresentacionProducto, IdMarca, IdLinea, NumeroTelefonico, MontoComision, PrecioSinImpuesto) 
		SELECT @FolioVenta, @IdProducto, @Cantidad, @Precio, @MontoDescuento, @IVA, @IEPS, @EsCaja, @Uom, @FechaAlta, @IdUsuarioAlta, @IdPresentacionProducto, @IdMarca, @IdLinea, @NumeroTelefonico, @MontoComision, @PrecioSinImpuesto; 
	END; 
 
    IF @existe > 0 
	BEGIN 
		UPDATE TOP(1) PVVentasDetalle SET Cantidad = Cantidad + @Cantidad  
		WHERE IdProducto = @IdProducto AND FolioVenta = @FolioVenta AND EsCaja = @EsCaja AND Uom = @Uom AND IdPresentacionProducto=@IdPresentacionProducto AND NumeroTelefonico=@NumeroTelefonico; 
	END; 
 
END 
 
 
 
 
--USE [PVLaJoya] 
--GO 
--/****** Object:  StoredProcedure [dbo].[PVAgregarProductoVenta]    Script Date: 24/08/2022 07:18:54 p. m. ******/ 
--SET ANSI_NULLS ON 
--GO 
--SET QUOTED_IDENTIFIER ON 
--GO 
 
--ALTER PROCEDURE [dbo].[PVAgregarProductoVenta] 
--( 
--    -- Add the parameters for the stored procedure here 
--    @FolioVenta VARCHAR(100),  
--	@IdProducto INT,  
--	@Cantidad FLOAT, 
--	@Precio FLOAT, 
--	@MontoDescuento FLOAT, 
--	@IVA FLOAT, 
--	@IEPS FLOAT, 
--	@EsCaja BIT, 
--	@Uom FLOAT, 
--	@FechaAlta DATETIME, 
--	@IdUsuarioAlta INT, 
--	@IdPresentacionProducto INT, 
--	@IdMarca INT, 
--	@IdLinea INT 
--) 
--AS 
--BEGIN 
--    -- SET NOCOUNT ON added to prevent extra result sets from 
--    -- interfering with SELECT statements. 
--    SET NOCOUNT ON 
 
--    -- Insert statements for procedure here 
 
--	--Revisa si el producto es en presentación indivudual y toma su nuevo ID 
--	DECLARE @IdProductoIndividual INT; 
--	SELECT @IdProductoIndividual = ISNULL(IdProductoIndividual,0) FROM PVPresentacionesVentaProd WHERE Id=@IdPresentacionProducto; 
 
--	IF(@IdProductoIndividual > 0) 
--		BEGIN 
--			SELECT @IdProducto=@IdProductoIndividual; 
--		END 
 
--	--EXISTE EL PRODUCTO EN LA VENTA? 
--	DECLARE @existe INT; 
--	SELECT  @existe = COUNT(IdPresentacionProducto) FROM PVVentasDetalle  
--		WHERE FolioVenta = @FolioVenta AND IdProducto = @IdProducto AND EsCaja = @EsCaja AND Uom = @Uom AND IdPresentacionProducto=@IdPresentacionProducto; 
 
 
--	IF @existe = 0 OR (@existe = 1) 
--	BEGIN 
--		INSERT INTO PVVentasDetalle 
--		(FolioVenta, IdProducto, Cantidad, Precio, MontoDescuento, IVA, IEPS, EsCaja, Uom, FechaAlta, IdUsuarioAlta, IdPresentacionProducto, IdMarca, IdLinea) 
--		SELECT @FolioVenta, @IdProducto, @Cantidad, @Precio, @MontoDescuento, @IVA, @IEPS, @EsCaja, @Uom, @FechaAlta, @IdUsuarioAlta, @IdPresentacionProducto, @IdMarca, @IdLinea; 
--	END; 
 
--    IF @existe > 0 
--	BEGIN 
--		UPDATE TOP(1) PVVentasDetalle SET Cantidad = Cantidad + @Cantidad  
--		WHERE IdProducto = @IdProducto AND FolioVenta = @FolioVenta AND EsCaja = @EsCaja AND Uom = @Uom AND IdPresentacionProducto=@IdPresentacionProducto; 
--	END; 
 
--END 
 