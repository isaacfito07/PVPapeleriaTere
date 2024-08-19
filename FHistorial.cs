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
    public partial class FHistorial : Form
    {
        ConSQL sql, sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;

        DataTable dtProductos, dtSubcategorias;

        //indices de las columnas
        static int indFolio = 0, indFolioTicket = 1, indCliente = 2, indProductos = 3, indTotal = 4, indFecha = 5,
            indUsuario = 6, indTerminada = 7, indNube = 8, indReimpresion = 10, indImpresionCorte=6, indTipoCorte=3;

        private void pbxLogo_Click(object sender, EventArgs e)
        {

        }

        //permiso para cancelar
        bool permisoCancelar = true;

        public FHistorial(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario,
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

            dtProductos = _dtProductos;
            dtSubcategorias = _dtSubcategorias;

            imgLstProductos = _imgLstProductos;
            imgLstCategorias = _imgLstCategorias;

            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja.Text = "CAJA: " + numCaja;

            permisoCancelar = (bool)sqlLoc.scalar("SELECT ISNULL(CancelarVenta, 0) " +
                "FROM PVUsuarios WHERE Id = " + idUsuario + "");
        }

        private void fHistorial_Load(object sender, EventArgs e)
        {
            cbxTipo.Text = "Ventas";
            CargarVentas(true);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (cbxTipo.Text == "Ventas")
            {
                CargarVentas(true);
            }
            else if (cbxTipo.Text == "Retiros") {
                CargaRetiros(true);
            }
            else if (cbxTipo.Text == "Cortes") {
                CargaCortes(true);
            }
        }

        private void CargarVentas(bool fechas = true)
        {
            dvgHistoria.DataSource = null;
            //Consultar ventas en el rango de fechas especificado
           string query = "SELECT V.FolioVenta, V.FolioTicket Folio, ISNULL(C.Nombre,V.NombreCliente) Cliente, VD.Cantidad Productos, V.TotalVenta, \n" +
                "V.FechaVenta Fecha, U.Nombres Cajero, V.Terminada, CASE WHEN V.Valido =0 THEN 'Cancelada' ELSE '' END Estatus, V.DisparadoNube Nube, \n" +
                "'Reimprimir' Reimprimir \n" +
                "FROM PVVentas V LEFT JOIN PVUsuarios U ON V.IdUsuarioVenta = U.Id \n" +
                "LEFT JOIN PVClientes C ON C.Id = V.Idcliente \n" +
                "LEFT JOIN ( \n" +
                "   SELECT DISTINCT VD.FolioVenta, SUM(VD.Cantidad) Cantidad \n" +
                "   FROM PVVentasDetalle VD GROUP BY VD.FolioVenta \n" +
                ") VD ON V.FolioVenta = VD.FolioVenta ";//\n" +
                ;//"WHERE V.Valido = 1 "
            if (fechas)
            {
                query += " WHERE (CAST(V.FechaVenta AS DATE) \n" +
                    "BETWEEN '" + dtpDe.Value.ToString("yyyy-MM-dd") + "' AND '" + dtpA.Value.ToString("yyyy-MM-dd") + "' ) \n";
            }
            else{
                query += " WHERE V.FolioTicket like '%" + txtFolio.Text.Trim() + "%'";
            }

            query += " AND V.IdSucursal = " + idSucursal + " ORDER BY V.FechaVenta ASC";

            DataTable dtHistoria = sqlLoc.selec(query);

            dvgHistoria.DataSource = dtHistoria;

            foreach (DataGridViewColumn col in dvgHistoria.Columns)
            {
                col.ReadOnly = true;

                if (col.Index == indFolio)
                {
                    col.Visible = false;
                }
                //Alinear total a la derecha
                if (col.Index == indTotal)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            if (dtHistoria.Rows.Count > 0)
            {
                dvgHistoria.DataSource = dtHistoria;
                dvgHistoria.Columns["Fecha"].Width = 240;
            }
        }

        private void CargaRetiros(bool fechas = true)
        {
            dvgHistoria.DataSource = null;
            //Consultar ventas en el rango de fechas especificado
            string query = "SELECT \n"
	                       + "     R.Id IdFolio, R.FolioRetiro Folio, R.FechaRetiro Fecha, R.Concepto, R.Retiro Monto,\n"
                           + "      U.Nombres Cajero, R.DisparadoNube Nube\n"
                           + " FROM PVRetiroCaja R \n"
                           + " LEFT JOIN PVUsuarios U ON R.IdUsuario = U.Id WHERE R.Id IS NOT NULL  ";
            if (fechas)
            {
                query += " AND (CAST(R.FechaRetiro AS DATE) \n" +
                    "BETWEEN '" + dtpDe.Value.ToString("yyyy-MM-dd") + "' AND '" + dtpA.Value.ToString("yyyy-MM-dd") + "' ) \n";
            }
            else
            {
                query += " AND R.FolioRetiro like '%" + txtFolio.Text.Trim() + "%'";
            }

            query += " AND R.IdSucursal = " + idSucursal;

            DataTable dtRetiros = sqlLoc.selec(query);
            dvgHistoria.DataSource = dtRetiros;

            foreach (DataGridViewColumn col in dvgHistoria.Columns)
            {
                col.ReadOnly = true;

                if (col.Index == indFolio)
                {
                    col.Visible = false;
                }
                //Alinear total a la derecha
                if (col.Index == indTotal)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            if (dtRetiros.Rows.Count > 0)
            {
                dvgHistoria.DataSource = dtRetiros;
                dvgHistoria.Columns["Fecha"].Width = 240;
            }
        }

        private void CargaCortes(bool fechas = true)
        {
            dvgHistoria.DataSource = null;
            //Consultar ventas en el rango de fechas especificado
            string query = "SELECT \n"
                           + "     R.Id IdFolio, R.FolioCorte Folio, R.FechaCorte Fecha, CASE WHEN R.CorteFinal=1 THEN 'Final' ELSE 'Parcial' END Corte,\n"
                           + "         U.Nombres Cajero, R.DisparadoNube Nube, 'Reimprimir' Reimprimir\n"
                           + " FROM PVCorteCaja R\n"
                           + " LEFT JOIN PVUsuarios U ON R.IdUsuarioCorte = U.Id WHERE R.Id IS NOT NULL  ";
            if (fechas)
            {
                query += " AND (CAST(R.FechaCorte AS DATE) \n" +
                    "BETWEEN '" + dtpDe.Value.ToString("yyyy-MM-dd") + "' AND '" + dtpA.Value.ToString("yyyy-MM-dd") + "' ) \n";
            }
            else
            {
                query += " AND R.FolioCorte like '%" + txtFolio.Text.Trim() + "%' ";
            }

            query += " AND R.IdSucursal = " + idSucursal;

            DataTable dtCortes = sqlLoc.selec(query);
            dvgHistoria.DataSource = dtCortes;

            foreach (DataGridViewColumn col in dvgHistoria.Columns)
            {
                col.ReadOnly = true;

                if (col.Index == indFolio)
                {
                    col.Visible = false;
                }
                //Alinear total a la derecha
                if (col.Index == indTotal)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            if (dtCortes.Rows.Count > 0)
            {
                dvgHistoria.DataSource = dtCortes;
                dvgHistoria.Columns["Fecha"].Width = 240;
            }
        }

        private void txtFolio_TextChanged(object sender, EventArgs e)
        {
            //if (cbxTipo.Text == "Venta")
            //{
            //    CargarVentas(false);
            //}
            //else
            //{
            //    CargaRetiros(false);
            //}

            if (cbxTipo.Text == "Ventas")
            {
                CargarVentas(false);
            }
            else if (cbxTipo.Text == "Retiros")
            {
                CargaRetiros(false);
            }
            else if (cbxTipo.Text == "Cortes")
            {
                CargaCortes(false);
            }
        }

        private void dvgHistoria_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                bool ReimpresionVenta = false;
                bool ReimpresionCorteParcial = false;
                bool ReimpresionCorteFinal = false;
                bool VisualizarTicket = true;

                //click para reimpresion
                /*if (e.ColumnIndex == indReimpresion)
                {
                    ReimpresionVenta = true;
                }*/

                //click para reimpresion
                if (e.ColumnIndex == indImpresionCorte)
                {
                    if ((string)dvgHistoria.Rows[e.RowIndex].Cells[indTipoCorte].Value == "Final")
                    {
                        ReimpresionCorteFinal = true;
                    }
                    else if ((string)dvgHistoria.Rows[e.RowIndex].Cells[indTipoCorte].Value == "Parcial")
                    {
                        ReimpresionCorteParcial = true;
                    }
                }

                if (ReimpresionVenta || VisualizarTicket) {
                    FTicket ticket = new FTicket(sqlLoc, (string)dvgHistoria.Rows[e.RowIndex].Cells[indFolio].Value, ReimpresionVenta);
                    ticket.ShowDialog();

                }

                if (ReimpresionCorteParcial)
                {
                    FImprimeCorteParcial ticket = new FImprimeCorteParcial(sqlLoc, (string)dvgHistoria.Rows[e.RowIndex].Cells[indFolioTicket].Value);
                    ticket.ShowDialog();
                }

                if (ReimpresionCorteFinal)
                {
                    //(ConSQL _sqlLoc, string _folioCorte, ConSQL _sql, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, string _IdCaja)
                    FImprimeCorteII ticket = new FImprimeCorteII(sqlLoc, (string)dvgHistoria.Rows[e.RowIndex].Cells[indFolioTicket].Value, sql, nombre, idSucursal, sucursal, idUsuario, numCaja);
                    ticket.ShowDialog();
                }

            }

          
        }
    }
}
