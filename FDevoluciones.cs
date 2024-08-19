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
    public partial class FDevoluciones : Form
    {
        ConSQL sql, sqlLoc;
        //ConSQLCE sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;

        //indices de las columnas
        static int indTotal = 8, indIdCliente = 9;

        //permiso para cancelar
        bool permisoCancelar = true;

        public FDevoluciones(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario,
            DataTable _dtProductos, DataTable _dtSubcategorias,
            ImageList _imgLstCategorias, ImageList _imgLstProductos, string _numCaja)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
            nombre = _nombre;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuario = _idUsuario;
            numCaja = _numCaja;

            imgLstProductos = _imgLstProductos;
            imgLstCategorias = _imgLstCategorias;

            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja.Text = "CAJA: " + numCaja;

            permisoCancelar = (bool)sqlLoc.scalar("SELECT ISNULL(CancelarVenta, 0) " +
                "FROM PVUsuarios WHERE Id = " + idUsuario + "");
        }

        private void dvgHistoria_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(e);
        }

        private void fHistorial_Load(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        private void CargarHistorial(bool fechas = true)
        {
            //Consultar ventas en el rango de fechas especificado o por folio de venta
            string query =
                " SELECT " +
                " V.FolioVenta, V.FolioTicket Folio, V.FechaVenta, U.Nombres Usuario, VD.Cantidad Productos, Terminada, \n" +
                " V.DisparadoNube 'En Nube', V.FechaDisparo Disparado, FORMAT(ROUND(V.TotalVenta,2), 'C') Total, \n" +
                " ISNULL(V.IdCliente,0) IdCliente \n" +
                " FROM PVVentas V \n" +
                " LEFT JOIN PVUsuarios U ON U.Id = V.IdUsuarioVenta \n" +
                " LEFT JOIN ( \n" +
                "   SELECT DISTINCT VD.FolioVenta, SUM(VD.Cantidad) Cantidad \n" +
                "   FROM PVVentasDetalle VD GROUP BY VD.FolioVenta \n" +
                " ) VD ON V.FolioVenta = VD.FolioVenta \n" +
                " WHERE V.IdSucursal = " + idSucursal + " AND V.Terminada = 1 AND \n";
            if (fechas)
            {
                query += " (CAST(FechaVenta AS DATE) \n" +
                    " BETWEEN '" + dtpDe.Value.ToString("yyyy-MM-dd") + "' " +
                    " AND '" + dtpA.Value.ToString("yyyy-MM-dd") + "' ) \n";
            }
            else
            {
                query += "V.Folioticket like '%" + txtFolio.Text.Trim() + "%'";
            }

            dvgHistoria.DataSource = sqlLoc.selec(query);

            foreach (DataGridViewColumn col in dvgHistoria.Columns)
            {
                col.ReadOnly = true;

                if (col.Index == 0)
                {
                    col.Visible = false;
                }
                //Alinear total a la derecha
                if (col.Index == indTotal)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                if(col.Index == indIdCliente)
                {
                    col.Visible = false;
                }
            }
            dvgHistoria.Select();
        }

        private void txtFolio_TextChanged(object sender, EventArgs e)
        {
            CargarHistorial(false);
        }

        private void dvgHistoria_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                string folioVenta = (string)dvgHistoria.Rows[e.RowIndex].Cells["FolioVenta"].Value.ToString();
                string folioTicket = (string)dvgHistoria.Rows[e.RowIndex].Cells["Folio"].Value.ToString();
                int idCliente = (int)dvgHistoria.Rows[e.RowIndex].Cells["IdCliente"].Value;
                //Mostrar detalle
                FDetalleVenta fDetalle = new FDetalleVenta(sql, sqlLoc, folioVenta, folioTicket, idSucursal, sucursal, idUsuario, idCliente);
                fDetalle.ShowDialog();
                this.Close();
            }
        }

        private void Atajos(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
