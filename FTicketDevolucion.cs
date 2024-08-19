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
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace PVLaJoya
{
    public partial class FTicketDevolucion : Form
    {
        ConSQL sqlLoc;
        string folioVenta, fechaAlta;

        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        bool imprimir;

        public FTicketDevolucion(ConSQL _sqlLoc, string _folioVenta, string _fechaHora, bool _imprimir)
        {
            InitializeComponent();

            folioVenta = _folioVenta;
            fechaAlta = _fechaHora;
            sqlLoc = _sqlLoc;
            imprimir = _imprimir;
        }

        private void FTicketDevolucion_Load(object sender, EventArgs e)
        {
            rpTicket1.Width = this.Width;
            rpTicket1.Height = this.Height;

            rpTicket1.LocalReport.ReportEmbeddedResource = "PVLaJoya.Devolucion.rdlc";
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

            string queryInfo = "SELECT \n"
	                           + "     V.FolioTicket FolioVenta, C.Nombre Cliente, V.FechaVenta, D.FechaAlta FechaDevolucion, U.Nombres Atendio, S.Nombre Sucursal, 0 SubTotal, 0 IvaIeps, \n"
                               + "     D.Motivo, D.InventarioDefectuoso, CASE WHEN ISNULL(D.Monedero,0) = 1 THEN 'Monedero electrónico' ELSE 'Cambio físico' END Accion,\n"
                               + "     SUM(ISNULL(D.MontoDevolucion,0)) Total, \n"
                               + "     .dbo.CantidadConLetraMoneda(SUM(ISNULL(D.MontoDevolucion,0))) MontoLetra, FORMAT(ROUND(ISNULL(MontoMonederoActual,0),2), 'C') MontoMonederoActual, \n"
                               + "     CASE WHEN ISNULL(D.Monedero,0) = 1 THEN FORMAT(ROUND(ISNULL(MontoMonederoActual,0)-SUM(ISNULL(D.MontoDevolucion,0)),2), 'C') ELSE FORMAT(ROUND(ISNULL(MontoMonederoActual,0),0), 'C') END MontoMonederoAnterior\n"
                               + " FROM PVDevoluciones D\n"
                               + " LEFT JOIN PVVentas V ON D.FolioVenta = V.FolioVenta\n"
                               + " LEFT JOIN PVClientes C ON D.IdCliente = C.Id\n"
                               + " LEFT JOIN PVUsuarios U ON D.IdUsuarioAlta = U.Id\n"
                               + " LEFT JOIN PVSucursales S ON V.IdSucursal = S.Id\n"
                               + " LEFT JOIN (\n"
                               + "  SELECT\n"
                               + "     M.IdCliente, CAST(SUM(ISNULL(M.Monto, 0)) AS DECIMAL (5,2)) + CAST(ISNULL(D.MontoDevolucion, 0) AS DECIMAL (5,2)) MontoMonederoActual\n"
                               + "     FROM PVMonederoCliente M\n"
                               + " LEFT JOIN\n"
                               + " (\n"
                               + "     SELECT\n"
                               + "         IdCliente, SUM(MontoDevolucion) MontoDevolucion\n"
                               + "     FROM PVDevoluciones WHERE Monedero = 1 AND Activo = 1  GROUP BY IdCliente\n"
                               + " ) D ON M.IdCliente = D.IdCliente\n"
                               + " WHERE M.Valido = 1 \n"
                               + " GROUP BY M.IdCliente, ISNULL(D.MontoDevolucion, 0)\n"
                               + " ) MC ON C.Id = MC.IdCliente\n"
                               + " WHERE D.Activo=1 AND D.FolioVenta='"+ folioVenta +"' AND D.FechaAlta='"+ fechaAlta +"'\n"
                               + " GROUP BY V.FolioTicket, C.Nombre, V.FechaVenta, U.Nombres, S.Nombre, MC.MontoMonederoActual,\n"
                               + " D.Motivo, D.InventarioDefectuoso, D.Monedero, D.FechaAlta";

            DataTable dtInfoTicket = sqlLoc.selec(queryInfo);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("Ticket", dtInfoTicket));


            string queryDetalle = " SELECT \n"
                                  + "     CONCAT(P.Descripcion, ' ', Marca, ' ', P.Presentacion, ' ', CASE WHEN D.EsCaja = 1 THEN CONCAT('C/', D.Uom) ELSE 'PZA' END) Producto,\n"
                                  + "     D.CantidadDevuelta Cantidad,  (MontoDevolucion / CASE WHEN ISNULL(CantidadDevuelta, 1) = 0 THEN 1 ELSE CantidadDevuelta END) Precio,\n"
                                  + "     0 Descuento,  D.MontoDevolucion Total\n"
                                  + " FROM PVDevoluciones D\n"
                                  + " LEFT JOIN PVProductos P ON D.IdProducto = P.Id\n"
                                  + " WHERE D.Activo = 1 AND D.FolioVenta = '"+ folioVenta + "' AND D.FechaAlta = '"+ fechaAlta + "'";

            DataTable dtDetalle = sqlLoc.selec(queryDetalle);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("DetalleVenta", dtDetalle));

           
          
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
