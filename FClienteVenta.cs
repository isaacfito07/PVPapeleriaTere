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
    public partial class FClienteVenta : Form
    {
        ConSQL sqlLoc, sql;
        int idCliente = 0;
        string nombreCliente = "";
        double porcentajeDescuento = 0;

        public FClienteVenta(ConSQL _sqlLoc, ConSQL _sql)
        {
            InitializeComponent();
            sqlLoc = _sqlLoc;
            sql = _sql;
        }

        private void FClienteVenta_Load(object sender, EventArgs e)
        {
            CargarGrid();
        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            CargarGrid(txtCliente.Text);
        }

        private void CargarGrid(string cliente = "")
        {
            //string query = "SELECT Id, Nombre, Telefono FROM PVClientes \n";
            //string query = "SELECT \n"
            //            + " 	C.Id, C.Nombre, C.Telefono, ROUND(ISNULL(MontoMonedero,0), 2) Monedero, \n"
            //            + " 	ISNULL(C.PorcentajeDescuento,0) Descuento\n"
            //            + " FROM PVClientes C\n"
            //            + " LEFT JOIN (SELECT IdCliente, SUM(Monto) MontoMonedero FROM PVMonederoCliente WHERE Valido=1 GROUP BY IdCliente) MC ON C.Id = MC.IdCliente\n"
            //            + " ";
            //string query = "SELECT \n"
            //               + "     C.Id, C.Nombre, C.Telefono, ROUND(ISNULL(MontoMonedero,0), 2) Monedero, \n"
            //               + "     ISNULL(C.PorcentajeDescuento,0) Descuento\n"
            //               + " FROM PVClientes C\n"
            //               + " LEFT JOIN (\n"
            //               + "     SELECT\n"
            //               + "         M.IdCliente, SUM(ISNULL(M.Monto,0)) + SUM(ISNULL(D.MontoDevolucion,0)) MontoMonedero\n"
            //               + "     FROM PVMonederoCliente M\n"
            //               + "     LEFT JOIN (SELECT IdCliente, SUM(MontoDevolucion) MontoDevolucion \n"
            //               + "     FROM PVDevoluciones WHERE Monedero=1 AND Activo=1 GROUP BY IdCliente) D ON M.IdCliente = D.IdCliente \n"
            //               + "     WHERE M.Valido=1 \n"
            //               + "     GROUP BY M.IdCliente\n"
            //               + " ) MC ON C.Id = MC.IdCliente";

            string query = "SELECT  C.Id, C.Nombre, C.Telefono, ROUND(ISNULL(MontoMonedero,0), 2) Monedero, \n"
                         + " ISNULL(C.PorcentajeDescuento, 0) Descuento FROM PVClientes C\n"    
                         + " LEFT JOIN (\n"
                         + "     SELECT \n"
                         + "        R.IdCliente, SUM(ISNULL(R.Monto,0)) MontoMonedero \n"
                         + "     FROM\n"
                         + "     (\n"
                         + "         SELECT\n"
                         + "             M.IdCliente, SUM(ISNULL(M.Monto,0)) Monto\n"
                         + "         FROM PVMonederoCliente M\n"
                         + "         WHERE M.Valido=1\n"
                         + "         GROUP BY M.IdCliente\n"
                         + "         UNION ALL\n"
                         + "         SELECT \n"
                         + "            IdCliente, SUM(MontoDevolucion) MontoDevolucion \n"
                         + "         FROM PVDevoluciones \n"
                         + "         WHERE Monedero=1 AND Activo=1 GROUP BY IdCliente\n"
                         + "    ) R GROUP BY  R.IdCliente\n"
                         + " ) MC ON C.Id = MC.IdCliente";

            if(!string.IsNullOrEmpty(query))
                query += " WHERE CONCAT(Nombre, ' ', Telefono ) like '%" + cliente + "%'";


            DataTable dtClientes = sqlLoc.selec(query);

            gvClientes.DataSource = null;

            if (dtClientes.Rows.Count > 0)
            {
                gvClientes.DataSource = dtClientes;
                gvClientes.Columns["Id"].Visible = false;
                gvClientes.Columns[0].Width = 150;
                gvClientes.Columns["Nombre"].Width = 300;
                gvClientes.Columns["Telefono"].Width = 200;
                gvClientes.Columns["Monedero"].Width = 100;
                gvClientes.Columns["Descuento"].Width = 100;


                lblCantClientes.Text = dtClientes.Rows.Count.ToString() + " Clientes";
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            //Refrescar catalogo clientes
            //Cat_Clientes
            string query = " SELECT Id, Clave, Nombre, RFC, Calle, NumInterior, \n" +
                "NumExterior, Colonia, CP, Telefono, Poblacion, Municipio, Estado, \n" +
                "CONVERT(int, ISNULL(UsoCFDI, 0)) UsoCFDI, RegimenFiscal, \n" +
                "DiasCredito, LimiteCredito, FechaAlta, IdUsuarioAlta, ISNULL(TieneDescuento,0) TieneDescuento, ISNULL(PorcentajeDescuento,0) PorcentajeDescuento \n" +
                "FROM PV_Cat_Clientes WHERE Activo = 1 ";
            DataTable dtClientes = sql.selec(query);
            sqlLoc.exec(" TRUNCATE TABLE PVClientes ");

            if (sqlLoc.copiaBulto(dtClientes, "PVClientes") > 0)
            {
                MessageBox.Show("Se descargó el catalogo de clientes", "Descarga",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void gvClientes_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                idCliente = int.Parse(gvClientes.Rows[e.RowIndex].Cells["Id"].Value.ToString());
                nombreCliente = gvClientes.Rows[e.RowIndex].Cells["Nombre"].Value.ToString();
                double PorcentajeDesc = 0;
                double.TryParse(gvClientes.Rows[e.RowIndex].Cells["Descuento"].Value.ToString(), out PorcentajeDesc);

                porcentajeDescuento = PorcentajeDesc;
                this.Close();
            }
        }

        public int IdCliente
        {
            get
            {
                return idCliente;
            }
        }

        public string NombreCliente
        {
            get
            {
                return nombreCliente;
            }
        }

        public double PorcentajeDescuento
        {
            get
            {
                return porcentajeDescuento;
            }
        }
    }
}
