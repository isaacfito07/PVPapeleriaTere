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
    public partial class FCajaFondo : Form
    {
        ConSQL sql, sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;
        
        public FCajaFondo(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string sucursal, string _idUsuario, string _numCaja)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
            nombre = _nombre;
            idSucursal = _idSucursal;
            idUsuario = _idUsuario;
            numCaja = _numCaja;
        }

        private void fCajaFondo_Load(object sender, EventArgs e)
        {

        }
        private void nudFondo_Click(object sender, EventArgs e)
        {
            nudFondo.Select(0, nudFondo.Text.Length);
        }

        private void fCajaFondo_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Muestra menú
            FMenu menu = new FMenu(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, numCaja);
            this.Hide();
            menu.ShowDialog();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string FolioFondoCaja = DateTime.Now.ToString("yyyyMMddTHHmmss-fff");

            //Inserta fondo de caja
            string query = " INSERT INTO PVFondoCaja (FolioFondoCaja, IdSucursal, " +
                "IdUsuario, Monto, Fecha, IdCaja, IdUsuarioAut) VALUES\n"
                    + " ('" + FolioFondoCaja + "'," + idSucursal + "," + idUsuario
                    + ", " + nudFondo.Value + ", '" + fechaHora + "', '" + numCaja + "', "
                    + idUsuario + " ) ";
            
            if (sqlLoc.exec(query) > 0)
            {
                //MessageBox.Show("Fondo ingresado con éxito.", "Fondo de caja!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error, favor de intentar de nuevo.", "Atención!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
