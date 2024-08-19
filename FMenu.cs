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
    public partial class FMenu : Form
    {
        ConSQL sql, sqlLoc;
        string nombre, idSucursal, sucursal, idUsuario, numCaja;
        DataTable dtProductos, dtSubcategorias;

        public FMenu(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, string _numCaja)
        {
            InitializeComponent();

            sql = _sql;
            sqlLoc = _sqlLoc;
            nombre = _nombre;
            sucursal = _sucursal;
            idSucursal = _idSucursal;
            idUsuario = _idUsuario;
            numCaja = _numCaja;

            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja.Text = "CAJA: " + numCaja;

            this.KeyPreview = true;
        }

        private void FMenu_Load(object sender, EventArgs e)
        {
            string fecha = DateTime.Now.ToString("yyyy-MM-dd");
            //Consultar si se ha echo un corte final en el día
            //var existeCorte = sqlLoc.scalar("SELECT FolioCorte FROM PVCOrteCaja \n" +
            //    "WHERE CONVERT(DATE,FechaCorte) = '" + fecha + "'");

 
            //if (existeCorte != null && existeCorte != System.DBNull.Value)
            //{
            //    //btnNuevaVenta.Enabled = false;
            //    //btnHistorial.Enabled = false;
            //    //btnDevoluciones.Enabled = false;
            //    //btnCorteCaja.Enabled = false;
              
            //}
            //else
            //{
              
            //}
            //Consultar cortes parciales de caja en el día

            string queryCP = "SELECT COUNT(Id) FROM PVCorteCaja WHERE Valido=1 AND CONVERT(Date,FechaCorte) = '" + fecha + "'";
            var cant = sqlLoc.scalar(queryCP);
            int cantCP = 0;
            if (cant != null)
            {
                cantCP = (int)cant;
            }

            //Consulta si ya hay un corte final en el dia actual
            string queryCF = "SELECT COUNT(Id) FROM PVCorteCaja WHERE Valido=1 AND CorteFinal=1 AND IdSucursal=" + idSucursal + " AND CONVERT(DATE, FechaCorte)='" + fecha + "'";
            var cant_cf = sqlLoc.scalar(queryCF);
            int cantCF = 0;
            if (cant_cf != null)
            {
                cantCF = (int)cant_cf;
            }

           //// //cantCF = 0;
           //// btnNuevaVenta.Enabled = cantCF ==0 ? true : false;
           //// btnHistorial.Enabled = cantCF == 0 ? true : false;
           //// btnDevoluciones.Enabled = cantCF == 0 ? true : false;
           //// btnCorteCaja.Enabled =cantCF == 0 ? true : false;
           //// btnArqueo.Enabled = cantCF == 0 ? true : false;
           ////// btnArqueo.Visible = false;

            //Cargar lista de productos
            string queryProductos = " SELECT DISTINCT P.Id, \n"
           + " CONCAT(ISNULL(P2.Descripcion,P.Descripcion), ' ', P.Marca, ' ', \n"
           + " P.Presentacion) Producto, \n"
           + " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n"
           + " Pres.CodigoBarras, Pres.Uom,\n"
           + " ROUND((ISNULL(Pres.Precio, 0)),2) Precio, P.IVA, P.IEPS, P.Foto, \n"
           + " (CASE WHEN Pres.Uom > 1 THEN 1 ELSE 0 END) EsCaja, ISNULL(Pres.Id, 0) IdPresentacionVenta, \n"
           + " P.IdMarca, P.IdLinea, (Pres.Precio + ((Pres.Precio * P.Iva) + (Pres.Precio * P.Ieps))) PrecioFinal, Pres.sku, P.Pesaje \n"
           + " FROM PVProductos P \n"
           + " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id \n"
           + " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n";

            //Nueva Version
            queryProductos = " SELECT DISTINCT P.Id, \n"
          + " CONCAT(ISNULL(P2.Descripcion,P.Descripcion), ' ', P.Marca, ' ', \n"
          + " P.Presentacion) Producto, \n"
          + " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n"
          + " Pres.CodigoBarras, Pres.Uom,\n"
          + " ROUND((ISNULL(Pres.Precio, 0)),2) Precio, P.IVA, P.IEPS, P.Foto, \n"
          + " (CASE WHEN Pres.Uom > 1 THEN 1 ELSE 0 END) EsCaja, ISNULL(Pres.Id, 0) IdPresentacionVenta, \n"
          + " P.IdMarca, P.IdLinea, (Pres.Precio + ((Pres.Precio * P.Iva) + (Pres.Precio * P.Ieps))) PrecioFinal, \n"
          + " (PVP.General + ((PVP.General * P.Iva) + (PVP.General * P.Ieps))) AS PrecioGeneral, (PVP.Talleres + ((PVP.Talleres * P.Iva) + (PVP.Talleres * P.Ieps))) AS PrecioTalleres,  (PVP.Distribuidores + ((PVP.Distribuidores * P.Iva) + (PVP.Distribuidores * P.Ieps))) AS PrecioDistribuidores, Pres.sku, P.Pesaje \n"
          + " FROM PVProductos P \n"
          + " LEFT JOIN ( \n"
          + " SELECT DISTINCT(idproducto), Pre.idSucursal, Pre.IdPresentacionVenta, Pre.General, Pre.Talleres, Pre.Distribuidores FROM PVPrecios Pre WHERE pre.idSucursal = " + idSucursal+" \n"
          + " ) PVP ON PVP.idproducto = P.Id \n"
          + " INNER JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id AND Pres.Id = PVP.IdPresentacionVenta \n"
          + " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n";

            dtProductos = sqlLoc.selec(queryProductos);
        }

        private void AcercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AcercaDe acercade = new AcercaDe();
            acercade.ShowDialog();
        }

        private void BtnNuevaVenta_Click(object sender, EventArgs e)
        {
            //Verificar fondo caja
            var fCaja = sqlLoc.scalar(" SELECT TOP(1) Id FROM PVFondoCaja "
                + "WHERE IdSucursal = '" + idSucursal + "'\n"
                + " AND IdUsuario='" + idUsuario + "' AND FolioCorteParcialCaja IS NULL "
                + " ORDER BY Id DESC"); //AND FolioCorteCaja IS NULL

            if (fCaja != null)
            {
                FVenta venta = new FVenta(sql, sqlLoc, nombre, idSucursal, sucursal,
                    idUsuario, dtProductos, imgLstCategorias, imgLstProductos,
                    "0", false, numCaja);
                venta.ShowDialog();
            }
            else
            {
                FCajaFondo fondoCaja = new FCajaFondo(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, numCaja);
                this.Hide();
                fondoCaja.ShowDialog();
            }

          
        }

        private void BtnHistorial_Click(object sender, EventArgs e)
        {
            FHistorial historial = new FHistorial(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, dtProductos, dtSubcategorias, imgLstCategorias, imgLstProductos, numCaja);
            historial.ShowDialog();
        }

        private void btnDevoluciones_Click(object sender, EventArgs e)
        {
            FDevoluciones fDevoluciones = new FDevoluciones(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, dtProductos, dtSubcategorias, imgLstCategorias, imgLstProductos, numCaja);
            fDevoluciones.ShowDialog();
        }

        private void btnRecibirProd_Click(object sender, EventArgs e)
        {
            FRecibirProductos fRecibirProd = new FRecibirProductos(sql, sqlLoc);
            fRecibirProd.ShowDialog();
        }

        private void btnArqueo_Click(object sender, EventArgs e)
        {
            FPassFondo passFondo = new FPassFondo();
            passFondo.ShowDialog();

            string usuario = passFondo.Usuario;
            string pass = passFondo.Contrasena;


            DataTable dtUsuario =
                sqlLoc.selec
                (
                    "SELECT * FROM PVUsuarios WHERE usuario = '" + usuario.Trim() + "'"
                );

            if (dtUsuario.Rows.Count > 0)
            {
                DataRow r = dtUsuario.Rows[0];

                if (pass.Trim() == r["Contrasena"].ToString().Trim())
                {
                    if (r["Arqueo"] != System.DBNull.Value)
                    {
                        if (Convert.ToBoolean(r["Arqueo"]))
                        {
                            Arqueo arqueo = new Arqueo(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
                            //this.Close();
                            arqueo.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("No tiene privilegios para realizar esta acción.", "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else {
                        MessageBox.Show("No tiene privilegios para realizar esta acción.", "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else {
                    MessageBox.Show("Verificar usuario y/o contraseña", "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Verificar usuario y/o contraseña", "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    btnDevoluciones_Click(sender, e);
                    break;
                case Keys.F9:
                    BtnCorteCaja_Click(sender, e);
                    break;
            }
        }

        private void btnCreditos_Click(object sender, EventArgs e)
        {
            FCredito fCredito = new FCredito(sql,sqlLoc,idUsuario,idSucursal);
            fCredito.ShowDialog();
        }

        private void configuraciónPuertosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FConfiguracionBascula ConfiguracionBascula = new FConfiguracionBascula(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
            ConfiguracionBascula.ShowDialog();
        }

        private void BtnCorteCaja_Click(object sender, EventArgs e)
        {
            FMenuCorte menuCorte = new FMenuCorte(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
            this.Close();
            menuCorte.ShowDialog();
        }

        private void ConsultasBDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FPassFondo passFondo = new FPassFondo();
            passFondo.ShowDialog();

            string usuario = passFondo.Usuario;
            string pass = passFondo.Contrasena;

            if (usuario == "LEDSCO" && pass == "OCSDEL123")
            {
                FConsultas consultas = new FConsultas(sqlLoc);
                consultas.ShowDialog();
            }
            else
            {
                MessageBox.Show("Verificar usuario y/o contraseña", "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EstadoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void BtnDisparo_Click(object sender, EventArgs e)
        {
            FDisparoNube disparoNube = new FDisparoNube(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, numCaja);
            disparoNube.ShowDialog();
        }

        private void FMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //ALE
            //if (MessageBox.Show("¿Quieres salir del programa?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            //{
            //    e.Cancel = true;
            //}
        }

        private void FMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ALE
            //  Application.Exit();
        }
    }
}
