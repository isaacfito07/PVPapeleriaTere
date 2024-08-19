using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FCredito : Form
    {
        ConSQL sql, sqlLoc;
        string idUsuario, idSucursal;
        DataTable dtCreditoCliente;
        BindingSource bindingSource = new BindingSource();

        public FCredito(ConSQL _sql, ConSQL _sqlLoc, string _idUsuario, string _idSucursal)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
            idUsuario = _idUsuario;
            idSucursal = _idSucursal;

            CargarCreditos();

            this.MaximizeBox = false; // Deshabilita el botón de maximizar
            this.MinimizeBox = false; // Deshabilita el botón de minimizar
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Fija el tamaño del borde de la ventana

            this.dvgCredito.RowHeadersVisible = false;
            this.dvgCredito.AllowUserToResizeColumns = false;
            this.dvgCredito.AllowUserToResizeRows = false;
        }

        private void dvgCredito_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        { 
            int columnaAbonar = 0;
            int columnaHistorial = 1;
            int columnaNombre = 2;
            if (e.RowIndex > -1 && e.ColumnIndex == columnaAbonar)
            {
                //Variables FIJAS
                int columnaSaldoPendiente = 4;

                string TotalVenta = (dvgCredito.Rows[e.RowIndex].Cells[columnaSaldoPendiente].Value).ToString();
                string NombreCliente = (dvgCredito.Rows[e.RowIndex].Cells[columnaNombre].Value).ToString();

                FPago fPago = new FPago(sqlLoc, sql, TotalVenta, 0.00, 0.00, idUsuario, idSucursal, false, NombreCliente, true);
                fPago.ShowDialog();

                if (fPago.Recibido == 0)
                {
                    DialogResult dr = MessageBox.Show(" Ocurrio un error al efectuar el abono", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                }
                else
                {
                    int idCliente = Convert.ToInt16(dtCreditoCliente.Rows[e.RowIndex]["idCliente"]);
                    Abonar(idCliente, fPago);
                }
            }

            if (e.RowIndex > -1 && e.ColumnIndex == columnaHistorial)
            {
                int idCliente = Convert.ToInt16(dtCreditoCliente.Rows[e.RowIndex]["idCliente"]);
                DataTable dtAbonos = BuscarAbonos(idCliente);
                string NombreCliente = (dvgCredito.Rows[e.RowIndex].Cells[columnaNombre].Value).ToString();

                FHistorialAbono fHistorialAbono = new FHistorialAbono(sql, sqlLoc, dtAbonos, NombreCliente, idCliente);
                fHistorialAbono.ShowDialog();
            }
        }

        private void dvgCredito_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.Enter)
            {
                int columnIndex = dvgCredito.CurrentCell.ColumnIndex;
                int columnaAbonar = 0;
                int columnaHistorial = 1;

                e.Handled = true; // Evitar la acción predeterminada
                e.SuppressKeyPress = true; // Suprimir el sonido de "ding"

                int RowIndex = dvgCredito.CurrentCell.RowIndex;
                MouseEventArgs mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
                DataGridViewCellMouseEventArgs ex = new DataGridViewCellMouseEventArgs(columnIndex, RowIndex, 0, 0, mouseEventArgs);

                if (columnIndex == columnaAbonar)
                {
                    dvgCredito_CellMouseDoubleClick(sender, ex);
                }

                if (columnIndex == columnaHistorial)
                {
                    dvgCredito_CellMouseDoubleClick(sender, ex);
                }
            }
        }

        private void CargarCreditos()
        {
            string queryCredito = "SELECT 'Ver' AS Historial, [Nombre del Cliente], idCliente AS idCliente, SUM([Saldo Abonado]) AS [Saldo Abonado], SUM([Saldo Pendiente]) AS [Saldo Pendiente] \n" +
                "FROM (SELECT c.Nombre AS [Nombre del Cliente], c.id as idCliente, \n " +
                "       (SELECT SUM(MontoRecibido) FROM PVVentaPago WHERE FolioVenta LIKE a.FolioVenta) AS [Saldo Abonado], \n " +
                "       ((a.MontoCredito) - (SELECT SUM(MontoRecibido) FROM PVVentaPago WHERE FolioVenta LIKE a.FolioVenta)) AS [Saldo Pendiente] \n " +
                "       FROM PVVentaPago a, PVVentas b, PVClientes c \n " +
                "       WHERE a.FolioVenta = b.FolioVenta AND c.id = b.IdCliente AND b.Pagado = 0 AND a.MontoCredito <> 0) AS TablaMasiva \n " +
                "GROUP BY idCliente, [Nombre del Cliente]";

            dtCreditoCliente = sqlLoc.selec(queryCredito);

            queryCredito = queryCredito.Replace("idCliente AS idCliente,", "");
            DataTable dtCreditos = sqlLoc.selec(queryCredito);

            bindingSource.DataSource = dtCreditos;
            dvgCredito.DataSource = dtCreditos;
            foreach (DataGridViewColumn col in dvgCredito.Columns)
            {
                col.ReadOnly = true;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dvgCredito.Rows.Count > 0)
            {
                dvgCredito.ColumnHeadersDefaultCellStyle.Font = new Font(dvgCredito.Font, FontStyle.Bold);

                dvgCredito.Columns["Historial"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dvgCredito.Columns["Historial"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dvgCredito.Columns["Historial"].DefaultCellStyle.BackColor = Color.DarkSlateGray;
                dvgCredito.Columns["Historial"].DefaultCellStyle.ForeColor = Color.White;

                dvgCredito.Columns["Nombre del Cliente"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                dvgCredito.Columns["Saldo Abonado"].DefaultCellStyle.Format = "C2";

                dvgCredito.Columns["Saldo Pendiente"].DefaultCellStyle.Format = "C2";

                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.Name = "Abonar";
                buttonColumn.HeaderText = "Abono";
                buttonColumn.Text = "Abonar";
                buttonColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                buttonColumn.UseColumnTextForButtonValue = true; // Muestra el texto en todos los botones
                if (!ExisteColumna(buttonColumn.Name,dvgCredito))
                {
                    dvgCredito.Columns.Add(buttonColumn);
                    dvgCredito.Columns[buttonColumn.Name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dvgCredito.Columns[buttonColumn.Name].DefaultCellStyle.BackColor = Color.LimeGreen;
                    dvgCredito.Columns[buttonColumn.Name].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
        }

        private void Abonar(int idCliente, FPago pago)
        {
            Dictionary<string, object> SPAbonar = new Dictionary<string, object>();
            SPAbonar.Add("NombreSP", "PVCanatlan_AbonarCreditos");
            SPAbonar.Add("@idCliente", idCliente);
            SPAbonar.Add("@MontoAbono", pago.Recibido);
            SPAbonar.Add("@Efectivo", pago.MontoEfectivo);
            SPAbonar.Add("@TarjetaDebito", pago.MontoTarjeta);
            SPAbonar.Add("@TarjetaCredito", pago.MontoTarjetaCredito);
            SPAbonar.Add("@Transferencia", pago.MontoTransferencia);
            SPAbonar.Add("@Vales", pago.MontoVales);
            SPAbonar.Add("@Cheques", pago.MontoCheques);
            SPAbonar.Add("@IdUsuario", idUsuario);
            SPAbonar.Add("@FolioTransferencia", (pago.FolioTransferencia == null ? string.Empty : pago.FolioTransferencia));
            SPAbonar.Add("@FolioCheque", (pago.FolioCheques == null ? string.Empty : pago.FolioCheques));
            SPAbonar.Add("@idAbono.OUTPUT", 0);

            Dictionary<string, object> VariablesOutput = sqlLoc.sp(SPAbonar);

            int idAbono = Convert.ToInt32(VariablesOutput["@idAbono"]);

            DialogResult dr = MessageBox.Show("¿Desea imprimir comprobante de pago?", "Ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.Yes)
            {
                FTicketAbono ticket = new FTicketAbono(sqlLoc, idAbono, true);
                ticket.ShowDialog();
            }

            CargarCreditos();
        }

        private void FCredito_Load(object sender, EventArgs e)
        {

        }

        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            // Obtener el texto de búsqueda
            string searchValue = txtCliente.Text.Trim();

            // Aplicar el filtro al BindingSource
            if (string.IsNullOrEmpty(searchValue))
            {
                bindingSource.RemoveFilter();
            }
            else
            {
                // Filtrar por la columna "Nombre"
                bindingSource.Filter = $"[Nombre del Cliente] LIKE '%{searchValue}%'";
            }
        }

        private DataTable BuscarAbonos(int idCliente)
        {
            string queryAbono = "SELECT a.id as Id, a.MontoRecibido AS Abono, a.FechaAlta AS Fecha, (CONCAT(\n " +
                "CASE WHEN a.MontoEfectivo <> 0 THEN CONCAT('-Efectivo (',FORMAT(a.MontoEfectivo, 'C2', 'en-US'),') ') ELSE '' END, \n" +
                "CASE WHEN a.MontoTarjeta <> 0 THEN CONCAT('-Tarjeta de Debito (',FORMAT(a.MontoTarjeta, 'C2', 'en-US'),') ') ELSE '' END, \n" +
                "CASE WHEN a.MontoTarjetaCredito <> 0 THEN CONCAT('-Tarjeta de Crédito (',FORMAT(a.MontoTarjetaCredito, 'C2', 'en-US'),') ') ELSE '' END, \n" +
                "CASE WHEN a.MontoTransferencia <> 0 THEN CONCAT('-Transferencia (',FORMAT(a.MontoTransferencia, 'C2', 'en-US'),') ') ELSE '' END, \n" +
                "CASE WHEN a.MontoCheque <> 0 THEN CONCAT('-Cheque (',FORMAT(a.MontoCheque, 'C2', 'en-US'),') ') ELSE '' END,  \n" +
                "CASE WHEN a.MontoVales <> 0 THEN CONCAT('-Vales (',FORMAT(a.MontoVales, 'C2', 'en-US'),') ') ELSE '' END)) AS [Metodo de Pago] \n" +
                "FROM PVVentaPago a, PVVentas b WHERE MontoCredito = 0 AND MontoRecibido <> 0 AND a.FolioVenta = b.FolioVenta AND b.Pagado = 0 AND b.IdCliente = " + idCliente+ " ORDER BY a.FolioVenta, a.FechaAlta";

            return sqlLoc.selec(queryAbono);
        }

        private bool ExisteColumna(string nombreColumna, DataGridView dgv)
        {
            foreach (DataGridViewColumn columna in dgv.Columns)
            {
                if (columna.Name == nombreColumna)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
