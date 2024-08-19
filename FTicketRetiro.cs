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
    public partial class FTicketRetiro : Form
    {
        ConSQL sqlLoc;
        //string folioVenta;
        string idSucursal;

        private IList<Stream> m_streams;
        private int m_currentPageIndex;

        public FTicketRetiro(ConSQL _sqlLoc, string _idSucursal)
        {
            InitializeComponent();

            //folioVenta = _folioVenta;
            idSucursal = _idSucursal;
            sqlLoc = _sqlLoc;
        }

        private void FTicketRetiro_Load(object sender, EventArgs e)
        {
            rpTicket1.Width = this.Width;
            rpTicket1.Height = this.Height;

            rpTicket1.LocalReport.ReportEmbeddedResource = "PVLaJoya.TicketRetiro.rdlc";
            rpTicket1.LocalReport.DataSources.Clear();

            DataTable dtRetiro = new DataTable();
            dtRetiro.Columns.Add("Fecha", typeof(string));
            dtRetiro.Columns.Add("Hora", typeof(string));
            dtRetiro.Columns.Add("VentaTotal", typeof(string));
            dtRetiro.Columns.Add("Efectivo", typeof(string));
            dtRetiro.Columns.Add("Tarjeta", typeof(string));
            dtRetiro.Columns.Add("Transferencia", typeof(string));
            dtRetiro.Columns.Add("Monedero", typeof(string));
            dtRetiro.Columns.Add("Vales", typeof(string));
            dtRetiro.Columns.Add("FondoCaja", typeof(string));
            dtRetiro.Columns.Add("Folio", typeof(string));
            dtRetiro.Columns.Add("Sucursal", typeof(string));
            dtRetiro.Columns.Add("Cajero", typeof(string));
            dtRetiro.Columns.Add("Retiro", typeof(string));
            dtRetiro.Columns.Add("Concepto", typeof(string));
            DataRow rRetiro = dtRetiro.NewRow();
            rRetiro[0] = "fecha";
            rRetiro[1] = "hora";
            rRetiro[2] = "100";
            rRetiro[3] = "100";
            rRetiro[4] = "100";
            rRetiro[5] = "100";
            rRetiro[6] = "100";
            rRetiro[7] = "100";
            rRetiro[8] = "100";
            rRetiro[9] = "folio";
            rRetiro[10] = "sucursal";
            rRetiro[11] = "cajero";
            rRetiro[12] = "100";
            rRetiro[13] = "concepto";
            dtRetiro.Rows.Add(rRetiro);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("TicketRetiro", dtRetiro));
            rpTicket1.RefreshReport();

            string querySucursal = "SELECT S.Id, Nombre, CONCAT(Colonia, ' ', Calle) Calle, " +
                "CP, NumInterior, NumExterior, Telefono \n" +
                "FROM PVSucursales S "+
                "WHERE Id = '" + idSucursal + "'";

            DataTable dtnfoSucursal = sqlLoc.selec(querySucursal);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("InfoSucursal", dtRetiro));
            Imprimir();
            this.Close();
        }

        private void Imprimir()
        {
            Export(rpTicket1.LocalReport);
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