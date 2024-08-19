CREATE PROCEDURE PVCanatlan_AbonarCreditos
    @idCliente INT,
    @MontoAbono DECIMAL(18, 2),
    @Efectivo DECIMAL(18,2),
    @TarjetaDebito DECIMAL(18,2),
    @TarjetaCredito DECIMAL(18,2),
    @Transferencia DECIMAL(18,2),
    @Vales DECIMAL(18,2),
    @Cheques DECIMAL(18,2),
    @IdUsuario INT,
    @FolioTransferencia VARCHAR(100),
    @FolioCheque VARCHAR(100),
    @idAbono INT OUTPUT
AS
BEGIN
    DECLARE @FolioVenta NVARCHAR(50)
    DECLARE @MontoPendienteFolio DECIMAL(18, 2)
    DECLARE @idEstadoActivo BIT = 1

    DECLARE @TablaFolioSaldo TABLE(
        FolioVenta VARCHAR(200),
        [Saldo Pendiente] NUMERIC(18,2)
    )

    INSERT INTO @TablaFolioSaldo
    SELECT a.FolioVenta,
    ((a.MontoCredito) - (SELECT SUM(MontoRecibido) FROM PVVentaPago WHERE FolioVenta LIKE a.FolioVenta)) AS [Saldo Pendiente] 
    FROM PVVentaPago a, PVVentas b, PVClientes c 
    WHERE a.FolioVenta = b.FolioVenta AND b.Pagado = 0 AND a.MontoCredito <> 0 AND b.IdCliente = c.Id AND c.id = @idCliente ORDER BY b.FechaVenta

    IF (SELECT COUNT(FolioVenta) FROM @TablaFolioSaldo) > 0
    BEGIN
        INSERT INTO PVAbono
        SELECT @MontoAbono, @idCliente, @idEstadoActivo, GETDATE(), @IdUsuario
        SET @idAbono = IDENT_CURRENT('PVAbono')
    END

    DECLARE creditos_cursor CURSOR FOR
    SELECT * FROM @TablaFolioSaldo

    OPEN creditos_cursor;
    FETCH NEXT FROM creditos_cursor INTO @FolioVenta, @MontoPendienteFolio;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF @MontoAbono > 0
        BEGIN
            DECLARE @MontoAAbonar DECIMAL(18, 2);
            DECLARE @MontoPrueba DECIMAL(18, 2);

            SET @MontoPrueba = @MontoAbono - @MontoPendienteFolio;
            IF @MontoPrueba > 0
            BEGIN
                SET @MontoAAbonar = @MontoPendienteFolio;
            END
            ELSE
            BEGIN
                SET @MontoAAbonar = @MontoAbono;
            END

            -- A partir de aqui se hace los segmentos para dividir el metodo de pago por el monto recibido
            DECLARE @MontoAAbonarMetodoPago NUMERIC(18,2) = @MontoAAbonar

            --Seccion para Efectivo
            DECLARE @MontoEfectivo NUMERIC(18,2) = 0.0
            IF @Efectivo > 0 AND @MontoAAbonarMetodoPago > 0
            BEGIN
                IF @MontoAAbonarMetodoPago > @Efectivo
                BEGIN
                    SET @MontoEfectivo = @Efectivo
                    SET @Efectivo = 0
                END
                ELSE
                BEGIN
                    SET @MontoEfectivo = @MontoAAbonarMetodoPago
                    SET @Efectivo = @Efectivo - @MontoAAbonarMetodoPago
                END
                SET @MontoAAbonarMetodoPago = @MontoAAbonarMetodoPago - @MontoEfectivo
            END

            --Seccion para Tarjeta de Debito
            DECLARE @MontoTarjetaDebito NUMERIC(18,2) = 0.0
            IF @TarjetaDebito > 0 AND @MontoAAbonarMetodoPago > 0
            BEGIN
                IF @MontoAAbonarMetodoPago > @TarjetaDebito
                BEGIN
                    SET @MontoTarjetaDebito = @TarjetaDebito
                    SET @TarjetaDebito = 0
                END
                ELSE
                BEGIN
                    SET @MontoTarjetaDebito = @MontoAAbonarMetodoPago
                    SET @TarjetaDebito = @TarjetaDebito - @MontoAAbonarMetodoPago
                END
                SET @MontoAAbonarMetodoPago = @MontoAAbonarMetodoPago - @MontoTarjetaDebito
            END
        
            --Seccion para Tarjeta de Credito
            DECLARE @MontoTarjetaCredito NUMERIC(18,2) = 0.0
            IF @TarjetaCredito > 0 AND @MontoAAbonarMetodoPago > 0
            BEGIN
                IF @MontoAAbonarMetodoPago > @TarjetaCredito
                BEGIN
                    SET @MontoTarjetaCredito = @TarjetaCredito
                    SET @TarjetaCredito = 0
                END
                ELSE
                BEGIN
                    SET @MontoTarjetaCredito = @MontoAAbonarMetodoPago
                    SET @TarjetaCredito = @TarjetaCredito - @MontoAAbonarMetodoPago
                END
                SET @MontoAAbonarMetodoPago = @MontoAAbonarMetodoPago - @MontoTarjetaCredito
            END
        
            --Seccion para Transferencia
            DECLARE @MontoTransferencia NUMERIC(18,2) = 0.0
            IF @Transferencia > 0 AND @MontoAAbonarMetodoPago > 0
            BEGIN
                IF @MontoAAbonarMetodoPago > @Transferencia
                BEGIN
                    SET @MontoTransferencia = @Transferencia
                    SET @Transferencia = 0
                END
                ELSE
                BEGIN
                    SET @MontoTransferencia = @MontoAAbonarMetodoPago
                    SET @Transferencia = @Transferencia - @MontoAAbonarMetodoPago
                END
                SET @MontoAAbonarMetodoPago = @MontoAAbonarMetodoPago - @MontoTransferencia
            END

            --Seccion para Vales
            DECLARE @MontoVales NUMERIC(18,2) = 0.0
            IF @Vales > 0 AND @MontoAAbonarMetodoPago > 0
            BEGIN
                IF @MontoAAbonarMetodoPago > @Vales
                BEGIN
                    SET @MontoVales = @Vales
                    SET @Vales = 0
                END
                ELSE
                BEGIN
                    SET @MontoVales = @MontoAAbonarMetodoPago
                    SET @Vales = @Vales - @MontoAAbonarMetodoPago
                END
                SET @MontoAAbonarMetodoPago = @MontoAAbonarMetodoPago - @MontoVales
            END

            --Seccion para Cheques
            DECLARE @MontoCheque NUMERIC(18,2) = 0.0
            IF @Cheques > 0 AND @MontoAAbonarMetodoPago > 0
            BEGIN
                IF @MontoAAbonarMetodoPago > @Cheques
                BEGIN
                    SET @MontoCheque = @Cheques
                    SET @Cheques = 0
                END
                ELSE
                BEGIN
                    SET @MontoCheque = @MontoAAbonarMetodoPago
                    SET @Cheques = @Cheques - @MontoAAbonarMetodoPago
                END
                SET @MontoAAbonarMetodoPago = @MontoAAbonarMetodoPago - @MontoCheque
            END

            -- Se inserta el dato
            INSERT INTO PVVentaPago (
                FolioVenta, MontoRecibido, Cambio,
                MontoEfectivo, MontoTarjeta, MontoTarjetaCredito,
                MontoTransferencia, MontoMonedero, MontoCredito,
                MontoVales, MontoCheque, TipoTarjeta, AutorizacionTarjeta,
                Respuesta, FolioTransferencia, FolioCheque, FechaAlta,
                IdUsuarioAlta, MontoDescuento, PorcentajeDescuento, idEsAbono, idAbono
            ) VALUES (
                @FolioVenta, @MontoAAbonar, 0,
                @MontoEfectivo, @MontoTarjetaDebito, @MontoTarjetaCredito,
                @MontoTransferencia, 0, 0,
                @MontoVales, @MontoCheque, '', '',
                '', IIF(@MontoTransferencia <> 0,@FolioTransferencia, ''), 
                IIF(@MontoCheque <> 0,@FolioCheque, ''), GETDATE(),
                @IdUsuario, 0, 0, 1, @idAbono
            );

            -- Ejecutar ValidarLiquidacionCredito si la inserción fue exitosa
            IF @@ROWCOUNT > 0
            BEGIN
                EXEC ValidarLiquidacionCredito @FolioVenta;
            END

            SET @MontoAbono = @MontoAbono - @MontoPendienteFolio;
        END
        FETCH NEXT FROM creditos_cursor INTO @FolioVenta, @MontoPendienteFolio;
    END

    CLOSE creditos_cursor;
    DEALLOCATE creditos_cursor;
END