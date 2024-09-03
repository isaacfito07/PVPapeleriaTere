using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PVLaJoya
{
    public partial class FDetalleDeuda : Form
    {
        ConSQL sql, sqlLoc;

        public FDetalleDeuda(ConSQL _sql, ConSQL _sqlLoc, int idCliente)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;

            // Cargar los abonos
            CargarDetalleDeuda(idCliente);

            // Load
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            dgvDetalleProducto.Focus();
            this.dgvDetalleProducto.RowHeadersVisible = false;
            this.dgvDetalleProducto.DefaultCellStyle.Font = new Font(dgvDetalleProducto.DefaultCellStyle.Font, FontStyle.Regular);
        }

        private void dgvHistorialAbono_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void cbFolioVenta_SelectedIndexChanged(object sender, EventArgs e)
        {
            string FolioVentaCombo = cbFolioVenta.Text;


            string queryDetalle = " SELECT \n"
                + "     CASE WHEN PVP.sku != '' OR PVP.sku IS NOT NULL THEN\n"
                + "         CONCAT(P.Descripcion, ' ', Marca, ' ', P.Presentacion, ' ', \n"
                + "         '#Ref. ', VD.NumeroTelefonico, '  #Trans. ', VD.FolioTransaccion) \n"
                + "     ELSE\n"
                + "         CONCAT(P.Descripcion, ' ', Marca, ' ', P.Presentacion, ' ', \n"
                + "         CASE WHEN EsCaja = 1 THEN CONCAT('C/',VD.Uom ) ELSE 'PZA' END)\n"
                + "     END\n"
                + "     Producto,\n"
                + "     VD.Cantidad AS [#], (VD.Precio) Precio, \n"
                + "     VD.MontoDescuento Descuento,  \n"
                + "     ROUND((VD.Cantidad * (VD.Precio) - VD.MontoDescuento), 2) Total\n"
                + " FROM PVVentasDetalle VD LEFT JOIN PVProductos P ON VD.IdProducto = P.Id \n"
                + " LEFT JOIN PVPresentacionesVentaProd PVP ON VD.IdPresentacionProducto = PVP.Id \n"
                + " WHERE VD.FolioVenta = '" + FolioVentaCombo + "' ";

            DataTable dtDetalle = sqlLoc.selec(queryDetalle);
            dgvDetalleProducto.DataSource = dtDetalle;

            dgvDetalleProducto.Columns["#"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvDetalleProducto.Columns["Precio"].DefaultCellStyle.Format = "C2";
            dgvDetalleProducto.Columns["Precio"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvDetalleProducto.Columns["Descuento"].DefaultCellStyle.Format = "C2";
            dgvDetalleProducto.Columns["Descuento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvDetalleProducto.Columns["Total"].DefaultCellStyle.Format = "C2";
            dgvDetalleProducto.Columns["Total"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void cbFolioVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void CargarDetalleDeuda(int idCliente)
        {
            string queryFoliosDeuda = "SELECT a.FolioVenta FROM PVVentaPago a, PVVentas b, PVClientes c \n" +
                "WHERE a.FolioVenta = b.FolioVenta AND c.id = b.IdCliente AND b.Pagado = 0 \n" +
                "AND a.MontoCredito <> 0AND b.IdCliente = "+idCliente+" ORDER BY a.FechaAlta";
            
            DataTable dtFoliosDeuda = sqlLoc.selec(queryFoliosDeuda);
            cbFolioVenta.SelectedIndexChanged -= cbFolioVenta_SelectedIndexChanged;
            sqlLoc.llenaCombo(cbFolioVenta, dtFoliosDeuda, "FolioVenta", "");
            cbFolioVenta.SelectedIndex = -1;
            cbFolioVenta.SelectedIndexChanged += cbFolioVenta_SelectedIndexChanged;

            cbFolioVenta.SelectedIndex = 0;
        }
    }
}
