using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FPesa : Form
    {
        double costoUnitario = 0;
        SerialPort serialPortPesa;
        ConSQL sqlLoc;

        public double Precio = 0, Cantidad = 0;
        public bool Correcto = false;

        private PrivateFontCollection privateFonts = new PrivateFontCollection();

        public FPesa(string _NombreProducto, double _costoUnitario, ConSQL _sqlLoc)
        {
            InitializeComponent();
            lbProducto.Text = _NombreProducto;
            this.costoUnitario = _costoUnitario;
            sqlLoc = _sqlLoc;
        }

        private void FPesa_Load(object sender, EventArgs e)
        {
            InitializeSerialPort();
            if (!txtPeso.Enabled)
            {
                IniciarEnvioPeriodico();
            }
            else
            {
                txtPeso.TextChanged += new EventHandler(txtPeso_TextChanged);
                txtPeso.KeyPress += new KeyPressEventHandler(txtPeso_KeyPress);
                txtPeso.KeyDown += new KeyEventHandler(txtPeso_KeyDown);
            }
            try
            {
                // Ruta del archivo de fuente
                string fontPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Resources", "fonts", "digital-7.ttf");

                // Cargar la fuente desde el archivo
                privateFonts.AddFontFile(fontPath);
                txtPeso.Font = new Font(privateFonts.Families[0], 26);
            }
            catch
            {

            }
        }

        private void FPesa_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(e);
        }

        private void Atajos(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.Enter)
            {
                if (txtPeso.Text != string.Empty || txtPrecio.Text != string.Empty)
                {
                    if (txtPrecio.Text == "0")
                    {
                        MessageBox.Show("No es posible registrar un precio en $0.00");
                    }
                    else
                    {
                        Correcto = true;
                        this.Close();
                    }
                }
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string data = sp.ReadExisting();

                string[] MultiplesLecturas = data.Split(new string[] { "kg" }, StringSplitOptions.None);
                int Cantidad = MultiplesLecturas.Length;
                string lectura = MultiplesLecturas[Cantidad - (Cantidad >= 2 ? 2 : 1)].Replace(" ", "").Replace("\r", "");

                if (!lectura.Equals(string.Empty))
                    CambiarDatos(lectura);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de lectura en el puerto");
            }
        }

        private void InitializeSerialPort()
        {
            DataTable dtDatosConfiguracion = sqlLoc.selec(" SELECT * FROM ConfiguracionBascula ");
            if (dtDatosConfiguracion.Rows.Count > 0)
            {
                serialPortPesa = new SerialPort();
                serialPortPesa.PortName = dtDatosConfiguracion.Rows[0]["Puerto"].ToString(); // Ajusta al puerto COM correcto COM05
                serialPortPesa.BaudRate = Convert.ToInt16(dtDatosConfiguracion.Rows[0]["VelocidadBaudios"].ToString()); // Ajusta la velocidad de baudios según la configuración de tu báscula
                serialPortPesa.Parity = Parity.None;
                serialPortPesa.DataBits = Convert.ToInt16(dtDatosConfiguracion.Rows[0]["BitDeDatos"].ToString());
                serialPortPesa.StopBits = StopBits.One;
                serialPortPesa.Handshake = Handshake.None;

                serialPortPesa.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                try
                {
                    serialPortPesa.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error abriendo el puerto: {ex.Message}");
                    txtPeso.Enabled = true;
                }
            }
        }

        private void CambiarDatos(string data)
        {
            string dataDouble = string.Empty;
            try
            {
                dataDouble = Math.Round(Convert.ToDouble(data) * costoUnitario, 2).ToString();
            }
            catch
            {
                dataDouble = "0";
                data = "0.000";
            }
            if (txtPeso.InvokeRequired)
            {
                txtPeso.Invoke(new Action(() =>
                {
                    txtPeso.Text = data;
                }));
            }
            else
            {
                if (!txtPeso.Enabled)
                {
                    txtPeso.Text = data;
                }
            }

            if (txtPrecio.InvokeRequired)
            {
                txtPrecio.Invoke(new Action(() =>
                {
                    txtPrecio.Text = dataDouble;
                }));
            }
            else
            {
                txtPrecio.Text = (Convert.ToDouble(data) * costoUnitario).ToString(); ;
            }
            Cantidad = Convert.ToDouble(data);
            Precio = Convert.ToDouble(txtPrecio.Text);
        }

        private void txtPeso_TextChanged(object sender, EventArgs e)
        {
            string data = txtPeso.Text;
            if (!txtPeso.Text.Equals(string.Empty))
            {
                data = "0";
            }
            CambiarDatos(txtPeso.Text);
        }

        private void txtPeso_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(e);
        }

        private void txtPeso_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números, la tecla de retroceso y el punto decimal
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // Cancela la entrada si no es un número
            }

            // Si ya existe un punto decimal, no permitir otro
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void EnviarComandoP()
        {
            if (serialPortPesa != null && serialPortPesa.IsOpen)
            {
                serialPortPesa.Write("P");
            }
        }

        private void IniciarEnvioPeriodico()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Tick += (s, e) => EnviarComandoP();
            timer.Interval = 200;
            timer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (serialPortPesa != null && serialPortPesa.IsOpen)
            {
                serialPortPesa.Close();
            }
        }
    }
}
