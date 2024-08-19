CREATE PROCEDURE [dbo].[PVAgregarProductoVenta]
(
    -- Add the parameters for the stored procedure here
    @IdVenta INT, 
	@IdProducto INT, 
	@Cantidad FLOAT,
	@Precio FLOAT,
	@IVA FLOAT,
	@IEPS FLOAT,
	@FolioLocal VARCHAR(150),
	@FechaVenta DATETIME,
	@IdUsuarioVenta INT

)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here

	--EXISTE EL PRODUCTO EN LA VENTA?
	DECLARE @existe INT;
	SELECT  @existe = COUNT(IdProducto) FROM PVVentasDetalle WHERE IdVenta = @IdVenta AND IdProducto = @IdProducto;

	IF @existe = 0 OR (@existe = 1)
	BEGIN
		INSERT INTO PVVentasDetalle
		(IdVenta, IdProducto, Cantidad, Precio, IVA, IEPS, FolioLocal, FechaVenta, IdUsuarioVenta)
		SELECT @IdVenta, @IdProducto, @Cantidad, @Precio, @IVA, @IEPS, @FolioLocal, @FechaVenta, @IdUsuarioVenta;
	END;

    IF @existe > 0
	BEGIN
		UPDATE TOP(1) PVVentasDetalle SET Cantidad = Cantidad + @Cantidad WHERE IdProducto = @IdProducto AND IdVenta = @IdVenta;
	END;

END