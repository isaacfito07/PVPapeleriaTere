using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FAlerta : Form
    {
        public FAlerta(string _Msg = "", string _Header = "")
        {
            InitializeComponent();

            if (_Msg != string.Empty && _Header != string.Empty)
            {
                lbMsg.Text = _Msg;
                lbHeader.Text = _Header;
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FAlerta_Load(object sender, EventArgs e)
        {
            btnCerrar.Focus();
            btnCerrar.Select();
            
            btnAceptar.Focus();
            btnAceptar.Select();
        }

        public void PlayMP3(string rutaArchivo)
        {
            // SoundPlayer simpleSound = new SoundPlayer(@"c:\"+ rutaArchivo +".wav");
            SoundPlayer sonido = new SoundPlayer(Application.StartupPath + "\\" + rutaArchivo);
            sonido.Play();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            btnCerrar.Focus();
            btnCerrar.Select();
          
        }

        private void FAlerta_KeyDown(object sender, KeyEventArgs e)
        {
            btnCerrar.Focus();
            btnCerrar.Select();
            try
            {
                PlayMP3("incorrecto.wav");
            }
            catch (Exception)
            {

            }

        }

        private void FAlerta_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEventArgs tmp = e;
            if (tmp.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnCerrar.Focus();
                btnCerrar.Select();
                try
                {
                    PlayMP3("incorrecto.wav");
                }
                catch (Exception)
                {

                }
            }
        }

        private void FAlerta_KeyUp(object sender, KeyEventArgs e)
        {
            btnCerrar.Focus();
            btnCerrar.Select();
            try
            {
                PlayMP3("incorrecto.wav");
            }
            catch (Exception)
            {

            }
        }

        private void FAlerta_Enter(object sender, EventArgs e)
        {
            btnCerrar.Focus();
            btnCerrar.Select();
            try
            {
                PlayMP3("incorrecto.wav");
            }
            catch (Exception)
            {

            }
        }

        private void btnAceptar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                this.Close(); //Cerrar con Enter o escape
            }
        }
    }
}
