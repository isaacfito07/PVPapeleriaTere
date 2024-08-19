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
    public partial class FTicketAbono : Form
    {
        ConSQL sqlLoc;
        int idAbono = 0;

        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        bool imprimir;

        public FTicketAbono(ConSQL _sqlLoc, int _idAbono, bool _imprimir)
        {
            InitializeComponent();

            idAbono = _idAbono;
            sqlLoc = _sqlLoc;
            imprimir = _imprimir;
        }

        private void fTicket_Load(object sender, EventArgs e)
        {
            rpTicket1.Width = this.Width;
            rpTicket1.Height = this.Height;

            rpTicket1.LocalReport.ReportEmbeddedResource = "PVLaJoya.TicketAbono.rdlc";
            rpTicket1.LocalReport.DataSources.Clear();

            string querySucursal = "SELECT TOP 1 Id, Nombre, CONCAT(Colonia, ' ', Calle) Calle, CP, NumInterior, NumExterior, Telefono \n" +
                "FROM PVSucursales";

            DataTable dtnfoSucursal = sqlLoc.selec(querySucursal);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("InfoSucursal", dtnfoSucursal));

            string queryAbono = "SELECT b.id, a.Nombre AS Cliente, b.Monto AS MontoAbono, FORMAT(b.FechaAlta, 'dd/MM/yyyy') AS Fecha \n" +
                "FROM PVClientes a, PVAbono b, PVVentaPago c " +
                "WHERE b.IdCliente = a.id AND c.idABono = b.id AND b.id = "+idAbono+" \n"+
                "GROUP BY c.idAbono, a.Nombre, b.monto, b.id, b.FechaAlta";

            DataTable dtInfoAbono = sqlLoc.selec(queryAbono);

            rpTicket1.LocalReport.DataSources.Add(new ReportDataSource("InfoTicketAbono", dtInfoAbono));

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
