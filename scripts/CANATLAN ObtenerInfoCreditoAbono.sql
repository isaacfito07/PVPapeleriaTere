CREATE PROCEDURE ObtenerInfoCreditoAbono
    @FolioCorte VARCHAR(100)
AS
BEGIN
    DECLARE @TableTemp TABLE(
        NumeroCreditos INT,
        TotalCreditos NUMERIC(18,2),
        NumeroAbonos INT,
        TotalAbonos NUMERIC(18,2)
    )

    INSERT INTO @TableTemp (NumeroCreditos, TotalCreditos)
    SELECT ISNULL(COUNT(*),0) AS NumeroCreditos, ISNULL(Round(SUM((b.MontoCredito - b.MontoDescuento)),2),0.00) AS TotalCreditos
    FROM PVVentas a, PVVentaPago b 
    WHERE a.Pagado = 0 
    AND a.FolioVenta = b.FolioVenta
    AND a.FolioCorteParcialCaja = @FolioCorte
    GROUP BY a.FolioCorteParcialCaja

    --Obtener la fecha del penultimo corte
    DECLARE @FechaUltimoCorte DATETIME
    SELECT TOP 2 @FechaUltimoCorte = FechaCorte FROM PVCorteCaja ORDER BY ID DESC

    DECLARE @NumeroAbonos INT = 0, @TotalAbonos NUMERIC(18,2) = 0.00
    --Obtener cantidad de abonos y total
    SELECT @NumeroAbonos = COUNT(*), @TotalAbonos = SUM(ROUND(MontoRecibido,2))
    FROM PVVentaPago 
    WHERE FechaAlta >= @FechaUltimoCorte 
    AND idEsAbono = 1

    --Validacion en caso de no haber creditos durante cortes
    IF (SELECT COUNT(*) FROM @TableTemp) > 0
    BEGIN
        UPDATE @TableTemp SET NumeroAbonos = ISNULL(@NumeroAbonos, 0), TotalAbonos = ISNULL(@TotalAbonos,0.00)
    END
    ELSE
    BEGIN
        INSERT INTO @TableTemp (NumeroCreditos,TotalCreditos,NumeroAbonos,TotalAbonos)
        VALUES(0,0.00,ISNULL(@NumeroAbonos,0),ISNULL(@TotalAbonos,0.00))
    END

    SELECT * FROM @TableTemp
END
