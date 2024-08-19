CREATE PROCEDURE ValidarLiquidacionCredito
    @FolioVenta VARCHAR(100)
AS
BEGIN
    DECLARE @idEstadoPagado INT = 1

    DECLARE @MontoAbonado NUMERIC(18,2)
    SET @MontoAbonado = (SELECT SUM(MontoRecibido) FROM PVVentaPago WHERE FolioVenta LIKE @FolioVenta)
    
    DECLARE @TotalVenta NUMERIC(18,2)
    SET @TotalVenta = (SELECT MontoCredito FROM PVVentaPago WHERE FolioVenta LIKE @FolioVenta AND MontoCredito != 0)
    
    IF @MontoAbonado >= @TotalVenta
    BEGIN
        UPDATE PVVentas set Pagado = @idEstadoPagado WHERE FolioVenta LIKE @FolioVenta
    END
END
