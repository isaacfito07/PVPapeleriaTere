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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FImprimeRetiro : Form
    {
        ConSQL sqlLoc;
        decimal ventaTotal, efectivo, tarjetaCred, tarjetaDeb, transferencia, monedero, vales, fondoCaja, retiro;
        string numCaja, cajero, folio, sucursal, fecha, hora, concepto, proveedor;

        private IList<Stream> m_streams;
        private int m_currentPageIndex;

        public FImprimeRetiro(ConSQL _sqlLoc, string _numCaja, string _cajero, string _folio, 
            string _sucursal, string _fecha, string _hora, decimal _ventaTotal, decimal _efectivo,
            decimal _tarjetaCred, decimal _tarjetaDeb, decimal _transferencia, decimal _monedero, decimal _vales, 
            decimal _fondoCaja, decimal _retiro, string _concepto, string _Proveedor)
        {
            InitializeComponent();
            sqlLoc = _sqlLoc;
            numCaja = _numCaja;
            cajero = _cajero;
            folio = _folio;
            sucursal = _sucursal;
            fecha = _fecha;
            hora = _hora;
            ventaTotal = _ventaTotal;
            efectivo = _efectivo;
            tarjetaCred = _tarjetaCred;
            tarjetaDeb = _tarjetaDeb;
            transferencia = _transferencia;
            monedero = _monedero;
            vales = _vales;
            fondoCaja = _fondoCaja;
            retiro = _retiro;
            concepto = _concepto;
            proveedor = _Proveedor;
        }

        private void fImprimeRetiro_Load(object sender, EventArgs e)
        {
            rpvRetiro.Width = this.Width;
            rpvRetiro.Height = this.Height;
            
            rpvRetiro.LocalReport.ReportEmbeddedResource = "PVLaJoya.TicketRetiro.rdlc";
            rpvRetiro.LocalReport.DataSources.Clear();

            DataTable dtRetiro = new DataTable();
            dtRetiro.Columns.Add("Fecha", typeof(string));
            dtRetiro.Columns.Add("Hora", typeof(string));
            dtRetiro.Columns.Add("VentaTotal", typeof(string));
            dtRetiro.Columns.Add("Efectivo", typeof(string));
            dtRetiro.Columns.Add("TarjetaCredito", typeof(string));
            dtRetiro.Columns.Add("TarjetaDebito", typeof(string));
            dtRetiro.Columns.Add("Transferencia", typeof(string));
            dtRetiro.Columns.Add("Monedero", typeof(string));
            dtRetiro.Columns.Add("Vales", typeof(string));
            dtRetiro.Columns.Add("FondoCaja", typeof(string));
            dtRetiro.Columns.Add("Folio", typeof(string));
            dtRetiro.Columns.Add("Sucursal", typeof(string));
            dtRetiro.Columns.Add("Cajero", typeof(string));
            dtRetiro.Columns.Add("Retiro", typeof(string));
            dtRetiro.Columns.Add("Concepto", typeof(string));
            dtRetiro.Columns.Add("Proveedor", typeof(string));
            DataRow rRetiro = dtRetiro.NewRow();
            rRetiro[0] = fecha;
            rRetiro[1] = hora;
            rRetiro[2] = ventaTotal.ToString("C2");
            rRetiro[3] = efectivo.ToString("C2");
            rRetiro[4] = tarjetaCred.ToString("C2");
            rRetiro[5] = tarjetaDeb.ToString("C2");
            rRetiro[6] = transferencia.ToString("C2");
            rRetiro[7] = monedero.ToString("C2");
            rRetiro[8] = vales.ToString("C2");
            rRetiro[9] = fondoCaja.ToString("C2");
            rRetiro[10] = folio;
            rRetiro[11] = sucursal;
            rRetiro[12] = cajero;
            rRetiro[13] = retiro.ToString("C2");
            rRetiro[14] = concepto;
            rRetiro[15] = proveedor;
            dtRetiro.Rows.Add(rRetiro);

            rpvRetiro.LocalReport.DataSources.Add(new ReportDataSource("TicketRetiro", dtRetiro));

            string querySucursal = "SELECT S.Id, Nombre, CONCAT(Colonia, ' ', Calle) Calle, " +
                "CP, NumInterior, NumExterior, Telefono \n" +
                "FROM PVSucursales S \n" +
                "WHERE S.Nombre = '" + sucursal + "'";

            DataTable dtnfoSucursal = sqlLoc.selec(querySucursal);

            rpvRetiro.LocalReport.DataSources.Add(new ReportDataSource("InfoSucursal", dtnfoSucursal));
            rpvRetiro.RefreshReport();
            Imprimir();
            this.Close();
        }

        private void Imprimir()
        {
            Export(rpvRetiro.LocalReport);
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
    }
}
