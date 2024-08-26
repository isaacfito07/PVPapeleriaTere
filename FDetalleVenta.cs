using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FDetalleVenta : Form
    {
        ConSQL sql, sqlLoc;
        //ConSQLCE sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja, folioVenta, folioTicket;
        string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        //double totalDevolucion;

        DataTable dtProductos, dtSubcategorias;

        //indices de las columnas
        static int indSelect = 0, indId = 1, indProducto = 2, indCantidadOriginal = 3, indCantidad = 4, 
            indCantidadDevolucion = 5, indPrecio = 6, indIva = 7, indIeps = 8, indTotal = 9, indIdProd = 10,
            indEsCaja = 11, indUom = 12;

        //permiso para cancelar
        bool permisoCancelar = true;
        int idCliente;

        public FDetalleVenta(ConSQL _sql, ConSQL _sqlLoc, string _folioVenta, string _folioTicket,
            string _idSucursal, string _sucursal, string _idUsuario, int _idCliente)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
            folioVenta = _folioVenta;
            folioTicket = _folioTicket;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuario = _idUsuario;
            idCliente = _idCliente;

            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja.Text = "CAJA: " + numCaja;

            permisoCancelar = (bool)sqlLoc.scalar("SELECT ISNULL(CancelarVenta, 0) " +
                "FROM PVUsuarios WHERE Id = " + idUsuario + "");
        }

        private void fHistorial_Load(object sender, EventArgs e)
        {
            lblVenta.Text = "Venta: " + folioTicket;
            lblTotal.Text = "Total Venta: ";

            if (idCliente != 0)
            {
                var nombreCliente = sqlLoc.scalar("SELECT CONCAT(Nombre, ' - ', Telefono ) \n" +
                    "FROM PVClientes WHERE ID = " + idCliente);
                lblCliente.Text = "Cliente: " + nombreCliente;
            }
            //Consultar detalle de la venta seleccionada
            string query = "SELECT DISTINCT VD.Id, \n" +
                " CONCAT(P.Descripcion, ' ', Marca, ' ', Presentacion, ' '," +
                "   CASE WHEN VD.EsCaja = 1 THEN CONCAT('C/',VD.Uom ) ELSE 'PZA' END) Producto, \n" +
                " VD.Cantidad 'Cantidad Original', " +
                " (VD.Cantidad - ISNULL(D.CantidadDevuelta,0)) 'Cantidad Restante', " +
                " (VD.Cantidad - ISNULL(D.CantidadDevuelta,0)) 'Cantidad Devolver', \n" +
                " FORMAT(ROUND(VD.PrecioSinImpuesto, 2),'C') Precio, \n" +
                " FORMAT(ROUND((VD.PrecioSinImpuesto * VD.Iva),2),'C') IVA, \n" +
                " FORMAT(ROUND((VD.PrecioSinImpuesto * VD.Ieps), 2),'C') IEPS, \n" +
                " FORMAT(ROUND((VD.Precio * Cantidad),2),'C') Total, VD.IdProducto," +
                " VD.EsCaja, VD.Uom  \n" +
                " FROM PVVentasDetalle VD " +
                " LEFT JOIN PVProductos p ON vd.IdProducto = p.Id \n" +
                //" LEFT JOIN (SELECT * FROM PVDevoluciones WHERE Activo=1) D ON VD.IdProducto = D.IdProducto AND VD.FolioVenta = D.FolioVenta \n" +
                " LEFT JOIN \n" +
                " (\n" +
	            "     SELECT FolioVenta, IdCliente, IdProducto, MAX(CantidadOriginal) CantidadOriginal, SUM(CantidadDevuelta) CantidadDevuelta, SUM(MontoDevolucion) MontoDevolucion\n" +
	            "     FROM PVDevoluciones WHERE Activo=1 GROUP BY FolioVenta, IdCliente, IdProducto\n" +
                " ) D ON VD.IdProducto = D.IdProducto AND VD.FolioVenta = D.FolioVenta \n" +
                " WHERE VD.FolioVenta = '" + folioVenta + "'\n" +
                " Order By Producto ";

            gvDetalle.DataSource = sqlLoc.selec(query);

            foreach (DataGridViewColumn col in gvDetalle.Columns)
            {
                if (col.Index == indId || col.Index == indIdProd || col.Index == indEsCaja || col.Index == indUom)
                {
                    col.Visible = false;
                }
                
                if ( col.Index == indProducto)
                {
                    col.Width = 500;
                }

                //col.ReadOnly = true;
                if ( col.Index == indProducto || col.Index == indCantidad 
                    || col.Index == indPrecio || col.Index == indIva
                    || col.Index == indIeps || col.Index == indTotal)
                {
                    col.ReadOnly = true;
                }

                //Alinear total a la derecha
                if (col.Index == indCantidad 
                    || col.Index == indCantidadDevolucion 
                    || col.Index == indPrecio || col.Index == indIva 
                    || col.Index == indIeps || col.Index == indTotal)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            double sum = 0;
            foreach (DataGridViewRow gvr in gvDetalle.Rows)
            {
                double total =
                    double.Parse(gvr.Cells[indTotal].Value.ToString(), NumberStyles.Currency, null);
                sum += total;

                gvr.Cells[indCantidadDevolucion].Style.BackColor = ColorTranslator.FromHtml("#bfd0f2");
            }

            lblTotal.Text = "Total Venta: " + sum.ToString("C2");
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (cbxMotivo.SelectedIndex > 0)
            {
                string mensaje = "";
                string data = "";
                double totalDev = 0;

                foreach (DataGridViewRow row in gvDetalle.Rows)
                {
                    var checkControl = gvDetalle[indSelect, row.Index] as DataGridViewCheckBoxCell;
                    checkControl.Value = checkControl.EditedFormattedValue;
                    double cantDevolucion = (double)row.Cells[indCantidadDevolucion].Value;
                    //Si se seleccionó para devolución
                    if ((bool)checkControl.Value && cantDevolucion > 0)
                    {
                        string idProducto = row.Cells[indIdProd].Value.ToString();
                        string producto = row.Cells[indProducto].Value.ToString();
                        double cantOriginal = (double)row.Cells[indCantidad].Value;
                        double total = Convert.ToDouble(this.gvDetalle.Rows[row.Index].Cells[indTotal].Value.ToString().Replace("$","").Replace(",",""));
                        double montoDevolucion;
                        if (cantDevolucion < cantOriginal)
                        {
                            montoDevolucion = (total / cantOriginal) * cantDevolucion;
                        }
                        else
                        {
                            montoDevolucion = total;
                        }
                        data += String.Format("{0,-65}\n {1,-20} {2, -10} \n",
                            "* " + producto, cantDevolucion, montoDevolucion.ToString("C2"));
                        totalDev += montoDevolucion;
                    }
                }
                if (totalDev <= 0)
                {
                    MessageBox.Show("Selecciona los articulos a devolver antes de continuar.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                if (cbxAccion.SelectedIndex <0) {
                    MessageBox.Show("Selecciona una acción.",
                                  "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                DialogResult dr = MessageBox.Show("Se devolverán los siguientes productos de la venta: \n" +
                    " - ---------------------- \n" +
                    "" + data + "\n" +
                    "- ---------------------- \n" +
                    "Total: " + totalDev.ToString("C2") + "\n" +
                    "¿Desea continuar?",
                    "Devolución", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                if (dr == DialogResult.No)
                    goto fin;

                if (cbxAccion.Text == "Monedero Cliente")
                {
                    bool cliente = false;
                    if (idCliente > 0)
                        cliente = true;
                    else
                        cliente = AsignarCliente();

                    if (cliente)
                    {
                        if (GuardarDevolucion(true))
                        {
                            MessageBox.Show("Devolución guardada.",
                                "Devolución", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Ocurrio un error al guardar la devolución.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                        }
                    }
                    else
                    {
                        //Mensaje Error
                    }
                }
                else
                {
                    if (GuardarDevolucion(false))
                    {
                        MessageBox.Show("Devolución guardada.", "Devolución",
                            MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                        this.Close();
                    }
                }

            }
            else {
                MessageBox.Show("Para continuar, favor de seleccionar el motivo de la devolución",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        fin:;
        }

        private bool AsignarCliente()
        {
            //FCliente fCliente = new FCliente(sqlLoc, sql, idUsuario);
            FClienteVenta fCliente = new FClienteVenta(sqlLoc, sql);
            fCliente.ShowDialog();
            
            if(fCliente.IdCliente > 0)
            {
                idCliente = fCliente.IdCliente;
                lblCliente.Text = "Cliente: " + fCliente.NombreCliente;
                return true;
            }
            return false;
        }

        private bool GuardarDevolucion(bool monedero)
        {
            bool defectuoso, incorrecto;

            if (cbxMotivo.SelectedIndex != 0)
            {
                defectuoso = cbxMotivo.SelectedIndex == 1 ? true : false;
                incorrecto = cbxMotivo.SelectedIndex == 2 ? true : false;

                foreach (DataGridViewRow row in gvDetalle.Rows)
                {
                    var checkControl = gvDetalle[indSelect, row.Index] as DataGridViewCheckBoxCell;
                    checkControl.Value = checkControl.EditedFormattedValue;
                    //Si se seleccionó para devolución
                    if ((bool)checkControl.Value)
                    {
                        string idProducto = row.Cells[indIdProd].Value.ToString();
                        double cantOriginal = (double)row.Cells[indCantidad].Value;
                        double cantDevolucion = (double)row.Cells[indCantidadDevolucion].Value;
                        double total = Convert.ToDouble(this.gvDetalle.Rows[row.Index].Cells[indTotal].Value.ToString().Replace("$","").Replace(",",""));
                        string esCaja = this.gvDetalle.Rows[row.Index].Cells[indEsCaja].Value.ToString();
                        double uom = (double)this.gvDetalle.Rows[row.Index].Cells[indUom].Value;
                        double montoDevolucion;
                        if (cantDevolucion < cantOriginal)
                        {
                            montoDevolucion = (total / cantOriginal) * cantDevolucion;
                        }
                        else
                        {
                            montoDevolucion = total;
                        }

                        string query = string.Format(" INSERT INTO PVDevoluciones " +
                            " (FolioVenta, IdCliente, IdProducto, " +
                            " CantidadOriginal, CantidadDevuelta, MontoDevolucion, Activo, Motivo, " +
                            " Monedero, FechaAlta, IdUsuarioAlta, DisparadoNube," +
                            " InventarioDefectuoso, Inventario, EsCaja, Uom)VALUES" +
                            " ({0},{1},{2},{3},{4},{5},{6},'{7}','{8}','{9}','{10}'," +
                            " {11},{12},{13},'{14}',{15})",
                        !string.IsNullOrEmpty(folioVenta) ? "'" + folioVenta + "'" : "NULL",
                        idCliente, idProducto, cantOriginal, cantDevolucion, montoDevolucion, 1,
                        cbxMotivo.Text, monedero, fechaHora, idUsuario, 0, 0, 0, esCaja, uom);

                        if(sqlLoc.exec(query) > 0)
                        {
                            //Afectar inventario y subir devoluciones a nube
                            Subir();
                        }
                        else
                        {
                            MessageBox.Show("Ocurrio un error, verifica la información e intentalo de nuevo",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            return false;
                        }
                    }
                }

                FTicketDevolucion ticket = new FTicketDevolucion(sqlLoc, folioVenta, fechaHora, true);
                ticket.ShowDialog();

            }
            return true;
        }

        private bool Subir()
        {
            if (!CheckForInternetConnection())
            {
                return false;
            }

            //Buscar devoluciones que no se hayan subido a la nube y no hayan afectado inventario 
            string queryDev = "SELECT Id, FolioVenta, IdCliente, IdProducto, CantidadOriginal, \n" +
                " CantidadDevuelta, MontoDevolucion, Activo, Motivo, Monedero, \n" +
                " Inventario, InventarioDefectuoso, FechaAlta, IdUsuarioAlta, EsCaja, Uom \n" +
                " FROM PVDevoluciones \n" +
                " WHERE DisparadoNube = 0";

            DataTable dtDevoluciones = sqlLoc.selec(queryDev);

            if (dtDevoluciones.Rows.Count > 0)
            {
                foreach (DataRow r in dtDevoluciones.Rows)
                {
                    int id = (int)r["Id"];
                    string folioVenta = r["FolioVenta"].ToString();
                    int idCliente = (int)r["IdCliente"];
                    int idProducto = (int)r["IdProducto"];
                    double cantidadOriginal = (double)r["CantidadOriginal"];
                    double cantidadDevolucion = (double)r["CantidadDevuelta"];
                    string MontoDevolucion = r["MontoDevolucion"].ToString();
                    string motivo = r["Motivo"].ToString();
                    bool monedero = (bool)r["Monedero"];
                    string fechaAlta = Convert.ToDateTime(r["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string idUsuarioAlta = r["IdUsuarioAlta"].ToString();
                    string esCaja = r["EsCaja"].ToString();
                    string uom = r["Uom"].ToString();

                    if(SubirDevolucionNube(id, folioVenta, idCliente, idProducto, 
                        cantidadOriginal, cantidadDevolucion, MontoDevolucion, 
                        motivo, monedero, 0, 0, 
                        fechaAlta, idUsuarioAlta, esCaja, uom))
                    {
                        //Marcar devolución como enviada a nube
                        sqlLoc.exec("UPDATE PVDevoluciones SET DisparadoNube = 1 \n" +
                            "WHERE Id = " + id);
                    }
                    
                }
            }
            return false;
        }

        private bool SubirDevolucionNube(int id, string folioVenta, int idCliente, 
            int idProducto, double cantidadOriginal, double cantidadDevolucion, 
            string montoDevolucion, string motivo, bool monedero, 
            int inventario, int inventarioDefectuoso, string fechaAlta, string idUsuarioAlta, 
            string escaja, string uom)
        {
            string queryDev = string.Format("INSERT INTO PVDevoluciones(FolioVenta, IdCliente, IdProducto, " +
                " CantidadOriginal, CantidadDevuelta, MontoDevolucion, Motivo," +
                " Monedero, Inventario, InventarioDefectuoso, FechaAlta, IdUsuarioAlta, " +
                " FechaDisparo, IdUsuarioDisparo, EsCaja, Uom)" +
                " VALUES('{0}',{1},{2},{3},{4},{5},'{6}','{7}','{8}',{9},'{10}',{11},'{12}',{13},'{14}',{15})",
                  folioVenta, idCliente, idProducto, cantidadOriginal, cantidadDevolucion, 
                  montoDevolucion, motivo, monedero, inventario, inventarioDefectuoso,
                  fechaAlta, idUsuarioAlta, fechaHora, idUsuario, escaja, uom);
            //Subir devolución a la nube
            if (sql.exec(queryDev) > 0)
                return true;

            return false;
        }
        
        private void gvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var x = this.gvDetalle.Rows[e.RowIndex].Cells[indTotal].Value;
            var checkControl = gvDetalle[indSelect, e.RowIndex] as DataGridViewCheckBoxCell;
            checkControl.Value = checkControl.EditedFormattedValue;

            double cantActual = (double)this.gvDetalle.Rows[e.RowIndex].Cells[indCantidad].Value;
            var cantDevolucion = (double)this.gvDetalle.Rows[e.RowIndex].Cells[indCantidadDevolucion].Value;
            
            if (cantActual > cantDevolucion && !(bool)checkControl.Value)
                checkControl.Value = true;
        }

        private void gvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (gvDetalle.Columns[e.ColumnIndex].Index == indCantidadDevolucion)
            {
                double cantActual = (double)gvDetalle.Rows[e.RowIndex].Cells[indCantidad].Value;
                double cantDev = (double)gvDetalle.Rows[e.RowIndex].Cells[indCantidadDevolucion].Value;

                if(cantActual < cantDev)
                {
                    MessageBox.Show("No se pueden devolver mas productos de los registrados en la venta",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button2);
                    gvDetalle.Rows[e.RowIndex].Cells[indCantidadDevolucion].Value = cantActual;
                }
            }
        }

        private void cbxMotivo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            cbxAccion.Items.Clear();
            //Defectuoso
            if (cbxMotivo.SelectedIndex == 2)
            {
                //cbxAccion.Items.Insert(0, "");
                cbxAccion.Items.Insert(0, "Cambio Fisico");
                cbxAccion.Items.Insert(1, "Monedero Cliente");
                //cbxAccion.Items.Insert(2, "Devolución Efectivo");
            }

            //Incorrecto
            if (cbxMotivo.SelectedIndex != 2)
            {
                cbxAccion.Items.Insert(0, "Monedero Cliente");
                //cbxAccion.Items.Insert(1, "Devolución Efectivo");
            }
            cbxAccion.SelectedIndex = 0;
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
