﻿using Microsoft.Reporting.WinForms;
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
    public partial class FImprimeArqueo : Form
    {
        ConSQL sqlLoc;
        ConSQL sql;
        string folioArqueo;
        double TotalRetiros, FondoCaja;
        double TotalEfectivoDeclarado, TotalTarjetaDebitoDeclarado, TotalTarjetaCreditoDeclarado;
        double TotalEfectivoVenta, TotalTarjetaDebitoVenta, TotalTarjetaCreditoVenta;
        double VentaReal;
        string FechaDe, FechaA, Sucursal, FolioCorteParcial, Cajero, NumCaja = "0";

        private IList<Stream> m_streams;
        private int m_currentPageIndex;
        public FImprimeArqueo(ConSQL _sqlLoc, ConSQL sql_, string _folioArqueo)
        {
            InitializeComponent();
            folioArqueo = _folioArqueo;
            sqlLoc = _sqlLoc;
            sql = sql_;
        }

        private void FImprimeArqueo_Load(object sender, EventArgs e)
        {
            rpvArqueo.Width = this.Width;
            rpvArqueo.Height = this.Height;

            rpvArqueo.LocalReport.ReportEmbeddedResource = "PVLaJoya.TicketArqueo.rdlc";
            rpvArqueo.LocalReport.DataSources.Clear();

            //Info Ticket Corte Parcial
            string queryInfo = "SELECT C.FolioCorte FolioCorteParcial, S.Nombre Sucursal, V.FechaInicial, \n" +
                "V.FechaFinal, U.Nombres Cajero, Efectivo, TarjetaCredito, TarjetaDebito, " +
                "Transferencia, Monedero, Vales, FondoCaja, Retiros SalidasCaja, " +
                "TotalCaja TotalDeclarado, TotalVentas, EfectivoDeclaracion EfectivoDeclarado, " +
                "TarjetaCredDeclaracion TarjetaCreditoDeclarado, \n" +
                "TarjetaDebitoDeclaracion TarjetaDebitoDeclarado, ValesDeclaracion ValesDeclarado, \n" +
                "RetirosDeclaracion SalidasDeclarado, \n" +
                "FI.FolioTicket FolioInicial, FF.FolioTicket FolioFinal \n" +
                "FROM PVCorteCajaArqueo C LEFT JOIN \n" +
                "( \n" +
                "   SELECT FolioCorteParcialCaja, MIN(FechaVenta) FechaInicial, \n" +
                "   MAX(FechaVenta) FechaFinal FROM PVVentas WHERE Terminada=1 AND FolioCorteParcialCaja IS NULL AND CorteTerminado = 0 \n" +
                "   GROUP BY FolioCorteParcialCaja \n" +
                ") V ON V.FolioCorteParcialCaja IS NULL \n" +
                "LEFT JOIN PVSucursales S ON C.IdSucursal = S.ID \n" +
                "LEFT JOIN PVUsuarios U ON U.Id = C.IdUsuarioCorte \n" +
                "LEFT JOIN ( \n" +
                "   SELECT DISTINCT FolioVenta, FolioTicket FROM PVVentas WHERE Terminada=1 AND FolioCorteParcialCaja IS NULL AND CorteTerminado = 0\n" +
                ")FI ON FolioVentaInicial = FI.FolioVenta \n" +
                "LEFT JOIN ( \n" +
                "   SELECT DISTINCT FolioVenta, FolioTicket FROM PVVentas WHERE Terminada=1 AND FolioCorteParcialCaja IS NULL AND CorteTerminado = 0\n" +
                ")FF ON FolioVentaFinal = FF.FolioVenta \n" +
                "WHERE  FolioCorte = '" + folioArqueo + "' AND CorteFinal = 0";

            DataTable dtInfo = sqlLoc.selec(queryInfo);
            rpvArqueo.LocalReport.DataSources.Add(new ReportDataSource("InfoTicket", TablaInfo(dtInfo)));

            //MovimientosCaja
            string queryMovimientos = "SELECT 'Salida' Tipo, Retiro Cantidad, Concepto " +
                "FROM PVRetiroCaja WHERE FolioCorteParcialCaja IS NULL";
            DataTable dtMovimientos = sqlLoc.selec(queryMovimientos);
            rpvArqueo.LocalReport.DataSources.Add(new ReportDataSource("MovimientosCaja", dtMovimientos));

            //Ventas por tipo producto
            string queryVentasTP = "SELECT P.Linea, COUNT(P.Linea) Cantidad, " +
                "(SUM(VD.Precio * VD.Cantidad) + SUM(((VD.Precio * VD.Cantidad) * VD.Iva) + (VD.Precio * VD.Ieps))) Importe " +
                "FROM PVProductos P JOIN PVVentasDetalle VD ON VD.IdProducto = P.Id " +
                "GROUP BY P.Linea";
            DataTable dtVentasTP = sqlLoc.selec(queryVentasTP);
            rpvArqueo.LocalReport.DataSources.Add(new ReportDataSource("VentaTipoProducto", dtVentasTP));

            string querySucursal = "SELECT Top 1 S.Id, Nombre, CONCAT(Colonia, ' ', Calle) Calle, " +
                "CP, NumInterior, NumExterior, Telefono \n" +
                "FROM PVSucursales S \n" +
                "JOIN PVCorteCajaArqueo C ON C.IdSucursal = S.Id\n" +
                "WHERE C.FolioCorte = '" + folioArqueo + "'";

            DataTable dtnfoSucursal = sqlLoc.selec(querySucursal);
            rpvArqueo.LocalReport.DataSources.Add(new ReportDataSource("InfoSucursal", dtnfoSucursal));

            rpvArqueo.RefreshReport();


            //Imprimir();
           // this.Close();
        }


        private DataTable TablaInfo(DataTable dtInfo)
        {
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

            return dtInfo;
        }

        private void Imprimir()
        {
            Export(rpvArqueo.LocalReport);
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
        //private void Export(LocalReport report)
        //{
        //    string deviceInfo =
        //      @"<DeviceInfo><OutputFormat>EMF</OutputFormat>
        //        <PageWidth>3.5in</PageWidth>
        //        <PageHeight>15in</PageHeight>
        //        <MarginTop>0.001in</MarginTop>
        //        <MarginLeft>0.2in</MarginLeft>
        //        <MarginRight>0.001in</MarginRight>
        //        <MarginBottom>0.001in</MarginBottom>
        //    </DeviceInfo>";
        //    Warning[] warnings;
        //    m_streams = new List<Stream>();
        //    report.Render("Image", deviceInfo, CreateStream,
        //       out warnings);
        //    foreach (Stream stream in m_streams)
        //        stream.Position = 0;
        //}

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

