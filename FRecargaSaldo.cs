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
    public partial class FRecargaSaldo : Form
    {
        public FRecargaSaldo()
        {
            InitializeComponent();
        }

        private void FRecargaSaldo_Load(object sender, EventArgs e)
        {
            //'(xxx) xxx-xxxx
            //TELEFONO
            mtbNumeroTelefonico.Mask = "(000) 000-00-00";
            mtbNumeroTelefonicoRepetir.Mask = "(000) 000-00-00";


            mtbNumeroTelefonico.MaskInputRejected += new MaskInputRejectedEventHandler(mtbNumeroTelefonico_MaskInputRejected);
            mtbNumeroTelefonico.KeyDown += new KeyEventHandler(mtbNumeroTelefonico_KeyDown);
            mtbNumeroTelefonico.Select(0, mtbNumeroTelefonico.Text.Length);

            ////REPETIR
            //mtbNumeroTelefonicoRepetir.Mask = "(000) 000-00-00";
            //mtbNumeroTelefonicoRepetir.MaskInputRejected += new MaskInputRejectedEventHandler(mtbNumeroTelefonicoRepetir_MaskInputRejected);
            //mtbNumeroTelefonicoRepetir.KeyDown += new KeyEventHandler(mtbNumeroTelefonicoRepetir_KeyDown);
            //mtbNumeroTelefonicoRepetir.Select(0, mtbNumeroTelefonicoRepetir.Text.Length);

        }

        //TELEFONO
        private void mtbNumeroTelefonico_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            if (mtbNumeroTelefonico.MaskFull)
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("No puedes ingresar más números en este campo.", mtbNumeroTelefonico, 0, -20, 5000);
            }
            else if (e.Position == mtbNumeroTelefonico.Mask.Length)
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("No puedes ingresar más números en este campo.", mtbNumeroTelefonico, 0, -20, 5000);
            }
            else
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("Sólo puedes ingresar números (0-9) en este campo.", mtbNumeroTelefonico, 0, -20, 5000);
            }
        }

        private void mtbNumeroTelefonico_KeyDown(object sender, KeyEventArgs e)
        {
            // The balloon tip is visible for five seconds; if the user types any data before it disappears, collapse it ourselves.
            toolTip1.Hide(mtbNumeroTelefonico);
        }

        private void mtxtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEventArgs tmp = e;
            if (tmp.KeyChar == Convert.ToChar(Keys.Enter))
            {
            }
        }

        //REPETIR TELEFONO
        private void mtbNumeroTelefonicoRepetir_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            if (mtbNumeroTelefonicoRepetir.MaskFull)
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("No puedes ingresar más números en este campo.", mtbNumeroTelefonicoRepetir, 0, -20, 5000);
            }
            else if (e.Position == mtbNumeroTelefonicoRepetir.Mask.Length)
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("No puedes ingresar más números en este campo.", mtbNumeroTelefonicoRepetir, 0, -20, 5000);
            }
            else
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("Sólo puedes ingresar números (0-9) en este campo.", mtbNumeroTelefonicoRepetir, 0, -20, 5000);
            }

            ValidaTelefono();
        }

        private void mtbNumeroTelefonicoRepetir_KeyDown(object sender, KeyEventArgs e)
        {
            // The balloon tip is visible for five seconds; if the user types any data before it disappears, collapse it ourselves.
            toolTip1.Hide(mtbNumeroTelefonicoRepetir);
        }

        private void mtbNumeroTelefonicoRepetir_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEventArgs tmp = e;
            if (tmp.KeyChar == Convert.ToChar(Keys.Enter))
            {
            }
        }

        private bool ValidaTelefono() {
            bool validado = false;

            if (mtbNumeroTelefonico.Text == mtbNumeroTelefonicoRepetir.Text)
                validado = true;


            return validado;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidaTelefono())
                this.Close();
            else {
                MessageBox.Show("Los teléfonos no coinciden, favor de verificar",
                          "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button2);
            }                
        }


        public string NumeroTelefonico
        {
            get
            {
                return mtbNumeroTelefonico.Text;
            }
        }

      
    }


}
