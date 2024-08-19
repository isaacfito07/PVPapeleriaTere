using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace PVLaJoya
{
    public partial class FConsultaProducto : Form
    {
        ConSQL sqlLoc, sql;
        DataTable dtProductos;
        string idSucursal = "";
        int idTipoCliente = 0;

        private SerialPort serialPort;

        bool Pesaje = false;

        public FConsultaProducto(ConSQL _sql, ConSQL _sqlLoc, string _idSucursal, DataTable _dtProductos, int _idTipoCliente = 1)
        {
            InitializeComponent();
            
            this.StartPosition = FormStartPosition.CenterScreen;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            sql = _sql;
            sqlLoc = _sqlLoc;
            idSucursal = _idSucursal;
            dtProductos = _dtProductos;
            idTipoCliente = _idTipoCliente;

            InitializeSerialPort();
        }

        private void FConsultaProducto_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    // Cerrar el formulario cuando se presiona Escape
                    this.Close();
                    break;
                case Keys.F1:
                    btnBuscarCodigo_Click(sender,e);
                    break;
            }
        }

        private void btnBuscarCodigo_Click(object sender, EventArgs e)
        {
            CargarProducto(AbrirBuscadorProducto());
        }

        private void txtScan_Leave(object sender, EventArgs e)
        {
            CargarProducto();
            txtScan.Text = string.Empty;
        }

        private string AbrirBuscadorProducto()
        {
            FBuscadorCodigos fBuscador = new FBuscadorCodigos(sqlLoc, sql, idSucursal,idTipoCliente);
            fBuscador.ShowDialog();

            return fBuscador.IdProducto;
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtScan_Leave(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void FConsultaProducto_Load(object sender, EventArgs e)
        {

        }

        private void CargarProducto(string idProducto = "")
        {
            var existe = dtProductos.Select(idProducto != string.Empty ? "Id='" + idProducto + "'" : "CodigoBarras='" + txtScan.Text.Trim() + "'");
            if (existe.Count() > 0)
            {
                string IdProducto = existe[0]["Id"].ToString();
                string esCaja = existe[0]["Escaja"].ToString();
                string uom = existe[0]["Uom"].ToString();
                string Producto = existe[0]["Producto"].ToString() + " " + existe[0]["Presentación"].ToString();
                string Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                string PrecioFinal = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString("C2");
                string Imagen = existe[0]["Foto"].ToString();
                string IVA__ = existe[0]["IVA"].ToString();
                string IEPS__ = existe[0]["IEPS"].ToString();
                string idPres = existe[0]["IdPresentacionVenta"].ToString();
                string idMarca = existe[0]["IdMarca"].ToString();
                string idLinea = existe[0]["IdLinea"].ToString();
                string sku = existe[0]["sku"].ToString();

                bool.TryParse(existe[0]["Pesaje"].ToString(), out Pesaje);

                switch(idTipoCliente)
                {
                    case 1:
                        PrecioFinal = Convert.ToDouble(existe[0]["PrecioGeneral"]).ToString("C2");
                        break;
                    case 2:
                        PrecioFinal = Convert.ToDouble(existe[0]["PrecioTalleres"]).ToString("C2");
                        break;
                    case 3:
                        PrecioFinal = Convert.ToDouble(existe[0]["PrecioDistribuidores"]).ToString("C2");
                        break;
                }

                lblDescProd.Text = Producto;
                lblPrecioProd.Text = PrecioFinal;

                // Envía el carácter 'P' para solicitar la lectura
                if (serialPort != null)
                {
                    serialPort.Write("P");
                }



                try
                {
                    var img = @"C:\PVLaJoya\Productos\" + Imagen;
                    //pbImagen.Image = Image.FromFile(img);
                }
                catch
                {
                    pbImagen.Image = null;
                }
            }
            else if (txtScan.Text != string.Empty)
            {
                FAlerta fAlerta = new FAlerta();
                fAlerta.ShowDialog();
            }
        }

        private void InitializeSerialPort()
        {
            DataTable dtDatosConfiguracion = sqlLoc.selec(" SELECT * FROM ConfiguracionBascula ");
            if (dtDatosConfiguracion.Rows.Count > 0)
            {
                serialPort = new SerialPort();
                serialPort.PortName = dtDatosConfiguracion.Rows[0]["Puerto"].ToString(); ; // Ajusta al puerto COM correcto COM05
                serialPort.BaudRate = Convert.ToInt16(dtDatosConfiguracion.Rows[0]["VelocidadBaudios"].ToString()); // Ajusta la velocidad de baudios según la configuración de tu báscula
                serialPort.Parity = Parity.None;
                serialPort.DataBits = Convert.ToInt16(dtDatosConfiguracion.Rows[0]["BitDeDatos"].ToString());
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;

                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                try
                {
                    serialPort.Open();

                    // Envía el carácter 'P' para solicitar la lectura
                    serialPort.Write("P");

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error abriendo el puerto: {ex.Message}");
                }
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadExisting();
                Console.WriteLine($"Datos recibidos: {data}"); // Para depuración
                this.BeginInvoke(new Action(() => ProcessData(data)));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leyendo datos: {ex.Message}");
            }
        }


        private void ProcessData(string data)
        {
            // Procesa los datos recibidos desde la báscula
            // Por ejemplo, actualiza un Label en el formulario
            lblPeso.Text = data;

            double PrecioFinal_ = 0;
            double.TryParse(lblPrecioProd.Text.Replace("$","").Replace(",",""), out PrecioFinal_);


            double Peso = 0;
            double.TryParse(lblPeso.Text.Trim().Replace("kg", "").Trim(), out Peso);

            double Total = 0;

            if (Pesaje)
                Total = PrecioFinal_ * Peso;
            else
                Total = PrecioFinal_;

           // lblTotal.Text = Total.ToString("C2");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
            base.OnFormClosing(e);
        }
    }
}
