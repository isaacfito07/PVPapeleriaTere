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
    public partial class FMenuCorte : Form
    {
        ConSQL sql, sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;
        bool permisoCorreccion = false;
        bool corteCaja = false;
        bool CorteFinalizado = false;

        public FMenuCorte(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, bool _consulta, string _numCaja)
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
            lblCaja.Text = "CAJA: " + numCaja;

            this.KeyPreview = true;
        }

        private void FMenuCorte_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void fMenuCorte_Load(object sender, EventArgs e)
        {
            //Consultar cortes parciales de caja en el día
            string fecha = DateTime.Now.ToString("yyyy-MM-dd");
            string queryCP = "SELECT COUNT(Id) FROM PVCorteCaja WHERE CONVERT(Date,FechaCorte) = '" + fecha + "'";
            var cant = sqlLoc.scalar(queryCP);
            int cantCP = 0;
            if(cant != null)
            {
                cantCP = (int)cant;
            }

            //Consulta si no hay ventas pendientes de generar corte parcial
            string queryVP = "SELECT COUNT(Id) FROM PVVentas WHERE Valido=1 AND IdSucursal="+ idSucursal + " AND FolioCorteParcialCaja IS NULL AND Terminada=1";
            var cant_ = sqlLoc.scalar(queryVP);
            int cantV = 0;
            if (cant_ != null)
            {
                cantV = (int)cant_;
            }


            //Consulta si ya hay un corte final en el dia actual
            string queryCF = "SELECT COUNT(Id) FROM PVCorteCaja WHERE Valido=1 AND CorteFinal=1 AND IdSucursal="+ idSucursal +" AND CONVERT(DATE, FechaCorte)='"+ fecha +"'";
            var cant_cf = sqlLoc.scalar(queryCF);
            int cantCF = 0;
            if (cant_cf != null)
            {
                cantCF = (int)cant_cf;
            }

            //cantCF = 0;
            btnCorteCaja.Enabled = cantCP >= 1 && cantV == 0 && cantCF == 0 ? true : false;
        }

        //Retiro de caja
        private void btnRetiroCaja_Click(object sender, EventArgs e)
        {
            FPassFondo passFondo = new FPassFondo();
            passFondo.ShowDialog();

            string usuario = passFondo.Usuario;
            string pass = passFondo.Contrasena;

            DataTable dtUsuario =
                sqlLoc.selec
                (
                    "SELECT Id, Usuario, Contrasena, RetiroCaja " +
                    "FROM PVUsuarios WHERE usuario = '" + usuario.Trim() + "'"
                );

            if (dtUsuario.Rows.Count > 0)
            {
                DataRow r = dtUsuario.Rows[0];

                if (pass.Trim() == r["Contrasena"].ToString().Trim())
                {
                    if (Convert.ToBoolean(r["RetiroCaja"]))
                    {
                        FRetiro retiroCaja = new FRetiro(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
                        this.Close();
                        retiroCaja.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No tienes el permiso realizar retiros, solicitalo a Dirección", "Sin permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Contraseña incorrecta", "Contraseña incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("No existe el usuario " + usuario.Trim(), "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;

            }

        }

        //Corte parcial de caja
        private void btnCorteParcialCaja_Click(object sender, EventArgs e)
        {
            FCorteParcial corteParcial = new FCorteParcial(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
            corteParcial.ShowDialog();
            CorteFinalizado = corteParcial.CorteFinalizadoCorrectamente;
            this.Close();
        }

        //Corte de caja
        private void btnCorteCaja_Click(object sender, EventArgs e)
        {
            FPassFondo passFondo = new FPassFondo();
            passFondo.ShowDialog();

            string usuario = passFondo.Usuario;
            string pass = passFondo.Contrasena;

            DataTable dtUsuario =
                sqlLoc.selec
                (
                    "SELECT Id, Usuario, Contrasena, CorteCaja " +
                    "FROM PVUsuarios WHERE usuario = '" + usuario.Trim() + "'"
                );

            if (dtUsuario.Rows.Count > 0)
            {
                DataRow r = dtUsuario.Rows[0];

                if (pass.Trim() == r["Contrasena"].ToString().Trim())
                {
                    if (Convert.ToBoolean(r["CorteCaja"]))
                    {
                        FCorteCajaII corteCaja = new FCorteCajaII(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
                        this.Close();
                        corteCaja.ShowDialog();
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
                MessageBox.Show("No existe el usuario ", "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
            }
        }

        private void fMenucorte_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CorteFinalizado)
            {
                FLogin fLogin = new FLogin();
                fLogin.Show();
            }
            else
            {
                FMenu menu = new FMenu(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, numCaja);
                menu.Show();
            }
        }
    }
}
