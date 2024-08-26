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
    public partial class FRetiro : Form
    {
        ConSQL sql, sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;

        string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        //string FI = "", FF = "";
        bool permisoCorreccion = false;

        public FRetiro(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, bool _consulta, string _numCaja)
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

        private void fRetiro_Load(object sender, EventArgs e)
        {
            if (sucursal != null)
            {
                lblSitioTitulo.Text = sucursal;
            }
            else
            {
                var s = sqlLoc.scalar("SELECT Nombre FROM PVSucursales S WHERE Id = " + idSucursal);
                if (s != null)
                    lblSitioTitulo.Text = s.ToString();
            }

            sql.llenaCombo(cbxProveedor, sqlLoc.selec(" SELECT Id, Nombre FROM PVProveedores WHERE Activo=1 ORDER BY Nombre  "), "Id", "Nombre");

            lblFecha.Text = DateTime.Now.ToString("dd-MM-yyyy");
            lblHora.Text = DateTime.Now.ToString("HH:mm");
            lblCajera.Text = nombre;

            string query = "SELECT * FROM PVVentas V JOIN PVVentaPago VP ON V.FolioVenta = VP.FolioVenta \n" +
                " WHERE V.IdSucursal = " + idSucursal + " AND V.IdUsuarioVenta = " + idUsuario + " \n" +
                " AND FolioCorteParcialCaja IS NULL AND FolioCorteCaja IS NULL AND Terminada = 1 ";            

            DataTable dtEstadoCaja = sqlLoc.selec(query);

            /*var fondo = sqlLoc.scalar(" SELECT Monto FROM PVFondoCaja " +
                "WHERE IdSucursal=" + idSucursal + " AND IdUsuario=" + idUsuario + " ORDER BY Id DESC ");

            lblFondoCaja.Text = Convert.ToDouble(0).ToString("C2");
            if (fondo != null)
            {
                lblFondoCaja.Text = Convert.ToDouble(fondo.ToString()).ToString("C2");
            }*/

            //hay ventas?
            double efectivo = 0, tarjeta = 0, transferencia = 0, vales = 0 , monedero = 0;
            double ventaTotal = 0, totalRetiro = 0;
            if (dtEstadoCaja.Rows.Count > 0)
            {
                //FOLIO INICIAL Y FINAL
                int ind = dtEstadoCaja.Rows.Count - 1;

                //Retiros 
                string queryR =
                    " SELECT SUM(RC.Retiro) Retiro FROM PVRetiroCaja RC " +
                    " WHERE RC.IdSucursal = " + idSucursal + " AND IdUsuario=" + idUsuario + 
                    " AND FolioCorteParcialCaja IS NULL  ";

                DataTable dtRetiros = sqlLoc.selec(queryR);
                if (dtRetiros.Rows.Count > 0)
                {
                    totalRetiro = (dtRetiros.Rows[0]["Retiro"] == DBNull.Value) 
                        ? 0 
                        : (double)dtRetiros.Rows[0]["Retiro"];
                }

                //PAGADO CON EFECTIVO
                var montoEfectivo = dtEstadoCaja.Compute("SUM(MontoEfectivo)", "");
                var montoCambio = dtEstadoCaja.Compute("SUM(Cambio)", "");
                var cambio = (montoCambio == DBNull.Value) ? 0 : (double)montoCambio;
                efectivo += (montoEfectivo == DBNull.Value) ? 0 : (double)montoEfectivo - cambio;
                efectivo = efectivo - totalRetiro;
                lblEfectivo.Text = efectivo.ToString("C2");

                //PAGADO CON TARJETA CRÉDITO
                var montoTarjetaCredito = dtEstadoCaja.Compute("SUM(MontoTarjeta)", "TipoTarjeta = 'Crédito'");
                tarjeta += (montoTarjetaCredito == DBNull.Value) ? 0 : (double)montoTarjetaCredito;
                lblTarjetaCred.Text = tarjeta.ToString("C2");

                //PAGADO CON TARJETA DÉBITO
                var montoTarjetaDebito = dtEstadoCaja.Compute("SUM(MontoTarjeta)", "TipoTarjeta = 'Débito'");
                tarjeta += (montoTarjetaDebito == DBNull.Value) ? 0 : (double)montoTarjetaDebito;
                lblTarjetaDeb.Text = tarjeta.ToString("C2");

                //PAGADO CON TRANSFERENCIA
                var montoTransferencia = dtEstadoCaja.Compute("SUM(MontoTransferencia)", "");
                transferencia += (montoTransferencia == DBNull.Value) ? 0 : (double)montoTransferencia;
                lblTransferencia.Text = tarjeta.ToString("C2");

                //PAGADO CON MONEDERO
                var montoMonedero = dtEstadoCaja.Compute("SUM(MontoMonedero)", "");
                monedero += (montoMonedero == DBNull.Value) ? 0 : (double)montoMonedero;
                lblMonedero.Text = monedero.ToString("C2");

                //PAGADO CON VALES
                var montoVales = dtEstadoCaja.Compute("SUM(montoVales)", "");
                vales += (montoVales == DBNull.Value) ? 0 : (double)montoVales;
                lblVales.Text = vales.ToString("C2");

                //Venta total
                //ventaTotal = efectivo + tarjeta + transferencia + monedero + vales;
                //lblVentaTotal.Text = ventaTotal.ToString("C1");
                lblVentaTotal.Text = Convert.ToDouble(dtEstadoCaja.Compute("SUM(TotalVenta)", "")).ToString("C2");

                //el maximo de retiro es lo que hay en caja
                //nudRetiro.Maximum = (decimal)efectivo;
            }

            //tiene permiso para corregir?
            permisoCorreccion = (bool)sqlLoc.scalar("SELECT ISNULL(RetiroCaja, 0) FROM PVUsuarios " +
                "WHERE Id = " + idUsuario + "");
            if (permisoCorreccion)
            {
                nudRetiro.Enabled = true;
                nudRetiro.ReadOnly = false;
                nudRetiro.Select(0, nudRetiro.Text.Length);
            }
            else
            {
                nudRetiro.Enabled = false;
                nudRetiro.ReadOnly = true;
                MessageBox.Show("No estás autorizado para realizar retiros", "Espera", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                this.Close();
            }
        }

        private void nudRetiro_ValueChanged(object sender, EventArgs e)
        {
            double max = 0;
            if(double.TryParse(lblEfectivo.Text.Replace("$", "").Trim(), out max)){ }

            if((double)nudRetiro.Value > max)
            {
                MessageBox.Show("No se puede retirar mas del efectivo disponible en caja.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button2);
                nudRetiro.Value = 0;
            }
        }

        //Guarda retiro
        private void btnAceptar_Click(object sender, EventArgs e)
        {

            int IdProveedor = 0;
            int.TryParse(cbxProveedor.SelectedValue.ToString(), out IdProveedor);

            //Consultar ultimo folio
            var folioUltimoCorte = sqlLoc.scalar("SELECT TOP 1 FolioCorteCaja " +
                "FROM PVRetiroCaja WHERE IdSucursal = " + idSucursal + " " +
                "ORDER BY FechaRetiro DESC");
            var consecutivo = "001";
            //Si se encuentra folioanterior tomar ultimos digitos para consecutivo
            if (folioUltimoCorte != null)
            {
                if (folioUltimoCorte != System.DBNull.Value)
                {
                    var split = folioUltimoCorte.ToString().Split('-');
                    int number = Convert.ToInt32(split[1]);
                    consecutivo = (++number).ToString("D3");
                }
            }
            string inicial = lblSitioTitulo.Text.Substring(0, 1);
            //Folio: inicialSucursal + hora + consecutivo
            string FolioRetiro = inicial + DateTime.Now.ToString("ddMMyyHHmmss") + "-" + consecutivo;


            //Verifica que cantidad a retirar sea igual o menor al efectivo en caja
            double efe = 0, retiro = 0;
            if (double.TryParse(lblEfectivo.Text.Replace("$", "").Trim(), out efe) && double.TryParse(nudRetiro.Value.ToString(), out retiro))
            {
                if (retiro > 0)
                {
                    if (retiro <= efe)
                    {

                        string ins = " INSERT INTO PVRetiroCaja \n"
                                  + " (FolioRetiro, IdUsuario, IdSucursal, FechaRetiro, Retiro, Concepto, IdProveedor) \n"
                                  + " VALUES ('" + FolioRetiro + "', " + idUsuario + ", " + idSucursal
                                  + ", '" + fechaHora + "', " + retiro + ", '" + txtConcepto.Text + "', "+ IdProveedor +");";

                        if (sqlLoc.exec(ins) > 0)
                        {
                            DateTime fechaRetiro = (DateTime)sqlLoc.scalar("SELECT FechaRetiro FROM PVRetiroCaja " +
                                "WHERE FolioRetiro = '" + FolioRetiro + "'");
                            lblFolio.Text = "Folio:" + FolioRetiro;
                            //sqlLoc.exec("UPDATE PVVentas SET IdCorteCaja = " + idCorte + " WHERE IdCorteCaja IS NULL AND Terminado = 1 ");

                            //Venta Total
                            decimal ventaTotal = 0;
                            if (decimal.TryParse(lblVentaTotal.Text.Replace("$", "").Trim(), out ventaTotal)) { }

                            //Efectivo
                            decimal efectivo = 0;
                            if (decimal.TryParse(lblEfectivo.Text.Replace("$", "").Trim(), out efectivo)) { }

                            //Tarjeta Crédito
                            decimal tarjetaCred = 0;
                            if (decimal.TryParse(lblTarjetaCred.Text.Replace("$", "").Trim(), out tarjetaCred)) { }

                            //Tarjeta Débito
                            decimal tarjetaDeb = 0;
                            if (decimal.TryParse(lblTarjetaDeb.Text.Replace("$", "").Trim(), out tarjetaDeb)) { }

                            //Transferencia
                            decimal transferencia = 0;
                            if (decimal.TryParse(lblTransferencia.Text.Replace("$", "").Trim(), out transferencia)) { }

                            //Monedero
                            decimal monedero = 0;
                            if (decimal.TryParse(lblMonedero.Text.Replace("$", "").Trim(), out monedero)) { }

                            //Vales
                            decimal vales = 0;
                            if (decimal.TryParse(lblVales.Text.Replace("$", "").Trim(), out vales)) { }

                            ////Fondo Caja
                            //decimal fondoCaja = 0;
                            //if (decimal.TryParse(lblFondoCaja.Text.Replace("$", "").Trim(), out fondoCaja)) { }

                            string fecha = Convert.ToDateTime(fechaRetiro).ToString("yyyy-MM-dd");
                            string hora = Convert.ToDateTime(fechaRetiro).ToString("HH:mm");

                            string NombreProveedor = cbxProveedor.SelectedItem.ToString();
                            

                            FImprimeRetiro ir = new FImprimeRetiro(sqlLoc, numCaja, nombre,
                                FolioRetiro, lblSitioTitulo.Text, fecha, hora, ventaTotal, efectivo,
                                tarjetaCred, tarjetaDeb, transferencia, monedero, vales, 0, (decimal)retiro, txtConcepto.Text, NombreProveedor);
                            ir.ShowDialog();
                            //FTicketRetiro tr = new FTicketRetiro(sqlLoc, FolioRetiro);
                            //tr.ShowDialog();
                            MessageBox.Show("Retiro exitoso!", "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se puede retirar mas del efectivo disponible en caja.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button2);
                    }
                }
            }
        }
    }
}
