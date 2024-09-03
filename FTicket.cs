using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FTicket : Form
    {
        ConSQL sqlLoc;
        string folioVenta;

        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        bool imprimir;

        public FTicket(ConSQL _sqlLoc, string _folioVenta, bool _imprimir)
        {
            InitializeComponent();

            folioVenta = _folioVenta;
            sqlLoc = _sqlLoc;
            imprimir = _imprimir;
        }

        private void fTicket_Load(object sender, EventArgs e)
        {
            rpTicket1.Width = this.Width;
            rpTicket1.Height = this.Height;

            rpTicket1.LocalReport.ReportEmbeddedResource = "PVLaJoya.TicketCliente.rdlc";
            rpTicket1.LocalReport.DataSources.Clear();

            //VENTA
            //string queryInfo = "SELECT V.FolioTicket FolioVenta, C.Nombre Cliente, \n" +
            //    "FORMAT(V.FechaVenta, 'dd/MM/yyyy HH:mm:ss tt') FechaVenta, U.Nombres Atendio, \n" +
            //    "S.Nombre Sucursal, VD.Subtotal, VD.IvaIeps, V.TotalVenta Total, \n" +
            //    "P.MontoRecibido Recibido, P.Cambio, \n" +
            //    ".dbo.CantidadConLetraMoneda(V.TotalVenta) MontoLetra \n" +
            //    "FROM PVVentas V \n" +
            //    "LEFT JOIN PVClientes C ON V.IdCliente = C.Id \n" +
            //    "LEFT JOIN PVUsuarios U ON V.IdUsuarioVenta = U.Id \n" +
            //    "LEFT JOIN PVSucursales S ON V.IdSucursal = S.Id \n" +
            //    "LEFT JOIN PVVentaPago P ON P.FolioVenta = V.FolioVenta \n" +
            //    "LEFT JOIN ( \n" +
            //    "   SELECT FolioVenta, SUM((Precio - MontoDescuento)* Cantidad) Subtotal, \n" +
            //    "   SUM((((Precio - MontoDescuento) * Cantidad) * iva) " +
            //    "       + (((Precio - MontoDescuento) * Cantidad) * ieps)) IvaIeps \n" +
            //    "   FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "' \n" +
            //    "   GROUP BY FolioVenta \n" +
            //    ") VD ON VD.FolioVenta = V.FolioVenta \n" +
            //    "WHERE V.FolioVenta = '" + folioVenta + "'";

            string queryInfo = "SELECT V.FolioTicket FolioVenta, C.Nombre Cliente, \n"
                            + " FORMAT(V.FechaVenta, 'dd/MM/yyyy HH:mm:ss tt') FechaVenta, U.Nombres Atendio, \n"
                            + " S.Nombre Sucursal, (VD.Subtotal - VD.IvaIeps) Subtotal, VD.IvaIeps, V.TotalVenta Total, \n"
                            + " P.MontoRecibido Recibido, P.Cambio, \n"
                            + " .dbo.CantidadConLetraMoneda(V.TotalVenta) MontoLetra, ISNULL(MC.MontoMonedero,0) MontoMonedero\n"
                            + " FROM PVVentas V \n"
                            + " LEFT JOIN PVClientes C ON V.IdCliente = C.Id \n"
                            + " LEFT JOIN PVUsuarios U ON V.IdUsuarioVenta = U.Id \n"
                            + " LEFT JOIN PVSucursales S ON V.IdSucursal = S.Id \n"
                            + " LEFT JOIN PVVentaPago P ON P.FolioVenta = V.FolioVenta \n"
                            + " LEFT JOIN ( \n"
                            + "    SELECT FolioVenta, SUM((Precio - MontoDescuento)* Cantidad) Subtotal, \n"
                            + "    SUM(Cantidad * ((PrecioSinImpuesto * Iva) + (PrecioSinImpuesto * Ieps))) AS IvaIeps \n"
                            + "    FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'\n"
                            + "    GROUP BY FolioVenta \n"
                            + " ) VD ON VD.FolioVenta = V.FolioVenta \n"
                             + " LEFT JOIN (\n"
                           + "     SELECT\n"
                            + "     M.IdCliente, CAST(SUM(ISNULL(M.Monto, 0)) AS DECIMAL (5,2)) + CAST(ISNULL(D.MontoDevolucion, 0) AS DECIMAL (5,2)) MontoMonedero,\n"
                            + "     SUM(M.Monto) Monto, ISNULL(D.MontoDevolucion, 0) MontoDevolucion\n"
                            + " FROM PVMonederoCliente M\n"
                            + " LEFT JOIN\n"
                            + " (\n"
                            + "     SELECT\n"
                            + "         IdCliente, SUM(MontoDevolucion) MontoDevolucion\n"
                            + "     FROM PVDevoluciones WHERE Monedero = 1 AND Activo = 1  GROUP BY IdCliente\n"
                            + " ) D ON M.IdCliente = D.IdCliente\n"
                            + " WHERE M.Valido = 1\n"
                            + " GROUP BY M.IdCliente, ISNULL(D.MontoDevolucion, 0)\n"
                           + " ) MC ON C.Id = MC.IdCliente\n"
                            + " WHERE V.FolioVenta = '" + folioVenta + "'";


            

            DataTable dtInfoTicket = sqlLoc.selec(queryInfo);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("Ticket", dtInfoTicket));

            //Detalle
            //string queryDetalle = "SELECT " +
            //    "CONCAT(P.Descripcion, ' ', Marca, ' ', Presentacion, ' ', " +
            //    "CASE WHEN EsCaja = 1 THEN CONCAT('C/',VD.Uom ) ELSE 'PZA' END) Producto, \n" +
            //    "VD.Cantidad, (VD.Precio + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps)) Precio, \n" + // + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps)
            //    "VD.MontoDescuento Descuento, \n" +
            //    "ROUND((VD.Cantidad * (VD.Precio  + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps)) " + // + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps)
            //    "   - VD.MontoDescuento), 2) Total \n" +
            //    "FROM PVVentasDetalle VD LEFT JOIN PVProductos P ON VD.IdProducto = P.Id \n" +
            //    "
            //    "WHERE VD.FolioVenta = '" + folioVenta + "'";

            string queryDetalle = " SELECT \n"
	                + "     CASE WHEN PVP.sku != '' OR PVP.sku IS NOT NULL THEN\n"
                    + "         CONCAT(P.Descripcion, ' ', Marca, ' ', P.Presentacion, ' ', \n"
                    + "         '#Ref. ', VD.NumeroTelefonico, '  #Trans. ', VD.FolioTransaccion) \n"
                    + "     ELSE\n"
                    + "         CONCAT(P.Descripcion, ' ', Marca, ' ', P.Presentacion, ' ', \n"
                    + "         CASE WHEN EsCaja = 1 THEN CONCAT('C/',VD.Uom ) ELSE 'PZA' END)\n"
                    + "     END\n"
                    + "     Producto,\n"
                    + "     VD.Cantidad, (VD.Precio) Precio,\n"
                    + "     VD.MontoDescuento Descuento, \n"
                    + "     ROUND((VD.Cantidad * (VD.Precio) - VD.MontoDescuento), 2) Total\n"
                    + " FROM PVVentasDetalle VD LEFT JOIN PVProductos P ON VD.IdProducto = P.Id \n"
                    + " LEFT JOIN PVPresentacionesVentaProd PVP ON VD.IdPresentacionProducto = PVP.Id \n"
                    + " WHERE VD.FolioVenta = '" + folioVenta + "' ";

            DataTable dtDetalle = sqlLoc.selec(queryDetalle);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("DetalleVenta", dtDetalle));

            //FormasPago
            //string queryFormasPago = "SELECT FormaPago, Value Cantidad, 0 Descuento FROM \n" +
            //    "( \n" +
            //    "   SELECT FolioVenta, MontoEfectivo as Efectivo, \n" +
            //    "   MontoTarjeta as Tarjeta, MontoTransferencia as Transferencia, \n" +
            //    "   MontoMonedero as Monedero, MontoVales as Vales \n" +
            //    "   FROM PVVentaPago \n" +
            //    ") P UNPIVOT \n" +
            //    "(Value FOR FormaPago IN \n" +
            //    "   (Efectivo, Tarjeta, Transferencia, Monedero, Vales) \n" +
            //    ")AS unpvt " +
            //    "WHERE Value > 0 AND FolioVenta = '" + folioVenta + "'";

            //string queryFormasPago = "SELECT FormaPago, Value Cantidad, 0 Descuento FROM \n"
            //                        + " ( \n"
            //                        + "     SELECT FolioVenta, MontoEfectivo as Efectivo, \n"
            //                        + "     ISNULL(CASE WHEN MontoTarjeta > 0 AND TipoTarjeta='Débito' THEN MontoTarjeta END,0) [Tarjeta Débito],\n"
            //                        + "     ISNULL(CASE WHEN MontoTarjeta > 0 AND TipoTarjeta='Crédito' THEN MontoTarjeta END,0) [Tarjeta Crédito], \n"
            //                        + "     MontoTransferencia as Transferencia, \n"
            //                        + "     MontoMonedero as Monedero, MontoVales as Vales \n"
            //                        + "     FROM PVVentaPago \n"
            //                        + " ) P UNPIVOT \n"
            //                        + " (Value FOR FormaPago IN \n"
            //                        + "     (Efectivo, [Tarjeta Débito], [Tarjeta Crédito], Transferencia, Monedero, Vales) \n"
            //                        + " )AS unpvt \n"
            //                        + " WHERE Value > 0 AND FolioVenta = '" + folioVenta + "'";

            string queryFormasPago = "SELECT FolioVenta, (MontoEfectivo + MontoTarjeta + MontoTarjetaCredito + MontoTransferencia + MontoMonedero + MontoVales + MontoCredito + MontoDescuento) Cantidad,\n"
                                    + " MontoEfectivo, MontoTarjeta, MontoTarjetaCredito, MontoTransferencia, MontoMonedero, MontoVales, MontoCredito, MontoDescuento,\n"
                                    + " (MontoEfectivo + MontoTarjeta + MontoTarjetaCredito + MontoTransferencia + MontoMonedero + MontoVales) Recibido, "
                                    + " CONCAT(\n"
                                    + "         CASE WHEN MontoEfectivo > 0 THEN ' -Efectivo' END, \n"
                                    + "         CASE WHEN MontoTarjeta > 0  THEN ' -Tarjeta de Débito' END,\n"
                                    + "         CASE WHEN MontoTarjetaCredito > 0  THEN ' -Tarjeta de Crédito' END,\n"
                                    + "         CASE WHEN MontoTransferencia > 0 THEN ' -Transferencia' END,\n"
                                    + "         CASE WHEN MontoMonedero > 0 THEN ' -Monedero' END,\n"
                                    + "         CASE WHEN MontoVales > 0 THEN ' -Vale' END,\n"
                                    + "         CASE WHEN MontoCredito > 0 THEN ' -Crédito' END\n"
                                    + "     ) FormaPago\n"
                                    + " FROM PVVentaPago \n"
                                    + " WHERE FolioVenta='" + folioVenta + "'";



            DataTable dtFormaPago = sqlLoc.selec(queryFormasPago);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("DetalleFormaPago", dtFormaPago));

            string querySucursal = "SELECT S.Id, Nombre, CONCAT(Colonia, ' ', Calle) Calle, " +
                "CP, NumInterior, NumExterior, Telefono \n" +
                "FROM PVSucursales S JOIN PVVentas V ON V.IdSucursal = S.Id \n" +
                "WHERE FolioVenta = '" + folioVenta + "'";

            DataTable dtnfoSucursal = sqlLoc.selec(querySucursal);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("InfoSucursal", dtnfoSucursal));

            rpTicket1.RefreshReport();

            Export(rpTicket1.LocalReport);
            if (imprimir)
            {
                Imprimir();
                this.Close();
            }
        }

        private void Imprimir()
        {
            //Export(rpTicket1.LocalReport);
            Print();
        }

        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo><OutputFormat>EMF</OutputFormat>
                <PageWidth>3.5in</PageWidth>
                <PageHeight>14.5in</PageHeight>
                <MarginTop>0.001in</MarginTop>
                <MarginLeft>0.2in</MarginLeft>
                <MarginRight>0.001in</MarginRight>
                <MarginBottom>0.001in</MarginBottom>
            </DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream,
               out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }

        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        private void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }

        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
               Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        private void rpTicket1_Load(object sender, EventArgs e)
        {

        }
    }
}
