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
    public partial class FRecibirProductos : Form
    {
        ConSQL sql, sqlLoc;
        public FRecibirProductos(ConSQL _sql, ConSQL _sqlLoc)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
        }

        private void fRecibirProductos_Load(object sender, EventArgs e)
        {
            string queryProv = "SELECT Id, Nombre FROM PV_Cat_Proveedores";

            sql.llenaCombo(cbxProv, sql.selec(queryProv), "Id","Nombre");
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string queryProd = "select P.Descripcion Producto, P.Presentacion, '' Cantidad \n" +
                "From PV_Cat_ProveedoresProd PP \n" +
                "LEFT JOIN PV_Cat_Productos P ON PP.IdProducto = P.Id \n" +
                "LEFT JOIN PV_Cat_Proveedores Prov ON PP.IdProveedor = Prov.Id \n" +
                "WHERE PP.Activo = 1";// AND PP.IdProveedor = " + cbxProv.SelectedValue;

            dvgHistoria.DataSource = sql.selec(queryProd);

            foreach (DataGridViewColumn col in dvgHistoria.Columns)
            {
                col.ReadOnly = true;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxMotivo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
