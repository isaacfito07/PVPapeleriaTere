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
    public partial class Arqueo : Form
    {
        ConSQL sql, sqlLoc;
        //ConSQLCE sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;

        //Folios (Inicial y final de venta)
        string FI = "", FF = "";

        double Efectivo = 0;
        double TarjetaDebito = 0;
        double TarjetaCredito = 0;
        double Transferencia = 0;
        double Monedero = 0;
        double Vales = 0;
        double VentaTotal = 0;
        double TotalRetiro = 0;
        double FondoCaja;
        public Arqueo(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, bool _consulta, string _numCaja)
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

        private void Arqueo_Load(object sender, EventArgs e)
        {
            CargaDatos();
        }

        public void CargaDatos()
        {
            //Toma las ventas de esa sucursal y del usuario loggeado
            string query =
                "SELECT V.FolioVenta, V.TotalVenta, PV.MontoRecibido, PV.Cambio, PV.MontoEfectivo, \n"
                + " PV.MontoTarjeta, PV.MontoTransferencia, PV.MontoMonedero, PV.MontoVales, PV.TipoTarjeta \n"
                + " FROM PVVentas V LEFT JOIN PVVentaPago PV ON V.FolioVenta = PV.FolioVenta \n"
                + " WHERE V.IdSucursal = " + idSucursal + " \n"
                + " AND V.Terminada = 1 AND  FolioCorteParcialCaja IS NULL AND CorteTerminado = 0 \n"
                + " ORDER BY V.FechaVenta";

            DataTable dtVentas = sqlLoc.selec(query);

            ////Toma el último fondo de caja ingresado de esa sucursal y usuario
            //var Fondo = sqlLoc.scalar(" SELECT TOP(1) Monto FROM PVFondoCaja \n" 
            //    + " WHERE IdSucursal='" + idSucursal + "' \n" 
            //    + " ORDER BY Fecha DESC ");

            FondoCaja = 0;
            //if (Fondo != null)
            //    double.TryParse(Fondo.ToString(), out FondoCaja);

            //Retiros 
            string queryR =
               " SELECT ISNULL(SUM(ISNULL(RC.Retiro,0)),0) Retiro FROM PVRetiroCaja RC \n"
             + " WHERE RC.IdSucursal = " + idSucursal + " \n"
             + " AND RC.FolioCorteParcialCaja IS NULL \n"
             + " AND RC.FolioCorteCaja IS NULL";

            var retiros = sqlLoc.scalar(queryR);
            if (retiros != null)
                TotalRetiro = (double)retiros;

            if (dtVentas.Rows.Count > 0)
            {
                //Folio inicial y final
                int ind = dtVentas.Rows.Count - 1;
                FI = dtVentas.Rows[0]["FolioVenta"].ToString();
                FF = dtVentas.Rows[ind]["FolioVenta"].ToString();

                //Pagado con efectivo
                var montoEfectivo = dtVentas.Compute("SUM(MontoEfectivo)", "");
                var montoCambio = dtVentas.Compute("SUM(Cambio)", "");

                var efectivo = (montoEfectivo == DBNull.Value) ? 0 : (double)montoEfectivo;
                var cambio = (montoCambio == DBNull.Value) ? 0 : (double)montoCambio;
                Efectivo = efectivo - cambio;

                //Pagado con tarjeta de débito
                var montoTarjetaDebito = dtVentas.Compute("SUM(MontoTarjeta)", "TipoTarjeta='Débito'");
                TarjetaDebito += (montoTarjetaDebito == DBNull.Value) ? 0 : (double)montoTarjetaDebito;

                //Pagado con vales
                var montoVales = dtVentas.Compute("SUM(MontoVales)", "");
                Vales += (montoVales == DBNull.Value) ? 0 : (double)montoVales;

                // Pagado con tarjeta de crédito
                var montoTarjetaCredito = dtVentas.Compute("SUM(MontoTarjeta)", "TipoTarjeta='Crédito'");
                TarjetaCredito += (montoTarjetaCredito == DBNull.Value) ? 0 : (double)montoTarjetaCredito;

                //Pagado con transferencia
                var montoTransferencia = dtVentas.Compute("SUM(MontoTransferencia)", "");
                Transferencia += (montoTransferencia == DBNull.Value) ? 0 : (double)montoTransferencia;

                nudTotalTransferencias.Value = (decimal)Transferencia;

                //Pagado con monedero
                var montoMonedero = dtVentas.Compute("SUM(MontoMonedero)", "");
                Monedero += (montoMonedero == DBNull.Value) ? 0 : (double)montoMonedero;

                nudTotalMonedero.Value = (decimal)Monedero;

                VentaTotal = (double)dtVentas.Compute("SUM(TotalVenta)", "");

                //nudTotalSalidas.Value = (decimal)TotalRetiro;

                SumaTotales();
                nud500.Select(0, nud500.Text.Length);
            }
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

        private void nud200_Enter(object sender, EventArgs e)
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

        private void nudM50C_Click(object sender, EventArgs e)
        {
            nudM50C.Select(0, nudM50C.Text.Length);
            SumaTotales();
        }

        private void nudTotalDebitoTerminal_Click(object sender, EventArgs e)
        {
            nudTotalDebitoTerminal.Select(0, nudTotalDebitoTerminal.Text.Length);
            SumaTotales();
        }

        private void nudTotalCreditoTerminal_Click(object sender, EventArgs e)
        {
            nudTotalCreditoTerminal.Select(0, nudTotalCreditoTerminal.Text.Length);
            SumaTotales();
        }

        private void nudTotalVales_Click(object sender, EventArgs e)
        {
            nudTotalSalidas.Select(0, nudTotalSalidas.Text.Length);
            SumaTotales();
        }

        private void nudTotalVales_ValueChanged(object sender, EventArgs e)
        {
            //
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
            if (nudTotalTransferencias.Value.ToString() == "")
                nudTotalTransferencias.Value = 0;
            if (nudTotalSalidas.Value.ToString() == "")
                nudTotalSalidas.Value = 0;

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
            totalReal += nudTotalTransferencias.Value;
            totalReal += nudTotalMonedero.Value;
            //totalReal = totalReal - nudTotalSalidas.Value;

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

            //Monedas
            totalEfectivo += nudM20.Value * 20;
            totalEfectivo += nudM10.Value * 10;
            totalEfectivo += nudM5.Value * 5;
            totalEfectivo += nudM2.Value * 2;
            totalEfectivo += nudM1.Value * 1;
            totalEfectivo += nudM50C.Value * (decimal)(0.5);

            return totalEfectivo;
        }

        //Guarda corte parcial

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (nudTotalCreditoTerminal.Value == 0 || nudTotalDebitoTerminal.Value == 0
                || nudTotalSalidas.Value == 0)
            {
                string mensaje = "La declaración para \n";
                if (nudTotalCreditoTerminal.Value == 0)
                    mensaje += " * Tarjeta de Credito \n";
                if (nudTotalDebitoTerminal.Value == 0)
                    mensaje += " * Tarjeta de Debito \n";
                if (nudTotalSalidas.Value == 0)
                    mensaje += " * Salidas \n";

                mensaje += "es 0 ¿Desea continuar?";

                DialogResult dr = MessageBox.Show(mensaje, "Declaración", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.No)
                {
                    goto fin;
                }
            }

            //Verificar permisos usuario
            FPassFondo passFondo = new FPassFondo();
            passFondo.ShowDialog();

            string usuarioAut = passFondo.Usuario;
            string pass = passFondo.Contrasena;

            DataTable dtUsuario =
                sqlLoc.selec
                (
                    "SELECT * FROM PVUsuarios WHERE usuario = '" + usuarioAut.Trim() + "'"
                );

            if (dtUsuario.Rows.Count > 0)
            {
                DataRow r = dtUsuario.Rows[0];

                if (pass.Trim() == r["Contrasena"].ToString().Trim())
                {
                    if (Convert.ToBoolean(r["CorteCaja"]))
                    {
                        string hoy = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //Consultar ultimo folio
                        var folioUltimoCorte = sqlLoc.scalar("SELECT TOP 1 FolioCorte " +
                            "FROM PVCorteCajaArqueo WHERE IdSucursal = " + idSucursal + " " +
                            "ORDER BY FechaCorte DESC");
                        var consecutivo = "001";
                        //Si se encuentra folioanterior tomar ultimos digitos para consecutivo
                        if (folioUltimoCorte != System.DBNull.Value)
                        {
                            if (folioUltimoCorte != null) {
                                var split = folioUltimoCorte.ToString().Split('-');
                                int number = Convert.ToInt32(split[1]);
                                consecutivo = (++number).ToString("D3");
                            }
                         
                        }
                        //Folio: CP + idSucursal (2 digitos) + fecha/hora + consecutivo
                        string FolioCorteParcial = "C" + DateTime.Now.ToString("ddMMyyHHmmss") + "-" + consecutivo;

                        double TotalEfectivoDeclaracion;
                        double TotalTarjetaDebitoDeclaracion;
                        double TotalTarjetaCreditoDeclaracion;
                        double TotalSalidasDeclaracion;
                        double MontoReal;
                        double SalidasReal;

                        double.TryParse(nudTotalReal.Value.ToString(), out MontoReal);
                        double.TryParse(TotalEfectivo().ToString(), out TotalEfectivoDeclaracion);
                        double.TryParse(nudTotalDebitoTerminal.Value.ToString(), out TotalTarjetaDebitoDeclaracion);
                        double.TryParse(nudTotalSalidas.Value.ToString(), out TotalSalidasDeclaracion);
                        double.TryParse(nudTotalCreditoTerminal.Value.ToString(), out TotalTarjetaCreditoDeclaracion);
                        double.TryParse(nudTotalReal.Value.ToString(), out MontoReal);
                        double.TryParse(nudTotalSalidas.Value.ToString(), out SalidasReal);

                        double totalCaja = MontoReal + Transferencia + Monedero + Vales;

                        string ins = string.Format(
                            " INSERT INTO PVCorteCajaArqueo (FolioCorte, FolioVentaInicial, FolioVentaFinal, \n" +
                            " IdSucursal, EfectivoDeclaracion, TarjetaCredDeclaracion, TarjetaDebitoDeclaracion, \n" +
                            " ValesDeclaracion, TotalCaja, Efectivo, TarjetaDebito, TarjetaCredito, \n" +
                            " Transferencia, Monedero, Vales, TotalVentas, Retiros, Valido, CorteFinal, \n" +
                            " IdUsuarioCorte, FechaCorte, FondoCaja, RetirosDeclaracion) \n" +
                            " VALUES ('{0}','{1}','{2}',{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}," +
                            " {14},{15},{16},{17},{18},{19},'{20}',{21},{22})", FolioCorteParcial, FI, FF, idSucursal,
                              TotalEfectivoDeclaracion, TotalTarjetaCreditoDeclaracion, TotalTarjetaDebitoDeclaracion,
                              TotalSalidasDeclaracion, totalCaja, Efectivo, (TarjetaDebito + Vales), TarjetaCredito, Transferencia,
                              Monedero, 0, VentaTotal, TotalRetiro, 1, 0, idUsuario, hoy, 0, SalidasReal);

                        if (sqlLoc.exec(ins) > 0)
                        {
                            lblFolioCorteParcial.Text = "Folio arqueo: " + FolioCorteParcial + "";

                            // Generar ticket de corte parcial
                            FImprimeArqueo fia = new FImprimeArqueo(sqlLoc, sql, FolioCorteParcial);
                            fia.ShowDialog();
                            MessageBox.Show("Arqueo terminado!", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();


                        }
                    }
                    else
                    {
                        MessageBox.Show("No tienes el permiso para corregir el fondo, solicitalo a Dirección", "Sin permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Contraseña incorrecta", "Contraseña incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No existe el usuario " + usuarioAut.Trim(), "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
            }
        fin:;
        }
    }
}

