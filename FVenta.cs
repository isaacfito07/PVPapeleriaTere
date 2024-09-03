using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using PVLaJoya.ProntiPagos; //< --Desarrollo
//using PVLaJoya.mx.prontipagos.ws; //<-- Producción
using System.Threading;
using System.Transactions;
using System.Net.NetworkInformation;
using System.Diagnostics.Tracing;
using NLog.Time;
using System.Data.Common;
using System.Runtime.InteropServices;
using System.IO.Ports;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Diagnostics;

namespace PVLaJoya
{
    public partial class FVenta : Form
    {
        ConSQL sql, sqlLoc;
        string nombre, sucursal, idSucursal, idUsuario, numCaja, folioVenta, folioticket;
        int IdVenta, IdCliente = 0, idTipoCliente = 1;
        DataTable dtProductos;
        bool modoEdicion = false, controlClienteExistente = true;
        bool cancelacion = false, pagoCancelado = false;

        //permiso para cancelar
        bool permisoCancelar = false;
        bool permisoDescuentos = false;
        bool SumarRestarPorTeclado = false;

        //permiso para modficacion de precios
        bool LoginSupervisorPrecios = false;
        bool modificacionPorCliente = false;


        private SerialPort serialPort;
        bool Pesaje = false;

        private string pesoLeido = string.Empty;
        private bool datosRecibidos = false;
        private readonly object datosRecibidosLock = new object();

        //indices de las columnas del grid
        static int
            indIdProd = 0,
            indProducto = 1,
            indResta = 2,
            indQty = 3,
            indSum = 4,
            indPrecio = 5,
            indPrecioInicial = 6,
            indTotal = 7,
            indIVA = 8,
            indIEPS = 9,
            indEsCaja = 10,
            indUom = 11,
            indIdPres = 12,
            indIdMarca = 13,
            indIdLinea = 14,
            indFechaAgregado = 15,
            indSku = 16,
            indMontoComision = 17,
            indOrden = 18;
        

        private FEspere Espere = new FEspere();
        private ToolTip toolTip;


        public FVenta(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, 
            string _sucursal, string _idUsuario, DataTable _dtProductos,
            ImageList _imgLstCategorias, ImageList _imgLstProductos, string _folioVenta, 
            bool _cancelacion, string _numCaja)
        {
            InitializeComponent();
           

            sql = _sql;
            sqlLoc = _sqlLoc;
            nombre = _nombre;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuario = _idUsuario;
            numCaja = _numCaja;

            InitializeSerialPort();

            dtProductos = _dtProductos;

            imgLstProductos = _imgLstProductos;
            imgLstCategorias = _imgLstCategorias;

            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja.Text = "CAJA: " + numCaja;

            //Verificar permiso del usuario para cancelar ventas
            string query = "SELECT ISNULL(CancelarVenta, 0) FROM PVUsuarios \n" +
                "WHERE Id = '" + idUsuario + "'";
            permisoCancelar = (bool)sqlLoc.scalar(query);
            if (!permisoCancelar)
            {
                btnCancelar.Enabled = false;
            }

            cancelacion = _cancelacion;

            //CREAR FOLIO DE VENTA
            if (string.IsNullOrEmpty(_folioVenta) || _folioVenta == "0")
            {
                nuevaVenta(); //Nueva venta
            }
            else
            {
                //IdVenta = Convert.ToInt32(_IdVenta); //continuar existente
                folioVenta = _folioVenta;
                modoEdicion = true;

                string queryEdicion = "SELECT VD.IdProducto, CONCAT(P.Descripcion, ' ', P.Marca, ' ', \n"
                    + "P.Presentacion, ' ', \n"
                    + "(CASE WHEN EsCaja = 1 THEN CONCAT('C/',VD.Uom ) ELSE 'PZA' END)) Producto, \n"
                    + "VD.Cantidad, VD.Precio + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps) Precio, \n"
                    + "ROUND(VD.Cantidad * VD.Precio + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps), 2) Total, \n"
                    + "ROW_NUMBER() OVER(ORDER BY VD.Id DESC) IdOrden, P.IVA, \n"
                    + "P.IEPS, VD.EsCaja,VD.Uom, VD.IdPresentacionProducto, VD.IdMarca, VD.IdLinea, ISNULL(OP.Orden,0) Orden \n"
                    + " FROM PVVentasDetalle VD \n"
                    + "   INNER JOIN PVVentas V ON V.FolioVenta = VD.FolioVenta\n"
                    + "   INNER JOIN PVProductos P ON P.Id = VD.IdProducto \n"
                    + "   LEFT JOIN (\n"
                    + "     SELECT * FROM\n"
                    + "     (\n"
                    + "         SELECT FolioVenta, IdProducto, Orden, ROW_NUMBER() OVER(PARTITION BY FolioVenta, IdProducto ORDER BY Id DESC) rn FROM PVOrdenProductos\n"
                    + "     ) R WHERE rn=1\n"
                    + " ) OP ON VD.FolioVenta = OP.FolioVenta AND VD.IdProducto = OP.IdProducto \n"
                    + " WHERE VD.FolioVenta = '" + folioVenta + "'";

                //llenar grid
                DataTable dtVenta = sqlLoc.selec(queryEdicion);
                //Buscar folioticket
                var folioT = sqlLoc.scalar("SELECT FolioTicket FROM PVVentas \n" +
                    "WHERE FolioVenta = '" + folioVenta + "'");

                lblFolio.Text = "Folio: " + folioT != null ? folioT.ToString() : "-";
                foreach (DataRow r in dtVenta.Rows)
                {
                    dgvVenta.Rows.Add(r["IdProducto"],
                        r["Producto"].ToString().Trim(), "-",
                        r["Cantidad"], "+",
                        Convert.ToDouble(r["Precio"]).ToString("C2"),
                        PrecioInicial(r["IVA"].ToString().Trim(), r["IEPS"].ToString().Trim(), Convert.ToDouble(r["Precio"])).ToString("C2"),
                        Convert.ToDouble(r["Total"]).ToString("C2"), //(dgvVenta.Rows.Count + 1),
                        r["IVA"].ToString().Trim(), r["IEPS"].ToString().Trim(),
                        r["EsCaja"].ToString().Trim(), r["Uom"].ToString(),
                        r["IdPresentacionProducto"].ToString(),r["IdMarca"].ToString(),
                        r["IdLinea"].ToString(), DateTime.Now.ToString("yyyy-MM-dd"), "",0, r["Orden"].ToString());


                    //dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", 1, "+", PrecioFinal, Precio, PrecioFinal,
                    //        //(dgvVenta.Rows.Count + 1),
                    //        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);
                }

                //sumar y ordenar
                sumarTotal();
            }
            // Ventas pendientes
            VentasPendientes();
            gvPendientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gvPendientes.MultiSelect = false;
            gvPendientes.DefaultCellStyle.SelectionBackColor = gvPendientes.DefaultCellStyle.BackColor;
            gvPendientes.DefaultCellStyle.SelectionForeColor = gvPendientes.DefaultCellStyle.ForeColor;

            //GridVenta
            dgvVenta.DefaultCellStyle.SelectionBackColor = dgvVenta.DefaultCellStyle.BackColor;
            dgvVenta.DefaultCellStyle.SelectionForeColor = dgvVenta.DefaultCellStyle.ForeColor;

            // Se insertan los datos de clientes en el combo
            CBCliente.SelectedIndexChanged -= CBCliente_SelectedIndexChanged; //Desactivar el evento de manera temporal para evitar ejecutarse al momento se insetar el datasource
            string queryClientes = "SELECT Id, Nombre FROM PVClientes";
            sqlLoc.llenaCombo(CBCliente, sqlLoc.selec(queryClientes), "Id", "Nombre");
            CBCliente.SelectedIndexChanged += CBCliente_SelectedIndexChanged;
            CBCliente.SelectedIndex = -1;

            //Set ToolTip
            toolTip = new ToolTip();
            toolTip.IsBalloon = true;
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = "IMPORTANTE";

            btnBorrarTxtCliente.Visible = false;
        }




        private void ReabrirFolio(string _folioVenta) {
            //IdVenta = Convert.ToInt32(_IdVenta); //continuar existente
            folioVenta = _folioVenta;
            modoEdicion = true;

            //int i = sqlLoc.exec(" UPDATE PVVentas SET EnEspera=0 WHERE IdSucursal=" + idSucursal + " AND FolioVenta = '" + folioVenta + "' ");

            //if (i != 0)
            //{

                string queryEdicion = "SELECT VD.IdProducto, CASE WHEN PVP.sku <> '' OR PVP.sku IS NOT NULL THEN \n"
                                + " 	CONCAT(P.Descripcion, ' ', P.Marca, ' ', \n"
                                + " 	P.Presentacion, ' ', '#', VD.NumeroTelefonico)\n"
                                + " ELSE\n"
                                + " 	CONCAT(P.Descripcion, ' ', P.Marca, ' ', \n"
                                + " 	P.Presentacion, ' ', \n"
                                + "     (CASE WHEN EsCaja = 1 THEN CONCAT('C/',VD.Uom ) ELSE 'PZA' END)) END Producto, \n"
                    + "VD.Cantidad, VD.Precio + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps) Precio, \n"
                    + "ROUND((VD.Cantidad * VD.Precio) + ((VD.Cantidad * VD.Precio) * VD.Iva) + ((VD.Cantidad * VD.Precio) * VD.Ieps), 2) Total, \n"
                    //+ "ROUND(VD.Cantidad * VD.Precio + (VD.Precio * VD.Iva) + (VD.Precio * VD.Ieps), 2) Total, \n"
                    + " ROW_NUMBER() OVER(ORDER BY VD.Id DESC) IdOrden, P.IVA, \n"
                    + "P.IEPS, VD.EsCaja,VD.Uom, VD.IdPresentacionProducto, VD.IdMarca, VD.IdLinea, PVP.sku, ISNULL(VD.MontoComision,0) MontoComision, ISNULL(OP.Orden,0) Orden, v.IdCliente, v.NombreCliente \n"
                    + " FROM PVVentasDetalle VD \n"
                    + "   INNER JOIN PVVentas V ON V.FolioVenta = VD.FolioVenta\n"
                    + "   INNER JOIN PVProductos P ON P.Id = VD.IdProducto \n"
                    + "   LEFT JOIN PVPresentacionesVentaProd PVP ON VD.IdPresentacionProducto = PVP.Id \n"
                    + "   LEFT JOIN (\n"
                    + "     SELECT * FROM\n"
                    + "     (\n"
                    + "         SELECT FolioVenta, IdProducto, Orden, ROW_NUMBER() OVER(PARTITION BY FolioVenta, IdProducto ORDER BY Id DESC) rn FROM PVOrdenProductos\n"
                    + "     ) R WHERE rn=1\n"
                    + " ) OP ON VD.FolioVenta = OP.FolioVenta AND VD.IdProducto = OP.IdProducto \n"
                    + " WHERE VD.FolioVenta = '" + folioVenta + "'";

                //llenar grid
                DataTable dtVenta = sqlLoc.selec(queryEdicion);
                //Buscar folioticket
                var folioT = sqlLoc.scalar("SELECT FolioTicket FROM PVVentas \n" +
                    "WHERE FolioVenta = '" + folioVenta + "'");

                lblFolio.Text = folioT != null ? folioT.ToString() : "-";
            lblFolio.Text = "Folio: " + lblFolio.Text;

            dgvVenta.Rows.Clear();

            //if (dgvVenta.Rows.Count > 0) {
            //        foreach (DataGridViewRow dgvr in dgvVenta.Rows)
            //        {
            //            dgvVenta.Rows.Remove(dgvr);
            //        }
            //    }
            //dgvVenta.DataSource = null;

            var idClienteVenta = dtVenta.Rows[0]["idCliente"];
            int idClienteVentaInt = Convert.ToInt32(idClienteVenta);

            if (idClienteVentaInt == 0)
            {
                EventArgs e = new EventArgs();
                btnNoExisteCliente_Click(this, e);

                var NombreClienteVenta = dtVenta.Rows[0]["NombreCliente"];
                txtNombreCliente.Text = NombreClienteVenta.ToString();
                txtNombreCliente.Enabled = false;
                btnBorrarTxtCliente.Visible = true;
                if (NombreClienteVenta.ToString() == string.Empty)
                {
                    txtNombreCliente.Enabled = true;
                    btnBorrarTxtCliente.Visible = false;
                }
            }
            else
            {
                EventArgs e = new EventArgs();
                btnYaExisteCliente_Click(this, e);

                CBCliente.SelectedValue = idClienteVentaInt;
            }

            foreach (DataRow r in dtVenta.Rows)
                {
                int noOrden = 0;
                int.TryParse(r["Orden"].ToString(), out noOrden);

                    dgvVenta.Rows.Add(r["IdProducto"],
                        r["Producto"].ToString().Trim(), "-",
                        r["Cantidad"], "+",
                        Convert.ToDouble(r["Precio"]).ToString("C2"),
                        PrecioInicial(r["IVA"].ToString().Trim(), r["IEPS"].ToString().Trim(), Convert.ToDouble(r["Precio"])).ToString("C2"),
                        Convert.ToDouble(r["Total"]).ToString("C2"), //(dgvVenta.Rows.Count + 1),
                        r["IVA"].ToString().Trim(), r["IEPS"].ToString().Trim(),
                        r["EsCaja"].ToString().Trim(), r["Uom"].ToString(),
                        r["IdPresentacionProducto"].ToString(), r["IdMarca"].ToString(),
                        r["IdLinea"].ToString(), DateTime.Now.ToString("yyyy-MM-dd"), r["sku"].ToString(), r["MontoComision"].ToString(), noOrden);

                    //dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", 1, "+", PrecioFinal, Precio, PrecioFinal,
                    //        //(dgvVenta.Rows.Count + 1),
                    //        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);
                }

                //sumar y ordenar
                sumarTotal();
            //}
            //else {
            //    MessageBox.Show("Ha ocurrido un error, intenta de nuevo!", "Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //}
            
        }
        private void CargarProductos()
        {
            //string queryProductos = " SELECT DISTINCT P.Id, \n"
            //   + " CONCAT(P.Descripcion, ' ', P.Marca, ' ', \n"
            //   + " P.Presentacion) Producto, \n"
            //   + " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n"
            //   + " Pres.CodigoBarras, Pres.Uom,\n"
            //   + " ROUND((ISNULL(Pres.Precio, 0)),2) Precio, P.IVA, P.IEPS, P.Foto, \n"
            //   + " (CASE WHEN Pres.Uom > 1 THEN 1 ELSE 0 END) EsCaja, ISNULL(Pres.Id, 0) IdPresentacionVenta, \n" 
            //   + " P.IdMarca, P.IdLinea, (Pres.Precio + ((Pres.Precio * P.Iva) + (Pres.Precio * P.Ieps))) PrecioFinal \n"
            //   + " FROM PVProductos P \n"
            //   + " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id \n";
         
            
            //   string queryProductos = " SELECT DISTINCT P.Id, \n"
         //+ " CONCAT(ISNULL(P2.Descripcion,P.Descripcion), ' ', P.Marca, ' ', \n"
         //+ " P.Presentacion) Producto, \n"
         //+ " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n"
         //+ " Pres.CodigoBarras, Pres.Uom,\n"
         //+ " ROUND((ISNULL(Pres.Precio, 0)),2) Precio, P.IVA, P.IEPS, P.Foto, \n"
         //+ " (CASE WHEN Pres.Uom > 1 THEN 1 ELSE 0 END) EsCaja, ISNULL(Pres.Id, 0) IdPresentacionVenta, \n"
         //+ " P.IdMarca, P.IdLinea, (Pres.Precio + ((Pres.Precio * P.Iva) + (Pres.Precio * P.Ieps))) PrecioFinal, Pres.sku \n"
         //+ " FROM PVProductos P \n"
         //+ " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id \n"
         //+ " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n";

            //Cargar lista de productos
            string queryProductos = " SELECT DISTINCT P.Id, \n"
           + " CONCAT(ISNULL(P2.Descripcion,P.Descripcion), ' ', P.Marca, ' ', \n"
           + " P.Presentacion) Producto, \n"
           + " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n"
           + " Pres.CodigoBarras, Pres.Uom,\n"
           + " ROUND((ISNULL(Pres.Precio, 0)),2) Precio, P.IVA, P.IEPS, P.Foto, \n"
           + " (CASE WHEN Pres.Uom > 1 THEN 1 ELSE 0 END) EsCaja, ISNULL(Pres.Id, 0) IdPresentacionVenta, \n"
           + " P.IdMarca, P.IdLinea, (Pres.Precio + ((Pres.Precio * P.Iva) + (Pres.Precio * P.Ieps))) PrecioFinal, Pres.sku, P.pesaje \n"
           + " FROM PVProductos P \n"
           + " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id \n"
           + " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n";

            //Nueva version
            queryProductos = " SELECT DISTINCT P.Id, \n"
            + " CONCAT(ISNULL(P2.Descripcion,P.Descripcion), ' ', P.Marca, ' ', \n"
            + " P.Presentacion) Producto, \n"
            + " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n"
            + " Pres.CodigoBarras, Pres.Uom,\n"
            + " ROUND((ISNULL(Pres.Precio, 0)),2) Precio, P.IVA, P.IEPS, P.Foto, \n"
            + " (CASE WHEN Pres.Uom > 1 THEN 1 ELSE 0 END) EsCaja, ISNULL(Pres.Id, 0) IdPresentacionVenta, \n"
            + " P.IdMarca, P.IdLinea, (Pres.Precio + ((Pres.Precio * P.Iva) + (Pres.Precio * P.Ieps))) PrecioFinal, \n"
            + " (PVP.General + ((PVP.General * P.Iva) + (PVP.General * P.Ieps))) AS PrecioGeneral, (PVP.Talleres + ((PVP.Talleres * P.Iva) + (PVP.Talleres * P.Ieps))) AS PrecioTalleres,  (PVP.Distribuidores + ((PVP.Distribuidores * P.Iva) + (PVP.Distribuidores * P.Ieps))) AS PrecioDistribuidores, \n"
            + " ISNULL(PVP.General, 0) AS PrecioGeneralSinIva, ISNULL(PVP.Talleres, 0) AS PrecioTalleresSinIva, ISNULL(PVP.Distribuidores,0) AS PrecioDistribuidoresSinIva, \n"
            + " Pres.sku, P.Pesaje \n"
            + " FROM PVProductos P \n"
            + " LEFT JOIN ( \n"
            + " SELECT DISTINCT(idproducto), Pre.idSucursal, Pre.IdPresentacionVenta, Pre.General, Pre.Talleres, Pre.Distribuidores FROM PVPrecios Pre WHERE pre.idSucursal = " + idSucursal + " \n"
            + " ) PVP ON PVP.idproducto = P.Id \n"
            + " INNER JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id AND Pres.Id = PVP.IdPresentacionVenta \n"
            + " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n";

            dtProductos = sqlLoc.selec(queryProductos);
        }

        private void VentasPendientes()
        {
            string queryPendientes = "SELECT DISTINCT V.FolioVenta, FolioTicket Folio,  ISNULL(VD.Cantidad,0) Productos, V.FechaVenta, IIF(V.IdCliente = 0,V.NombreCliente, VC.Nombre) AS Cliente \n"
                                    +"FROM PVVentas V \n"
                                    +"LEFT JOIN(SELECT DISTINCT VD.FolioVenta, SUM(VD.Cantidad) Cantidad FROM PVVentasDetalle VD GROUP BY VD.FolioVenta) VD ON V.FolioVenta = VD.FolioVenta "
                                    +"LEFT JOIN PVClientes VC ON VC.id = V.IdCliente \n" +
                                    " WHERE V.Valido = 1 AND V.Cancelado = 0 AND V.Terminada = 0 AND V.IdSucursal = " + idSucursal + // AND V.EnEspera=1
                                    " AND V.FolioVenta != '" + folioVenta + "' ORDER BY V.FolioVenta DESC";

            DataTable dtPendientes = sqlLoc.selec(queryPendientes);
            gvPendientes.DataSource = null;

            try
            {
                if (gvPendientes.Rows.Count > 0)
                {
                    foreach (DataGridViewRow dgvr in gvPendientes.Rows)
                    {
                        gvPendientes.Rows.Remove(dgvr);
                    }
                }
            }
            catch (Exception)
            {

            }

            gvPendientes.Rows.Clear();


            foreach (DataRow r in dtPendientes.Rows)
            {
                gvPendientes.Rows.Add(r["FolioVenta"], r["Folio"], r["Cliente"], r["Productos"], r["FechaVenta"]);
            }

            foreach (DataGridViewColumn col in gvPendientes.Columns)
            {
                if (col.Index == 0)
                {
                    //col.Visible = false;
                }
            }
        }

        private void nuevaVenta()
        {
            //Verifica si hay folios sin detalle de venta, si sí, lo utiliza, si no, crea una nueva venta
            //var hayVentas = sqlLoc.scalar("  SELECT TOP(1) V.FolioVenta \n" //, FolioTicket Folio,  ISNULL(VD.Cantidad,0) Productos, V.FechaVenta
            //                             + " FROM PVVentas V \n"
            //                             + " LEFT JOIN( \n"
            //                             + "   SELECT DISTINCT VD.FolioVenta, SUM(VD.Cantidad) Cantidad \n"
            //                             + "   FROM PVVentasDetalle VD GROUP BY VD.FolioVenta \n"
            //                             + " ) VD ON V.FolioVenta = VD.FolioVenta \n"
            //                             + " WHERE V.Valido = 1 AND V.Terminada = 0 AND V.IdSucursal = "+ idSucursal +" AND ISNULL(VD.Cantidad,0) = 0\n"
            //                             + " ORDER BY V.Id DESC   ");

            var hayVentas = sqlLoc.scalar("  SELECT TOP(1) V.FolioVenta \n" //, FolioTicket Folio,  ISNULL(VD.Cantidad,0) Productos, V.FechaVenta
                                         + " FROM PVVentas V \n"
                                         + " LEFT JOIN( \n"
                                         + "   SELECT DISTINCT VD.FolioVenta, SUM(VD.Cantidad) Cantidad \n"
                                         + "   FROM PVVentasDetalle VD GROUP BY VD.FolioVenta \n"
                                         + " ) VD ON V.FolioVenta = VD.FolioVenta \n"
                                         + " LEFT JOIN(\n"
                                         + "     SELECT* FROM PVVentaPago\n"
                                         + " ) VP ON V.FolioVenta = VP.FolioVenta\n"
                                         + " WHERE V.Valido = 1 AND V.Cancelado = 0 AND V.Terminada = 0 AND V.IdSucursal = " + idSucursal + " AND ISNULL(VD.Cantidad,0) = 0 AND VP.Id IS NULL\n"
                                         + " ORDER BY V.Id DESC   ");
            


            if (hayVentas != null)
            {
                folioVenta = hayVentas.ToString();

                string inicial = "";
                if (sucursal != null)
                {
                    inicial = sucursal.Substring(0, 1);
                }
                else
                {
                    var s = sqlLoc.scalar("SELECT Nombre FROM PVSucursales S WHERE Id = " + idSucursal);
                    if (s != null)
                        inicial = s.ToString().Substring(0, 1);
                }
                folioticket = inicial + folioVenta.Split('-')[1].ToString();

                lblFolio.Text = "Folio: " + folioticket;

                //limpiar campos
                lblTotal.Text = "$-";

                lblDescProd.Text = "-";
                lblPrecioProd.Text = "-";
                pbImagen.Image = null;

                dgvVenta.Rows.Clear();
            }
            else {

                //Consultar ultimo folio
                var folioUltimaVenta = sqlLoc.scalar("SELECT TOP(1) FolioVenta \n" +
                    "FROM PVVentas WHERE IdSucursal = " + idSucursal + " \n" +
                    "ORDER BY Id DESC");
                var consecutivo = "001";
               
                //Si se encuentra folioanterior tomar ultimos digitos para consecutivo
                if (folioUltimaVenta != System.DBNull.Value)
                {
                    if (folioUltimaVenta != null)
                    {

                        var split = folioUltimaVenta.ToString().Split('-');
                        int number = Convert.ToInt32(split[1]);
                        consecutivo = (++number).ToString("D3");
                    }
                }

                string inicial = "";
                if (sucursal != null)
                {
                    inicial = sucursal.Substring(0, 1);
                }
                else
                {
                    var s = sqlLoc.scalar("SELECT Nombre FROM PVSucursales S WHERE Id = " + idSucursal);
                    if (s != null)
                        inicial = s.ToString().Substring(0, 1);
                }

                //Folio: V + idSucursal (2 digitos) + fecha/hora + consecutivo
                folioVenta = inicial + idSucursal.PadLeft(2, '0')
                    + DateTime.Now.ToString("ddMMyyTHHmmss") + "-" + consecutivo;
           
                folioticket = inicial + consecutivo;

                string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                string ins = string.Format("INSERT INTO PVVentas(FolioVenta, Folioticket, IdSucursal,\n" +
                    "IdCliente, TotalVenta,Valido,Terminada,FechaVenta,IdUsuarioVenta,DisparadoNube) \n" +
                    "OUTPUT INSERTED.Id \n" +
                    "VALUES('{0}','{1}',{2},{3},{4},{5},{6},'{7}',{8},{9})",
                    folioVenta, folioticket, idSucursal, IdCliente, 0, 1, 0, fecha, idUsuario, 0);

                //Insertar nueva venta
                IdVenta = (int)sqlLoc.scalar(ins);

                if (IdVenta > 0)
                {
                    lblFolio.Text = "Folio: " + folioticket;

                    //limpiar campos
                    lblTotal.Text = "$-";

                    lblDescProd.Text = "-";
                    lblPrecioProd.Text = "-";
                    pbImagen.Image = null;

                    dgvVenta.Rows.Clear();
                }
                else {
                    nuevaVenta();
                    MessageBox.Show("Error al crear el folio de venta, se reintentará. Intentando....!", "Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                }

            }
            lblIVA.Text = (0).ToString();
            lblIEPS.Text = (0).ToString();
        }
        //private void nuevaVenta()
        //{

        //    //Consultar ultimo folio
        //    var folioUltimaVenta = sqlLoc.scalar("SELECT MAX(FolioVenta) FolioVenta \n" +
        //        "FROM PVVentas WHERE IdSucursal = " + idSucursal + " \n" +
        //        "ORDER BY FolioVenta DESC");
        //    var consecutivo = "001";
        //    //Si se encuentra folioanterior tomar ultimos digitos para consecutivo
        //    if (folioUltimaVenta != System.DBNull.Value)
        //    {
        //        if (folioUltimaVenta != null) {

        //            var split = folioUltimaVenta.ToString().Split('-');
        //            int number = Convert.ToInt32(split[1]);
        //            consecutivo = (++number).ToString("D3");
        //        }
        //    }

        //    bool VentaPendiente = false;
        //    //Revisar si el último folio registrado tiene artículos ingresados, si no, lo toma, si sí, crea uno nuevo. 
        //    var folioPendienteVenta = sqlLoc.scalar(" SELECT TOP(1) \n"
        //                                        + "     MAX(V.FolioVenta) FolioVenta, VD.Id\n"
        //                                           + " FROM PVVentas V\n"
        //                                           + " LEFT JOIN PVVentasDetalle VD ON V.FolioVenta = VD.FolioVenta\n"
        //                                           + " WHERE IdSucursal = " + idSucursal + "  AND VD.Id IS NULL\n"
        //                                           + " GROUP BY VD.Id\n"
        //                                           + " ORDER BY FolioVenta DESC ");

        //    if (folioPendienteVenta != System.DBNull.Value) { 
        //        if(folioPendienteVenta != null)
        //        {
        //            var split = folioUltimaVenta.ToString().Split('-');
        //            int number = Convert.ToInt32(split[1]);
        //            consecutivo = (number).ToString("D3");
        //            VentaPendiente = true;
        //        }
        //    }



        //    //Folio: V + idSucursal (2 digitos) + fecha/hora + consecutivo
        //    folioVenta = "V" + idSucursal.PadLeft(2, '0')
        //        + DateTime.Now.ToString("ddMMyyTHHmmss") + "-" + consecutivo;
        //    string inicial = "";
        //    if (sucursal != null)
        //    {
        //        inicial = sucursal.Substring(0, 1);
        //    }
        //    else
        //    {
        //        var s = sqlLoc.scalar("SELECT Nombre FROM PVSucursales S WHERE Id = " + idSucursal);
        //        if (s != null)
        //            inicial = s.ToString().Substring(0, 1);
        //    }
        //    folioticket = inicial + consecutivo;

        //    string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        //    if (VentaPendiente == false)
        //    {
        //        string ins = string.Format("INSERT INTO PVVentas(FolioVenta, Folioticket, IdSucursal,\n" +
        //            "IdCliente, TotalVenta,Valido,Terminada,FechaVenta,IdUsuarioVenta,DisparadoNube) \n" +
        //            "OUTPUT INSERTED.Id \n" +
        //            "VALUES('{0}','{1}',{2},{3},{4},{5},{6},'{7}',{8},{9})",
        //            folioVenta, folioticket, idSucursal, IdCliente, 0, 1, 0, fecha, idUsuario, 0);

        //        //Insertar nueva venta
        //        IdVenta = (int)sqlLoc.scalar(ins);

        //        if (IdVenta > 0)
        //        {
        //            lblFolio.Text = "Folio: " + folioticket;

        //            //limpiar campos
        //            lblTotal.Text = "$-";

        //            lblDescProd.Text = "-";
        //            lblPrecioProd.Text = "-";
        //            pbImagen.Image = null;

        //            dgvVenta.Rows.Clear();
        //        }
        //    }
        //    else {
        //            ReabrirFolio(folioPendienteVenta.ToString());
        //    }



        //}

        private void fVenta_Load(object sender, EventArgs e)
        {


            //ProntiPagos.ProntipagosTopUpServiceEndPoint cc = new ProntiPagos.ProntipagosTopUpServiceEndPoint();
            //cc.Credentials = new System.Net.NetworkCredential("pruebasPronti@pagos.com", "ProntiP30%");
            //ProntiPagos.responseCatalogProductTO catalogo = cc.obtainCatalogProducts();

            //mx.prontipagos.ws.ProntipagosTopUpServiceEndPoint cc = new mx.prontipagos.ws.ProntipagosTopUpServiceEndPoint();
            //cc.Credentials = new System.Net.NetworkCredential("oscar@cenor.com.mx", "Joya2022.");
            //mx.prontipagos.ws.responseCatalogProductTO catalogo = cc.obtainCatalogProducts();

            //mx.prontipagos.ws.ProntipagosTopUpServiceEndPoint cc = new mx.prontipagos.ws.ProntipagosTopUpServiceEndPoint();
            //cc.Credentials = new System.Net.NetworkCredential("oscar@cenor.com.mx", "Joya2022.");
            //mx.prontipagos.ws.responseCatalogProductTO catalogo = cc.obtainCatalogProducts();

            //ProntiPagosWSProduccion.ProntipagosTopUpServiceEndPointClient cc = new ProntiPagosWSProduccion.ProntipagosTopUpServiceEndPointClient();
            //cc.ClientCredentials.UserName.UserName = "oscar@cenor.com.mx";
            //cc.ClientCredentials.UserName.Password = "Joya2022";
            //cc.obtainCatalogProducts();
            //ProntiPagosWSProduccion.responseCatalogProductTO catalogo = cc.obtainCatalogProducts();


            //DataTable dt = new DataTable();

            //if (catalogo.products.Count() > 0)
            //{
            //    Type arrype = catalogo.products[0].GetType();
            //    dt = new DataTable(arrype.Name);

            //    foreach (PropertyInfo propInfo in arrype.GetProperties())
            //    {
            //        dt.Columns.Add(new DataColumn(propInfo.Name));
            //    }

            //    foreach (object obj in catalogo.products)
            //    {
            //        DataRow dr = dt.NewRow();

            //        foreach (DataColumn dc in dt.Columns)
            //        {
            //            dr[dc.ColumnName] = obj.GetType().GetProperty(dc.ColumnName).GetValue(obj, null);
            //        }
            //        dt.Rows.Add(dr);
            //    }
            //}

            //sql.llenaCombo(cbxProductosRecargas, dt, "sku", "productName");

            //this.dgvVenta.DefaultCellStyle.Format = "N2";

            ////asignar image list a listview
            //lvCategorias.LargeImageList = imgLstCategorias;
            ////llenar listview
            //foreach (DataRow dr in dtSubcategorias.Rows)
            //{
            //    ListViewItem lvi = new ListViewItem(dr["Subcategoria"].ToString(), dr["IdSubcategoria"].ToString());
            //    lvCategorias.Items.Add(lvi);
            //}
        }

        private void btnRecarga_Click(object sender, EventArgs e)
        {
            ////ProntiPagos
            //ProntiPagos.ProntipagosTopUpServiceEndPoint cc = new ProntiPagos.ProntipagosTopUpServiceEndPoint();
            //cc.Credentials = new System.Net.NetworkCredential("pruebasPronti@pagos.com", "ProntiP30%");
            

            

            ////Solicitud
            //transactionResponseDto resp = new transactionResponseDto();
            //resp = cc.sellService(50, true, "5555555555", "S3AXTELMXN", "3");

            ////Respuesta
            //string codeDescription = resp.codeDescription;
            //string fechaTransaccion = resp.dateTransaction;
            //string idTransaction = resp.transactionId;
            //string codeTransaction = resp.codeTransaction;

            ////Estatus del servicio
            //transactionResponseTO transResp = new transactionResponseTO();
            //transResp = cc.checkStatusService(idTransaction, 1);
            
            //string codeTransactionResp = transResp.codeTransaction;
            //string statusTransactionResp = transResp.statusTransaction;
            //string dateTransactionResp = transResp.dateTransaction;
            //string transactionIdRes = transResp.transactionId;
            //string codeDescriptionResp = transResp.codeDescription;
            //string folioTransactionResp = transResp.folioTransaction;
        }

        private void btnTerminar_Click(object sender, EventArgs e)
        {
           
            if (dgvVenta.Rows.Count == 0)
            {
                MessageBox.Show("No hay productos para la venta!", "Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                goto fin;
            }

            //Se revisan las promociones aplicables 
            // AplicarPromociones();

            //Se registran los productos y pago de la venta
            if (RegistrarPago())
            {

                //Sincronizar
                SubirNube(); //<<-- COMENTADO PARA PRUEBAS
                             //Crear nueva venta o salir si se esta editando
                             //
                
                lblEspere.Visible = false;
                if (modoEdicion)
                {
                    IdVenta = 0;
                    folioVenta = "";
                    lblPromo.Text = "";
                    dgvVenta.Rows.Clear();
                    //this.Close();
                    nuevaVenta();
                    VentasPendientes();
                }
                else
                {
                    nuevaVenta();
                    VentasPendientes();
                }
            }
            else
            {
                
                string sku = "";

                foreach (DataGridViewRow rw in dgvVenta.Rows)
                {
                    sku += rw.Cells[indSku].Value.ToString();
                }

                //Borrar el detalle de la venta si el producto no fue de prontipagos
                if (sku == "")
                {
                    int v = sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "';\n"
                        + " UPDATE PVVentaPago SET FolioVenta = CONCAT('ERROR-', FolioVenta) WHERE FolioVenta='" + folioVenta + "';\n"
                        + " UPDATE PVVentaPago SET FolioVenta = CONCAT('ERROR-', FolioVenta) WHERE FolioVenta='" + folioVenta + "';\n"
                        + " UPDATE PVMonederoCliente SET Valido=0 WHERE FolioVenta= '" + folioVenta + "' AND Valido=1;");

                    lblEspere.Visible = false;
                    if (!pagoCancelado)
                    {
                        MessageBox.Show("Ocurrió un error al guardar la venta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }


                    //if (v > 0)
                    //{
                    //    lblEspere.Visible = false;
                    //    MessageBox.Show("Ocurrió un error al guardar la venta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    //}
                    //else
                    //{
                    //    lblEspere.Visible = false;
                    //    MessageBox.Show("Ocurrió un error al guardar la venta. Favor de cancelarla y generar una nueva.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    //}
                }
                else
                {
                    lblEspere.Visible = false;
                    MessageBox.Show("Ocurrió un error al guardar la venta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }

            fin:;
        }


        

        private int RandomNumber()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        private bool RegistrarPago()
        {
            bool ClienteConPosibleCredito = false;
            if (CBCliente.Visible && CBCliente.SelectedIndex != -1)
            {
                ClienteConPosibleCredito = true;
            }

            string NombreCliente = CBCliente.SelectedIndex == -1 ? txtNombreCliente.Text : CBCliente.Text;

            double total = Convert.ToDouble(lblTotal.Text.Trim().Replace("Total:$", "")) - (Convert.ToDouble(lblIVA.Text.Trim().Replace("$", "")) + Convert.ToDouble(lblIEPS.Text.Trim().Replace("$", "")));
            FPago pago = new FPago(sqlLoc, sql, total.ToString("C2"), 
                Convert.ToDouble(lblIVA.Text.Trim()), 
                Convert.ToDouble(lblIEPS.Text.Trim()), idUsuario, idSucursal, HabilitarCreditoComoFormaPago(ClienteConPosibleCredito), NombreCliente, false, Convert.ToInt32(CBCliente.SelectedValue));
            pago.ShowDialog();
            IdCliente = pago.IdCliente;

            lblEspere.Visible = true;
            if (pago.PagoCancelado)
            {
                pagoCancelado = pago.PagoCancelado;
                return false;
            }

            if (pago.Recibido <= 0)
            {
                MessageBox.Show("No se seleccionó ningun metodo de pago", "Falta pago", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }

            if(pago.Recibido < pago.VentaTotal)
            {
                MessageBox.Show("No se cubrio el total de la venta", "Falta pago", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }


            if(!AgregarProductos())
            {
                MessageBox.Show("Ocurrió un error al guardar los productos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }

            if (CBCliente.Visible && CBCliente.SelectedIndex != -1)
            {
                IdCliente = Convert.ToInt32(CBCliente.SelectedValue);
            }

            string queryVenta = string.Format("UPDATE PVVentas SET IdCliente = {0}, TotalVenta = {1}, NombreCliente = '{2}' \n" +
                " WHERE FolioVenta = '{3}'",
                pago.IdCliente, (pago.TotalVentaOriginal == 0 ? pago.VentaTotal : pago.TotalVentaOriginal), txtNombreCliente.Text, folioVenta);

            if (sqlLoc.exec(queryVenta) > 0)
            {

                //Actualizar cliente y usuario actual en la venta
                sqlLoc.exec("UPDATE PVVentas SET IdUsuarioVenta=" + idUsuario + ", IdCliente =  " + IdCliente + (pago.MontoCredito != 0 ? " ,Pagado = 0 ":"") +
                    "WHERE FolioVenta = '" + folioVenta + "'");
                string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string queryPagos = string.Format("INSERT INTO PVVentaPago (FolioVenta,MontoRecibido,Cambio, \n" +
                    "MontoEfectivo,MontoTarjeta,MontoTarjetaCredito,MontoTransferencia,MontoMonedero,MontoCredito,MontoVales,TipoTarjeta, \n" +
                    "AutorizacionTarjeta,Respuesta,FolioTransferencia,FechaAlta,IdUsuarioAlta, MontoDescuento, PorcentajeDescuento, MontoCheque, FolioCheque) \n" +
                    "VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},'{14}',{15},{16},{17},{18},{19})",
                    !string.IsNullOrEmpty(folioVenta) ? "'" + folioVenta + "'" : "NULL",
                    pago.MontoCredito == 0 ? (pago.Recibido - pago.MontoDescuento) : 0, //Monto Recibido, si se usa credito debe ser cero
                    pago.MontoCambio,
                    pago.MontoEfectivo,
                    pago.MontoTarjeta,
                    pago.MontoTarjetaCredito,
                    pago.MontoTransferencia,
                    0, //Monto Monedero
                    pago.MontoCredito,
                    pago.MontoVales,
                    !string.IsNullOrEmpty(pago.TipoTarjeta) ? "'" + pago.TipoTarjeta + "'" : "NULL",
                    !string.IsNullOrEmpty(pago.Autorizacion) ? "'" + pago.Autorizacion + "'" : "NULL",
                    !string.IsNullOrEmpty(pago.Respuesta) ? "'" + pago.Respuesta + "'" : "NULL",
                    !string.IsNullOrEmpty(pago.FolioTransferencia) ? "'" + pago.FolioTransferencia + "'" : "NULL",
                    fechaH,
                    idUsuario,
                    Math.Round(pago.Descuento, 2), //pago.MontoDescuento,
                    pago.PorcentajeDescuento,
                    pago.MontoCheques,
                    !string.IsNullOrEmpty(pago.FolioCheques) ? "'" + pago.FolioCheques + "'" : "NULL"
                    );
                
                if (sqlLoc.exec(queryPagos) > 0)
                {
                    /*if (pago.MontoCredito > 0)
                    {
                        GuardarPagoMonedero(pago.MontoCredito);
                    }*/

                    int i = 0;
                    //Terminar venta
                    TerminarVenta:
                    string terminarVenta = "UPDATE PVVentas SET Terminada = 1, FechaVenta = '" + fechaH + "' \n" +
                        "WHERE FolioVenta = '" + folioVenta + "'";
                    if (sqlLoc.exec(terminarVenta) > 0)
                    {
                        //Borrar orden de productos para no ocupar espacio en la BD
                        sqlLoc.exec(" DELETE FROM [PVOrdenProductos] WHERE FolioVenta = '" + folioVenta + "' ");

                        if (pago.Factura)
                        {
                            FFacturacion facturacion = new FFacturacion(sqlLoc, sql, idUsuario, folioVenta);
                            facturacion.ShowDialog();
                        }
                        //string fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //string updateFecha = "UPDATE PVVentas SET FechaVenta = '" + fechaH + "' \n" +
                        //    "WHERE FolioVenta = '" + folioVenta + "'" ;
                        //sqlLoc.exec(updateFecha); ///*****

                        
                        DialogResult dr = MessageBox.Show("¿Desea imprimir el ticket?", "Ticket", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                        if (dr == DialogResult.Yes)
                        {
                            FTicket ticket = new FTicket(sqlLoc, folioVenta, true);
                            ticket.ShowDialog();
                        }
                        else
                        {
                            TicketVacio ticketVacio = new TicketVacio();
                            ticketVacio.ShowDialog();
                        }
                        SetPanelClienteNew();
                        return true;
                    }
                    else
                    {
                        if (i <= 5) {
                            i++;
                            goto TerminarVenta;
                        }

                        if (i > 5)
                            return false;
                        
                    }
                    SetPanelClienteNew();
                    return true;
                }
            }
            
           
            return false;
        }

        /*private void GuardarPagoMonedero(double monto)
        {
            string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string query = string.Format("INSERT INTO PVMonederoCliente(FolioVenta, IdCliente, " +
                "Monto, Valido, FechaAlta, IdUsuarioAlta, DisparadoNube)" +
                "VALUES('{0}',{1},{2},{3},'{4}',{5},{6})", 
                folioVenta, IdCliente, monto*(-1), 1, fechaH, idUsuario, 0);

            sqlLoc.exec(query);
        }*/

        private bool AgregarProductos()
        {
            //si los parametros de pago son correctos, se hace la venta
            string ins = "";
            string IdProducto = "";
            string Precio = "";
            string PrecioInicial = "";
            string MontoDescuento = "";
            string Cantidad = "";
            string IVA = "0";
            string IEPS = "0";
            string EsCaja = "0";
            string Uom = "0";
            string idPres;
            string idMarca;
            string idLinea;
            string sku = "";
            string descripcionProducto = "";
            string NumeroTelefonico = "";
            string PrecioSinImpuestos = string.Empty;

            string trans = "";
            string textoTrans = "";

            string montoComision = "0";

            string CodigoTransaccionRecarga = "";

            int ContPagosErrores = 0;


            //Borra todos los productos de la venta del folio que no ha sido pagado (para evitar que salgan otros que no están en pantalla)
            //sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'");

            foreach (DataGridViewRow rw in dgvVenta.Rows)
            {
                IdProducto = rw.Cells[indIdProd].Value.ToString();

                switch (ObtenerTipoCliente())
                {
                    case 1:
                        PrecioSinImpuestos = ((dtProductos.Select("id = " + IdProducto))[0][17]).ToString();
                        break;
                    case 2:
                        PrecioSinImpuestos = ((dtProductos.Select("id = " + IdProducto))[0][18]).ToString();
                        break;
                    case 3:
                        PrecioSinImpuestos = ((dtProductos.Select("id = " + IdProducto))[0][19]).ToString();
                        break;
                }

                Precio = double.Parse(rw.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null).ToString();
                PrecioInicial = double.Parse(rw.Cells[indPrecioInicial].Value.ToString(), NumberStyles.Currency, null).ToString();
                Cantidad = rw.Cells[indQty].Value.ToString();
                if (rw.Cells[indIVA].Value != null)
                    IVA = rw.Cells[indIVA].Value.ToString();
                if (rw.Cells[indIEPS].Value != null)
                    IEPS = rw.Cells[indIEPS].Value.ToString();
                if (rw.Cells[indEsCaja].Value != null)
                    EsCaja = rw.Cells[indEsCaja].Value.ToString();
                if (rw.Cells[indUom].Value != null)
                    Uom = rw.Cells[indUom].Value.ToString();
                idPres = rw.Cells[indIdPres].Value.ToString();
                idMarca = rw.Cells[indIdMarca].Value.ToString();
                idLinea = rw.Cells[indIdLinea].Value.ToString();
                sku = rw.Cells[indSku].Value.ToString();
                descripcionProducto = rw.Cells[indProducto].Value.ToString();
                NumeroTelefonico = descripcionProducto.Trim();
                if (descripcionProducto.Contains("#"))
                {
                    NumeroTelefonico = descripcionProducto.Split('#')[1].Trim();
                    //.Trim().Replace("(", "").Replace(")", "").Replace("-", "").Trim()
                }
                else
                    NumeroTelefonico = "";

                montoComision = rw.Cells[indMontoComision].Value.ToString();

                //var existe = dtProductos.Select("Id ='" + IdProducto + "'");
                //string PrecioProd = 0.ToString();
                //if (existe.Count() > 0)
                //PrecioProd = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString();

                //MontoDescuento = (double.Parse(PrecioProd) - double.Parse(Precio)).ToString();
                string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ins += " EXEC dbo.PVAgregarProductoVenta " +
                    " '" + folioVenta + "', " + IdProducto + ", " +
                    " " + Cantidad + ", " + Precio + ", " +
                    " " + 0 + ", " +
                    " " + IVA + ", " + IEPS + ", " +
                    " " + EsCaja + ", " + Uom + ", " +
                    " '" + fechaH + "' ," + idUsuario + ", " +
                    " " + idPres + ", " + idMarca + ", " + idLinea + ", '" + NumeroTelefonico + "', " + montoComision + "," + PrecioSinImpuestos + ";";


                //Si el producto es una recarga, realizar la recarga
                if (sku != "") {
                    //Crear la recarga
                    double MontoRecarga = 0;
                    double.TryParse(PrecioInicial, out MontoRecarga);

                    double MC = 0;
                    double.TryParse(montoComision, out MC);

                    MontoRecarga = MontoRecarga - MC;

                    //int VentaReferencia = 0;
                    ////string VentaReferencia = System.Guid.NewGuid().ToString();
                    ////string UUID = System.Guid.NewGuid().ToString();
                    ////int.TryParse(UUID.Replace("-",""), out VentaReferencia);
                    //int.TryParse(folioVenta.Split('-')[1].Trim(), out VentaReferencia);

                    //VentaReferencia = 43;


                    int Intentos = 0;
                //Reintentar Transacción
                ReintentarTransaccion:
                    Intentos++;


                    string Referencia = descripcionProducto.Split('#')[1].Trim().Replace("(", "").Replace(")", "").Replace("-", "").Trim().Replace(" ", "");

                    //Folio de transacción
                    string Transaccion = string.Empty; //Recarga(MontoRecarga, Referencia, sku, "");//VentaReferencia.ToString()
                    //string Transaccion = "00" + "~" + "00" + "~" + "00" + "~" + "2022-09-05" + "~" + "123" + "~" + "TEST";


                    string FolioTransaccion = Transaccion.Split('~')[0].Trim();
                    string CodigoTransaccion = Transaccion.Split('~')[1] != null ? Transaccion.Split('~')[1].Trim() : "";
                    string StatusTransaccion = Transaccion.Split('~')[2] != null ? Transaccion.Split('~')[2].Trim() : "";
                    string FechaTransaccion = Transaccion.Split('~')[3] != null ? Transaccion.Split('~')[3].Trim() : "";
                    string TransaccionID = Transaccion.Split('~')[4] != null ? Transaccion.Split('~')[4].Trim() : "";
                    string CodigoDescripcion = Transaccion.Split('~')[5] != null ? Transaccion.Split('~')[5].Trim() : "";

                    //Actualiza venta con el estatus de la transacción
                    trans += " UPDATE PVVentasDetalle SET \n"
                                    + " FolioTransaccion='"+ FolioTransaccion +"'\n"
                                    + " , CodigoTransaccion='"+ CodigoTransaccion +"'\n"
                                    + " , StatusTransaccion='"+ StatusTransaccion +"'\n"
                                    + " , FechaTransaccion='"+ FechaTransaccion +"'\n"
                                    + " , TransaccionID='"+ TransaccionID +"'\n"
                                    + " , CodigoDescripcion='"+ CodigoDescripcion +"'\n"
                                    + " WHERE FolioVenta='"+ folioVenta + "' AND IdProducto="+ IdProducto + " \n"
                                    + " AND IdPresentacionProducto="+ idPres + " AND NumeroTelefonico = '"+ NumeroTelefonico +"';";

                //                    textoTrans += "Folio de transacción de la recarga: " + FolioTransaccion;

                    CodigoTransaccionRecarga = CodigoTransaccion;

                    //Acatualiza venta con el folio de la transacción

                    if (FolioTransaccion != "")
                    {
                        //Acatualiza venta con el folio de la transacción
                        MessageBox.Show("Folio de transacción: " + FolioTransaccion,
                          "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information,
                          MessageBoxDefaultButton.Button2);

                    }
                    else
                    {

                        MessageBox.Show("Error al hacer la transacción. Referencia: " + NumeroTelefonico + "" + " | " + CodigoDescripcion,
                                "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2);

                        //Borra del detalle los productos ingresados
                        
                        //sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "' ");
                        ContPagosErrores++;
                    }
                }
            }



            //si se esta continuando una venta previa
            if (modoEdicion)
            {
                sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'");
            }

            // o tuvo errores en prontipagos, se borran los productos anteriores y se vuelven a tomar en el ciclo del grid
            if (ContPagosErrores > 0)
            {
                sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'");
                return false;
            }

            sumarTotal();

            if (sqlLoc.exec(ins) != 0) {

                if (sku != "")
                {
                    if (trans != "")
                    {
                        sqlLoc.exec(trans);

                        if (CodigoTransaccionRecarga == "00")
                            return true;
                        else
                        {
                            return false;
                        }
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return true;
                }
            }
             

            return false;
        }

        /*private string Recarga(double monto, string numeroTelefonico, string sku, string referenciaCliente) {

            bool TransaccionExitosa = false;
            string FolioTransaccion = "";
            ////ProntiPagos TEST
            //ProntiPagos.ProntipagosTopUpServiceEndPoint cc = new ProntiPagos.ProntipagosTopUpServiceEndPoint();
            //cc.Credentials = new System.Net.NetworkCredential("pruebasPronti@pagos.com", "ProntiP30%");

            //Prontipagos PRODUCCION
            mx.prontipagos.ws.ProntipagosTopUpServiceEndPoint cc = new mx.prontipagos.ws.ProntipagosTopUpServiceEndPoint();
            //cc.Credentials = new System.Net.NetworkCredential("oscar@cenor.com.mx", "");

            string referencia = numeroTelefonico;

            //Solicitud
            transactionResponseDto resp = new transactionResponseDto();
            resp = cc.sellService(monto, true, referencia, sku, null); //<-- PRODUCCION
            //resp = cc.sellService(50, true, "5555555555", "S3AXTELMXN", "13"); //<-- PARA TEST


            //Respuesta
            string codeDescription = resp.codeDescription;
            string fechaTransaccion = resp.dateTransaction;
            string idTransaction = resp.transactionId;
            string codeTransaction = resp.codeTransaction;

            //Estatus del servicio
            //transactionResponseTO transResp = new transactionResponseTO();

            int Intentos = 0;
        //Reintentar Transacción por 60 segundos (cada 2 segundos) hasta obtener una respuesta.
        ReintentarCheckStatus:
            Intentos++;

            //Si utiliza clientReference en	el parámetro transactionId, colocar el numero(1), 
            //si utiliza  transactionId de  Siveta en  el parámetro   transactionId enviar  null 
            //Es muy importante enviar  el numero(1) en clientReference para no  causar perdida o error   de la  plataforma
            transResp = cc.checkStatusService(idTransaction, 1);


            string codeTransactionResp = transResp.codeTransaction;
            string statusTransactionResp = transResp.statusTransaction;
            string dateTransactionResp = transResp.dateTransaction;
            string transactionIdRes = transResp.transactionId;
            string codeDescriptionResp = transResp.codeDescription;
            string folioTransactionResp = transResp.folioTransaction;

            if (codeTransactionResp == "00")
            {
                TransaccionExitosa = true;
                FolioTransaccion = folioTransactionResp;
            }
            else
            {
                //reintentar 1 vez
                if (Intentos <= 30)
                {
                    //Intentar a los 2 segundos
                    Thread.Sleep(2000);
                    goto ReintentarCheckStatus;
                }
            }

            return FolioTransaccion + "~" + codeTransactionResp + "~" + statusTransactionResp + "~" + dateTransactionResp + "~" + transactionIdRes + "~" + codeDescriptionResp;
           // return FolioTransaccion + "~" + "11221122" + "~" + "00" + "~" + "00" + "~" + "00" + "~" + "test";
        }*/

        private void Factura()
        {
            //
        }

        private void SubirNube()
        {
            if (!CheckForInternetConnection())
                goto fin;

            //Sincronizar lista de precios y promociones
            try
            {
                //precios
                //Cat_Precios


                //DataTable dtCopiar = sql.selec(" SELECT P.Id, P.IdProducto, P.IdPresentacion, \n" +
                //    "Pres.Presentacion, Uom, CodigoBarras, Precio, P.VerPV, P.IdProductoIndividual \n" +
                //    "FROM PV_Cat_PresentacionesVenta_Productos P \n" +
                //    "LEFT JOIN PV_Cat_PresentacionesAlmacen Pres ON P.IdPresentacion = Pres.Id \n" +
                //    "WHERE P.Activo = 1 ");

                //Codigos de barra / precios
                DataTable dtCopiar = sql.selec("SELECT P.Id, P.IdProducto, P.IdPresentacion, Pres.Presentacion, Uom, CodigoBarras, \n" +
                    "Precio, P.VerPV, P.IdProductoIndividual, P.sku \n" +
                    "FROM PV_Cat_PresentacionesVenta_Productos P \n" +
                    "LEFT JOIN PV_Cat_PresentacionesAlmacen Pres ON P.IdPresentacion = Pres.Id \n" +
                    "WHERE P.Activo = 1");

                sqlLoc.exec(" TRUNCATE TABLE PVPresentacionesVentaProd ");
                sqlLoc.copiaBulto(dtCopiar, "PVPresentacionesVentaProd");
                CargarProductos();

                //Promociones
                DataTable dtPromociones = sql.selec("SELECT P.Id, P.IdSucursal, P.TipoPromocion, P.Estatus, \n" +
                    "P.FechaInicio, P.FechaFin, P.Descripcion, D.IdProducto, D.Precio, D.IdMarca, \n" +
                    "D.IdLinea, D.Cantidad, D.PrecioPromocion, D.Descuento, D.IdProductoRegalo, \n" +
                    "D.CantidadRegalo, P.Combinado, P.Multiplo, ISNULL(P.Individual,0) Individual \n" +
                    "FROM PVPromociones P \n" +
                    "LEFT JOIN( \n" +
                    "   SELECT IdPromocion, STRING_AGG(IdProducto, ',') IdProducto, \n" +
                    "   MAX(Precio) Precio, MAX(IdMarca) IdMarca, MAX(IdLinea) IdLinea, \n" +
                    "   (CASE WHEN COUNT(IdProducto) > 1 THEN 1 ELSE SUM(Cantidad) END) Cantidad, \n" +
                    "   MAX(PrecioPromocion) PrecioPromocion, MAX(Descuento) Descuento, \n" +
                    "   STRING_AGG(IdProductoRegalo, ',') IdProductoRegalo, \n" +
                    "   (CASE WHEN COUNT(IdProductoRegalo) > 1 THEN 1 " +
                    "   ELSE SUM(CantidadRegalo) END) CantidadRegalo \n" +
                    "   FROM PVPromocionesDetalle D \n" +
                    "   GROUP BY IdPromocion \n" +
                    ")D ON D.IdPromocion = P.Id \n" +
                    "WHERE Estatus = 2 AND P.Valida=1");
                sqlLoc.exec(" TRUNCATE TABLE PVPromociones ");
                sqlLoc.copiaBulto(dtPromociones, "PVPromociones");

                //Promociones Detalle
                DataTable dtPromocionesDetalle = sql.selec("SELECT PD.Id, PD.IdPromocion, PD.IdProducto IdPresProducto, \n" +
                    "PD.Precio, PD.IdMarca, PD.IdLinea, PD.Cantidad, PD.PrecioPromocion, \n" +
                    "PD.Descuento, PD.IdProductoRegalo, PD.CantidadRegalo \n" +
                    "FROM PVPromocionesDetalle PD \n" +
                    "LEFT JOIN PVPromociones P ON PD.IdPromocion = P.Id \n" +
                    "WHERE P.Estatus = 2");
                sqlLoc.exec(" TRUNCATE TABLE PVPromocionesDetalle ");
                sqlLoc.copiaBulto(dtPromocionesDetalle, "PVPromocionesDetalle");

                //DataTable dtPromociones = sql.selec("" +
                //    "SELECT PD.Id, PD.IdPromocion, PD.IdProducto IdPresProducto, \n" +
                //    "PD.Precio, PD.IdMarca, PD.IdLinea, PD.Cantidad, PD.PrecioPromocion, \n" +
                //    "PD.Descuento, PD.IdProductoRegalo, PD.CantidadRegalo, P.Combinado, P.Multiplo \n" +
                //    "FROM PVPromocionesDetalle PD \n" +
                //    "LEFT JOIN PVPromociones P ON PD.IdPromocion = P.Id \n" +
                //    "WHERE P.Estatus = 2");
                //sqlLoc.exec(" TRUNCATE TABLE PVPromociones ");
                //sqlLoc.copiaBulto(dtPromociones, "PVPromociones");

                //dtCopiar = sql.selec("SELECT PD.Id, PD.IdPromocion, PD.IdProducto IdPresProducto, \n" +
                //    "PD.Precio, PD.IdMarca, PD.IdLinea, PD.Cantidad, PD.PrecioPromocion, \n" +
                //    "PD.Descuento, PD.IdProductoRegalo, PD.CantidadRegalo \n" +
                //    "FROM PVPromocionesDetalle PD \n" +
                //    "LEFT JOIN PVPromociones P ON PD.IdPromocion = P.Id \n" +
                //    "WHERE P.Estatus = 2");
                //sqlLoc.exec(" TRUNCATE TABLE PVPromocionesDetalle ");
                //sqlLoc.copiaBulto(dtCopiar, "PVPromocionesDetalle");
            }
            catch {}
            try
            {
                //Enviar la venta a la nube
                string sitio, usuario, folioV, folioT, fecha, IdCorteParcialCaja, FolioCorteParcialCaja,
                    IdCorteCaja, FolioCorteCaja, CorteTerminado;

                //Buscar ventas que no se hayan enviado a la nube
                string queryVentas = "SELECT DISTINCT FolioVenta, FolioTicket, IdSucursal, IdCliente, Factura, \n" +
                    "RFCFactura, FechaFactura, UsuarioFactura, FolioFactura, TotalVenta, \n" +
                    "IdCorteParcialCaja, FoliocorteParcialCaja, IdCorteCaja, FolioCorteCaja, \n" +
                    "CorteTerminado, Valido, Terminada, FechaVenta, IdUsuarioVenta \n" +
                    "FROM PVVentas WHERE DisparadoNube = 0 AND Terminada = 1";
                DataTable dtVentas = sqlLoc.selec(queryVentas);

                if (dtVentas.Rows.Count > 0)
                {
                    foreach (DataRow r in dtVentas.Rows)
                    {
                        sitio = r["IdSucursal"].ToString();
                        usuario = r["IdUsuarioVenta"].ToString();
                        folioV = r["FolioVenta"].ToString();
                        folioT = r["FolioTicket"].ToString();
                        fecha = Convert.ToDateTime(r["FechaVenta"]).ToString("yyyy-MM-dd HH:mm:ss");

                        IdCorteParcialCaja = r["IdCorteParcialCaja"] != System.DBNull.Value
                            ? r["IdCorteParcialCaja"].ToString() : "NULL";

                        FolioCorteParcialCaja = r["FolioCorteParcialCaja"] != System.DBNull.Value
                            ? "'" + r["FolioCorteParcialCaja"].ToString() + "'" : "NULL";

                        IdCorteCaja = r["IdCorteCaja"] != System.DBNull.Value ?
                            IdCorteCaja = r["IdCorteCaja"].ToString() : "NULL";

                        FolioCorteCaja = "NULL";
                        CorteTerminado = "0";
                        if (r["FolioCorteCaja"] != System.DBNull.Value)
                        {
                            FolioCorteCaja = "'" + r["FolioCorteCaja"].ToString() + "'";
                            CorteTerminado = "1";
                        }
                        string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        //Meter renglon y regresar id
                        string insVenta =
                            " INSERT INTO PVVentas (FolioVenta, Folioticket, IdSucursal, FechaVenta, IdUsuarioVenta, \n" +
                            "Factura, RFCFactura, FechaFactura, UsuarioFactura, FolioFactura, IdCliente, \n" +
                            "CorteTerminado, Valido, FechaDisparo, IdUsuarioDisparo, IdCorteParcialCaja, \n" +
                            "FolioCorteParcialCaja, IdCorteCaja, FolioCorteCaja, TotalVenta) \n" +
                            "VALUES( \n"
                            + " '" + folioV + "', '" + folioT +"', " 
                            + " " + sitio + ","
                            + " '" + fecha + "', " + usuario + ","
                            + " '" + r["Factura"] + "', '" + r["RFCFactura"] + "', "
                            + " '" + r["FechaFactura"] + "', '" + r["UsuarioFactura"] + "', "
                            + " '" + r["FolioFactura"] + "', " + r["IdCliente"] + ", "
                            + CorteTerminado + ", '" + r["Valido"] + "',"
                            + " '" + fechaH + "', " + idUsuario + ", " + IdCorteParcialCaja + ", " 
                            + " " + FolioCorteParcialCaja + ", " + IdCorteCaja + ", " 
                            + "" + FolioCorteCaja + ", " + " " + r["TotalVenta"] + ")";

                        if (sql.exec(insVenta) > 0)
                        {
                            //Actualizar BD local para marcar la venta como enviada a la nube
                            sqlLoc.exec(" UPDATE TOP(1) PVVentas SET DisparadoNube = 1, FechaDisparo='" + fechaH + "', "
                                + "IdUsuarioDisparo=" + idUsuario + " WHERE FolioVenta = '" + folioV + "'");
                        }
                    }
                }
                //Consultar pagos pendientes de subir a la nube
                string queryPagos = "SELECT VP.FolioVenta, VP.MontoRecibido, VP.Cambio, VP.MontoEfectivo,\n" +
                    "VP.MontoTarjeta, VP.MontoTransferencia, VP.MontoMonedero, VP.MontoVales, VP.MontoCheque, \n" +
                    "VP.TipoTarjeta, VP.AutorizacionTarjeta, VP.Respuesta, VP.FolioTransferencia, VP.FolioCheque, \n" +
                    "VP.FechaAlta, VP.IdUsuarioAlta \n" +
                    "FROM PVVentaPago VP \n" +
                    "JOIN PVVentas V ON VP.FolioVenta = V.FolioVenta \n" +
                    "WHERE V.Terminada = 1 AND VP.DisparadoNube = 0";

                DataTable dtPagos = sqlLoc.selec(queryPagos);

                if (dtPagos.Rows.Count > 0)
                {
                    foreach (DataRow r in dtPagos.Rows)
                    {
                        fecha = Convert.ToDateTime(r["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string insPagos = string.Format("INSERT INTO PVVentasPagos" +
                        "(FolioVenta, MontoRecibido, Cambio, MontoEfectivo, MontoTarjeta, MontoTransferencia," +
                        "MontoMonedero, MontoVales, TipoTarjeta, AutorizacionTarjeta, Respuesta," +
                        "FolioTransferencia, FechaAlta, IdUsuarioAlta, FechaDisparo, IdUsuarioDisparo, MontoCheque, FolioCheque)" +
                        "VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},'{12}',{13},'{14}',{15},{16},'{17}')",
                        !string.IsNullOrEmpty(r["FolioVenta"].ToString()) ? "'" + r["FolioVenta"].ToString() + "'" : "NULL",
                        r["MontoRecibido"].ToString(), r["Cambio"].ToString(),
                        r["MontoEfectivo"].ToString(), r["MontoTarjeta"].ToString(),
                        r["MontoTransferencia"].ToString(), r["MontoMonedero"].ToString(),
                        r["MontoVales"].ToString(),
                        !string.IsNullOrEmpty(r["TipoTarjeta"].ToString())
                        ? "'" + r["TipoTarjeta"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(r["AutorizacionTarjeta"].ToString())
                        ? "'" + r["AutorizacionTarjeta"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(r["Respuesta"].ToString())
                        ? "'" + r["Respuesta"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(r["FolioTransferencia"].ToString())
                        ? "'" + r["FolioTransferencia"].ToString() + "'" : "NULL",
                        fecha, r["IdUsuarioAlta"].ToString(), fechaH, idUsuario,
                        r["MontoCheque"], r["FolioCheque"]);

                        if (sql.exec(insPagos) > 0)
                        {
                            //Actualizar BD local para marcar los pagos como enviados a la nube
                            sqlLoc.exec(" UPDATE TOP(1) PVVentaPago \n" +
                                "SET DisparadoNube = 1, FechaDisparo='" + fechaH + "', " +
                                "IdUsuarioDisparo=" + idUsuario + " \n" +
                                "WHERE FolioVenta = '" + r["FolioVenta"].ToString() + "'");
                        }
                    }
                }

                //Consultar Detalle de la venta 
                string queryDetalle = "SELECT VD.Id, VD.FolioVenta, VD.IdProducto, VD.Cantidad, \n" +
                    " VD.Precio, VD.MontoDescuento, VD.Iva, VD.Ieps,VD.FechaAlta, VD.IdUsuarioAlta, \n" +
                    " VD.EsCaja, VD.Uom, VD.NumeroTelefonico, VD.FolioTransaccion, VD.CodigoTransaccion, VD.StatusTransaccion, \n" +
                    " VD.FechaTransaccion, VD.TransaccionID, VD.CodigoDescripcion, ISNULL(VD.MontoComision,0) MontoComision, VD.IdPresentacionProducto \n" +
                    " FROM PVVentasDetalle VD JOIN PVVentas V ON VD.FolioVenta = V.FolioVenta \n" +
                    " WHERE V.Terminada = 1 AND VD.DisparadoNube = 0";

                DataTable dtDetalle = sqlLoc.selec(queryDetalle);

                if (dtDetalle.Rows.Count > 0)
                {
                    string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    foreach (DataRow dr in dtDetalle.Rows)
                    {
                        var d = dr["Precio"];
                        fecha = Convert.ToDateTime(dr["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string insVentaDetalle = string.Format("INSERT INTO PVVentasDetalle \n" +
                            " (FolioVenta, IdProducto, Cantidad, Precio, Iva, Ieps, FechaAlta, " +
                            " IdUsuarioAlta, FechaDisparo, IdUsuarioDisparo, MontoDescuento,EsCaja,Uom, NumeroTelefonico, FolioTransaccion, CodigoTransaccion, StatusTransaccion, FechaTransaccion, TransaccionID, CodigoDescripcion, MontoComision, IdPresentacionProducto)\n " +
                            " VALUES({0},{1},{2},{3},{4},{5},'{6}',{7},'{8}',{9},{10},'{11}',{12},'{13}','{14}','{15}','{16}','{17}','{18}','{19}',{20},{21})",
                            !string.IsNullOrEmpty(dr["FolioVenta"].ToString())
                            ? "'" + dr["FolioVenta"].ToString() + "'" : "NULL",
                            dr["IdProducto"].ToString(),     dr["Cantidad"].ToString(),
                            dr["Precio"].ToString(), dr["Iva"].ToString(),
                            dr["Ieps"].ToString(),
                            fecha, dr["IdUsuarioAlta"].ToString(), fechaH, idUsuario,
                            dr["MontoDescuento"].ToString(), dr["EsCaja"].ToString(), dr["Uom"].ToString(), 
                            dr["NumeroTelefonico"].ToString(), dr["FolioTransaccion"].ToString(), dr["CodigoTransaccion"].ToString(),
                            dr["StatusTransaccion"].ToString(), dr["FechaTransaccion"].ToString(), dr["TransaccionID"].ToString(),
                            dr["CodigoDescripcion"].ToString(), dr["MontoComision"].ToString(), dr["IdPresentacionProducto"]
                            );

                        if (sql.exec(insVentaDetalle) > 0)
                        {
                            //Actualizar BD local para marcar el detalle como enviado a la nube
                            string updtDetalle = " UPDATE PVVentasDetalle SET DisparadoNube = 1, " +
                                "FechaDisparo = '" + fechaH + "', IdUsuarioDisparo = " + idUsuario
                                + " WHERE FolioVenta = '" + dr["FolioVenta"].ToString() + "' AND Id="+ dr["Id"].ToString();
                            var x = sqlLoc.exec(updtDetalle);
                        }
                    }
                }

                //Consultar monedero
                string queryMonedero = "SELECT FolioVenta, IdCliente, Monto, Valido, " +
                    "FechaAlta, IdUsuarioAlta \n" +
                    "FROM PVMonederoCliente WHERE DisparadoNube = 0";
                DataTable dtMonedero = sqlLoc.selec(queryMonedero);

                if (dtMonedero.Rows.Count > 0)
                {
                    string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    foreach (DataRow dr in dtMonedero.Rows)
                    {
                        string fechaAlta = Convert.ToDateTime(dr["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string ins = string.Format("INSERT INTO PVMonederoCliente " +
                            " (FolioVenta, Monto, Valido, FechaAlta, IdUsuarioAlta, " +
                            " FechaDisparo, IdUsuarioDisparo, IdCliente) \n" +
                            " VALUES({0},{1},'{2}','{3}',{4},'{5}',{6},{7})",
                            !string.IsNullOrEmpty(dr["FolioVenta"].ToString()) ? "'" + dr["FolioVenta"].ToString() + "'" : "NULL",
                            dr["Monto"].ToString(), dr["Valido"].ToString(), fechaAlta,
                            dr["IdUsuarioAlta"].ToString(), fechaH, idUsuario, IdCliente);

                        if (sql.exec(ins) > 0)
                        {
                            //Actualizar BD Local para marcar monedero como subido a la nube
                            sqlLoc.exec("UPDATE PVMonederoCliente SET DisparadoNube = 1 " +
                                "WHERE FolioVenta = '" + dr["FolioVenta"].ToString() + "'");
                        }
                    }
                }
            }
            catch { }
            fin:;
        }

        private void pbImagen_Click(object sender, EventArgs e)
        {

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

        private void lblPrecioProd_Click(object sender, EventArgs e)
        {

        }

        private void fVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dgvVenta.Rows.Count <= 0)
            {
                //var x = sqlLoc.exec(" DELETE FROM PVVentas WHERE FolioVenta = '" + folioVenta + "'");
                //var x = sqlLoc.exec(" UPDATE PVVentas SET Valido=0 WHERE FolioVenta = '" + folioVenta + "'");
                goto fin;
            }

            if (MessageBox.Show("Si abandonas la venta podrás continuarla después ¿Salir?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                if (!cancelacion)
                {
                    btnEspera_Click(sender, e);
                    //string ins = "";

                    //string IdProducto = "";
                    //string Precio = "";
                    //string PrecioInicial = "";
                    //string MontoDescuento = "";
                    //string Cantidad = "";
                    //string IVA___ = "0";
                    //string IEPS___ = "0";
                    //string EsCaja__ = "0";
                    //string Uom__ = "0";
                    //string idPres;
                    //string idMarca;
                    //string idLinea;
                    //string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //foreach (DataGridViewRow rw in dgvVenta.Rows)
                    //{
                    //    IdProducto = rw.Cells[indIdProd].Value.ToString();
                    //    Precio = double.Parse(rw.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null).ToString();
                    //    PrecioInicial = double.Parse(rw.Cells[indPrecioInicial].Value.ToString(), NumberStyles.Currency, null).ToString();
                    //    Cantidad = rw.Cells[indQty].Value.ToString();
                    //    if (rw.Cells[indIVA].Value != null)
                    //        IVA___ = rw.Cells[indIVA].Value.ToString();
                    //    if (rw.Cells[indIEPS].Value != null)
                    //        IEPS___ = rw.Cells[indIEPS].Value.ToString();
                    //    if (rw.Cells[indEsCaja].Value != null)
                    //        IEPS___ = rw.Cells[indUom].Value.ToString();
                    //    idPres = rw.Cells[indIdPres].Value.ToString();
                    //    idMarca = rw.Cells[indIdMarca].Value.ToString();
                    //    idLinea = rw.Cells[indIdLinea].Value.ToString();

                    //    //var existe = dtProductos.Select("Id ='" + IdProducto + "'");
                    //    //string PrecioProd = 0.ToString();
                    //    //if (existe.Count() > 0)
                    //    //    PrecioProd = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString();

                    //    //MontoDescuento = (double.Parse(PrecioProd) - double.Parse(Precio)).ToString();

                    //    //ins += " EXEC dbo.PVAgregarProducto '" + IdVenta + "', " + IdProducto + ", " + Cantidad + ", " + Precio + "";
                    //    ins = " EXEC dbo.PVAgregarProductoVenta " +
                    //    " '" + folioVenta + "', " + IdProducto + ", " +
                    //    " " + Cantidad + ", " + PrecioInicial + ", " +
                    //    " " + 0 + ", " +
                    //    " " + IVA___ + ", " + IEPS___ + ", " +
                    //    " " + EsCaja__ + ", " + Uom__ + ", " +
                    //    " '" + fechaH + "' ," + idUsuario + ", " + idPres +"," +
                    //    " " + idMarca + ", " + idLinea + ";";

                    //    sqlLoc.exec(ins);
                    //}

                    ////si se esta editando primero borrar lo que ya estaba
                    //if (modoEdicion)
                    //{
                    //    sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'");
                    //}
                }
            }

        fin:;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Cancelar Venta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                string ins =
                    " UPDATE TOP(1) PVVentas SET Valido = 0 WHERE FolioVenta = '" + folioVenta + "'";
                sqlLoc.exec(ins);

                //Elimina de orden productos para no ocupar espacio
                sqlLoc.exec(" DELETE FROM [PVOrdenProductos] WHERE FolioVenta = '" + folioVenta + "' ");

                //MessageBox.Show("Venta Cancelada!", "Cancelada", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                //dgvVenta.Rows.Clear();
                //crear nueva venta o salir si se esta editando
                if (modoEdicion)
                {
                    VentasPendientes();
                    IdVenta = 0;
                    nuevaVenta();
                    //this.Close();
                }
                else
                {
                    nuevaVenta();
                }
            }
        }

        private void dgvVenta_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(() =>
            {
                if (e.RowIndex > -1)
            {
                //2= resta
                //4= suma
                var dgvR = dgvVenta.Rows[e.RowIndex];

                if (e.ColumnIndex == indQty)
                {
                    if (dgvR.Cells[indSku].Value.ToString() != "")
                    {
                        MessageBox.Show("No se puede modificar la cantidad para este tipo de producto",
                           "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                           MessageBoxDefaultButton.Button2);

                        dgvR.Cells[indQty].Value = 1;
                    }
                    else {


                        double qty_ = Convert.ToDouble(dgvR.Cells[indQty].Value);

                        //cantidad
                        double qtyFinal = 0;

                        if (qty_ > 100)
                        {
                            qtyFinal = 100;
                            dgvR.Cells[indQty].Value = 100;
                        }
                        else
                            qtyFinal = Convert.ToDouble(dgvR.Cells[indQty].Value);

                        //total
                        dgvR.Cells[indTotal].Value = Math.Round(Convert.ToDouble(dgvR.Cells[indQty].Value) * double.Parse(dgvR.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");

                        if (qtyFinal == 0)
                        {
                            dgvVenta.Rows.Remove(dgvR);
                        }

                        if (qtyFinal > 100)
                        {
                            qtyFinal = 100;
                            dgvR.Cells[indTotal].Value = Math.Round(Convert.ToInt32(100) * double.Parse(dgvR.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");
                        }

                        int noOrden = ObtieneOrdenProducto(dgvR.Cells[indIdProd].Value.ToString());
                        BuscarPromocion(dgvR.Cells[indIdProd].Value.ToString(), dgvR.Cells[indUom].Value.ToString(), dgvR.Cells[indProducto].Value.ToString(), dgvR.Cells[indPrecioInicial].Value.ToString(), dgvR.Cells[indIVA].Value.ToString(), dgvR.Cells[indIEPS].Value.ToString(), dgvR.Cells[indEsCaja].Value.ToString(), dgvR.Cells[indIdPres].Value.ToString(), dgvR.Cells[indIdMarca].Value.ToString(), dgvR.Cells[indIdLinea].Value.ToString(), Convert.ToDouble(dgvR.Cells[indQty].Value), Convert.ToDouble(dgvR.Cells[indPrecio].Value.ToString().Replace("$", "").Replace(",", "")), noOrden);

                        sumarTotal();

                        if (dgvVenta.Rows.Count > 0)
                        {
                            try
                            {
                                foreach (DataGridViewColumn column in dgvVenta.Columns)
                                {
                                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                                    if (column.Name == "IdOrden") //colIndice
                                    {
                                        column.SortMode = DataGridViewColumnSortMode.Programmatic;
                                        dgvVenta.Sort(column, ListSortDirection.Ascending);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {

                            }
                           
                        }

                    }

                  
                }
            }
            }));
        }


        private void dgvVenta_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            int indexPrecio = 5;
            if (!modificacionPorCliente && dgvVenta.Columns[e.ColumnIndex].Name == "colPrecio")
            {
                var existe = dtProductos.Select("Id ='" + dgvVenta.Rows[e.RowIndex].Cells[indIdProd].Value + "'");
                string Precio = 0.ToString("C2");
                if(existe.Count() > 0)
                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                string precioGrid = dgvVenta.Rows[e.RowIndex].Cells[indPrecio].Value.ToString();
                try
                {
                    precioGrid = Convert.ToDouble(dgvVenta.Rows[e.RowIndex].Cells[indPrecio].Value).ToString("C2");
                }
                catch { }
                if (Precio != precioGrid)
                {
                    if (!permisoDescuentos)
                    {
                        permisoDescuentos = PermisoDescuentos();
                        if (!permisoDescuentos)
                        {
                            dgvVenta.Rows[e.RowIndex].Cells[indPrecio].Value = Precio;
                            return;
                        }
                    }

                    var gvr = dgvVenta.Rows[e.RowIndex];
                    dgvVenta.Rows[e.RowIndex].Cells[indTotal].Value =
                         Math.Round(Convert.ToInt32(gvr.Cells[indQty].Value)
                            * double.Parse(gvr.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null),
                            2).ToString("C2");

                    sumarTotal();
                }
            }
            
            if (e.ColumnIndex == indexPrecio)
            {
                CambiarPrecioPorUsuario(e);
            }
        }

        private bool PermisoDescuentos()
        {
            FPassFondo passFondo = new FPassFondo();
            passFondo.ShowDialog();

            string usuarioAut = passFondo.Usuario;
            string pass = passFondo.Contrasena;

            DataTable dtUsuario =
            sqlLoc.selec
            (
                "SELECT * FROM PVUsuarios WHERE usuario = '" + usuarioAut.Trim() + "'"
            );

            if (dtUsuario.Rows.Count > 0)
            {
                DataRow r = dtUsuario.Rows[0];

                if (pass.Trim() == r["Contrasena"].ToString().Trim())
                {
                    if (Convert.ToBoolean(r["Descuentos"]))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("No tienes el permiso para realizar descuentos", "Sin permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("La contraseña no es valida.", "Contraseña Incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("No se encontró el usuario.", "Usuario incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private void btnNoExisteCliente_Click(object sender, EventArgs e)
        {
            btnNoExisteCliente.Visible = false;
            CBCliente.Visible = false;
            btnYaExisteCliente.Visible = true;
            txtNombreCliente.Visible = true;
            CBCliente.SelectedIndex = -1;
            txtNombreCliente.Focus();
        }

        private void btnYaExisteCliente_Click(object sender, EventArgs e)
        {
            btnBorrarTxtCliente.Visible = false;
            txtNombreCliente.Enabled = true;
            btnNoExisteCliente.Visible = true;
            CBCliente.Visible = true;
            btnYaExisteCliente.Visible = false;
            txtNombreCliente.Visible = false;
            txtNombreCliente.Text = "";
            InsertarNombreClienteVenta();
        }

        private void btnEspera_Click(object sender, EventArgs e)
        {

            var tieneProductos = sqlLoc.scalar("SELECT COUNT(*) FROM PVVentasDetalle " +
                "WHERE FolioVenta = '" + folioVenta + "'"); //* ***************

            if (tieneProductos != null)
            {
                sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'");
            }


            string ins;

            string IdProducto;
            string Precio;
            //string PrecioInicial;
            string PrecioInicial_;
            string MontoDescuento;
            string Cantidad;
            string IVA___ = "0";
            string IEPS___ = "0";
            string esCaja__ = "0";
            string uom__ = "0";
            string idPres;
            string idMarca;
            string idLinea;
            string descripcionProducto;
            string sku;
            string NumeroTelefonico;
            string MontoComision = "0";

                if (dgvVenta.Rows.Count > 0)
                {

                    foreach (DataGridViewRow rw in dgvVenta.Rows)
                    {
                        IdProducto = rw.Cells[indIdProd].Value.ToString();
                        Precio = double.Parse(rw.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null).ToString();
                        //PrecioInicial_ = double.Parse(rw.Cells[indPrecioInicial].Value.ToString(), NumberStyles.Currency, null).ToString();
                        PrecioInicial_ = PrecioInicial(rw.Cells[indIVA].Value.ToString(), rw.Cells[indIEPS].Value.ToString(), double.Parse(rw.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null)).ToString();

                        Cantidad = rw.Cells[indQty].Value.ToString();
                        if (rw.Cells[indIVA].Value != null)
                            IVA___ = rw.Cells[indIVA].Value.ToString();
                        if (rw.Cells[indIEPS].Value != null)
                            IEPS___ = rw.Cells[indIEPS].Value.ToString();
                        if (rw.Cells[indEsCaja].Value != null)
                            esCaja__ = rw.Cells[indEsCaja].Value.ToString();
                        if (rw.Cells[indUom].Value != null)
                            uom__ = rw.Cells[indUom].Value.ToString();
                        idPres = rw.Cells[indIdPres].Value.ToString();
                        idMarca = rw.Cells[indIdMarca].Value.ToString();
                        idLinea = rw.Cells[indIdLinea].Value.ToString();
                        descripcionProducto = rw.Cells[indProducto].Value.ToString();
                    if (descripcionProducto.Contains("#"))
                        NumeroTelefonico = descripcionProducto.Split('#')[1] != null ? descripcionProducto.Split('#')[1] : "";
                    else
                        NumeroTelefonico = "";


                        sku = rw.Cells[indSku].Value.ToString();
                        MontoComision = rw.Cells[indMontoComision].Value.ToString();




                    string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        //var existe = dtProductos.Select("Id ='" + IdProducto + "'");
                        //string PrecioProd = 0.ToString();
                        //if (existe.Count() > 0)
                        //    PrecioProd = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString();

                        //MontoDescuento = (double.Parse(PrecioProd) - double.Parse(Precio)).ToString();

                        ins = " EXEC dbo.PVAgregarProductoVenta " +
                        " '" + folioVenta + "', " + IdProducto + ", " +
                        " " + Cantidad + ", " + PrecioInicial_ + ", " +
                        " " + 0 + ", " +
                        " " + IVA___ + ", " + IEPS___ + ", " +
                        " " + esCaja__ + ", " + uom__ + ", " +
                        " '" + fechaH + "' ," + idUsuario + ", " + idPres + "," +
                        " " + idMarca + ", " + idLinea + ", '" + NumeroTelefonico + "', "+ MontoComision +";";

                        sqlLoc.exec(ins);
                    }

                nuevaVenta();
                VentasPendientes();
                SetPanelClienteNew();
                txtScan.Select();
            }


            //int i = sqlLoc.exec(" UPDATE PVVentas SET EnEspera=1 WHERE IdSucursal=" + idSucursal + " AND FolioVenta = '" + folioVenta + "' ");

            //if (i != 0)
            //{

            //    int y = 0;
            //    var tieneProductos = sql.scalar("SELECT TOP(1) Id FROM PVVentasDetalle WHERE FolioVenta = '" + Folio + "'");

            //    if (tieneProductos != null)
            //    {
            //        y = sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE FolioVenta = '" + folioVenta + "'");
            //    }

            //    string ins;

            //    string IdProducto;
            //    string Precio;
            //    //string PrecioInicial;
            //    string PrecioInicial_;
            //    string MontoDescuento;
            //    string Cantidad;
            //    string IVA___ = "0";
            //    string IEPS___ = "0";
            //    string esCaja__ = "0";
            //    string uom__ = "0";
            //    string idPres;
            //    string idMarca;
            //    string idLinea;

            //    if (dgvVenta.Rows.Count > 0)
            //    {

            //        foreach (DataGridViewRow rw in dgvVenta.Rows)
            //        {
            //            IdProducto = rw.Cells[indIdProd].Value.ToString();
            //            Precio = double.Parse(rw.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null).ToString();
            //            //PrecioInicial_ = double.Parse(rw.Cells[indPrecioInicial].Value.ToString(), NumberStyles.Currency, null).ToString();
            //            PrecioInicial_ = PrecioInicial(rw.Cells[indIVA].Value.ToString(), rw.Cells[indIEPS].Value.ToString(), double.Parse(rw.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null)).ToString();

            //            Cantidad = rw.Cells[indQty].Value.ToString();
            //            if (rw.Cells[indIVA].Value != null)
            //                IVA___ = rw.Cells[indIVA].Value.ToString();
            //            if (rw.Cells[indIEPS].Value != null)
            //                IEPS___ = rw.Cells[indIEPS].Value.ToString();
            //            if (rw.Cells[indEsCaja].Value != null)
            //                esCaja__ = rw.Cells[indEsCaja].Value.ToString();
            //            if (rw.Cells[indUom].Value != null)
            //                uom__ = rw.Cells[indUom].Value.ToString();
            //            idPres = rw.Cells[indIdPres].Value.ToString();
            //            idMarca = rw.Cells[indIdMarca].Value.ToString();
            //            idLinea = rw.Cells[indIdLinea].Value.ToString();

            //            string fechaH = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //            ins = " EXEC dbo.PVAgregarProductoVenta " +
            //            " '" + folioVenta + "', " + IdProducto + ", " +
            //            " " + Cantidad + ", " + PrecioInicial_ + ", " +
            //            " " + 0 + ", " +
            //            " " + IVA___ + ", " + IEPS___ + ", " +
            //            " " + esCaja__ + ", " + uom__ + ", " +
            //            " '" + fechaH + "' ," + idUsuario + ", " + idPres + "," +
            //            " " + idMarca + ", " + idLinea + ";";

            //            sqlLoc.exec(ins);
            //        }

            //        nuevaVenta();
            //        VentasPendientes();
            //    }
            //    else
            //    {
            //        MessageBox.Show("No hay productos para la venta!", "Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //    }

            //}
            //else {
            //    MessageBox.Show("Ha ocurrido un error, intenta de nuevo!", "Venta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            //}



        }

        private void gvPendientes_CellContenDoubletClick(object sender, DataGridViewCellEventArgs e)
        {
            string folioPendiente = gvPendientes.Rows[e.RowIndex].Cells[0].Value.ToString();

            ReabrirFolio(folioPendiente);


            if (dgvVenta.Rows.Count > 0)
            {
                //recorrer el scroll hacia abajo
                //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                foreach (DataGridViewColumn column in dgvVenta.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (column.Name == "IdOrden") //colIndice
                    {
                        column.SortMode = DataGridViewColumnSortMode.Programmatic;
                        dgvVenta.Sort(column, ListSortDirection.Ascending);
                    }
                }
            }

            //FVenta venta = new FVenta(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, 
            //    dtProductos, imgLstCategorias, imgLstProductos, folioPendiente, false, numCaja);
            //this.Close();

            //venta.ShowDialog();

        }

        private void CBCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            modificacionPorCliente = true;
            string queryActualizarCliente = string.Empty;
            if (CBCliente.SelectedIndex != -1)
            {
                queryActualizarCliente = "UPDATE PVVentas SET idCliente = "+CBCliente.SelectedValue+ " WHERE FolioVenta LIKE '"+folioVenta+ "' AND Terminada = 0";
            }
            else
            {
                queryActualizarCliente = "UPDATE PVVentas SET idCliente = 0 WHERE FolioVenta LIKE '" + folioVenta + "' AND Terminada = 0";
            }
            sqlLoc.exec(queryActualizarCliente);

            CambiarPrecioClientes();
            modificacionPorCliente = false;
        }

        private void btnBuscarCodigo_Click(object sender, EventArgs e)
        {
            int idTipoCliente = ObtenerTipoCliente();

            FBuscadorCodigos fBuscador = new FBuscadorCodigos(sqlLoc, sql, idSucursal, idTipoCliente);
            fBuscador.ShowDialog();

            string idProducto = fBuscador.IdProducto;
            string cantidad = fBuscador.Cantidad;
            string idPresentacion = fBuscador.IdPresentacionVenta;

            //buscar por codigo de producto
            if (idProducto != "" && cantidad != "")
            {
                SoundPlayer player;

                //lblMsj.Text = "";

                //var existe = dtProductos.Select("Id='" + idProducto + "' AND Uom = " + cantidad + "");
                var existe = dtProductos.Select("IdPresentacionVenta='" + idPresentacion + "'");
                if (existe.Count() > 0)
                {

                    string IdProducto = existe[0]["Id"].ToString();
                    string esCaja = existe[0]["Escaja"].ToString();
                    string uom = existe[0]["Uom"].ToString();
                    string Producto = existe[0]["Producto"].ToString() +" "+ existe[0]["Presentación"].ToString();
                    string Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                    string PrecioFinal = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString("C2");
                    string Imagen = existe[0]["Foto"].ToString();
                    string IVA__ = existe[0]["IVA"].ToString();
                    string IEPS__ = existe[0]["IEPS"].ToString();
                    string idPres = existe[0]["IdPresentacionVenta"].ToString();
                    string idMarca = existe[0]["IdMarca"].ToString();
                    string idLinea = existe[0]["IdLinea"].ToString();
                    string sku = existe[0]["sku"].ToString();

                    switch (idTipoCliente)
                    {
                        case 1:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioGeneral"]).ToString("C2");
                            break;
                        case 2:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioTalleres"]).ToString("C2");
                            break;
                        case 3:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioDistribuidores"]).ToString("C2");
                            break;
                    }

                    try
                    {
                        bool.TryParse(existe[0]["Pesaje"].ToString(), out Pesaje);
                    }
                    catch
                    {

                    }
                    if (Pesaje)
                    {
                        // Envía el carácter 'P' para solicitar la lectura
                        lock (datosRecibidosLock)
                        {
                            datosRecibidos = false;
                        }
                        if (serialPort != null)
                            serialPort.Write("P");

                        // Esperar la recepción de datos
                        DateTime timeout = DateTime.Now.AddSeconds(5);
                        while (!datosRecibidos && DateTime.Now < timeout)
                        {
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(50);
                        }

                        if (datosRecibidos)
                        {
                            lock (datosRecibidosLock)
                            {
                                lblPeso.Text = pesoLeido;
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se recibió respuesta de la báscula.");
                        }
                    }
                    else
                    {
                        lblPeso.Text = "-----------";
                    }
            

                    bool yaEstaba = false;

                    if (sku == "")
                    {
                        if (dgvVenta.Rows.Count > 0)
                        {
                            int CantSku = 0;
                            //Si el grid ya contiene un servicio, no dejar agregar más.
                            foreach (DataGridViewRow gvr in dgvVenta.Rows)
                            {
                                string sku_servicio = gvr.Cells[indSku].Value.ToString();
                                if (sku_servicio != "")
                                {
                                    CantSku++;
                                }
                            }

                            if (CantSku > 0)
                            {
                                MessageBox.Show("No es posible mezclar servicio con producto. Favor de realizar las ventas por separado.",
                                                       "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                       MessageBoxDefaultButton.Button2);
                                goto Terminar;
                            }
                        }

                        double Peso = 0;
                        double.TryParse(lblPeso.Text.Trim().Replace("kg", "").Trim(), out Peso);

                        double Cantidad = 1;
                        if (Pesaje)
                            Cantidad = Peso;

                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            //Ya tenia el producto con la presentación seleccionada en la venta
                            if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                            //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                            {
                                //cantidad
                                gvr.Cells[indQty].Value = Convert.ToDouble(gvr.Cells[indQty].Value) + Cantidad;//1;
                                //total
                                gvr.Cells[indTotal].Value = Math.Round(Convert.ToDouble(gvr.Cells[indQty].Value) * double.Parse(gvr.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");
                                yaEstaba = true;
                                goto fin;
                                break;
                            }
                        }
                    }
                   
                fin:
                    if (!yaEstaba)
                    {
                        if (sku != "") {


                            if (dgvVenta.Rows.Count > 0)
                            {
                                int CantSku = 0;
                                int CantProd = 0;
                                //Si el grid ya contiene un servicio, no dejar agregar más.
                                foreach (DataGridViewRow gvr in dgvVenta.Rows)
                                {
                                    string sku_servicio = gvr.Cells[indSku].Value.ToString();
                                    if (sku_servicio != "")
                                    {
                                        CantSku++;
                                    }

                                    if (sku_servicio == "") {
                                        CantProd++;
                                    }
                                }

                                if (CantSku > 0) {
                                    MessageBox.Show("Solo se puede realizar el pago de un servicio a la vez.",
                                                           "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                           MessageBoxDefaultButton.Button2);
                                    goto Terminar;
                                }

                                if (CantProd > 0) {
                                    MessageBox.Show("No es posible mezclar servicio con producto. Favor de realizar las ventas por separado.",
                                                       "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                       MessageBoxDefaultButton.Button2);
                                    goto Terminar;
                                }


                            }


                            string NumeroTelefonico = "";
                            string ReferenciaServicio = "";
                            string MontoServicio = "";
                            string MontoComision = "";
                            

                            if (sku.Contains("JMAS") || sku.Contains("LUZCFE") || sku.Contains("GASNAT") || sku.Contains("ECOGAS") || sku.Contains("TELMEX") || sku.Contains("UBER") || sku.Contains("DISH") || sku.Contains("SKY")|| sku.Contains("IZI") || sku.Contains("CREDHIPOTECA") ||  sku.Contains("NETFLIX")) {
                                //Abre pantalla para ingresar número telefónico
                                FReferenciaServicio FServicio = new FReferenciaServicio(sqlLoc, sku);
                                FServicio.ShowDialog();

                                ReferenciaServicio = FServicio.ReferenciaServicio;
                                MontoServicio = FServicio.MontoServicio;
                                MontoComision = FServicio.MontoComision;

                                double MS = 0;
                                double MC = 0;

                                double.TryParse(MontoServicio.Replace("$", "").Replace(",", ""), out MS);
                                double.TryParse(MontoComision.Replace("$","").Replace(",",""), out MC);


                                if (sku.Contains("UBER"))
                                {
                                    MontoComision = "0";
                                    MC = 0;
                                }

                                double MontoTotal = 0;
                                MontoTotal = MS + MC;


                                if (ReferenciaServicio != "" && MontoServicio != "")
                                {
                                    bool ContieneReferenciaServicio = false;
                                    //Si el número ya existe , no agregarlo
                                    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                                    {
                                        string desc = gvr.Cells[indProducto].Value.ToString();

                                        //Ya tenia el producto con la presentación seleccionada en la venta
                                        if (desc.Contains(ReferenciaServicio))
                                        {
                                            ContieneReferenciaServicio = true;
                                        }
                                    }

                                    if (ContieneReferenciaServicio)
                                    {
                                        MessageBox.Show("La referencia ya se encuentra agregada.",
                                                        "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button2);
                                        goto fin;
                                    }
                                    else
                                    {
                                        //Verifica cuál es el último orden para agregarlo
                                        int noOrden = ObtieneOrdenProducto(idProducto);
                                        //Guarda en bd

                                        dgvVenta.Rows.Add(IdProducto, Producto.Trim().Replace("PIEZA (0)", "") + " # " + ReferenciaServicio, "-", 1, "+",
                                        MontoTotal.ToString("C2"), MontoTotal.ToString("C2"), MontoTotal.ToString("C2"),
                                        //PrecioFinal, Precio, PrecioFinal,
                                        //(dgvVenta.Rows.Count + 1),
                                        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku, MC, noOrden);

                                    }
                                }
                                else
                                {

                                    MessageBox.Show("No se agregó ninguna referencia.",
                                                           "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                           MessageBoxDefaultButton.Button2);
                                }
                            }
                            else {

                                //Abre pantalla para ingresar referencia de pago y monto
                                FRecargaSaldo FRecarga = new FRecargaSaldo();
                                FRecarga.ShowDialog();
                              
                                NumeroTelefonico = FRecarga.NumeroTelefonico;


                                if (NumeroTelefonico != "(   )    -  -" && NumeroTelefonico.Length == 15)
                                {
                                    bool ContieneNumeroTelefonico = false;
                                    //Si el número ya existe , no agregarlo
                                    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                                    {
                                        string desc = gvr.Cells[indProducto].Value.ToString();

                                        //Ya tenia el producto con la presentación seleccionada en la venta
                                        if (desc.Contains(NumeroTelefonico))
                                        {
                                            ContieneNumeroTelefonico = true;

                                        }
                                    }

                                    if (ContieneNumeroTelefonico)
                                    {
                                        MessageBox.Show("El número telefónico ya se encuentra agregado.",
                                                        "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button2);
                                        goto fin;
                                    }
                                    else
                                    {
                                        //Verifica cuál es el último orden para agregarlo
                                        int noOrden = ObtieneOrdenProducto(idProducto);

                                        dgvVenta.Rows.Add(IdProducto, Producto.Trim().Replace("PIEZA (0)", "") + " # " + NumeroTelefonico, "-", 1, "+", PrecioFinal, Precio, PrecioFinal,
                                        //(dgvVenta.Rows.Count + 1),
                                        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku, 0, noOrden);

                                    }
                                }
                                else
                                {

                                    MessageBox.Show("No se agregó ningún número telefónico.",
                                                           "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                           MessageBoxDefaultButton.Button2);
                                }
                            }

                        }
                        else {

                            //Verifica cuál es el último orden para agregarlo
                            int noOrden = ObtieneOrdenProducto(idProducto);
                         

                            double Peso = 0;
                            double.TryParse(lblPeso.Text.Trim().Replace("kg", "").Trim(), out Peso);

                            double Cantidad = 1;
                            if (Pesaje)
                                Cantidad = Peso;

                            dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", Cantidad, "+", PrecioFinal, Precio, PrecioFinal,
                                //(dgvVenta.Rows.Count + 1),
                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku,0, noOrden);

                        }

                        lblDescProd.Text = Producto;
                        lblPrecioProd.Text = PrecioFinal;
                        try
                        {
                            var img = @"C:\ImgPuntoVenta\Productos\" + Imagen;
                            pbImagen.Image = Image.FromFile(img);
                        }
                        catch
                        {
                            pbImagen.Image = null;
                        }
                    }

                    if (sku == "")
                    {
                        ExistePromocion(idPres);

                        double cantTotalProd = 0;
                        //Busca cantidad final del producto
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            //Ya tenia el producto con la presentación seleccionada en la venta
                            //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                            if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                            {
                                double cant = 0;
                                double.TryParse(gvr.Cells[indQty].Value.ToString(), out cant);
                                cantTotalProd += cant;

                            }
                        }
                        double PrecioF = 0;
                        double.TryParse(PrecioFinal.Replace("$", ""), out PrecioF);
                        int noOrden = ObtieneOrdenProducto(IdProducto);

                        BuscarPromocion(IdProducto, uom, Producto, Precio, IVA__, IEPS__, esCaja, idPres, idMarca, idLinea, cantTotalProd, PrecioF, noOrden);
                    }


                    ////if (dgvVenta.Rows.Count > 0)
                    ////{
                    ////    //recorrer el scroll hacia abajo
                    ////    //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                    ////    foreach (DataGridViewColumn column in dgvVenta.Columns)
                    ////    {
                    ////        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    ////        if (column.Name == "IdOrden") //colIndice
                    ////        {
                    ////            column.SortMode = DataGridViewColumnSortMode.Programmatic;
                    ////            dgvVenta.Sort(column, ListSortDirection.Ascending);
                    ////        }
                    ////    }
                    ////}
                    sumarTotal();
                    //txtCodigo.Focus();
                    txtScan.Focus();

                    //player = new SoundPlayer(Properties.Resources._202530__kalisemorrison__scanner_beep);
                    //player.Play();
                }
                else
                {
                    //player = new SoundPlayer(Properties.Resources._450616__breviceps__8_bit_error);
                    //player.Play();
                    //MessageBox.Show("No se encontró el producto",
                    //    "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information,
                    //    MessageBoxDefaultButton.Button2);
                    //txtScan.Focus();

                    try
                    {
                        PlayMP3("incorrecto.wav");
                    }
                    catch (Exception)
                    {

                    }
                    FAlerta Alerta = new FAlerta();
                    Alerta.ShowDialog();
                    txtScan.Focus();
                }
            }


            if (dgvVenta.Rows.Count > 0)
            {
                //recorrer el scroll hacia abajo
                //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                foreach (DataGridViewColumn column in dgvVenta.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (column.Name == "IdOrden") //colIndice
                    {
                        column.SortMode = DataGridViewColumnSortMode.Programmatic;
                        dgvVenta.Sort(column, ListSortDirection.Ascending);
                    }
                }
            }

           // lblPeso.Text = "-----------";
        Terminar:;
        }

        private void btnBorrarTxtCliente_Click(object sender, EventArgs e)
        {
            txtNombreCliente.Enabled = true;
            btnBorrarTxtCliente.Visible = false;
        }

        private void btnReimprimir_Click(object sender, EventArgs e)
        {
            FEspera espera = new FEspera();
            espera.Text = "IMPRIMIENDO TICKET...";
            espera.Show();

            string queryUltimaVentaTerminada = "SELECT TOP 1 FolioVenta FROM PVVentas where Terminada = 1 ORDER BY FechaVenta DESC";
            string FolioVenta = ((sqlLoc.selec(queryUltimaVentaTerminada).Rows)[0]["FolioVenta"]).ToString();

            FTicket ticket = new FTicket(sqlLoc, FolioVenta, true);
            ticket.ShowDialog();

            espera.Close();
        }

        private void txtNombreCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && txtNombreCliente.Text != string.Empty)
            {
                txtNombreCliente.Enabled = false;
                e.SuppressKeyPress = true;
                btnBorrarTxtCliente.Visible = true;
                InsertarNombreClienteVenta();
            }
            else
            {
                if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.F2 || e.KeyCode == Keys.F3 ||
                    e.KeyCode == Keys.F4 || e.KeyCode == Keys.F5 || e.KeyCode == Keys.F6 ||
                    e.KeyCode == Keys.F7 || e.KeyCode == Keys.F8 || e.KeyCode == Keys.F9 ||
                    e.KeyCode == Keys.F10 || e.KeyCode == Keys.Tab)
                {
                    Atajos(sender, e);
                }
                else
                {
                    toolTip.Show("Presiona ENTER para confirmar ", txtNombreCliente, 0, txtNombreCliente.Height - 110, 5000);
                }
            }
        }

        private int ObtieneOrdenProducto(string IdProducto) {
            //Verifica cuál es el último orden para agregarlo
            int noOrden = 0;
            var ordenProducto = sqlLoc.scalar(" SELECT ISNULL(Orden,0) Orden FROM PVOrdenProductos WHERE FolioVenta='" + folioVenta + "' AND IdProducto=" + IdProducto + " ");
            if (ordenProducto != null)
            {
                int.TryParse(ordenProducto.ToString(), out noOrden);
            }
            else
            {
                //Toma el último orden registrado para la venta
                var ordenProducto2 = sqlLoc.scalar(" SELECT MAX(ISNULL(Orden,0)) Orden FROM PVOrdenProductos WHERE FolioVenta='" + folioVenta + "'");
                if (ordenProducto2 != null)
                {
                    int.TryParse(ordenProducto2.ToString(), out noOrden);
                    noOrden ++;

                    //INSERTA registro
                    sqlLoc.exec("INSERT INTO PVOrdenProductos (FolioVenta, IdProducto, Orden) VALUES ('"+ folioVenta +"', "+ IdProducto +", "+ noOrden +")");
                }
            }

            return noOrden;
        }

        private void txtScan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtScan_Leave(sender, e);
            }
            if (e.KeyCode == Keys.Down && dgvVenta.CurrentCell != null)
            {
                dgvVenta.Select();
            }

            Atajos(sender, e);
        }

        private void txtNombreCliente_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelar_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(sender, e);
        }

        private void btnTerminar_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(sender, e);
        }

        private void fVenta_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(sender, e);
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '+' || e.KeyChar == '-')
            {
                e.Handled = true; // Se cancela el evento de tecla
            }
        }

        private void btnDevoluciones_Click(object sender, EventArgs e)
        {
            AbrirDevoluciones();
        }

        private void btnConsultarPrecio_Click(object sender, EventArgs e)
        {
            AbrirConsultaProductos();
        }

        private void dgvVenta_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                //2= resta
                //4= suma
                var dgvR = dgvVenta.Rows[e.RowIndex];
                

                if (e.ColumnIndex == indResta || e.ColumnIndex == indSum)
                {
                    if (dgvR.Cells[indSku].Value.ToString() != "") {
                        if (e.ColumnIndex == indSum) {
                            MessageBox.Show("No se puede modificar la cantidad para este tipo de producto",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                            goto TerminarFuncion;
                        }
                        else{
                            //cantidad
                            int qtyFinal2 = Convert.ToInt32(dgvR.Cells[indQty].Value);
                            if (e.ColumnIndex == indResta) //resta
                            {
                                qtyFinal2 = Convert.ToInt32(dgvR.Cells[indQty].Value) - 1;
                            }

                       
                            //nueva cantidad
                            dgvR.Cells[indQty].Value = qtyFinal2.ToString();
                            //total
                            dgvR.Cells[indTotal].Value = Math.Round(Convert.ToInt32(dgvR.Cells[indQty].Value) * double.Parse(dgvR.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");

                            if (qtyFinal2 == 0)
                            {
                                dgvVenta.Rows.Remove(dgvR);
                            }

                            goto TerminarFuncion;
                        }
                       
                    }
                    else {
                        goto Funcion;
                    }

                    Funcion:
                    //cantidad
                    int qtyFinal = Convert.ToInt32(dgvR.Cells[indQty].Value);
                    if (e.ColumnIndex == indResta) //resta
                    {
                        qtyFinal = Convert.ToInt32(dgvR.Cells[indQty].Value) - 1;
                    }

                    if (e.ColumnIndex == indSum) //suma
                    {
                        qtyFinal = Convert.ToInt32(dgvR.Cells[indQty].Value) + 1;
                    }

                    //nueva cantidad
                    dgvR.Cells[indQty].Value = qtyFinal.ToString();
                    //total
                    dgvR.Cells[indTotal].Value = Math.Round(Convert.ToInt32(dgvR.Cells[indQty].Value) * double.Parse(dgvR.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");

                    if (qtyFinal <= 0)
                    {
                        dgvVenta.Rows.Remove(dgvR);
                    }

                    var existeP = ((dtProductos.Select("Id='" + dgvR.Cells[indIdProd].Value + "'")).CopyToDataTable()).Select("Uom = " + dgvR.Cells[indUom].Value);
                    if (existeP.Count() > 0)
                    {
                        string IdProducto = existeP[0]["Id"].ToString();
                        string esCaja = existeP[0]["Escaja"].ToString();
                        string uom = existeP[0]["Uom"].ToString();
                        string Producto = existeP[0]["Producto"].ToString() + " " + existeP[0]["Presentación"].ToString();
                        string Precio = Convert.ToDouble(existeP[0]["Precio"]).ToString("C2");
                        string PrecioFinal = Convert.ToDouble(existeP[0]["PrecioFinal"]).ToString("C2");
                        string Imagen = existeP[0]["Foto"].ToString();
                        string IVA__ = existeP[0]["IVA"].ToString();
                        string IEPS__ = existeP[0]["IEPS"].ToString();
                        string idPres = existeP[0]["IdPresentacionVenta"].ToString();
                        string idMarca = existeP[0]["IdMarca"].ToString();
                        string idLinea = existeP[0]["IdLinea"].ToString();

                        switch (ObtenerTipoCliente())
                        {
                            case 1:
                                PrecioFinal = Convert.ToDouble(existeP[0]["PrecioGeneral"]).ToString("C2");
                                break;
                            case 2:
                                PrecioFinal = Convert.ToDouble(existeP[0]["PrecioTalleres"]).ToString("C2");
                                break;
                            case 3:
                                PrecioFinal = Convert.ToDouble(existeP[0]["PrecioDistribuidores"]).ToString("C2");
                                break;
                        }

                        lblDescProd.Text = Producto;
                        lblPrecioProd.Text = PrecioFinal;
                        try
                        {
                            var img = @"C:\ImgPuntoVenta\Productos\" + Imagen;
                            pbImagen.Image = Image.FromFile(img);
                        }
                        catch
                        {
                            pbImagen.Image = null;
                        }

                        //bool yaEstaba = false;
                        //foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        //{
                        //    //Ya tenia el producto con la presentación seleccionada en la venta
                        //    if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                        //    {
                        //        //cantidad
                        //        gvr.Cells[indQty].Value = Convert.ToInt32(gvr.Cells[indQty].Value) + 1;
                        //        //total
                        //        gvr.Cells[indTotal].Value = Math.Round(Convert.ToInt32(gvr.Cells[indQty].Value) * double.Parse(gvr.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");
                        //        yaEstaba = true;
                        //        goto fin;
                        //        break;
                        //    }
                        //}

                        //fin:
                        //    if (!yaEstaba)
                        //    {
                        //        dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", 1, "+", PrecioFinal, Precio, PrecioFinal,
                        //            //(dgvVenta.Rows.Count + 1),
                        //            IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);

                        //        lblDescProd.Text = Producto;
                        //        lblPrecioProd.Text = PrecioFinal;
                        //        try
                        //        {
                        //            var img = @"C:\PVLaJoya\Productos\" + Imagen;
                        //            pbImagen.Image = Image.FromFile(img);
                        //        }
                        //        catch
                        //        {
                        //            pbImagen.Image = null;
                        //        }
                        //    }

                        DataRow dtPromociones = ExistePromocion(idPres);
                        
                        double cantTotalProd = 0;
                        //Busca cantidad final del producto
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            //Ya tenia el producto con la presentación seleccionada en la venta
                            //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                            if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                            {
                                double cant = 0;
                                double.TryParse(gvr.Cells[indQty].Value.ToString(), out cant);
                                cantTotalProd += cant;

                            }
                        }
                        double PrecioF = 0;
                        double.TryParse(PrecioFinal.Replace("$", ""), out PrecioF);
                        int noOrden = ObtieneOrdenProducto(IdProducto);
                        if (dtPromociones != null)
                        {
                            BuscarPromocion(IdProducto, uom, Producto, Precio, IVA__, IEPS__, esCaja, idPres, idMarca, idLinea, cantTotalProd, PrecioF, noOrden);
                        }


                        if (dgvVenta.Rows.Count > 0)
                        {
                            //recorrer el scroll hacia abajo
                            //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                            foreach (DataGridViewColumn column in dgvVenta.Columns)
                            {
                                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                                if (column.Name == "IdOrden") //colIndice
                                {
                                    column.SortMode = DataGridViewColumnSortMode.Programmatic;
                                    dgvVenta.Sort(column, ListSortDirection.Ascending);
                                }
                            }
                        }
                        sumarTotal();
                        //txtCodigo.Focus();
                        txtScan.Focus();

                        //player = new SoundPlayer(Properties.Resources._202530__kalisemorrison__scanner_beep);
                        //player.Play();
                    }


                    if (dgvVenta.Rows.Count > 0)
                    {
                        foreach (DataGridViewColumn column in dgvVenta.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                            if (column.Name == "IdOrden")//colIndice
                            {
                                column.SortMode = DataGridViewColumnSortMode.Programmatic;
                                dgvVenta.Sort(column, ListSortDirection.Ascending);
                            }
                        }
                    }
                    sumarTotal();

                TerminarFuncion:;

                }
                else if (e.ColumnIndex == indQty)
                {

                }

                string idProducto = dgvR.Cells[indIdProd].Value.ToString();
                string idPresentacion = dgvR.Cells[indIdPres].Value.ToString();

                var existe = dtProductos.Select("idPresentacionVenta='" + idPresentacion + "'");
                if (existe.Count() > 0)
                {
                    string Producto = existe[0]["Producto"].ToString() + " " + existe[0]["Presentación"].ToString();
                    string Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                    string Imagen = existe[0]["Foto"].ToString();
                    string PrecioFinal = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString("C2");

                    switch (idTipoCliente)
                    {
                        case 1:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioGeneral"]).ToString("C2");
                            break;
                        case 2:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioTalleres"]).ToString("C2");
                            break;
                        case 3:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioDistribuidores"]).ToString("C2");
                            break;
                    }

                    lblDescProd.Text = Producto;
                    lblPrecioProd.Text = PrecioFinal;
                    try
                    {
                        var img = @"C:\ImgPuntoVenta\Productos\" + Imagen;
                        pbImagen.Image = Image.FromFile(img);
                    }
                    catch
                    {
                        pbImagen.Image = null;
                    }
                }

                sumarTotal();

                if (dgvVenta.Rows.Count > 0)
                {
                    //recorrer el scroll hacia abajo
                    //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                    foreach (DataGridViewColumn column in dgvVenta.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                        if (column.Name == "IdOrden") //colIndice
                        {
                            column.SortMode = DataGridViewColumnSortMode.Programmatic;
                            dgvVenta.Sort(column, ListSortDirection.Ascending);
                        }
                    }
                }
                if (SumarRestarPorTeclado)
                {
                    dgvVenta.Select();
                    SumarRestarPorTeclado = false;
                }
            }
        }

        private void btnCreditos_Click(object sender, EventArgs e)
        {
            AbrirCreditos();
        }

        private void txtScan_Leave(object sender, EventArgs e)
        {
            //buscar por codigo de barra
            if (txtScan.Text.Trim() != "")
            {
                //var existe = dtProductos.Select("CodigoBarras='" + txtScan.Text.Trim() + "'");

                //if (existe.Count() > 0)
                //{
                //    string IdProducto = existe[0]["Id"].ToString();
                //    string esCaja = existe[0]["Escaja"].ToString();
                //    string uom = existe[0]["Uom"].ToString();
                //    string Producto = existe[0]["Producto"].ToString() + existe[0]["Presentación"].ToString();
                //    string Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                //    string PrecioFinal = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString("C2");
                //    string Imagen = existe[0]["Foto"].ToString();
                //    string IVA__ = existe[0]["IVA"].ToString();
                //    string IEPS__ = existe[0]["IEPS"].ToString();
                //    string idPres = existe[0]["IdPresentacionVenta"].ToString();
                //    string idMarca = existe[0]["IdMarca"].ToString();
                //    string idLinea = existe[0]["IdLinea"].ToString();

                //    lblDescProd.Text = Producto;
                //    lblPrecioProd.Text = PrecioFinal;
                //    try
                //    {
                //        var img = @"C:\PuntoVenta\Productos\" + Imagen;
                //        pbImagen.Image = Image.FromFile(img);
                //    }
                //    catch
                //    {
                //        pbImagen.Image = null;
                //    }

                //    bool yaEstaba = false;
                //    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                //    {
                //        //Ya tenia el producto con la presentación seleccionada en la venta
                //        if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                //        //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                //        {
                //            //cantidad
                //            gvr.Cells[indQty].Value = Convert.ToInt32(gvr.Cells[indQty].Value) + 1;
                //            //total
                //            gvr.Cells[indTotal].Value = Math.Round(Convert.ToInt32(gvr.Cells[indQty].Value) * double.Parse(gvr.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");
                //            yaEstaba = true;
                //            lblDescProd.Text = Producto;
                //            lblPrecioProd.Text = PrecioFinal;

                //            try
                //            {
                //                var img = @"C:\PVLaJoya\Productos\" + Imagen;
                //                pbImagen.Image = Image.FromFile(img);
                //            }
                //            catch
                //            {
                //                pbImagen.Image = null;
                //            }

                //            break;
                //        }
                //    }

                //    if (!yaEstaba)
                //    {
                //        //dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", 1, "+", Precio, Precio, 
                //        //    IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);
                //        dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", 1, "+", PrecioFinal, Precio, PrecioFinal,
                //            //(dgvVenta.Rows.Count + 1),
                //            IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), "");

                //        lblDescProd.Text = Producto;
                //        lblPrecioProd.Text = PrecioFinal;
                //        try
                //        {
                //            var img = @"C:\PVLaJoya\Productos\" + Imagen;
                //            pbImagen.Image = Image.FromFile(img);
                //        }
                //        catch
                //        {
                //            pbImagen.Image = null;
                //        }
                //    }

                //    ExistePromocion(idPres);


                //    double cantTotalProd = 0;
                //    //Busca cantidad final del producto
                //    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                //    {
                //        //Ya tenia el producto con la presentación seleccionada en la venta
                //        if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                //        //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                //        {
                //            double cant = 0;
                //            double.TryParse(gvr.Cells[indQty].Value.ToString(), out cant);
                //            cantTotalProd += cant;

                //        }
                //    }
                //    double PrecioF = 0;
                //    double.TryParse(PrecioFinal.Replace("$","").Replace(",",""), out PrecioF);
                //    BuscarPromocion(IdProducto, uom, Producto, Precio, IVA__, IEPS__, esCaja, idPres, idMarca, idLinea, cantTotalProd, PrecioF);


                //    if (dgvVenta.Rows.Count > 0)
                //    {
                //        //recorrer el scroll hacia abajo
                //        //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                //        foreach (DataGridViewColumn column in dgvVenta.Columns)
                //        {
                //            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                //            if (column.Name == "colIndice")
                //            {
                //                column.SortMode = DataGridViewColumnSortMode.Programmatic;
                //                dgvVenta.Sort(column, ListSortDirection.Descending);
                //            }
                //        }
                //    }



                //    sumarTotal();
                //    txtScan.Focus();
                //}
                //Variables de info del Producto
                string IdProducto = string.Empty;
                string esCaja = string.Empty;
                string uom = string.Empty;
                string Producto = string.Empty;
                string Precio = string.Empty;
                string PrecioFinal = string.Empty;
                string Imagen = string.Empty;
                string IVA__ = string.Empty;
                string IEPS__ = string.Empty;
                string idPres = string.Empty;
                string idMarca = string.Empty;
                string idLinea = string.Empty;
                string sku = string.Empty;

                string cadenaCodigo = txtScan.Text.Trim();
                string banderaCodigo = cadenaCodigo.Substring(0, 3);
                if (banderaCodigo.Equals("200") && cadenaCodigo.Length == 13) //Condiciones para leer por codigo en pesa
                {
                    string cadenaCodigoProducto = cadenaCodigo.Substring(3,3);
                    var existeProducto = dtProductos.Select("CodigoBarras='" + cadenaCodigoProducto + "'");
                    if (existeProducto.Count() > 0)
                    {

                        string PrecioFinalPesa = cadenaCodigo.Substring(6);
                        PrecioFinalPesa = PrecioFinalPesa.Substring(0,4) + "." + PrecioFinalPesa.Substring(4);
                        Double PrecioPesa = Convert.ToDouble(PrecioFinalPesa);

                        Double precioUnitario = Convert.ToDouble(existeProducto[0]["Precio"]);

                        //Variable para cantidad en proporcion
                        Double cantidad = Math.Round(((1.0 / precioUnitario) * PrecioPesa), 2);

                        IdProducto = existeProducto[0]["Id"].ToString();
                        esCaja = existeProducto[0]["Escaja"].ToString();
                        uom = existeProducto[0]["Uom"].ToString();
                        Producto = existeProducto[0]["Producto"].ToString() + " " + existeProducto[0]["Presentación"].ToString();
                        Precio = Convert.ToDouble(existeProducto[0]["Precio"]).ToString("C2");
                        PrecioFinal = PrecioPesa.ToString("C2");
                        Imagen = existeProducto[0]["Foto"].ToString();
                        IVA__ = existeProducto[0]["IVA"].ToString();
                        IEPS__ = existeProducto[0]["IEPS"].ToString();
                        idPres = existeProducto[0]["IdPresentacionVenta"].ToString();
                        idMarca = existeProducto[0]["IdMarca"].ToString();
                        idLinea = existeProducto[0]["IdLinea"].ToString();
                        sku = existeProducto[0]["sku"].ToString();
             

                        int noOrden = ObtieneOrdenProducto(IdProducto);
                        dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", cantidad, "+", /*PrecioFinal*/Precio, Precio, PrecioFinal,
                            //(dgvVenta.Rows.Count + 1),
                            IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku, 0, noOrden);
                    }
                }


                var existe = dtProductos.Select("CodigoBarras='" + txtScan.Text.Trim() + "'");
                if (existe.Count() > 0)
                {
                    IdProducto = existe[0]["Id"].ToString();
                    esCaja = existe[0]["Escaja"].ToString();
                    uom = existe[0]["Uom"].ToString();
                    Producto = existe[0]["Producto"].ToString() + " " + existe[0]["Presentación"].ToString();
                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                    PrecioFinal = Convert.ToDouble(existe[0]["PrecioFinal"]).ToString("C2");
                    Imagen = existe[0]["Foto"].ToString();
                    IVA__ = existe[0]["IVA"].ToString();
                    IEPS__ = existe[0]["IEPS"].ToString();
                    idPres = existe[0]["IdPresentacionVenta"].ToString();
                    idMarca = existe[0]["IdMarca"].ToString();
                    idLinea = existe[0]["IdLinea"].ToString();
                    sku = existe[0]["sku"].ToString();
                    bool.TryParse(existe[0]["Pesaje"].ToString(), out Pesaje);

                    switch (ObtenerTipoCliente())
                    {
                        case 1:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioGeneral"]).ToString("C2");
                            break;
                        case 2:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioTalleres"]).ToString("C2");
                            break;
                        case 3:
                            PrecioFinal = Convert.ToDouble(existe[0]["PrecioDistribuidores"]).ToString("C2");
                            break;
                    }


                    if (Pesaje)
                    {
                        // Envía el carácter 'P' para solicitar la lectura
                        lock (datosRecibidosLock)
                        {
                            datosRecibidos = false;
                        }
                        serialPort.Write("P");

                        // Esperar la recepción de datos
                        DateTime timeout = DateTime.Now.AddSeconds(5);
                        while (!datosRecibidos && DateTime.Now < timeout)
                        {
                            Application.DoEvents();
                            System.Threading.Thread.Sleep(50);
                        }

                        if (datosRecibidos)
                        {
                            lock (datosRecibidosLock)
                            {
                                lblPeso.Text = pesoLeido;
                            }
                        }
                        else
                        {
                            MessageBox.Show("No se recibió respuesta de la báscula.");
                        }
                    }
                    else
                    {
                        lblPeso.Text = "-----------";
                    }



                    double Peso = 0;
                    double.TryParse(lblPeso.Text.Trim().Replace("kg", "").Trim(), out Peso);


                    double Cantidad = 1;
                    if (Pesaje)
                        Cantidad = Peso;



                    bool yaEstaba = false;
                    if (sku == "")
                    {
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            //Ya tenia el producto con la presentación seleccionada en la venta
                            if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                            //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                            {
                                //cantidad
                                gvr.Cells[indQty].Value = Convert.ToDouble(gvr.Cells[indQty].Value) + Cantidad; //+1
                                //total
                                gvr.Cells[indTotal].Value = Math.Round(Convert.ToDouble(gvr.Cells[indQty].Value) * double.Parse(gvr.Cells[indPrecio].Value.ToString(), NumberStyles.Currency, null), 2).ToString("C2");
                                yaEstaba = true;
                                goto fin;
                                break;
                            }
                        }
                    }


                fin:
                    if (!yaEstaba)
                    {
                        if (sku != "")
                        {
                            string NumeroTelefonico = "";
                            string ReferenciaServicio = "";
                            string MontoServicio = "";
                            string MontoComision = "";

                            if (sku.Contains("JMAS") || sku.Contains("LUZCFE") || sku.Contains("GASNAT") || sku.Contains("ECOGAS") || sku.Contains("TELMEX") || sku.Contains("UBER") || sku.Contains("DISH") || sku.Contains("SKY") || sku.Contains("IZI") || sku.Contains("CREDHIPOTECA") || sku.Contains("NETFLIX")) 
                            {
                                //Abre pantalla para ingresar número telefónico
                                FReferenciaServicio FServicio = new FReferenciaServicio(sqlLoc, sku);
                                FServicio.ShowDialog();

                                ReferenciaServicio = FServicio.ReferenciaServicio;
                                MontoServicio = FServicio.MontoServicio;
                                MontoComision = FServicio.MontoComision;

                                double MS = 0;
                                double MC = 0;

                                double.TryParse(MontoServicio, out MS);
                                double.TryParse(MontoComision, out MC);

                                double MontoTotal = 0;

                                if (sku.Contains("UBER"))
                                {
                                    MontoComision = "0";
                                    MC = 0;
                                }

                                MontoTotal = MS + MC;


                                    

                                if (ReferenciaServicio != "" && MontoServicio != "")
                                {
                                    bool ContieneReferenciaServicio = false;
                                    //Si el número ya existe , no agregarlo
                                    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                                    {
                                        string desc = gvr.Cells[indProducto].Value.ToString();

                                        //Ya tenia el producto con la presentación seleccionada en la venta
                                        if (desc.Contains(ReferenciaServicio))
                                        {
                                            ContieneReferenciaServicio = true;
                                        }
                                    }

                                    if (ContieneReferenciaServicio)
                                    {
                                        MessageBox.Show("La referencia ya se encuentra agregada.",
                                                        "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button2);
                                        goto fin;
                                    }
                                    else
                                    {
                                        int noOrden = ObtieneOrdenProducto(IdProducto);
                                        dgvVenta.Rows.Add(IdProducto, Producto.Trim().Replace("PIEZA (0)", "") + " # " + ReferenciaServicio, "-", 1, "+",
                                        MontoTotal, MontoTotal, MontoTotal,
                                        //PrecioFinal, Precio, PrecioFinal,
                                        //(dgvVenta.Rows.Count + 1),
                                        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku, MC, noOrden);

                                    }
                                }
                                else
                                {

                                    MessageBox.Show("No se agregó ninguna referencia.",
                                                           "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                           MessageBoxDefaultButton.Button2);
                                }
                            }
                            else
                            {

                                //Abre pantalla para ingresar referencia de pago y monto
                                FRecargaSaldo FRecarga = new FRecargaSaldo();
                                FRecarga.ShowDialog();

                                NumeroTelefonico = FRecarga.NumeroTelefonico;


                                if (NumeroTelefonico != "(   )    -  -" && NumeroTelefonico.Length == 15)
                                {
                                    bool ContieneNumeroTelefonico = false;
                                    //Si el número ya existe , no agregarlo
                                    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                                    {
                                        string desc = gvr.Cells[indProducto].Value.ToString();

                                        //Ya tenia el producto con la presentación seleccionada en la venta
                                        if (desc.Contains(NumeroTelefonico))
                                        {
                                            ContieneNumeroTelefonico = true;

                                        }
                                    }

                                    if (ContieneNumeroTelefonico)
                                    {
                                        MessageBox.Show("El número telefónico ya se encuentra agregado.",
                                                        "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button2);
                                        goto fin;
                                    }
                                    else
                                    {
                                        int noOrden = ObtieneOrdenProducto(IdProducto);
                                        dgvVenta.Rows.Add(IdProducto, Producto.Trim().Replace("PIEZA (0)", "") + " # " + NumeroTelefonico, "-", 1, "+", PrecioFinal, Precio, PrecioFinal,
                                        //(dgvVenta.Rows.Count + 1),
                                        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku, 0, noOrden);

                                    }
                                }
                                else
                                {

                                    MessageBox.Show("No se agregó ningún número telefónico.",
                                                           "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                                                           MessageBoxDefaultButton.Button2);
                                }
                            }





                        }
                        else
                        {



                            int noOrden = ObtieneOrdenProducto(IdProducto);
                            dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", Cantidad, "+", PrecioFinal, Precio, PrecioFinal,
                                //(dgvVenta.Rows.Count + 1),
                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), sku, 0, noOrden);

                        }

                        lblDescProd.Text = Producto;
                        lblPrecioProd.Text = PrecioFinal;
                        try
                        {
                            var img = @"C:\ImgPuntoVenta\Productos\" + Imagen;
                            pbImagen.Image = Image.FromFile(img);
                        }
                        catch
                        {
                            pbImagen.Image = null;
                        }
                    }

                    if (sku == "")
                    {
                        ExistePromocion(idPres);

                        double cantTotalProd = 0;
                        //Busca cantidad final del producto
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            //Ya tenia el producto con la presentación seleccionada en la venta
                            //if (gvr.Cells[indIdProd].Value.ToString() == IdProducto && gvr.Cells[indUom].Value.ToString() == uom)
                            if (gvr.Cells[indIdPres].Value.ToString() == idPres)
                            {
                                double cant = 0;
                                double.TryParse(gvr.Cells[indQty].Value.ToString(), out cant);
                                cantTotalProd += cant;

                            }
                        }
                        double PrecioF = 0;
                        double.TryParse(PrecioFinal.Replace("$", ""), out PrecioF);
                        int noOrden = ObtieneOrdenProducto(IdProducto);
                        BuscarPromocion(IdProducto, uom, Producto, Precio, IVA__, IEPS__, esCaja, idPres, idMarca, idLinea, cantTotalProd, PrecioF, noOrden);
                    }


                    if (dgvVenta.Rows.Count > 0)
                    {
                        //recorrer el scroll hacia abajo
                        //dgvVenta.FirstDisplayedScrollingRowIndex = dgvVenta.RowCount - 1;
                        foreach (DataGridViewColumn column in dgvVenta.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                            if (column.Name == "IdOrden") //colIndice
                            {
                                column.SortMode = DataGridViewColumnSortMode.Programmatic;
                                dgvVenta.Sort(column, ListSortDirection.Ascending);
                            }
                        }
                    }
                    sumarTotal();
                    //txtCodigo.Focus();
                    txtScan.Focus();

                    //player = new SoundPlayer(Properties.Resources._202530__kalisemorrison__scanner_beep);
                    //player.Play();
                }
                else
                {
                    //player = new SoundPlayer(Properties.Resources._450616__breviceps__8_bit_error);
                    //player.Play();
                    //MessageBox.Show("No se encontró el producto",
                    //    "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information,
                    //    MessageBoxDefaultButton.Button2);
                    //txtScan.Focus();

                    try
                    {
                        PlayMP3("incorrecto.wav");
                    }
                    catch (Exception)
                    {

                    }
                    FAlerta Alerta = new FAlerta();
                    Alerta.ShowDialog();
                    txtScan.Focus();
                }
            }
            txtScan.Text = "";
        }

        private void btnCortes_Click(object sender, EventArgs e)
        {
            AbrirMenuCortes();
        }

        private void btnEnvioVentas_Click(object sender, EventArgs e)
        {
            AbrirEnvioDeVentas();
        }

        public void PlayMP3(string rutaArchivo)
        {
            // SoundPlayer simpleSound = new SoundPlayer(@"c:\"+ rutaArchivo +".wav");
            SoundPlayer sonido = new SoundPlayer(Application.StartupPath + "\\" + rutaArchivo);
            sonido.Play();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            DesplegarComboCliente(sender);
        }

        private void CBCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                KeyEventArgs esc = new KeyEventArgs(Keys.Tab);
                Atajos(sender, esc);
            }
            else
            {
                Atajos(sender, e);
            }
        }

        private void dgvVenta_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int IndexPrecio = 5;

            if (e.ColumnIndex == IndexPrecio)
            {
                CambiarPrecioEnVenta();
            }
        }

        private void dgvVenta_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            CambiarPrecioPorUsuario(e);
        }

        private double CantTotalProd(string IdPres) {

            double cantidad = 0;

            foreach (DataGridViewRow dr in dgvVenta.Rows) {
                if (dr.Cells[indIdPres].Value.ToString() == IdPres) {
                    cantidad += Convert.ToDouble(dr.Cells[indQty].Value);
                }
            }

            return cantidad;
        }

        private double CantTotalProdCombinados(string[] IdsProductos)
        {

            double cantidad = 0;

            foreach (DataGridViewRow dr in dgvVenta.Rows)
            {
                if (IdsProductos.Contains(dr.Cells[indIdPres].Value.ToString()))
                {
                    cantidad += Convert.ToDouble(dr.Cells[indQty].Value);
                }
            }

            return cantidad;
        }

        private void btnFacturar_Click(object sender, EventArgs e)
        {
            AbrirFacturacion();
        }

        private void gvPendientes_Enter(object sender, EventArgs e)
        {
            if (gvPendientes.Rows.Count > 0)
            {
                gvPendientes.FirstDisplayedScrollingRowIndex = 0;
                gvPendientes.FirstDisplayedScrollingColumnIndex = 1;
                gvPendientes.Rows[0].Selected = true;
            }
            gvPendientes.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 134, 228); ;
            gvPendientes.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void gvPendientes_Leave(object sender, EventArgs e)
        {
            if (gvPendientes.Rows.Count > 0)
            {
                gvPendientes.FirstDisplayedScrollingRowIndex = 0;
                gvPendientes.FirstDisplayedScrollingColumnIndex = 1;
            }
            gvPendientes.DefaultCellStyle.SelectionBackColor = gvPendientes.DefaultCellStyle.BackColor;
            gvPendientes.DefaultCellStyle.SelectionForeColor = gvPendientes.DefaultCellStyle.ForeColor;
        }

        private void gvPendientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DataGridViewCellEventArgs ex = new DataGridViewCellEventArgs(0,gvPendientes.CurrentCell.RowIndex);
                gvPendientes_CellContenDoubletClick(sender, ex);
            }
            else
            {
                if (e.KeyCode == Keys.Tab)
                {
                    e.Handled = true;
                }
                if (e.KeyCode == Keys.Back)
                {
                    if (gvPendientes.Rows.Count > 0)
                    {
                        var count = gvPendientes.SelectedRows[0].Cells[1].Value;
                        if (count != null)
                        {
                            BorrarVentaPendiente(count.ToString());
                        }
                    }
                }
                Atajos(sender, e);
            }
        }

        private void BorrarVentaPendiente(string FolioVenta)
        {
            DialogResult dr = MessageBox.Show("Eliminar venta con folio " + FolioVenta + "?", "Eliminar", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            if (dr == DialogResult.Yes)
            {
                string Folio = gvPendientes.Rows[gvPendientes.CurrentCell.RowIndex].Cells[0].Value.ToString();
                string QueryCancelarPendiente = "UPDATE PVVentas SET Cancelado = 1 WHERE FolioVenta = '" + Folio + "';";
                int execute = sqlLoc.exec(QueryCancelarPendiente);
                VentasPendientes();
            }
        }

        private void btnAbrirCajon_Click(object sender, EventArgs e)
        {
            ExpulsarCajon();
        }

        private void dgvVenta_Enter(object sender, EventArgs e)
        {
            if (dgvVenta.Rows.Count > 0)
            {
                dgvVenta.Rows[0].Cells[0].Selected = true;
                dgvVenta.CurrentCell = dgvVenta.Rows[0].Cells[1];
            }
            dgvVenta.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 134, 228); ;
            dgvVenta.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void dgvVenta_Leave(object sender, EventArgs e)
        {
            dgvVenta.DefaultCellStyle.SelectionBackColor = dgvVenta.DefaultCellStyle.BackColor;
            dgvVenta.DefaultCellStyle.SelectionForeColor = dgvVenta.DefaultCellStyle.ForeColor;
        }

        private double SumarTotalIndividual(string ListaProductosPromo)
        {
            double cantidad = 0;
            var ListProds = ListaProductosPromo.Split(',');
            foreach (DataGridViewRow dr in dgvVenta.Rows)
            {
                if (ListProds.Contains(dr.Cells[indIdPres].Value.ToString()))
                {
                    cantidad += Convert.ToDouble(dr.Cells[indQty].Value);
                }
            }

            return cantidad;
        }


        private double BuscarPromocion(string IdProducto, string uom, string Producto, string Precio,
            string IVA__, string IEPS__, string esCaja, string idPres, string idMarca, string idLinea,  
            double CantidadSolicitada, double PrecioActual, int nOrden)
        {

            double PrecioFinal = PrecioActual;

            List<DataGridViewRow> quitarRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow gvr in dgvVenta.Rows)
            {
                //Ya tenia el producto con la presentación seleccionada en la venta
                if (gvr.Cells[indIdPres].Value.ToString() == idPres ) //&& gvr.Cells[indUom].Value.ToString() == uom
                {
                    //dgvVenta.Rows.Remove(gvr);
                    quitarRows.Add(gvr);

                   
                }
            }


            //Revisa si hay promoción
            DataRow dr = ExistePromocion(idPres);
            if (dr != null)
            {
                string IdPromocion = dr["IdPromocion"].ToString();
                bool Multiplo = false;
                bool.TryParse(dr["Multiplo"].ToString(), out Multiplo);

                bool Individual = false;
                bool.TryParse(dr["Individual"].ToString(), out Individual);

                bool Combinado = false;
                bool.TryParse(dr["Combinado"].ToString(), out Combinado);

                double PrecioPromo = 0;
                double.TryParse(dr["PrecioPromocion"].ToString(), out PrecioPromo);

                double CantidadPromo = 0;
                double.TryParse(dr["Cantidad"].ToString(), out CantidadPromo);
                
                bool VariosProd = dr["Productos"].ToString().Contains(",");

                List<string> listaProductos = new List<string>();

                if (VariosProd && !Individual && !Combinado) // 
                {
                    int cont = 0;
                    //Cuando existe promoción de varios productos en una sola.
                    var arr = dr["Productos"].ToString().Split(',');
                    foreach (DataGridViewRow gvr in dgvVenta.Rows)
                    {
                        if (arr.Contains(gvr.Cells[indIdPres].Value))
                        {
                            //double cantComprada = 0;
                            //double.TryParse(gvr.Cells[indQty].Value.ToString(), out cantComprada);

                            if (CantTotalProd(gvr.Cells[indIdPres].Value.ToString()) >= CantidadPromo && !listaProductos.Contains(gvr.Cells[indIdPres].Value.ToString()))
                            {
                                listaProductos.Add(gvr.Cells[indIdPres].Value.ToString());
                                cont++;
                            }
                        }
                    }



                    if (cont == arr.Count())
                    {
                        //Verificar cuánta cantidad sobra de c/producto.

                        double ConPromo = 0;
                        double SinPromo = 0;

                        DataTable dtPromos = new DataTable();
                        dtPromos.Columns.Add("IdProducto", typeof(int));
                        dtPromos.Columns.Add("IdPressProd", typeof(int));
                        dtPromos.Columns.Add("Producto", typeof(string));
                        dtPromos.Columns.Add("PrecioPromo", typeof(double));
                        dtPromos.Columns.Add("Cantidad", typeof(double));
                        dtPromos.Columns.Add("Uom", typeof(string));
                        dtPromos.Columns.Add("Precio", typeof(string));
                        dtPromos.Columns.Add("NoOrden", typeof(int));


                        List<string> listaProductos2 = new List<string>();
                        List<DataGridViewRow> quitarRows2 = new List<DataGridViewRow>();
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                           
                            if (arr.Contains(gvr.Cells[indIdPres].Value) && !listaProductos2.Contains(gvr.Cells[indIdPres].Value.ToString()))
                            {
                                int noOrden = ObtieneOrdenProducto(gvr.Cells[indIdProd].Value.ToString());
                                double cantComprada = CantTotalProd(gvr.Cells[indIdPres].Value.ToString());

                                if (Multiplo)
                                {

                                    if (CantidadPromo <= cantComprada)
                                    {
                                        double CantidadProdPromo = Math.Floor(cantComprada / CantidadPromo);
                                        //dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadPromo * CantidadProdPromo, "+", PrecioPromo, PrecioPromo, PrecioPromo,
                                        //       IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);

                                        //Revisar si los demás productos también cumplen con la promo (Si la cantidad del producto actual es igual a la cantidad de los productos que tienen promoción)
                                        //----------------- PENDIENTE MULTIPLO POR VARIOS PRODUCTOS --------------------

                                        
                                        dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioPromo, CantidadProdPromo * CantidadPromo, gvr.Cells[indUom].Value, 0, noOrden);


                                        double CantidadSinPromo = cantComprada % CantidadPromo;
                                        if (CantidadSinPromo > 0)
                                        {
                                            //dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadSinPromo, "+", PrecioFinal, Precio, PrecioFinal,
                                            //   IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);
                                            dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioFinal, CantidadSinPromo, gvr.Cells[indUom].Value,0, noOrden);
                                        }

                                    }

                                    //if ((CantidadPromo > cantComprada))
                                    //{
                                    //    dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", cantComprada, "+", PrecioFinal, Precio, PrecioFinal,
                                    //    IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea);
                                    //}

                                    listaProductos2.Add(gvr.Cells[indIdPres].Value.ToString());
                                }
                                else
                                {
                                    ConPromo = Math.Floor(cantComprada / CantidadPromo);
                                    if (ConPromo > 0)
                                        dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioPromo, ConPromo * CantidadPromo, gvr.Cells[indUom].Value, 0, noOrden);

                                    SinPromo = Math.Ceiling(cantComprada % CantidadPromo);
                                    if (SinPromo > 0)
                                        dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioFinal, SinPromo, gvr.Cells[indUom].Value, 0, noOrden);

                                    listaProductos2.Add(gvr.Cells[indIdPres].Value.ToString());
                                }
                            }

                            if (arr.Contains(gvr.Cells[indIdPres].Value))
                                quitarRows2.Add(gvr);


                        }


                        foreach (DataGridViewRow gvr in quitarRows2)
                        {
                            dgvVenta.Rows.Remove(gvr);
                        }

                        //Agregar rows con promo y sin promo de los productos selecionados
                        if (dtPromos.Rows.Count > 0)
                        {
                            foreach (DataRow drow in dtPromos.Rows)
                            {

                                //var existe = dtProductos.Select("Id = '" + drow["IdProducto"].ToString() + "' AND Uom = " + drow["Uom"].ToString() + "");
                                var existe = dtProductos.Select("IdPresentacionVenta='" + drow["IdPressProd"].ToString() + "'");
                                if (existe.Count() > 0)
                                {
                                    IdProducto = existe[0]["Id"].ToString();
                                    esCaja = existe[0]["Escaja"].ToString();
                                    uom = existe[0]["Uom"].ToString();
                                    Producto = existe[0]["Producto"].ToString() + existe[0]["Presentación"].ToString();
                                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                                    string Imagen = existe[0]["Foto"].ToString();
                                    IVA__ = existe[0]["IVA"].ToString();
                                    IEPS__ = existe[0]["IEPS"].ToString();
                                    idPres = existe[0]["IdPresentacionVenta"].ToString();
                                    idMarca = existe[0]["IdMarca"].ToString();
                                    idLinea = existe[0]["IdLinea"].ToString();
                                }

                                double PrecioProm = 0;
                                double.TryParse(drow["PrecioPromo"].ToString(), out PrecioProm);

                                double Cant = 0;
                                double.TryParse(drow["Cantidad"].ToString(), out Cant);


                                int noOrden = 0;
                                int.TryParse(drow["NoOrden"].ToString(), out noOrden);

                                dgvVenta.Rows.Add(IdProducto, Producto, "-", drow["Cantidad"].ToString(), "+", PrecioProm.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioProm).ToString("C2"), (Cant * PrecioProm).ToString("C2"),
                                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), "", 0, noOrden);

                            }

                        }

                    }

                }
                else if (VariosProd && Individual && !Combinado) {

                    DataTable dtPromos = new DataTable();
                    dtPromos.Columns.Add("IdProducto", typeof(int));
                    dtPromos.Columns.Add("IdPressProd", typeof(int));
                    dtPromos.Columns.Add("Producto", typeof(string));
                    dtPromos.Columns.Add("PrecioPromo", typeof(double));
                    dtPromos.Columns.Add("Cantidad", typeof(double));
                    dtPromos.Columns.Add("Uom", typeof(string));
                    dtPromos.Columns.Add("Precio", typeof(string));
                    dtPromos.Columns.Add("NoOrden", typeof(int));

                    var SumaCantSol = SumarTotalIndividual(dr["Productos"].ToString());
                    var ProdPromoIndividual = dr["Productos"].ToString().Split(',');

                    List<DataGridViewRow> listaBorrarIndividual = new List<DataGridViewRow>();

                    if (SumaCantSol >= CantidadPromo)
                    {
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                           
                            if (ProdPromoIndividual.Contains(gvr.Cells[indIdPres].Value.ToString()))
                            {
                                int noOrden = ObtieneOrdenProducto(gvr.Cells[indIdProd].Value.ToString());
                                listaBorrarIndividual.Add(gvr);
                                dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioPromo, gvr.Cells[indQty].Value, gvr.Cells[indUom].Value, 0, noOrden);
                            }
                        }

                        foreach (DataGridViewRow item in listaBorrarIndividual)
                        {
                            dgvVenta.Rows.Remove(item);
                        }

                        //Agregar productos al grid
                        if (dtPromos.Rows.Count > 0)
                        {
                            foreach (DataRow drow in dtPromos.Rows)
                            {

                                var existe = dtProductos.Select("IdPresentacionVenta = " + drow["IdPressProd"].ToString() + "");

                                if (existe.Count() > 0)
                                {
                                    IdProducto = existe[0]["Id"].ToString();
                                    esCaja = existe[0]["Escaja"].ToString();
                                    uom = existe[0]["Uom"].ToString();
                                    Producto = existe[0]["Producto"].ToString() + existe[0]["Presentación"].ToString();
                                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                                    string Imagen = existe[0]["Foto"].ToString();
                                    IVA__ = existe[0]["IVA"].ToString();
                                    IEPS__ = existe[0]["IEPS"].ToString();
                                    idPres = existe[0]["IdPresentacionVenta"].ToString();
                                    idMarca = existe[0]["IdMarca"].ToString();
                                    idLinea = existe[0]["IdLinea"].ToString();
                                }

                                double PrecioProm = 0;
                                double.TryParse(drow["PrecioPromo"].ToString(), out PrecioProm);

                                double Cant = 0;
                                double.TryParse(drow["Cantidad"].ToString(), out Cant);

                                int noOrden = 0;
                                int.TryParse(drow["NoOrden"].ToString(), out noOrden);

                                dgvVenta.Rows.Add(IdProducto, Producto, "-", drow["Cantidad"].ToString(), "+", PrecioProm.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioProm).ToString("C2"), (Cant * PrecioProm).ToString("C2"),
                                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, noOrden);
                            }
                        }

                    }
                    else {
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                          

                            if (ProdPromoIndividual.Contains(gvr.Cells[indIdPres].Value.ToString()))
                            {
                                int noOrden = ObtieneOrdenProducto(gvr.Cells[indIdProd].Value.ToString());
                                listaBorrarIndividual.Add(gvr);
                                dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioPromo, gvr.Cells[indQty].Value, gvr.Cells[indUom].Value, 0, noOrden);
                            }
                        }

                        foreach (DataGridViewRow item in listaBorrarIndividual)
                        {
                            dgvVenta.Rows.Remove(item);
                        }

                        //Agregar productos al grid
                        if (dtPromos.Rows.Count > 0)
                        {
                            foreach (DataRow drow in dtPromos.Rows)
                            {

                                var existe = dtProductos.Select("IdPresentacionVenta = " + drow["IdPressProd"].ToString() + "");

                                if (existe.Count() > 0)
                                {
                                    IdProducto = existe[0]["Id"].ToString();
                                    esCaja = existe[0]["Escaja"].ToString();
                                    uom = existe[0]["Uom"].ToString();
                                    Producto = existe[0]["Producto"].ToString() + existe[0]["Presentación"].ToString();
                                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                                    string Imagen = existe[0]["Foto"].ToString();
                                    IVA__ = existe[0]["IVA"].ToString();
                                    IEPS__ = existe[0]["IEPS"].ToString();
                                    idPres = existe[0]["IdPresentacionVenta"].ToString();
                                    idMarca = existe[0]["IdMarca"].ToString();
                                    idLinea = existe[0]["IdLinea"].ToString();
                                }

                                double PrecioF = 0;
                                double.TryParse(existe[0]["PrecioFinal"].ToString(), out PrecioF);

                                double Cant = 0;
                                double.TryParse(drow["Cantidad"].ToString(), out Cant);

                                int noOrden = 0;
                                int.TryParse(drow["NoOrden"].ToString(), out noOrden); 

                                dgvVenta.Rows.Add(IdProducto, Producto, "-", drow["Cantidad"].ToString(), "+", PrecioF.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioF).ToString("C2"), (Cant * PrecioF).ToString("C2"),
                                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, noOrden);
                            }
                        }

                    }

                }
                else if (VariosProd && !Individual && Combinado) {
                    int cont = 0;
                    //Cuando existe promoción de varios productos en una sola.
                    var arr = dr["Productos"].ToString().Split(',');

                    if (CantTotalProdCombinados(arr) >= CantidadPromo && !listaProductos.Contains(idPres))
                    {
                        listaProductos.Add(idPres);

                        DataTable dtPromos = new DataTable();
                        dtPromos.Columns.Add("IdProducto", typeof(int));
                        dtPromos.Columns.Add("IdPressProd", typeof(int));
                        dtPromos.Columns.Add("Producto", typeof(string));
                        dtPromos.Columns.Add("PrecioPromo", typeof(double));
                        dtPromos.Columns.Add("Cantidad", typeof(double));
                        dtPromos.Columns.Add("Uom", typeof(string));
                        dtPromos.Columns.Add("Precio", typeof(string));
                        dtPromos.Columns.Add("NoOrden", typeof(int));

                        List<string> listaProductos2 = new List<string>();
                        List<DataGridViewRow> quitarRows2 = new List<DataGridViewRow>();
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            if (arr.Contains(gvr.Cells[indIdPres].Value))
                            {
                                int noOrden = ObtieneOrdenProducto(gvr.Cells[indIdProd].Value.ToString());

                                dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioPromo, gvr.Cells[indQty].Value, gvr.Cells[indUom].Value, 0, noOrden);
                                listaProductos2.Add(gvr.Cells[indIdPres].Value.ToString());
                            }

                            if (arr.Contains(gvr.Cells[indIdPres].Value))
                                quitarRows2.Add(gvr);
                        }


                        foreach (DataGridViewRow gvr in quitarRows2)
                        {
                            dgvVenta.Rows.Remove(gvr);
                        }

                        //Agregar rows con promo y sin promo de los productos selecionados
                        if (dtPromos.Rows.Count > 0)
                        {
                            foreach (DataRow drow in dtPromos.Rows)
                            {

                                //var existe = dtProductos.Select("Id = '" + drow["IdProducto"].ToString() + "' AND Uom = " + drow["Uom"].ToString() + "");
                                var existe = dtProductos.Select("IdPresentacionVenta='" + drow["IdPressProd"].ToString() + "'");
                                if (existe.Count() > 0)
                                {
                                    IdProducto = existe[0]["Id"].ToString();
                                    esCaja = existe[0]["Escaja"].ToString();
                                    uom = existe[0]["Uom"].ToString();
                                    Producto = existe[0]["Producto"].ToString() + existe[0]["Presentación"].ToString();
                                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                                    string Imagen = existe[0]["Foto"].ToString();
                                    IVA__ = existe[0]["IVA"].ToString();
                                    IEPS__ = existe[0]["IEPS"].ToString();
                                    idPres = existe[0]["IdPresentacionVenta"].ToString();
                                    idMarca = existe[0]["IdMarca"].ToString();
                                    idLinea = existe[0]["IdLinea"].ToString();
                                }

                                double PrecioProm = 0;
                                double.TryParse(drow["PrecioPromo"].ToString(), out PrecioProm);

                                double Cant = 0;
                                double.TryParse(drow["Cantidad"].ToString(), out Cant);

                                int noOrden = 0;
                                int.TryParse(drow["NoOrden"].ToString(), out noOrden);

                                dgvVenta.Rows.Add(IdProducto, Producto, "-", drow["Cantidad"].ToString(), "+", PrecioProm.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioProm).ToString("C2"), (Cant * PrecioProm).ToString("C2"),
                                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, noOrden);

                            }

                        }

                    }
                    else {
                        listaProductos.Add(idPres);

                        DataTable dtPromos = new DataTable();
                        dtPromos.Columns.Add("IdProducto", typeof(int));
                        dtPromos.Columns.Add("IdPressProd", typeof(int));
                        dtPromos.Columns.Add("Producto", typeof(string));
                        dtPromos.Columns.Add("PrecioPromo", typeof(double));
                        dtPromos.Columns.Add("Cantidad", typeof(double));
                        dtPromos.Columns.Add("Uom", typeof(string));
                        dtPromos.Columns.Add("Precio", typeof(string));
                        dtPromos.Columns.Add("NoOrden", typeof(int));

                        List<string> listaProductos2 = new List<string>();
                        List<DataGridViewRow> quitarRows2 = new List<DataGridViewRow>();
                        foreach (DataGridViewRow gvr in dgvVenta.Rows)
                        {
                            if (arr.Contains(gvr.Cells[indIdPres].Value))
                            {
                                int noOrden = ObtieneOrdenProducto(gvr.Cells[indIdProd].Value.ToString());
                                dtPromos.Rows.Add(gvr.Cells[indIdProd].Value, gvr.Cells[indIdPres].Value, gvr.Cells[indProducto].Value, PrecioFinal, gvr.Cells[indQty].Value, gvr.Cells[indUom].Value, 0, noOrden);
                                listaProductos2.Add(gvr.Cells[indIdPres].Value.ToString());
                            }

                            if (arr.Contains(gvr.Cells[indIdPres].Value))
                                quitarRows2.Add(gvr);
                        }


                        foreach (DataGridViewRow gvr in quitarRows2)
                        {
                            dgvVenta.Rows.Remove(gvr);
                        }

                        //Agregar rows con promo y sin promo de los productos selecionados
                        if (dtPromos.Rows.Count > 0)
                        {
                            foreach (DataRow drow in dtPromos.Rows)
                            {

                                //var existe = dtProductos.Select("Id = '" + drow["IdProducto"].ToString() + "' AND Uom = " + drow["Uom"].ToString() + "");
                                var existe = dtProductos.Select("IdPresentacionVenta='" + drow["IdPressProd"].ToString() + "'");
                                if (existe.Count() > 0)
                                {
                                    IdProducto = existe[0]["Id"].ToString();
                                    esCaja = existe[0]["Escaja"].ToString();
                                    uom = existe[0]["Uom"].ToString();
                                    Producto = existe[0]["Producto"].ToString() + existe[0]["Presentación"].ToString();
                                    Precio = Convert.ToDouble(existe[0]["Precio"]).ToString("C2");
                                    string Imagen = existe[0]["Foto"].ToString();
                                    IVA__ = existe[0]["IVA"].ToString();
                                    IEPS__ = existe[0]["IEPS"].ToString();
                                    idPres = existe[0]["IdPresentacionVenta"].ToString();
                                    idMarca = existe[0]["IdMarca"].ToString();
                                    idLinea = existe[0]["IdLinea"].ToString();
                                }

                                double PrecioProm = 0;
                                double.TryParse(drow["PrecioPromo"].ToString(), out PrecioProm);

                                double Cant = 0;
                                double.TryParse(drow["Cantidad"].ToString(), out Cant);

                                int noOrden = 0;
                                int.TryParse(drow["NoOrden"].ToString(), out noOrden);


                                dgvVenta.Rows.Add(IdProducto, Producto, "-", drow["Cantidad"].ToString(), "+", PrecioProm.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioProm).ToString("C2"), (Cant * PrecioProm).ToString("C2"),
                                                IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, noOrden);

                            }

                        }
                    }
                }
                else
                {

                    foreach (DataGridViewRow gvr in quitarRows)
                    {
                        dgvVenta.Rows.Remove(gvr);
                    }

                    //Cuando existe promoción de solo un producto ya sea múltiplo o no.
                    if ((CantidadPromo <= CantidadSolicitada) && !Multiplo)
                    {
                        PrecioFinal = PrecioPromo;
                        dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadSolicitada, "+", PrecioPromo.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioPromo).ToString("C2"), (CantidadSolicitada * PrecioPromo).ToString("C2"),
                               IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, nOrden);
                    }

                    if ((CantidadPromo <= CantidadSolicitada) && Multiplo)
                    {
                        double CantidadProdPromo = Math.Floor(CantidadSolicitada / CantidadPromo);
                        dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadPromo * CantidadProdPromo, "+", PrecioPromo.ToString("C2"), PrecioInicial(IVA__, IEPS__, PrecioPromo).ToString("C2"), (CantidadProdPromo * PrecioPromo).ToString("C2"),
                               IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, nOrden);


                        double CantidadSinPromo = CantidadSolicitada % CantidadPromo;
                        if (CantidadSinPromo > 0)
                        {
                            dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadSinPromo, "+", PrecioFinal.ToString("C2"), PrecioInicial(IVA__, IEPS__, Convert.ToDouble(Precio.Replace("$", "").Replace(",", ""))).ToString("C2"), (CantidadSinPromo * PrecioFinal).ToString("C2"),
                               IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, nOrden);
                        }

                    }

                    if ((CantidadPromo > CantidadSolicitada && CantidadSolicitada > 0))
                    {
                        //dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadSolicitada, "+", PrecioFinal.ToString("C2"), PrecioInicial(IVA__, IEPS__, Convert.ToDouble(Precio.Replace("$", "").Replace(",", ""))).ToString("C2"), (CantidadSolicitada * PrecioFinal).ToString("C2"),
                        //IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0);

                        dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadSolicitada, "+", PrecioFinal.ToString("C2"), Precio, (CantidadSolicitada * PrecioFinal).ToString("C2"),
                        IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"), "", 0, nOrden);
                    }
                }
            }
            else {
                try
                {
                    foreach (DataGridViewRow gvr in quitarRows)
                    {
                        dgvVenta.Rows.Remove(gvr);
                    }

                }
                catch (Exception e)
                {

                }
               
                if (CantidadSolicitada > 0) {
                    dgvVenta.Rows.Add(IdProducto, Producto.Trim(), "-", CantidadSolicitada, "+", PrecioFinal.ToString("C2"), Convert.ToDouble(Precio.Replace("$", "").Replace(",", "")).ToString("C2"), (CantidadSolicitada * PrecioFinal).ToString("C2"),
                    IVA__, IEPS__, esCaja, uom, idPres, idMarca, idLinea, DateTime.Now.ToString("yyyy-MM-dd"),"", 0, nOrden);
                }

            }

            //dgvVenta.Sort(this.dgvVenta.Columns[indFechaAgregado], ListSortDirection.Ascending);
            return PrecioFinal;
        }

        public double PrecioInicial(string IVA__, string IEPS__, double PrecioProm) {
            double precioInicial = 0;

            double _Iva = 0;
            double.TryParse(IVA__, out _Iva);
            if (_Iva > 0)
            {
                precioInicial = PrecioProm / (1 + _Iva);
            }

            double _Ieps = 0;
            double.TryParse(IEPS__, out _Ieps);
            if (_Ieps > 0)
            {
                precioInicial = PrecioProm / (1 + _Ieps);
            }

            if (_Iva == 0 && _Ieps == 0)
                precioInicial = PrecioProm;

            return precioInicial;
        }

      
        private void sumarTotal()
        {
            double sum = 0;
            double ivaTotal = 0;
            double iepsTotal = 0;

            foreach (DataGridViewRow gvr in dgvVenta.Rows)
            {
                double iva = 0;
                double ieps = 0;
                double subtotal = 0;
                double total = 0;

                switch (ObtenerTipoCliente())
                {
                    case 1:
                        total = Convert.ToDouble(((dtProductos.Select("id = " + gvr.Cells[indIdProd].Value.ToString() + " AND EsCaja = " + gvr.Cells[indEsCaja].Value.ToString()))[0][17]));
                        break;
                    case 2:
                        total = Convert.ToDouble(((dtProductos.Select("id = " + gvr.Cells[indIdProd].Value.ToString() + " AND EsCaja = " + gvr.Cells[indEsCaja].Value.ToString()))[0][18]));
                        break;
                    case 3:
                        total = Convert.ToDouble(((dtProductos.Select("id = " + gvr.Cells[indIdProd].Value.ToString() + " AND EsCaja = " + gvr.Cells[indEsCaja].Value.ToString()))[0][19]));
                        break;
                }

                if (gvr.Cells[indIVA].Value != null)
                    if (double.TryParse(gvr.Cells[indIVA].Value.ToString(), out iva)) { }

                if (gvr.Cells[indIEPS].Value != null)
                    if (double.TryParse(gvr.Cells[indIEPS].Value.ToString(), out ieps)) { }

                sum += double.Parse(gvr.Cells[indTotal].Value.ToString(), NumberStyles.Currency, null);

                if (iva > 0)
                {
                    ivaTotal += ((total * iva) * Convert.ToInt32(gvr.Cells[indQty].Value));
                }
                
                if (ieps > 0)
                {
                    iepsTotal += ((total * ieps) * Convert.ToInt32(gvr.Cells[indQty].Value));
                }
            }

            lblTotal.Text = "Total:" + sum.ToString("C2");
            lblIVA.Text = ivaTotal.ToString();
            lblIEPS.Text = iepsTotal.ToString();

            txtScan.Select();
        }

        private void btnAtras_Click(object sender, EventArgs e)
        {
            
        }

        private void lvProductos_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(sender, e);
        }

        private void dgvVenta_KeyDown(object sender, KeyEventArgs e)
        {
            int ColumnPrecio = 5;
            //Atajos(sender, e);
            // Permitir que las teclas de navegación funcionen
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down ||
                e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.Home || e.KeyCode == Keys.End ||
                e.KeyCode == Keys.PageUp || e.KeyCode == Keys.PageDown)
            {
                if (e.KeyCode == Keys.Up && (dgvVenta.CurrentCell == null || dgvVenta.CurrentCell.RowIndex == 0))
                {
                    txtScan.Select();
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }

            if (dgvVenta.Rows.Count > 0)
            {
                if (dgvVenta.CurrentCell.ColumnIndex == ColumnPrecio && e.KeyCode == Keys.Enter)
                {
                    CambiarPrecioEnVenta();
                }
            }

            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract)
            { 
                SumarRestarProductos(sender, e); 
            }
            Atajos(sender, e);
        }

        //private void TienePromocion(int IdProducto)
        //{
        //    string query = " SELECT IdPromocion, Descripcion \n" +
        //        "FROM PVPromocionesDetalle D \n" +
        //        "JOIN PVPromociones P ON P.Id = D.IdPromocion \n" +
        //        "WHERE IdProducto = " + IdProducto;
        //    DataTable dtPromociones = sql.selec(query);

        //    if (dtPromociones.Rows.Count > 0)
        //    {
        //        string promo = "";
        //        foreach (DataRow r in dtPromociones.Rows)
        //        {
        //            promo += r["Descripcion"] + ". \n";
        //        }

        //        //lblPromo.Text = promo;
        //        //labelPromo.Visible = true;
        //    }
        //    else
        //    {
        //        //labelPromo.Visible = false;
        //        //lblPromo.Text = "";
        //    }
        //}



        private void AplicarPromociones()
        {
            //List<ListaProductos> listaCompra = (from row in dgvVenta.Rows.OfType<DataGridViewRow>()
            //    select new ListaProductos()
            //    {
            //        IdPresProducto = int.Parse(row.Cells[indIdPres].Value.ToString()),
            //        Cantidad = int.Parse(row.Cells[indQty].Value.ToString()),
            //        IdMarca = int.Parse(row.Cells[indIdMarca].Value.ToString()),
            //        IdLinea = int.Parse(row.Cells[indIdLinea].Value.ToString())
            //    }).ToList();

            //List<ListaPromociones> listaPromociones = new List<ListaPromociones>();

            //foreach (DataGridViewRow r in dgvVenta.Rows)
            //{
            //    //Verificar si la presentacion del producto tiene una promocion activa
            //    int IdPromocion = ExistePromocion(int.Parse(r.Cells[indIdPres].Value.ToString()));
            //    if (IdPromocion > 0)
            //    {
            //        //Consultar detalle de la promoción
            //        string queryDetalle = "SELECT TipoPromocion, IdProducto, IdMarca, IdLinea, \n" +
            //            "Cantidad, Precio, PrecioPromocion, Descuento, IdProductoRegalo, \n" +
            //            "IdProductoRegalo, CantidadRegalo FROM PVPromociones \n" +
            //            "WHERE Id = " + IdPromocion;
            //        DataTable detallePromo = sqlLoc.selec(queryDetalle);
            //        //Si el tipo de promoción es descuento
            //        if (detallePromo.Rows[0]["TipoPromocion"].ToString() == "Descuento")
            //        {
            //            string[] idsPres = detallePromo.Rows[0]["IdProducto"].ToString().Split(',');
            //            //Si hay mas de 1 producto en la promoción buscar los otros productos
            //            if (idsPres.Length > 1)
            //            {
            //                //Buscar en la lista de compras los productos de la promocion
            //                bool existenProductos = listaCompra.Any(x => 
            //                    idsPres.Any(y => y == x.IdPresProducto.ToString()));
            //                if (existenProductos)
            //                {
            //                    foreach (string i in idsPres)
            //                    {
            //                        listaPromociones.Add(
            //                            new ListaPromociones
            //                            {
            //                                IdPromocion = IdPromocion,
            //                                IdPresProducto = i,
            //                                Cantidad = 0
            //                            }
            //                        );
            //                    }
            //                }
            //            }
            //            else
            //            {

            //            }
            //        }
            //    }
            //}
        }

        private DataRow ExistePromocion(string IdProducto)
        {
            DataRow dr = null;

            DataTable dtPromo = sqlLoc.selec("SELECT D.IdPromocion, ISNULL(P.Multiplo,0) Multiplo,  ISNULL(P.Individual,0) Individual, ISNULL(P.Combinado,0) Combinado, D.PrecioPromocion, D.Cantidad, P.IdProducto Productos, P.Descripcion, D.IdPresProducto FROM PVPromocionesDetalle D \n" +
                "LEFT JOIN PVPromociones P ON D.IdPromocion = P.Id \n" +
                "WHERE CONVERT(DATE,P.FechaFin) >=  '"+ DateTime.Now.ToString("yyyy-MM-dd") +"' AND (P.IdSucursal=0 OR P.IdSucursal="+ idSucursal +")\n" + //CONVERT(DATE,GETDATE())
                "AND D.IdPresProducto = " + IdProducto);

            if (dtPromo.Rows.Count > 0)
            {
                dr = dtPromo.Rows[0];


                string promo = "";
                foreach (DataRow r in dtPromo.Rows)
                {
                    promo += r["Descripcion"] + ". \n";
                }

                lblPromo.Text = "PROMO:  " + promo;
                //lblPromo.Text = promo;
                //labelPromo.Visible = true;
            }
            else {
                lblPromo.Text = "";
                //labelPromo.Visible = false;
                //lblPromo.Text = "";
            }

            return dr;
        }

        private void Atajos(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1:
                    btnBuscarCodigo_Click(btnBuscarCodigo, e);
                    break;
                case Keys.F2:
                    //btnTerminar_Click(sender, e);
                    AbrirDevoluciones();
                    break;
                case Keys.F3:
                    btnEspera_Click(sender, e);
                    break;
                case Keys.F4:
                    AbrirConsultaProductos();
                    break;
                case Keys.F5:
                    //btnCancelar_Click(sender, e);
                    AbrirCreditos();
                    break;
                case Keys.F6:
                    DesplegarComboCliente(sender);
                    break;
                case Keys.F7:
                    btnReimprimir_Click(sender, e);
                    break;
                case Keys.F8:
                    btnTerminar_Click(sender, e);
                    break;
                case Keys.F9:
                    AbrirMenuCortes();
                    break;
                case Keys.F10:
                    AbrirEnvioDeVentas();
                    break;
                case Keys.F11:
                    AbrirFacturacion();
                    break;
                case Keys.F12:
                    gvPendientes.Select();
                    break;
                case Keys.Insert:
                    ExpulsarCajon();
                    break;
                case Keys.Tab:
                    txtScan.Select();
                    break;
                case Keys.Delete:
                    btnCancelar_Click(sender, e);
                    break;
            }
        }
        private void AbrirDevoluciones()
        {
            FDevoluciones fDevoluciones = new FDevoluciones(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, dtProductos, null, imgLstCategorias, imgLstProductos, numCaja);
            fDevoluciones.ShowDialog();
        }

        private void AbrirConsultaProductos()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
            FConsultaProducto fConsultaProducto = new FConsultaProducto(sql, sqlLoc, idSucursal, dtProductos, ObtenerTipoCliente());
            fConsultaProducto.ShowDialog();

        }

        private void AbrirCreditos()
        {
            FCredito fCredito = new FCredito(sql, sqlLoc, idUsuario, idSucursal);
            fCredito.ShowDialog();
        }

        private void AbrirEnvioDeVentas()
        {
            FDisparoNube disparoNube = new FDisparoNube(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, numCaja);
            disparoNube.ShowDialog();
        }

        private void AbrirMenuCortes()
        {
            FMenuCorte menuCorte = new FMenuCorte(sql, sqlLoc, nombre, idSucursal, sucursal, idUsuario, false, numCaja);
            menuCorte.ShowDialog();
        }

        private void DesplegarComboCliente(object sender)
        {
            EventArgs e = new EventArgs();
            if (controlClienteExistente)
            {
                btnYaExisteCliente_Click(sender, e);
                CBCliente.Select();
                CBCliente.DroppedDown = true;
                controlClienteExistente = false;
            }
            else
            {
                btnNoExisteCliente_Click(sender, e);
                controlClienteExistente = true;
            }
        }

        private void SumarRestarProductos(object sender,KeyEventArgs e)
        {
            int Sumar = 4; int Restar = 2;
            int Operacion = e.KeyCode == Keys.Add ? Sumar : Restar;

            if (dgvVenta.Rows.Count > 0)
            {
                SumarRestarPorTeclado = true;
                int indexProducto = dgvVenta.CurrentCell.RowIndex;

                DataGridViewCellEventArgs expresion = new DataGridViewCellEventArgs(Operacion, indexProducto);
                dgvVenta_CellClick(sender, expresion);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private bool HabilitarCreditoComoFormaPago(bool esCliente)
        {
            if (!esCliente)
            {
                return false;
            }
            else
            {
                string idClienteCredito = CBCliente.SelectedValue.ToString();
                string queryClienteConCredito = "SELECT * FROM PVClientes WHERE id = "+idClienteCredito+" AND DiasCredito > 0";
                DataTable dtClienteCredito = sqlLoc.selec(queryClienteConCredito);
                return (dtClienteCredito.Rows.Count > 0);
            }
        }

        private void InsertarNombreClienteVenta()
        {
            string queryInsertarNombre = "UPDATE PVVentas SET NombreCliente = '"+txtNombreCliente.Text+"' WHERE FolioVenta LIKE '"+folioVenta+"'";
            sqlLoc.exec(queryInsertarNombre);
        }

        private int ObtenerTipoCliente()
        {
            if (CBCliente.SelectedIndex == -1)
            {
                return 1;
            }
            else
            {
                string QueryTipoCliente = "SELECT idTipoCliente FROM PVClientes WHERE id = "+ (Convert.ToInt32(CBCliente.SelectedValue));
                DataTable R = sqlLoc.selec(QueryTipoCliente);
                if(R.Rows.Count > 0)
                {
                    return Convert.ToInt32(R.Rows[0]["idTipoCliente"]);
                }
                else
                {
                    return 1;
                }
            }
        }

        private void CambiarPrecioClientes()
        {
            if (dgvVenta.Rows.Count > 0)
            {
                int idTipoCliente = ObtenerTipoCliente();
                double sum = 0;
                foreach(DataGridViewRow row in dgvVenta.Rows)
                {
                    switch (idTipoCliente)
                    {
                        case 1:
                            row.Cells[5].Value = Convert.ToDouble((dtProductos.Select("Id = " + row.Cells[0].Value))[0]["PrecioGeneral"]).ToString("C2");
                            row.Cells[7].Value = Convert.ToDouble(Convert.ToInt32(row.Cells[3].Value) * Convert.ToDouble((dtProductos.Select("Id = " + row.Cells[0].Value))[0]["PrecioGeneral"])).ToString("C2");
                            break;
                        case 2:
                            row.Cells[5].Value = Convert.ToDouble((dtProductos.Select("Id = " + row.Cells[0].Value))[0]["PrecioTalleres"]).ToString("C2");
                            row.Cells[7].Value = Convert.ToDouble(Convert.ToInt32(row.Cells[3].Value) * Convert.ToDouble((dtProductos.Select("Id = " + row.Cells[0].Value))[0]["PrecioTalleres"])).ToString("C2");
                            break;
                        case 3:
                            row.Cells[5].Value = Convert.ToDouble((dtProductos.Select("Id = " + row.Cells[0].Value))[0]["PrecioDistribuidores"]).ToString("C2");
                            row.Cells[7].Value = Convert.ToDouble(Convert.ToInt32(row.Cells[3].Value) * Convert.ToDouble((dtProductos.Select("Id = " + row.Cells[0].Value))[0]["PrecioDistribuidores"])).ToString("C2");
                            break;
                    }
                    sum += Convert.ToDouble(row.Cells[7].Value.ToString().Replace("$", ""));
                }
                lblTotal.Text = "Total:" + sum.ToString("C2");
            }
        }

        private void CambiarPrecioPorUsuario(DataGridViewCellEventArgs e)
        {
            if (LoginSupervisorPrecios)
            {
                int IndexCantidad = 3;
                int IndexPrecio = 5;
                int IndexTotal = 7;
                double sum = 0;
                if (e.ColumnIndex == IndexPrecio && dgvVenta.Rows[e.RowIndex].Cells[IndexPrecio].Value != null)
                {
                    dgvVenta.Rows[e.RowIndex].Cells[IndexTotal].Value = (Convert.ToInt32(dgvVenta.Rows[e.RowIndex].Cells[IndexCantidad].Value) * (Convert.ToDouble((dgvVenta.Rows[e.RowIndex].Cells[IndexPrecio].Value).ToString().Replace("$", "")))).ToString("C2");
                    dgvVenta.Rows[e.RowIndex].Cells[IndexPrecio].Value = (Convert.ToDouble((dgvVenta.Rows[e.RowIndex].Cells[IndexPrecio].Value).ToString().Replace("$", ""))).ToString("C2");
                }

                foreach (DataGridViewRow row in dgvVenta.Rows)
                {
                    sum += Convert.ToDouble(row.Cells[7].Value.ToString().Replace("$", ""));
                }
                lblTotal.Text = "Total:" + sum.ToString("C2");
            }
        }
        private void SetPanelClienteNew()
        {
            CBCliente.Visible = true;
            CBCliente.SelectedIndex = -1;
            btnNoExisteCliente.Visible = true;

            txtNombreCliente.Visible = false;
            txtNombreCliente.Text = string.Empty;
            btnYaExisteCliente.Visible = false;
            btnBorrarTxtCliente.Visible = false;
        }

        private void AbrirFacturacion()
        {
            FEspera espera = new FEspera();
            espera.Text = "IMPRIMIENDO TICKET...";
            espera.Show();

            string queryUltimaVentaTerminada = "SELECT TOP 1 FolioTicket, FechaVenta, FolioVenta FROM PVVentas where Terminada = 1 ORDER BY FechaVenta DESC";
            DataTable dtFolioVenta = sqlLoc.selec(queryUltimaVentaTerminada);

            string FolioTicket = "";
            string FechaVenta = "";
            string FolioVenta = "";
            if (dtFolioVenta.Rows.Count > 0)
            {
                FolioTicket = dtFolioVenta.Rows[0]["FolioTicket"].ToString();
                FechaVenta = dtFolioVenta.Rows[0]["FechaVenta"].ToString();
                FolioVenta = dtFolioVenta.Rows[0]["FolioVenta"].ToString();
            }

            string urlBase = "https://terepapeleria.ledsco.com.mx/FacturacionMasiva.aspx";
            string parametros = "?s=" + idSucursal + "&f=" + FolioTicket + "&fv=" + FechaVenta + "&ff=" + FolioVenta + "&idu=" + idUsuario;
            string url = urlBase + parametros;

            // Abre la URL en el navegador predeterminado
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });

            espera.Close();
        }


        //=================== CONEXIÓN BASCULA ===================================


        private void InitializeSerialPort()
        {
            DataTable dtDatosConfiguracion = sqlLoc.selec(" SELECT * FROM ConfiguracionBascula ");
            if (dtDatosConfiguracion.Rows.Count > 0)
            {
                serialPort = new SerialPort();
                serialPort.PortName = dtDatosConfiguracion.Rows[0]["Puerto"].ToString(); ; // Ajusta al puerto COM correcto COM05
                serialPort.BaudRate = Convert.ToInt16(dtDatosConfiguracion.Rows[0]["VelocidadBaudios"].ToString()); // Ajusta la velocidad de baudios según la configuración de tu báscula
                serialPort.Parity = Parity.None;
                serialPort.DataBits = Convert.ToInt16(dtDatosConfiguracion.Rows[0]["BitDeDatos"].ToString());
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;

                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                try
                {
                    serialPort.Open();
                    // Envía el carácter 'P' para solicitar la lectura
                    // serialPort.Write("P");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error abriendo el puerto: {ex.Message}");
                }

            }

           
        }
        private void ExpulsarCajon()
        {
            TicketVacio ticketVacio = new TicketVacio();
            ticketVacio.ShowDialog();
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadExisting();
                Console.WriteLine($"Datos recibidos: {data}"); // Para depuración

                lock (datosRecibidosLock)
                {
                    pesoLeido = data.Trim();
                    datosRecibidos = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error leyendo datos: {ex.Message}");
            }
        }

        private void CambiarPrecioEnVenta()
        {
            if (!LoginSupervisorPrecios)
            {
                FPassFondo passFondo = new FPassFondo();
                passFondo.ShowDialog();

                string usuario = passFondo.Usuario;
                string pass = passFondo.Contrasena;

                string queryUsuarioAdmin = "SELECT TOP 1 Id, Usuario, Contrasena FROM PVUsuarios WHERE ModificarPrecioVenta = 1" +
                    " AND Usuario LIKE '" + usuario + "' ORDER BY id DESC";

                DataTable dtUserAdmin = sqlLoc.selec(queryUsuarioAdmin);
                string dtId = string.Empty;
                string dtUser = string.Empty;
                string dtPass = string.Empty;

                if (dtUserAdmin.Rows.Count > 0)
                {
                    dtId = dtUserAdmin.Rows[0]["Id"].ToString();
                    dtUser = dtUserAdmin.Rows[0]["Usuario"].ToString();
                    dtPass = dtUserAdmin.Rows[0]["Contrasena"].ToString();

                    if (usuario.Equals(dtUser, StringComparison.OrdinalIgnoreCase) && pass == dtPass)
                    {
                        LoginSupervisorPrecios = true;
                        dgvVenta.Columns[5].ReadOnly = false;
                    }
                    else
                    {
                        MessageBox.Show("Verificar contraseña", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Usuario no permitido", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        //private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        string data = serialPort.ReadExisting();
        //        Console.WriteLine($"Datos recibidos: {data}"); // Para depuración

        //        // Verifica si la invocación es necesaria
        //        if (this.InvokeRequired)
        //        {
        //            this.BeginInvoke(new Action(() => ProcessData(data)));
        //        }
        //        else
        //        {
        //            // Si no es necesario, llama directamente a ProcessData
        //            ProcessData(data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error leyendo datos: {ex.Message}");
        //    }
        //}


        private void ProcessData(string data)
        {
            // Procesa los datos recibidos desde la báscula y actualiza un Label en el formulario
            lblPeso.Text = data;

            //if (data == "")
            //    lblPeso.Text = "-----------";


            // lblPesoTitulo.Text = data;

            //double PrecioFinal_ = 0;
            //double.TryParse(lblPrecioProd.Text.Replace("$", "").Replace(",", ""), out PrecioFinal_);

            //double Peso = 0;
            //double.TryParse(lblPeso.Text.Trim().Replace("kg", "").Trim(), out Peso);

            //double Total = 0;

            //if (Pesaje)
            //    Total = PrecioFinal_ * Peso;
            //else
            //    Total = PrecioFinal_;


            //lblTotal.Text = Total.ToString("C2");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
            base.OnFormClosing(e);
        }
    }
}
