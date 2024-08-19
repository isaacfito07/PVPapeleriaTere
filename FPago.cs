using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PVLaJoya
{
    public partial class FPago : Form
    {
        ConSQL sqlLoc, sql;

        double TotalVenta = 0, TotalOriginalSinDescuento = 0, monederoDisponible = 0, VentaSinDescuento = 0;
        public double PorcentajeDescuento = 0, Descuento = 0, TotalVentaOriginal = 0;
        public bool PagoCancelado = false;

        double MontoTotalRecibido = 0;
        double MontoCubierto = 0;

        double MontoEfectivoRecibido = 0;
        double MontoTarjetaRecibido = 0;
        double MontoTransferenciaRecibido = 0;
        double MontoMonederoRecibido = 0;
        double MontoValesRecibido = 0;

        decimal Cambio = 0, Monedero = 0;

        string autorizacion, numerotarjeta, respuesta, folioTran;
        bool GlobalClienteConCredito = false;

        private bool LoginSupervisor = false;

        bool EsAbono = false;
        bool NoCambiarDescuento = true;

        //Facturación
        bool reqFactura = false;
        string rfcCliente;

        //Usuario y sucursal
        string idUsuario = "", idSucursal = "";
        int idCliente = 0;

        public FPago(ConSQL _sqlLoc, ConSQL _sql, string _totalVenta, double _iva, double _ieps, string _idUsuario, string _idSucursal, bool ClienteConCredito, string NombreCliente, bool esAbono = false, int  _idCliente = 0)
        {
            InitializeComponent();
            //se inicia la conexion
            sqlLoc = _sqlLoc;
            sql = _sql;

            idUsuario = _idUsuario;

            idSucursal = _idSucursal;

            EsAbono = esAbono;

            //se pone el total de la venta en el label

            lblTotal.Text = (double.Parse(_totalVenta.Replace("Total:", ""), NumberStyles.Currency, null)).ToString("C2");
            lblImpuestos.Text = (_iva + _ieps).ToString("C2");
            lblTotalFinal.Text = (double.Parse(_totalVenta.Replace("Total:", ""), NumberStyles.Currency, null) + _iva + _ieps).ToString("C2");
            TotalVenta = Math.Round(
                (double.Parse(_totalVenta.Replace("Total:", ""), NumberStyles.Currency, null) + _iva + _ieps
                ), 2);
            TotalOriginalSinDescuento = TotalVenta;

            //Valida que el cliente tenga la posibilidad de credito
            btnCredito.Enabled = ClienteConCredito;
            pnlCredito.Enabled = ClienteConCredito;
            nudCredito.Enabled = ClienteConCredito;
            GlobalClienteConCredito = ClienteConCredito;

            //Cargar credito
            if (!NombreCliente.Equals(string.Empty))
            {
                lblCliente.Text += " " + NombreCliente;
            }
            else
            {
                lblCliente.Text = string.Empty;
            }
            

            //Area para Abono
            if (esAbono)
            {
                lbImporte.Visible = false;
                lblTotal.Visible = false;
                labelIva.Visible = false;
                lblImpuestos.Visible = false;
                lb.Visible = false;
                txtDescuento.Visible = false;
                lbPercent.Visible = false;
                //lblCambio.Visible = false;
                lblR.Visible = false;
                lblRestante.Visible = false;

                lbTotalPagar.Text = "Pago sugerido:";
                lblTitBalance.Text = "Total:";
            }
            //Area para descuento
            if (_idCliente > 0)
            {
                txtDescuento.Text = (DescuentoDelCliente(_idCliente));
                txtDescuento.Enabled = false;
                NoCambiarDescuento = false;
            }
        }

        private void fPago_Load(object sender, EventArgs e)
        {
            lblRestante.Text = TotalVenta.ToString("C2");
            lblCambio.Text = "CAMBIO: " + 0.ToString("C2");

            //Pago efectivo por default
            //PagoEfectivo();
            btnEfectivo.Focus();
            nudEfectivo.Select(0, nudEfectivo.Text.Length);

            nudCredito.Enabled = false;
            VentaTotal = Convert.ToDouble((lblTotalFinal.Text).Replace("$", string.Empty));
            VentaSinDescuento = VentaTotal;
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            nudEfectivo.Value = 0;
            nudEfectivo.Enabled = true;
            nudEfectivo.ReadOnly = false;
            pnlEfectivo.Enabled = true;

            nudTarjeta.Value = 0;
            nudTarjeta.Enabled = true;
            nudTarjeta.ReadOnly = false;
            pnlTarjeta.Enabled = true;

            nudTarjetaCredito.Value = 0;
            nudTarjetaCredito.Enabled = true;
            nudTarjetaCredito.ReadOnly = false;
            pnlTarjetaCredito.Enabled = true;

            nudTran.Value = 0;
            nudTran.Enabled = true;
            nudTran.ReadOnly = false;
            pnlTran.Enabled = true;
            txtFolioComprobante.Enabled = true;
            lblMonedero.Text = "Monedero: -";
            //lblCliente.Text = "Cliente:";
            idCliente = 0;


            nudCheques.Value = 0;
            nudCheques.Enabled = true;
            nudCheques.ReadOnly = false;
            pnlCheques.Enabled = true;

            btnEfectivo.Enabled = true;
            btnTarjeta.Enabled = true;
            btnTarjetaCredito.Enabled = true;
            btnTran.Enabled = true;
            btnCheques.Enabled = true;
            if (NoCambiarDescuento)
            {
                txtDescuento.Enabled = true;
            }

            btnEfectivo.Select();
            btnEfectivo.Focus();
            MontoEfectivoRecibido = (double)nudEfectivo.Value;
            MontoCubierto = 0;
            MontoTotalRecibido = 0;

            if (GlobalClienteConCredito)
            {
                nudCredito.Value = 0;
                pnlCredito.Enabled = true;
                btnCredito.Enabled = true;
            }

            btnQuitarDescuento_Click(sender, e);
            lblTotalFinal.Text = (VentaSinDescuento).ToString("C2");
            lblRestante.Text = lblTotalFinal.Text;

            Balance();
        }

        private double Balance()
        {

            double PorcentajeDescuento = 0;
            double.TryParse(lbMontoDescuento.Text, out PorcentajeDescuento);

            double TotalDescuento = 0;
            if (PorcentajeDescuento > 0) {
                TotalDescuento = (TotalVenta * PorcentajeDescuento);
            }

            //Monto total recibido tomando en cuenta el cambio
            MontoTotalRecibido = (double)MontoNoEfectivo() + (double)nudEfectivo.Value + TotalDescuento;

            if (MontoTotalRecibido >= TotalVenta)
            {
                //Si el monto recibido es mayor o igual a lo que se debe, deshabilitar inputs
                nudEfectivo.Enabled = false;
                nudTarjeta.Enabled = false;
                nudTarjetaCredito.Enabled = false;
                nudTran.Enabled = false;
                nudCheques.Enabled = false;
                nudCredito.Enabled = false;
                btnEfectivo.Enabled = false;
                btnTarjeta.Enabled = false;
                btnTarjetaCredito.Enabled = false;
                btnTran.Enabled = false;
                btnCredito.Enabled = false;
                btnCheques.Enabled = false;
                txtDescuento.Enabled = false;
                //txtFolioComprobante.Enabled = false;

                if ((double)nudEfectivo.Value > 0)
                    MontoEfectivoRecibido = (double)nudEfectivo.Value - (double)Cambio;

                MontoCubierto = TotalVenta;
                btnPagar.Select();
            }
            else
            {
                if ((double)nudEfectivo.Value > 0)
                    MontoEfectivoRecibido = (double)nudEfectivo.Value - (double)Cambio;

                MontoCubierto = MontoEfectivoRecibido + (double)MontoNoEfectivo() + TotalDescuento;
            }

            lblBalance.Text = MontoCubierto.ToString("C2");
            lblRestante.Text = (TotalVenta - MontoCubierto).ToString("C2");
            CalcularCambio();
            return MontoCubierto;
        }

        //Pago con efectivo
        private void btnEfectivo_Click(object sender, EventArgs e)
        {
            PagoEfectivo();
        }

        private void PagoEfectivo()
        {
            //Si se tiene el pago comenzado con otros metodos de pago cubrir el resto con efectivo
            if (MontoCubierto < TotalVenta)
            {
                nudEfectivo.Value += (decimal)TotalVenta - (decimal)MontoCubierto;
                HabilitarCredito(false);
            }

            nudEfectivo.Select(0, nudEfectivo.Text.Length);
        }

        private void PagoTarjeta(Boolean esTDC = false)
        {
            //Si se tiene el pago comenzado con otros metodos de pago cubrir el resto con efectivo
            if (MontoCubierto < TotalVenta)
            {
                if (esTDC)
                {
                    nudTarjetaCredito.Value += (decimal)TotalVenta - (decimal)MontoCubierto;
                }
                else
                {
                    nudTarjeta.Value += (decimal)TotalVenta - (decimal)MontoCubierto;
                }
                HabilitarCredito(false);
            }

            //nudTarjeta.Select(0, nudTarjeta.Text.Length);
        }

        private void PagoTransferencia()
        {
            //Si se tiene el pago comenzado con otros metodos de pago cubrir el resto
            if (MontoCubierto < TotalVenta)
            {
                nudTran.Value += (decimal)TotalVenta - (decimal)MontoCubierto;
                HabilitarCredito(false);
            }

            nudTran.Select(0, nudTran.Text.Length);
        }

        private void PagoCheques()
        {
            //Si se tiene el pago comenzado con otros metodos de pago cubrir el resto
            if (MontoCubierto < TotalVenta)
            {
                nudCheques.Value += (decimal)TotalVenta - (decimal)MontoCubierto;
                HabilitarCredito(false);
            }

            nudCheques.Select(0, nudCheques.Text.Length);
        }

        /*private void PagoMonedero()
        {
            //Si se tiene el pago comenzado con otros metodos de pago cubrir el resto
            if (MontoCubierto < TotalVenta)
            {
                nudMonedero.Value += (decimal)TotalVenta - (decimal)MontoCubierto;
            }

            nudMonedero.Select(0, nudMonedero.Text.Length);
        }*/

        private decimal MontoNoEfectivo()
        {
            decimal monto = (decimal)nudTarjeta.Value + (decimal)nudTarjetaCredito.Value + (decimal)nudTran.Value
                + (decimal)nudCheques.Value + (decimal)nudCredito.Value;

            return monto;
        }

        private void CalcularCambio()
        {
            //&& (decimal)nudEfectivo.Value > (decimal)TotalVenta)
            if ((decimal)nudEfectivo.Value > 0)
                Cambio = (MontoNoEfectivo() + (decimal)nudEfectivo.Value) - (decimal)TotalVenta;

            if ((decimal)nudEfectivo.Value <= 0 || Cambio < 0)
                Cambio = 0;

            lblCambio.Text = "CAMBIO: " + Cambio.ToString("C2");
        }

        private void nudEfectivo_ValueChanged(object sender, EventArgs e)
        {
            HabilitarCredito(false);
            Balance();
        }

        private void nudTarjeta_ValueChanged(object sender, EventArgs e)
        {
            HabilitarCredito(false);
            Balance();
        }

        private void nudTran_ValueChanged(object sender, EventArgs e)
        {
            HabilitarCredito(false);
            Balance();
        }

        private void nudVales_ValueChanged(object sender, EventArgs e)
        {
            HabilitarCredito(false);
            Balance();
        }

        private void nudEfectivo_Click(object sender, EventArgs e)
        {
            nudEfectivo.Select(0, nudEfectivo.Text.Length);
        }

        private void nudEfectivo_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void btnEfectivo_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        //Pago con tarjeta
        private void btnTarjeta_Click(object sender, EventArgs e)
        {
            PagoTarjeta();
        }

        private void nudTarjeta_Click(object sender, EventArgs e)
        {
            nudTarjeta.Select(0, nudTarjeta.Text.Length);
        }

        private void btnTarjeta_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }
        private void nudTarjeta_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void btnOKTarjeta_Click(object sender, EventArgs e)
        {
            //si se cubrió con tarjeta se termina
            if (nudTarjeta.Value == 0)
                goto fin;

            //leer tarjeta
            //si no se hace el pago ir a fin:
            DialogResult dr = MessageBox.Show("Se aprobó el pago en la terminal?", "Aprobación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.No)
            {
                //btnEfectivo.Enabled = true;
                //btnOKEfectivo.Enabled = true;
                ////nudEfectivo.Enabled = true;
                //nudRecibido.Enabled = true;

                //nudEfectivo.ReadOnly = false;
                //nudEfectivo.Increment = 1;
                goto fin;
            }

            //::::::::: FALTA INFORMACION DE LA TERMINAL

            if (Balance() == TotalVenta)
                this.Close();

            fin:;

        }


        //Descuento con gafete (Minas) -- Pendiente
        private void btnGafete_Click(object sender, EventArgs e)
        {
            //pnlEfectivo.Visible = false;
            //esEfectivo = false;

            // pnlTarjeta.Visible = false;
            //esTarjeta = false;
            //nudTarjeta.Value = 0;
        }

        private void txtGafete_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void btnOKGafete_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        //Al cerrar la ventana
        private void fPago_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Balance();
            if ((MontoCubierto < TotalVenta && EsAbono == false) || Recibido == 0)//  || ((decimal)nudTarjeta.Value) > 0&& esTarjeta == false)
            {
                DialogResult dr = MessageBox.Show(" No se ha completado el pago, salir? ", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    //esEfectivo = false;
                    //esTarjeta = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }

            if (EsAbono && Recibido != 0.0)
            {
                DialogResult dr = MessageBox.Show(" Se va a efectuar un pago de "+(Recibido.ToString("C2") +"\n \n ¿Continuar?"), "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnTran_Click(object sender, EventArgs e)
        {
            PagoTransferencia();
        }


        /*private bool ClienteMonedero()
        {
            if (idCliente == 0)
            {
                FClienteVenta fCliente = new FClienteVenta(sqlLoc, sql);
                fCliente.ShowDialog();
                idCliente = fCliente.IdCliente;

                if (idCliente > 0)
                {
                    lblCliente.Text = "Cliente: " + fCliente.NombreCliente;
                    //Consultar Monedero
                    //var monedero = sqlLoc.scalar("SELECT ROUND(SUM(ISNULL(D.MontoDevolucion,0)) - SUM(ISNULL(M.Monto,0)),2) Total \n" +
                    //    "FROM PVDevoluciones D \n" +
                    //    "LEFT JOIN PVMonederoCliente M ON M.IdCliente = D.IdCliente \n" +
                    //    "WHERE Monedero = 1 AND D.IdCliente = " + idCliente + " GROUP BY D.IdCliente");

                    //var monedero = sqlLoc.scalar("SELECT ROUND(SUM(ISNULL(D.MontoDevolucion,0)) - SUM(ISNULL(M.Monto,0)),2) Total \n"
                    //                            + " FROM (SELECT * FROM PVDevoluciones  WHERE Activo=1) D \n"
                    //                            + " LEFT JOIN PVMonederoCliente M ON M.IdCliente = D.IdCliente \n"
                    //                            + " WHERE Monedero = 1 AND D.IdCliente = " + idCliente + " AND M.Valido=1 GROUP BY D.IdCliente");


                    //var monedero = sqlLoc.scalar(" SELECT SUM(ISNULL(M.Monto,0)) + SUM(ISNULL(D.MontoDevolucion,0)) Total FROM PVMonederoCliente M\n"
                    //                            + " LEFT JOIN PVDevoluciones D ON M.IdCliente = D.IdCliente\n"
                    //                            + " WHERE D.Monedero=1 AND M.Valido=1 AND M.IdCliente=" + idCliente + " ");

                    //,\n"
                   // +"     SUM(ISNULL(M.Monto,0)) MM, SUM(ISNULL(D.MontoDevolucion,0)) MD\n"
                    //var monedero = sqlLoc.scalar(" SELECT \n"
                    //                            + "     SUM(ISNULL(M.Monto,0)) + SUM(ISNULL(D.MontoDevolucion,0)) Total\n"
                    //                            + " FROM PVMonederoCliente M\n"
                    //                            + " LEFT JOIN (SELECT IdCliente, SUM(MontoDevolucion) MontoDevolucion FROM PVDevoluciones WHERE Monedero=1 AND Activo=1 GROUP BY IdCliente) D ON M.IdCliente = D.IdCliente\n"
                    //                            + " WHERE M.Valido=1 AND M.IdCliente=" + idCliente + " ");

                    var monedero = sqlLoc.scalar(" SELECT MC.MontoMonedero FROM PVClientes C\n"
                                                + " LEFT JOIN (\n"
                                                + "     SELECT \n"
                                                + "     R.IdCliente, SUM(ISNULL(R.Monto,0)) MontoMonedero \n"
                                                + "     FROM\n"
                                                + "     (\n"
                                                + "         SELECT\n"
                                                + "             M.IdCliente, SUM(ISNULL(M.Monto,0)) Monto\n"
                                                + "         FROM PVMonederoCliente M\n"
                                                + "         WHERE M.Valido=1\n"
                                                + "         GROUP BY M.IdCliente\n"
                                                + "         UNION ALL\n"
                                                + "         SELECT \n"
                                                + "         IdCliente, SUM(MontoDevolucion) MontoDevolucion \n"
                                                + "         FROM PVDevoluciones \n"
                                                + "         WHERE Monedero=1 AND Activo=1 GROUP BY IdCliente\n"
                                                + "     ) R GROUP BY  R.IdCliente\n"
                                                + " ) MC ON C.Id = MC.IdCliente WHERE C.Id="+ IdCliente +" ");


                    if (monedero != null)
                    {
                        //REVISAR ERROR, SI NO HAY DESCUENTO MARCA ERROR EN LA CONVERSION A DOUBLE
                        lblMonedero.Text = "Monedero: " + monedero.ToString();
                        if(monedero.ToString() != "")
                            monederoDisponible = (double)monedero > 0 ? (double)monedero : 0;
                    }
                    return true;
                }
                return false;
            }
            return true;
        }*/

        private void btnCheques_Click(object sender, EventArgs e)
        {
            PagoCheques();
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            //Verificar que se cubrio el pago total
            if (MontoCubierto != TotalVenta && EsAbono == false)
            {
                DialogResult dr = MessageBox.Show("No se puede realizar el pago sin cubrir el monto total.",
                    "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                goto fin;
            }

            //Si hay pago con tarjeta
            if (nudTarjeta.Value > 0 || nudTarjetaCredito.Value > 0)
            {
                DialogResult dr = MessageBox.Show("¿Se aprobó el pago en la terminal?", "Aprobación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.No)
                    goto fin;
            }

            if (nudTran.Value > 0 && string.IsNullOrEmpty(txtFolioComprobante.Text))
            {
                DialogResult dr = MessageBox.Show("Ingresa el folio de la transferencia antes de continuar",
                    "Aprobación", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                txtFolioComprobante.Select();
                goto fin;
            }

            if (nudCheques.Value > 0 && string.IsNullOrEmpty(txtFolioCheque.Text))
            {
                DialogResult dr = MessageBox.Show("Ingresa el folio del cheque antes de continuar",
                    "Aprobación", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                txtFolioCheque.Select();
                goto fin;
            }

            //Si el pago con monedeo es mayor a lo que se tiene disponible
            /*if (nudCredito.Value > (decimal)monederoDisponible)
            {
                DialogResult dr = MessageBox.Show("No se puede pagar con monedero de cliente.",
                    "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                goto fin;
            }*/

            if (MontoCubierto == TotalVenta || EsAbono) {
                this.Close();
                //FVenta fVenta = new FVenta(sql, sqlLoc, nombre, idSucursal, sucursal,
                //    idUsuario, dtProductos, imgLstCategorias, imgLstProductos,
                //    "0", false, numCaja);
                //fVenta.Show();

            }



        fin:;
        }

        private void AtajosTeclado(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    PagoEfectivo();
                    btnEfectivo.Select();
                    btnEfectivo.Focus();
                    break;
                case Keys.F2:
                    btnTarjeta_Click(btnTarjeta, new EventArgs());
                    btnTarjeta.Select();
                    btnTarjeta.Focus();
                    break;
                case Keys.F3:
                    btnTarjetaCredito_Click(btnTarjetaCredito, new EventArgs());
                    btnTarjetaCredito.Select();
                    btnTarjetaCredito.Focus();
                    break;
                case Keys.F4:
                    btnTran_Click(btnTran, new EventArgs());
                    btnTran.Select();
                    btnTran.Focus();
                    break;
                case Keys.F5:
                    btnCheques_Click(btnCheques, new EventArgs());
                    btnCheques.Select();
                    btnCheques.Focus();
                    break;
                case Keys.F6:
                    if (GlobalClienteConCredito)
                    {
                        btnCredito_Click(btnCheques, new EventArgs());
                    }
                    break;
                case Keys.F10:
                    btnCancelarPago_Click(btnCancelarPago, new EventArgs());
                    break;
                case Keys.F11:
                    btnLimpiar_Click(btnLimpiar, new EventArgs());
                    break;
                case Keys.F12:
                    btnPagar_Click(btnPagar, new EventArgs());
                    break;
            }
        }

        public string TipoTarjeta
        {
            get
            {
                if (nudTarjeta.Value != 0 || nudTarjetaCredito.Value != 0)
                {
                    if (nudTarjetaCredito.Value != 0)
                    {
                        return "Credito";
                    }
                    else
                    {
                        return "Debito";
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        //Factura
        public bool Factura
        {
            get
            {
                reqFactura = false;
                if (cbRequiereFactura.Checked)
                    reqFactura = true;

                return reqFactura;
            }
        }

        public string RFCFactura
        {
            get
            {
                return rfcCliente;
            }
        }

        //Returns
        public int IdCliente
        {
            get
            {
                return idCliente;
            }
        }

        public double VentaTotal
        {
            get
            {
                return TotalVenta;
            }
            set
            {
                TotalVenta = value;
            }
        }

        public double Recibido
        {
            get
            {
               
                return MontoTotalRecibido;
            }
        }

        //Efectivo
        public double MontoEfectivo
        {
            get
            {
                return (double)MontoEfectivoRecibido;
            }
        }

        public decimal MontoCambio
        {
            get
            {
                return Cambio;
            }
        }

        //Tarjeta
        public double MontoTarjeta
        {
            get
            {
                return (double)nudTarjeta.Value;
            }
        }

        public double MontoTarjetaCredito
        {
            get
            {
                return (double)nudTarjetaCredito.Value;
            }
        }

        //MontoDescuento
        public double MontoDescuento
        {
            get
            {
                double PorcentajeDescuento = 0;
                double.TryParse(lbMontoDescuento.Text, out PorcentajeDescuento);

                double TotalDescuento = 0;
                if (PorcentajeDescuento > 0)
                {
                    TotalDescuento = (TotalVenta * PorcentajeDescuento);
                }

               
                return TotalDescuento;
            }
        }


        private void lblTitBalance_Click(object sender, EventArgs e)
        {

        }

        private void lblBalance_Click(object sender, EventArgs e)
        {

        }

        private void lblRestante_Click(object sender, EventArgs e)
        {

        }

        private void lblR_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnDescuento_Click(object sender, EventArgs e)
        {
            
                FClienteVenta fCliente = new FClienteVenta(sqlLoc, sql);
                fCliente.ShowDialog();
                idCliente = fCliente.IdCliente;

                if (idCliente > 0)
                {
                    lblCliente.Text = "Cliente: " + fCliente.NombreCliente;
                    lblDescuento.Text = "Descuento: " + (fCliente.PorcentajeDescuento * 10).ToString() + "%";
                    lbMontoDescuento.Text = (fCliente.PorcentajeDescuento).ToString();

                    if (fCliente.PorcentajeDescuento > 0)
                    {
                        Balance();
                    }
                }
          
            
        }

        private void btnQuitarDescuento_Click(object sender, EventArgs e)
        {
            lblDescuento.Text = "Descuento: -";
            lbMontoDescuento.Text = "0";
            
            Balance();
        }

        private void btnCancelarPago_Click(object sender, EventArgs e)
        {
            //    nudEfectivo.Value = 0;
            //    nudTarjeta.Value = 0;
            //    nudTran.Value = 0;
            //    nudMonedero.Value = 0;
            //    nudVales.Value = 0;
            btnLimpiar_Click(sender, e);
            btnQuitarDescuento_Click(sender, e);
            Balance();
            PagoCancelado = true;
            this.Close();

        }

        private void btnTarjetaCredito_Click(object sender, EventArgs e)
        {
            PagoTarjeta(true);
        }

        private void nudTarjetaCredito_ValueChanged(object sender, EventArgs e)
        {
            HabilitarCredito(false);
            Balance();
        }

        private void btnCredito_Click(object sender, EventArgs e)
        {
            nudCredito.Value = (decimal)TotalVenta;
        }

        private void HabilitarCredito(bool Habilitar)
        {
            btnCredito.Enabled = Habilitar;
            pnlCredito.Enabled = Habilitar;
            nudCredito.Enabled = Habilitar;
        }

        private void nudCredito_ValueChanged(object sender, EventArgs e)
        {
            Balance();
        }

        private void txtDescuento_MouseClick(object sender, MouseEventArgs e)
        {
            if (!LoginSupervisor)
            {
                FPassFondo passFondo = new FPassFondo();
                passFondo.ShowDialog();

                string usuario = passFondo.Usuario;
                string pass = passFondo.Contrasena;

                string queryUsuarioAdmin = "SELECT TOP 1 Id, Usuario, Contrasena FROM PVUsuarios WHERE PuntoVenta = 1 \n" +
                    "AND CancelarVenta = 1 AND CorteCaja = 1 AND EstadoCaja = 1 AND CorreccionesCaja = 1 \n" +
                    "AND RetiroCaja = 1 AND Descuentos = 1 AND Devoluciones = 1 AND Arqueo = 1 AND Usuario LIKE '"+usuario+"' ORDER BY id DESC";
                
                DataTable dtUserAdmin = sqlLoc.selec(queryUsuarioAdmin);
                string dtId = string.Empty;
                string dtUser = string.Empty;
                string dtPass = string.Empty;

                if (dtUserAdmin.Rows.Count > 0)
                {
                    dtId = dtUserAdmin.Rows[0]["Id"].ToString();
                    dtUser = dtUserAdmin.Rows[0]["Usuario"].ToString();
                    dtPass = dtUserAdmin.Rows[0]["Contrasena"].ToString();
                    if (usuario == dtUser && pass == dtPass)
                    {
                        LoginSupervisor = true;
                    }
                    else
                    {
                        MessageBox.Show("Verificar usuario y/o contraseña", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Verifique el usuario", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void nudTarjetaCredito_Click(object sender, EventArgs e)
        {
            nudTarjetaCredito.Select(0, nudTarjetaCredito.Text.Length);
        }

        private void nudTarjetaCredito_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void btnPagar_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender,e);
        }

        private void txtDescuento_TextChanged(object sender, EventArgs e)
        {
            if (LoginSupervisor)
            {
                if (txtDescuento.Text != string.Empty)
                {
                    PorcentajeDescuento = Convert.ToDouble(txtDescuento.Text);
                    if (PorcentajeDescuento > 100)
                    {
                        MessageBox.Show("El descuento no puede ser mayor al 100%", "Error en Descuento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDescuento.Text = string.Empty;
                        lblTotalFinal.Text = (TotalOriginalSinDescuento).ToString("C2");
                        VentaTotal = TotalOriginalSinDescuento;
                    }
                    else
                    {
                        double _TotalVenta = TotalOriginalSinDescuento;
                        Descuento = (double)((PorcentajeDescuento / 100) * (_TotalVenta));

                        lblTotalFinal.Text = (_TotalVenta - Descuento).ToString("C2");
                        TotalVenta = _TotalVenta - Descuento;
                    }
                }
                else
                {
                    VentaTotal = TotalOriginalSinDescuento;
                    lblTotalFinal.Text = (TotalOriginalSinDescuento).ToString("C2");
                }
                lblRestante.Text = lblTotalFinal.Text;
            }
            else
            {
                if (txtDescuento.Text != string.Empty)
                {
                    txtDescuento.Text = string.Empty;
                }
            }
        }

        private string DescuentoDelCliente(int idCliente)
        {
            string queryClienteDescuento = "SELECT PorcentajeDescuento FROM PVClientes WHERE id =" + idCliente;
            DataTable R = sqlLoc.selec(queryClienteDescuento);
            if (R.Rows.Count > 0)
            {
                return ((R.Rows[0][0]).ToString());
            }
            return "0";
        }

        private void nudTran_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender,e);
        }

        private void nudTran_Click(object sender, EventArgs e)
        {
            nudTran.Select(0, nudTran.Text.Length);
        }

        private void nudCheques_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender,e);
        }

        private void nudCheques_Click(object sender, EventArgs e)
        {
            nudCheques.Select(0, nudCheques.Text.Length);
        }

        private void txtFolioComprobante_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void txtFolioCheque_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void txtDescuento_KeyDown(object sender, KeyEventArgs e)
        {
            AtajosTeclado(sender, e);
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números (0-9) y teclas de control (como Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // No permitir el caracter
            }
        }

        private void txtDescuento_Leave(object sender, EventArgs e)
        {
            TotalVentaOriginal = TotalVenta;
            TotalVenta = Convert.ToDouble((lblTotalFinal.Text).Replace("$",string.Empty));
        }

        public string Autorizacion
        {
            get
            {
                return autorizacion;
            }
        }

        public string NumeroTarjeta
        {
            get
            {
                return numerotarjeta;
            }
        }

        public string Respuesta
        {
            get
            {
                return respuesta;
            }
        }

        //Transferencia
        public double MontoTransferencia
        {
            get
            {
                return (double)nudTran.Value;
            }
        }

        public string FolioTransferencia
        {
            get
            {
                return txtFolioComprobante.Text;
            }
        }

        //Monedero
        public double MontoCredito
        {
            get
            {
                return (double)nudCredito.Value; ;
            }
        }

        //Vales
        public double MontoVales
        {
            get
            {
                return (double)0;
            }
        }

        //Cheques
        public double MontoCheques
        {
            get
            {
                return (double)nudCheques.Value;
            }
        }

        public string FolioCheques
        {
            get
            {
                return txtFolioCheque.Text;
            }
        }
    }
}
