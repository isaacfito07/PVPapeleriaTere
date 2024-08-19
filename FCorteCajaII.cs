using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FCorteCajaII : Form
    {
        ConSQL sql, sqlLoc;
        //ConSQLCE sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja, usuarioAutGuardado;

        //Folios (Inicial y final de venta)
        string FI = "", FF = "";

        double Efectivo = 0;
        double TarjetaDebito = 0;
        double TarjetaCredito = 0;
        double Transferencia = 0;
        double Monedero = 0;
        double Vales = 0;
        double Cheques = 0;
        double VentaTotal = 0;
        double TotalRetiro = 0;
        double FondoCaja = 0;

        bool esCorteFinal = false;

        int Folio;

        public FCorteCajaII(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, bool _consulta, string _numCaja)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
            nombre = _nombre;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuario = _idUsuario;
            numCaja = _numCaja;

            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja_.Text = "CAJA: " + numCaja;
        }

        private void fCorteCajaII_Load(object sender, EventArgs e)
        {
            CargaDatos(0);
        }

        public void CargaDatos(int CF)
        {
            //0 Corte Parcial
            //1 Corte Final
            
            esCorteFinal = true;
            

            //Toma las ventas de esa sucursal que no tenfa folio corte fina pero si corte parcial
            string query =
                "SELECT V.FolioVenta, V.FolioTicket, V.TotalVenta, PV.MontoRecibido, PV.Cambio, PV.MontoEfectivo, \n"
                + " PV.MontoTarjeta, PV.MontoTransferencia, PV.MontoMonedero, PV.MontoVales, PV.MontoCheque, PV.TipoTarjeta \n"
                + " FROM PVVentas V LEFT JOIN PVVentaPago PV ON V.FolioVenta = PV.FolioVenta \n"
                + " WHERE V.IdSucursal = " + idSucursal + " \n"
                + " AND V.Terminada = 1 AND V.Valido=1 AND FolioCorteParcialCaja IS NOT NULL \n" 
                + " AND FolioCorteCaja IS NULL AND CorteTerminado = 0 \n"
                + " ORDER BY V.FechaVenta";

            DataTable dtVentas = sqlLoc.selec(query);

            /*Toma el último fondo de caja ingresado de esa sucursal y usuario
            var Fondo = sqlLoc.scalar(" SELECT TOP(1) Monto FROM PVFondoCaja \n"
                + " WHERE IdSucursal='" + idSucursal + "' " 
                + " " + ConcatCorteFondo + " ORDER BY Fecha DESC ");*/

            FondoCaja = 0;
            /*if (Fondo != null)
                if (double.TryParse(Fondo.ToString(), out FondoCaja)) { }*/

            //Retiros que no tengan corte final pero si corte parcial
            string queryR = " SELECT ISNULL(SUM(ISNULL(RC.Retiro,0)),0) Retiro FROM PVRetiroCaja RC \n"
             + " WHERE RC.IdSucursal = " + idSucursal + " " 
             + " AND RC.FolioCorteParcialCaja IS NOT NULL AND RC.FolioCorteCaja IS NULL";

            DataTable dtRetiros = sqlLoc.selec(queryR);
            if (dtRetiros.Rows.Count > 0)
                TotalRetiro = (dtRetiros.Rows[0]["Retiro"] == DBNull.Value) ? 0 : (double)dtRetiros.Rows[0]["Retiro"];

            if (dtVentas.Rows.Count > 0)
            {
                //Folio inicial y final
                int ind = dtVentas.Rows.Count - 1;
                FI = dtVentas.Rows[0]["FolioTicket"].ToString();
                FF = dtVentas.Rows[ind]["FolioTicket"].ToString();

                //Pagado con efectivo
                var montoEfectivo = dtVentas.Compute("SUM(MontoEfectivo)", "");
                var montoCambio = dtVentas.Compute("SUM(Cambio)", "");

                var efectivo = (montoEfectivo == DBNull.Value) ? 0 : (double)montoEfectivo;
                var cambio = (montoCambio == DBNull.Value) ? 0 : (double)montoCambio;
                Efectivo = efectivo - cambio;

                //Pagado con tarjeta de débito
                var montoTarjetaDebito = dtVentas.Compute("SUM(MontoTarjeta)", "TipoTarjeta='Débito'");
                TarjetaDebito += (montoTarjetaDebito == DBNull.Value) ? 0 : (double)montoTarjetaDebito;

                // Pagado con tarjeta de crédito
                var montoTarjetaCredito = dtVentas.Compute("SUM(MontoTarjeta)", "TipoTarjeta='Crédito'");
                TarjetaCredito += (montoTarjetaCredito == DBNull.Value) ? 0 : (double)montoTarjetaCredito;

                //Pagado con transferencia
                var montoTransferencia = dtVentas.Compute("SUM(MontoTransferencia)", "");
                Transferencia += (montoTransferencia == DBNull.Value) ? 0 : (double)montoTransferencia;

                //Pagado con monedero
                var montoMonedero = dtVentas.Compute("SUM(MontoMonedero)", "");
                Monedero += (montoMonedero == DBNull.Value) ? 0 : (double)montoMonedero;

                //Pagado con vales
                var montoVales = dtVentas.Compute("SUM(MontoVales)", "");
                Vales += (montoVales == DBNull.Value) ? 0 : (double)montoVales;

                //pagado con cheques
                var montoCheques = dtVentas.Compute("SUM(MontoCheque)", "");
                Cheques += (montoCheques == DBNull.Value) ? 0 : (double)montoCheques;

                VentaTotal = (double)dtVentas.Compute("SUM(TotalVenta)", "");


                //SumaTotales();
                //nud500.Select(0, nud500.Text.Length);
            }

            GenerarCorte();
        }

        //Sumatoria de totales
        private void nud500_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nud200_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nud100_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nud50_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nud20_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudM20_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudM10_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudM5_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudM2_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudM1_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudM50C_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        //--------------------- Selec a cada control para evitar borrar 0

        private void nud500_Click(object sender, EventArgs e)
        {
            nud500.Select(0, nud500.Text.Length);
            SumaTotales();
        }

        private void nud200_Click(object sender, EventArgs e)
        {
            nud200.Select(0, nud200.Text.Length);
            SumaTotales();
        }

        private void nud100_Click(object sender, EventArgs e)
        {
            nud100.Select(0, nud100.Text.Length);
            SumaTotales();
        }

        private void nud50_Click(object sender, EventArgs e)
        {
            nud50.Select(0, nud50.Text.Length);
            SumaTotales();
        }

        private void nud20_Click(object sender, EventArgs e)
        {
            nud20.Select(0, nud20.Text.Length);
            SumaTotales();
        }

        private void nudM20_Click(object sender, EventArgs e)
        {
            nudM20.Select(0, nudM20.Text.Length);
            SumaTotales();
        }

        private void nudM10_Click(object sender, EventArgs e)
        {
            nudM10.Select(0, nudM10.Text.Length);
            SumaTotales();
        }

        private void nudM5_Click(object sender, EventArgs e)
        {
            nudM5.Select(0, nudM5.Text.Length);
            SumaTotales();
        }

        private void nudM2_Click(object sender, EventArgs e)
        {
            nudM2.Select(0, nudM2.Text.Length);
            SumaTotales();
        }

        private void nudM1_Click(object sender, EventArgs e)
        {
            nudM1.Select(0, nudM1.Text.Length);
            SumaTotales();
        }

        private void nudTotalVales_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudTotalVales_Click(object sender, EventArgs e)
        {
            nudTotalVales.Select(0, nudTotalVales.Text.Length);
            SumaTotales();
        }

        private void nudM50C_Click(object sender, EventArgs e)
        {
            nudM50C.Select(0, nudM50C.Text.Length);
            SumaTotales();
        }

        private void btnDeclaracionCorte_Click(object sender, EventArgs e)
        {

        }

        private void nudTotalDebitoTerminal_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }

        private void nudTotalCreditoTerminal_ValueChanged(object sender, EventArgs e)
        {
            SumaTotales();
        }


        private void nudTotalReal_ValueChanged(object sender, EventArgs e)
        {
            nudTotalReal.Text = nudTotalReal.Text.Replace(',', '.');
        }

        //-----------------------------------------------------

        public void SumaTotales()
        {
            decimal totalReal = 0;

            if (nud500.Value.ToString() == "")
                nud500.Value = 0;
            if (nud200.Value.ToString() == "")
                nud200.Value = 0;
            if (nud100.Value.ToString() == "")
                nud100.Value = 0;
            if (nud50.Value.ToString() == "")
                nud50.Value = 0;
            if (nud20.Value.ToString() == "")
                nud20.Value = 0;
            if (nudM20.Value.ToString() == "")
                nudM20.Value = 0;
            if (nudM10.Value.ToString() == "")
                nudM10.Value = 0;
            if (nudM5.Value.ToString() == "")
                nudM5.Value = 0;
            if (nudM2.Value.ToString() == "")
                nudM2.Value = 0;
            if (nudM1.Value.ToString() == "")
                nudM1.Value = 0;
            if (nudM50C.Value.ToString() == "")
                nudM50C.Value = 0;

            //Billetes
            totalReal += nud500.Value * 500;
            totalReal += nud200.Value * 200;
            totalReal += nud100.Value * 100;
            totalReal += nud50.Value * 50;
            totalReal += nud20.Value * 20;

            //Modedas
            totalReal += nudM20.Value * 20;
            totalReal += nudM10.Value * 10;
            totalReal += nudM5.Value * 5;
            totalReal += nudM2.Value * 2;
            totalReal += nudM1.Value * 1;
            totalReal += nudM50C.Value * (decimal)(0.5);

            //Total terminal
            totalReal += nudTotalDebitoTerminal.Value;
            totalReal += nudTotalCreditoTerminal.Value;
            totalReal += nudTotalVales.Value;

            nudTotalReal.Value = totalReal;
        }

        public decimal TotalEfectivo()
        {
            decimal totalEfectivo = 0;

            //Billetes
            totalEfectivo += nud500.Value * 500;
            totalEfectivo += nud200.Value * 200;
            totalEfectivo += nud100.Value * 100;
            totalEfectivo += nud50.Value * 50;
            totalEfectivo += nud20.Value * 20;

            //Modedas
            totalEfectivo += nudM20.Value * 20;
            totalEfectivo += nudM10.Value * 10;
            totalEfectivo += nudM5.Value * 5;
            totalEfectivo += nudM2.Value * 2;
            totalEfectivo += nudM1.Value * 1;
            totalEfectivo += nudM50C.Value * (decimal)(0.5);

            return totalEfectivo;
        }

        //Guarda corte parcial
        private void GenerarCorte()
        {       
            //usuarioAut = usuarioAutGuardado;
            string idUsuarioAut = idUsuario;

            string hoy = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        
            //Consultar folios parciales para tomar declaraciones 
            string queryCP = "SELECT FolioCorte, IdSucursal, EfectivoDeclaracion, " +
                "TarjetaCredDeclaracion, TarjetaDebitoDeclaracion, ValesDeclaracion \n" +
                "FROM PVCorteCaja WHERE CorteFinal = 0 AND Foliocorte in " +
                "(" +
                "   SELECT DISTINCT FolioCorteParcialCaja FROM PVVentas V \n" +
                "   WHERE V.Terminada = 1 AND FolioCorteParcialCaja IS NOT NULL \n" +
                "   AND FolioCorteCaja IS NULL " +
                ") \n";

            DataTable dtCortesParciales = sqlLoc.selec(queryCP);

            double TotalEfectivoDeclaracion = 0;
            double TotalTarjetaCreditoDeclaracion = 0;
            double TotalTarjetaDebitoDeclaracion = 0;
            double TotalValesDeclaracion = 0;
            double MontoReal = 0;

            double TotalDeclarado = 0;
            
            if (dtCortesParciales.Rows.Count > 0)
            {
                var montoEfectivoDeclarado = dtCortesParciales.Compute("SUM(EfectivoDeclaracion)", "");
                TotalEfectivoDeclaracion = 
                    (montoEfectivoDeclarado == DBNull.Value) ? 0 : (double)montoEfectivoDeclarado;

                var montoTCDeclarado = dtCortesParciales.Compute("SUM(TarjetaCredDeclaracion)", "");
                TotalTarjetaCreditoDeclaracion = 
                    (montoTCDeclarado == DBNull.Value) ? 0 : (double)montoTCDeclarado;

                var montoTDDeclarado = dtCortesParciales.Compute("SUM(TarjetaDebitoDeclaracion)", "");
                TotalTarjetaDebitoDeclaracion =
                    (montoTDDeclarado == DBNull.Value) ? 0 : (double)montoTDDeclarado;
            }

            TotalDeclarado = TotalEfectivoDeclaracion + TotalTarjetaCreditoDeclaracion + TotalTarjetaDebitoDeclaracion;

            //double.TryParse(nudTotalReal.Value.ToString(), out MontoReal);
            //double.TryParse(TotalEfectivo().ToString(), out TotalEfectivoDeclaracion);
            //double.TryParse(nudTotalDebitoTerminal.Value.ToString(), out TotalTarjetaDebitoDeclaracion);
            //double.TryParse(nudTotalCreditoTerminal.Value.ToString(), out TotalTarjetaCreditoDeclaracion);
            //double.TryParse(nudTotalVales.Value.ToString(), out TotalValesDeclaracion);
            //double.TryParse(nudTotalReal.Value.ToString(), out MontoReal);

            double totalCaja = MontoReal + Transferencia + Monedero + Vales;

            string folioCorte = FolioCorte();

            string ins = string.Format(
                " INSERT INTO PVCorteCaja (FolioCorte, FolioVentaInicial, FolioVentaFinal, \n" +
                " IdSucursal, EfectivoDeclaracion, TarjetaCredDeclaracion, TarjetaDebitoDeclaracion, \n" +
                " ValesDeclaracion, TotalCaja, Efectivo, TarjetaDebito, TarjetaCredito, \n" +
                " Transferencia, Monedero, Vales, TotalVentas, Retiros, Valido, CorteFinal, \n" +
                " IdUsuarioCorte, FechaCorte, FondoCaja, TotalDeclarado, Cheques) \n" +
                " VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},'{10}',{11},{12},{13}," +
                " {14},{15},{16},{17},'{18}',{19},'{20}',{21},{22},{23})", folioCorte, FI, FF, idSucursal,
                    TotalEfectivoDeclaracion, TotalTarjetaCreditoDeclaracion, 
                    TotalTarjetaDebitoDeclaracion, TotalValesDeclaracion, totalCaja, Efectivo, 
                    (TarjetaDebito + Vales), TarjetaCredito, Transferencia, Monedero, 0, VentaTotal, 
                    TotalRetiro, 1, esCorteFinal, idUsuarioAut, hoy, 0, TotalDeclarado, Cheques);

            if (sqlLoc.exec(ins) > 0)
            {
                lblCorteFinal.Text = "Folio corte: " + folioCorte + "";
                string queryUpdtVentas = "UPDATE PVVentas \n" +
                " SET FolioCorteCaja = '" + folioCorte + "' \n" +
                " WHERE Valido = 1 AND Terminada = 1 AND FolioCorteCaja IS NULL \n" +
                " AND FolioCorteParcialCaja IS NOT NULL \n" +
                //" AND IdUsuarioVenta = " + idUsuario + " " +
                " AND IdSucursal = " + idSucursal + " ";

                if (sqlLoc.exec(queryUpdtVentas) > 0)
                {
                    //Actualiza fondos de caja con el corte final
                    //var r1 = sqlLoc.exec(" UPDATE PVFondoCaja \n" +
                    //    " SET FolioCorteCaja = '" + folioCorte + "' \n" +
                    //    " WHERE FolioCorteParcialCaja IS NOT NULL \n" +
                    //    " AND FolioCorteCaja IS NULL \n " +
                    //    " AND IdSucursal = " + idSucursal);
                    //Actualiza retiros de caja con el corte final
                    var x1 =sqlLoc.exec(" UPDATE PVRetiroCaja SET FolioCorteCaja= '" + folioCorte + "' \n" +
                        "WHERE FolioCorteParcialCaja IS NOT NULL \n" +
                        "AND FolioCorteCaja IS NULL \n" +
                        "AND IdSucursal = " + idSucursal + " ");

                    //Actualizar ventas que no tienen folio de corte y ya están en la nube
                    //ActualizarFoliosCortes();

                    //Generar ticket de corte final
                    FImprimeCorteII icp = new FImprimeCorteII(sqlLoc, folioCorte, sql, nombre, idSucursal, sucursal, idUsuario, numCaja);

                    icp.ShowDialog();
                    MessageBox.Show("Corte final terminado!", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    this.Close();

                    
                }
            }
        }

        private void ActualizarFoliosCortes()
        {
            //Actualizar cortes en las ventas enviadas (Las cuales se enviaron sin uno)

            //Ventas, retiros y fondo de caja en la nube sin corte
            //----- Ventas -------
            DataTable dtVentasNube = sql.selec(" SELECT * FROM PVVentas WHERE IdSucursal=" + idSucursal + " AND (FolioCorteParcialCaja IS NULL OR FolioCorteParcialCaja = 'NULL') "); //FolioCorteParcialCaja IS NULL OR
            if (dtVentasNube.Rows.Count > 0)
            {
                string IdsVentas = "";
                foreach (DataRow dr in dtVentasNube.Rows)
                {
                    IdsVentas += "'" + dr["FolioVenta"].ToString() + "',";
                }

                if (IdsVentas.Length > 0)
                {
                    IdsVentas = IdsVentas.Substring(0, IdsVentas.Length - 1);

                    //---- Ventas locales ----
                    DataTable dtVentasLocales = sqlLoc.selec(" SELECT * FROM PVVentas WHERE IdSucursal=" + idSucursal + " AND FolioVenta IN (" + IdsVentas + ") AND (FolioCorteCaja IS NOT NULL AND FolioCorteCaja <> 'NULL') ");
                    if (dtVentasLocales.Rows.Count > 0)
                    {
                        string update = "";
                        foreach (DataRow drV in dtVentasNube.Rows)
                        {
                            var vBuscar = dtVentasLocales.Select("IdSucursal = " + idSucursal + " AND FolioVenta = '" + drV["FolioVenta"].ToString() + "'", "");

                            if (vBuscar.Count() > 0)
                            {
                                DataTable dtBuscar = new DataTable();
                                dtBuscar = vBuscar.CopyToDataTable();
                                if (dtBuscar.Rows.Count > 0)
                                {
                                    //Actualizar row
                                    update += "UPDATE TOP(1) PVVentas SET \n"
                                            //+ " IdCorteParcialCaja=" + dtBuscar.Rows[0]["IdCorteParcialCaja"].ToString() + ", \n"
                                            + " FolioCorteParcialCaja='" + dtBuscar.Rows[0]["FolioCorteParcialCaja"].ToString() + "', \n"
                                            //+ " IdCorteCaja=" + dtBuscar.Rows[0]["IdCorteCaja"].ToString() + ", \n"
                                            + " FolioCorteCaja='" + dtBuscar.Rows[0]["FolioCorteCaja"].ToString() + "' \n"
                                            + " WHERE IdSucursal=" + idSucursal + " AND FolioVenta = '" + drV["FolioVenta"].ToString() + "';";
                                }
                            }

                        }

                        if (update.Length > 0)
                        {
                            int i = sql.exec(update);
                        }
                    }
                }
            }

            //----- Retiros -------
            DataTable dtRetirosNube = sql.selec(" SELECT * FROM PVRetirosCaja WHERE IdSucursal=" + idSucursal + " AND (FolioCorteCaja IS NULL OR FolioCorteCaja = 'NULL') "); //FolioCorteParcialCaja IS NULL OR
            if (dtRetirosNube.Rows.Count > 0)
            {
                string IdsRetiros = "";
                foreach (DataRow dr in dtRetirosNube.Rows)
                {
                    IdsRetiros += "'" + dr["FolioRetiro"].ToString() + "',";
                }

                if (IdsRetiros.Length > 0)
                {
                    IdsRetiros = IdsRetiros.Substring(0, IdsRetiros.Length - 1);

                    //---- Ventas Detalle locales ----
                    DataTable dtRetirosLocales = sqlLoc.selec(" SELECT * FROM PVRetiroCaja WHERE IdSucursal=" + idSucursal + " AND FolioRetiro IN (" + IdsRetiros + ") AND (FolioCorteCaja IS NOT NULL AND FolioCorteCaja <> 'NULL') ");
                    if (dtRetirosLocales.Rows.Count > 0)
                    {
                        string update = "";
                        foreach (DataRow drV in dtRetirosNube.Rows)
                        {
                            var vBuscar = dtRetirosLocales.Select("IdSucursal = " + idSucursal + " AND FolioRetiro = '" + drV["FolioRetiro"].ToString() + "'", "");
                            if (vBuscar.Count() > 0)
                            {
                                DataTable dtBuscar = new DataTable();
                                dtBuscar = vBuscar.CopyToDataTable();
                                if (dtBuscar.Rows.Count > 0)
                                {
                                    //Actualizar row
                                    update += "UPDATE TOP(1) PVRetirosCaja SET \n"
                                            //  + " IdCorteParcialCaja=" + dtBuscar.Rows[0]["IdCorteParcialCaja"].ToString() + ", \n"
                                            + " FolioCorteParcialCaja='" + dtBuscar.Rows[0]["FolioCorteParcialCaja"].ToString() + "', \n"
                                            // + " IdCorteCaja=" + dtBuscar.Rows[0]["IdCorteCaja"].ToString() + ", \n"
                                            + " FolioCorteCaja='" + dtBuscar.Rows[0]["FolioCorteCaja"].ToString() + "' \n"
                                            + " WHERE IdSucursal=" + idSucursal + " AND FolioRetiro = '" + drV["FolioRetiro"].ToString() + "';";
                                }
                            }
                        }

                        if (update.Length > 0)
                        {
                            int i = sql.exec(update);
                        }
                    }
                }


            }

        }

        private string FolioCorte()
        {
            //Consultar ultimo folio
            var folioUltimoCorte = sqlLoc.scalar("SELECT TOP 1 FolioCorte " +
                "FROM PVCorteCaja WHERE IdSucursal = " + idSucursal + " " +
                "ORDER BY FechaCorte DESC");
            var consecutivo = "000";

            //Si se encuentra folio anterior tomar ultimos digitos para consecutivo
            if (folioUltimoCorte != null)
            {
                var split = folioUltimoCorte.ToString().Split('-');
                int number = Convert.ToInt32(split[1]);
                consecutivo = (++number).ToString("D3");
            }
            //Folio: C + idSucursal (3 digitos) + fecha/hora + consecutivo
            string FolioCorte = "C"+ DateTime.Now.ToString("ddMMyyHHmmss") + "-" + consecutivo;

            return FolioCorte;
        }
    }
}
