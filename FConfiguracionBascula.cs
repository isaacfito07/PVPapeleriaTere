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
    public partial class FConfiguracionBascula : Form
    {

        ConSQL sql, sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;

        public FConfiguracionBascula(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, bool _consulta, string _numCaja)
        {
            InitializeComponent();
            sql = _sql;
            sqlLoc = _sqlLoc;
            nombre = _nombre;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuario = _idUsuario;
            numCaja = _numCaja;
        }

        private void FConfiguracionBascula_Load(object sender, EventArgs e)
        {
            DataTable dtDatosConfiguracion = sqlLoc.selec(" SELECT * FROM ConfiguracionBascula ");
            if (dtDatosConfiguracion.Rows.Count > 0) {
                txtPuerto.Text = dtDatosConfiguracion.Rows[0]["Puerto"].ToString();
                txtBaudios.Text = dtDatosConfiguracion.Rows[0]["VelocidadBaudios"].ToString();
                txtBitDatos.Text = dtDatosConfiguracion.Rows[0]["BitDeDatos"].ToString();
            }
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            int i = 0;
            DataTable dtDatosConfiguracion = sqlLoc.selec(" SELECT * FROM ConfiguracionBascula ");
            if (dtDatosConfiguracion.Rows.Count > 0)
            {
                i = sqlLoc.exec(" UPDATE ConfiguracionBascula SET Puerto='"+ txtPuerto.Text.Trim() +"', VelocidadBaudios='"+ txtBaudios.Text.Trim() +"', BitDeDatos='"+ txtBitDatos.Text.Trim() +"' ");

            }
            else {
                i = sqlLoc.exec(" INSERT INTO ConfiguracionBascula (Puerto, VelocidadBaudios, BitDeDatos) VALUES ('" + txtPuerto.Text.Trim() +"', '"+ txtBaudios.Text.Trim() +"', '"+ txtBitDatos.Text.Trim() +"') ");
            }

            if (i > 0)
            {
                MessageBox.Show("Configuración Báscula", "Datos actualizados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show("Configuración Báscula", "Datos actualizados", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
