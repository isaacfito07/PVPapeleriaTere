select * from PVRetiroCaja

SELECT 'Salida' Tipo, Retiro, Concepto FROM PVRetiroCaja WHERE FolioCorteParcialCaja = ''

SELECT P.Linea, COUNT(P.Linea) Cantidad,
(SUM(VD.Precio * VD.Cantidad) + SUM(((VD.Precio * VD.Cantidad) * VD.Iva) + (VD.Precio * VD.Ieps))) Importe
FROM PVProductos P LEFT JOIN PVVentasDetalle VD ON VD.IdProducto = P.Id
GROUP BY P.Linea

