using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FImprimeCorteII : Form
    {
        ConSQL sqlLoc;
        ConSQL sql;
        string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        //ConSQLCE sqlLoc;
        string nombre, idSucursal, sucursal, idUsuarioGlob, IdCaja;
        //int idCorte;
        string folioCorte;
        double TotalRetiros, FondoCaja;
        double TotalEfectivoDeclarado, TotalTarjetaDebitoDeclarado, TotalTarjetaCreditoDeclarado;
        double TotalEfectivoVenta, TotalTarjetaDebitoVenta, TotalTarjetaCreditoVenta;
        double VentaReal;
        string FechaCorte, FechaDe, FechaA, Sucursal, FolioCorte = "", Cajero, NumCaja = "0", DeFolio = "", AFolio = "", NumInicialVenta = "N/A", NumFinalVenta = "N/A";
        string FoliosCortes = "", FoliosRetiros = "-", TotalCortesParciales;
        string DifEfectivo, DifTarjetADebito, DifTarjetaCredito;

        private IList<Stream> m_streams;
        private int m_currentPageIndex;

        public FImprimeCorteII(ConSQL _sqlLoc, string _folioCorte, ConSQL _sql, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, string _IdCaja)
        {
            InitializeComponent();
            folioCorte = _folioCorte;
            sqlLoc = _sqlLoc;

            sql = _sql;
            nombre = _nombre;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuarioGlob = _idUsuario;
            IdCaja = _IdCaja;
        }

        private void fImprimeCorteII_Load(object sender, EventArgs e)
        {
            rpvCorte.Width = this.Width;
            rpvCorte.Height = this.Height;

            rpvCorte.LocalReport.ReportEmbeddedResource = "PVLaJoya.TicketCorte.rdlc";
            rpvCorte.LocalReport.DataSources.Clear();

            //Info Ticket Corte
            string queryInfo = "SELECT C.FolioCorte FolioCorteParcial, S.Nombre Sucursal, " +
                "V.FechaInicial, \n" +
                "V.FechaFinal, U.Nombres Cajero, Efectivo, TarjetaCredito, TarjetaDebito, " +
                "Transferencia, Monedero, Vales, Cheques, FondoCaja, Retiros SalidasCaja, " +
                "TotalCaja TotalDeclarado, TotalVentas, EfectivoDeclaracion EfectivoDeclarado, " +
                "TarjetaCredDeclaracion TarjetaCreditoDeclarado, \n" +
                "TarjetaDebitoDeclaracion TarjetaDebitoDeclarado, FI.FolioTicket FolioInicial, \n" +
                "RetirosDeclaracion SalidasDeclarado, \n" +
                "FF.FolioTicket FolioFinal, ValesDeclaracion ValesDeclarado \n" +
                "FROM PVCorteCaja C LEFT JOIN \n" +
                "( \n" +
                "   SELECT FolioCorteCaja, MIN(FechaVenta) FechaInicial, \n" +
                "   MAX(FechaVenta) FechaFinal FROM PVVentas \n" +
                "   GROUP BY FolioCorteCaja \n" +
                ") V ON V.FolioCorteCaja = C.FolioCorte \n" +
                "LEFT JOIN PVSucursales S ON C.IdSucursal = S.ID \n" +
                "LEFT JOIN PVUsuarios U ON U.Id = C.IdUsuarioCorte \n" +
                "LEFT JOIN ( \n" +
                "   SELECT DISTINCT FolioVenta, FolioTicket FROM PVVentas \n" +
                ")FI ON FolioVentaInicial = FI.FolioVenta \n" +
                "LEFT JOIN ( \n" +
                "   SELECT DISTINCT FolioVenta, FolioTicket FROM PVVentas \n" +
                ")FF ON FolioVentaFinal = FF.FolioVenta \n" +
                "WHERE FolioCorte = '" + folioCorte + "' AND CorteFinal = 1";

            DataTable dtInfo = sqlLoc.selec(queryInfo);
            rpvCorte.LocalReport.DataSources.Add(new ReportDataSource("InfoTicket", TablaInfo(dtInfo)));

            //MovimientosCaja
            string queryMovimientos = "SELECT 'Salida' Tipo, Retiro Cantidad, Concepto " +
                "FROM PVRetiroCaja WHERE FolioCorteCaja = '" + folioCorte + "'";
            DataTable dtMovimientos = sqlLoc.selec(queryMovimientos);
            rpvCorte.LocalReport.DataSources.Add(new ReportDataSource("MovimientosCaja", dtMovimientos));

            //Ventas por tipo producto
            string queryVentasTP = "SELECT P.Linea, COUNT(P.Linea) Cantidad, " +
                "(SUM(VD.Precio * VD.Cantidad) + SUM(((VD.Precio * VD.Cantidad) * VD.Iva) + (VD.Precio * VD.Ieps))) Importe " +
                "FROM PVProductos P JOIN PVVentasDetalle VD ON VD.IdProducto = P.Id " +
                "JOIN PVVentas V ON VD.FolioVenta = V.FolioVenta " +
                "WHERE V.FolioCorteCaja = '" + folioCorte + "'" +
                "GROUP BY P.Linea";
            DataTable dtVentasTP = sqlLoc.selec(queryVentasTP);
            rpvCorte.LocalReport.DataSources.Add(new ReportDataSource("VentaTipoProducto", dtVentasTP));

            //Ventas por tipo producto
            string queryCierres = "SELECT FolioCorte Folio, U.Nombres Usuario \n" +
                "FROM PVCorteCaja C LEFT JOIN PVUsuarios U ON C.IdUsuarioCorte = U.Id \n" +
                "WHERE FolioCorte IN " +
                "(" +
                "   SELECT DISTINCT FolioCorteParcialCaja FROM PVVentas WHERE " +
                "   FolioCorteCaja = '" + folioCorte + "'" +
                ")" +
                "Group by FolioCorte, U.Nombres";
            DataTable dtCierres = sqlLoc.selec(queryCierres);
            rpvCorte.LocalReport.DataSources.Add(new ReportDataSource("CierresParciales", dtCierres));

            string querySucursal = "SELECT Top 1 S.Id, Nombre, CONCAT(Colonia, ' ', Calle) Calle, " +
                "CP, NumInterior, NumExterior, Telefono \n" +
                "FROM PVSucursales S \n" +
                "JOIN PVCorteCaja C ON C.IdSucursal = S.Id\n" +
                "WHERE C.FolioCorte = '" + folioCorte + "'";
            DataTable dtnfoSucursal = sqlLoc.selec(querySucursal);
            rpvCorte.LocalReport.DataSources.Add(new ReportDataSource("InfoSucursal", dtnfoSucursal));
            
            string queryVentasUsuario = "SELECT ROUND(SUM(ISNULL(V.TotalVenta,0)),2) Total, \n" +
                "U.Nombres Usuario \n" +
                "FROM PVVentas V LEFT JOIN PVUsuarios U ON V.IdUsuarioventa = U.Id \n" +
                "WHERE FolioCorteCaja = '" + folioCorte + "' GROUP BY U.Nombres";
            DataTable dtVentasUsuario = sqlLoc.selec(queryVentasUsuario);
            rpvCorte.LocalReport.DataSources.Add(new ReportDataSource("VentasUsuario", dtVentasUsuario));

            rpvCorte.RefreshReport();
            Imprimir();

            FDisparoNube disparoNube = new FDisparoNube(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuarioGlob, IdCaja);
            disparoNube.ShowDialog();

            this.Close();
        }

        private DataTable TablaInfo(DataTable dtInfo)
        {
            if (dtInfo.Rows.Count > 0) {
                DataColumn efectivoFinal = new DataColumn("EfectivoFinal", typeof(string));
                double sumEfectivo =
                    (double)dtInfo.Rows[0]["Efectivo"] - (double)dtInfo.Rows[0]["SalidasCaja"];
                efectivoFinal.DefaultValue = sumEfectivo.ToString();
                dtInfo.Columns.Add(efectivoFinal);

                DataColumn noEfectivoFinal = new DataColumn("NoEfectivoFinal", typeof(string));
                double sumNoEfectivo = (double)dtInfo.Rows[0]["TarjetaCredito"] + (double)dtInfo.Rows[0]["TarjetaDebito"]
                    + (double)dtInfo.Rows[0]["Transferencia"] + (double)dtInfo.Rows[0]["Monedero"]
                    + (double)dtInfo.Rows[0]["Vales"];
                efectivoFinal.DefaultValue = sumNoEfectivo.ToString();

                DataColumn totalCaja = new DataColumn("TotalCaja", typeof(string));
                efectivoFinal.DefaultValue = (sumNoEfectivo + sumEfectivo).ToString();
            }
            return dtInfo;
        }

        private void Imprimir()
        {
            Export(rpvCorte.LocalReport);
            Print();
        }

        //<MarginTop>0.1in</MarginTop>
        //       <MarginLeft>0.1in</MarginLeft>
        //       <MarginRight>0.1in</MarginRight>
        //       <MarginBottom>0.1in</MarginBottom>
        private void Export(LocalReport report)
        {
            string deviceInfo =
              @"<DeviceInfo><OutputFormat>EMF</OutputFormat>
                <PageWidth>3.5in</PageWidth>
                <PageHeight>12in</PageHeight>
                <MarginTop>0.001in</MarginTop>
                <MarginLeft>0.2in</MarginLeft>
                <MarginRight>0.001in</MarginRight>
                <MarginBottom>0.2in</MarginBottom>
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
    }
}
