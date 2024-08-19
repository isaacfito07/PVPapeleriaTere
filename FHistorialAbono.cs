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
    public partial class FHistorialAbono : Form
    {
        ConSQL sql, sqlLoc;
        int idCliente = 0;

        public FHistorialAbono(ConSQL _sql, ConSQL _sqlLoc, DataTable _dtAbonos, string NombreCliente, int _idCliente)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;

            // Cargar los abonos
            CargarAbonos(_dtAbonos);

            // Load
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            dgvHistorialAbono.Focus();
            this.dgvHistorialAbono.RowHeadersVisible = false;
            this.dgvHistorialAbono.DefaultCellStyle.Font = new Font(dgvHistorialAbono.DefaultCellStyle.Font, FontStyle.Regular);

            //Nombre Cliente
            lbNombreCliente.Text = NombreCliente;
            idCliente = _idCliente;
        }

        private void dgvHistorialAbono_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            FDetalleDeuda fDetalleDeuda = new FDetalleDeuda(sql, sqlLoc, idCliente);
            fDetalleDeuda.Show();
        }

        private void CargarAbonos(DataTable dtAbono)
        {
            dgvHistorialAbono.DataSource = dtAbono;

            dgvHistorialAbono.Columns["Id"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvHistorialAbono.Columns["Abono"].DefaultCellStyle.Format = "C2";
            dgvHistorialAbono.Columns["Abono"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            dgvHistorialAbono.Columns["Fecha"].DefaultCellStyle.Format = "dd-MM-yyyy";
            dgvHistorialAbono.Columns["Fecha"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
    }
}
