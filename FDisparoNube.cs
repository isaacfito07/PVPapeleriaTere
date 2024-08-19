using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FDisparoNube : Form
    {
        ConSQL sql, sqlLoc;
        string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        //ConSQLCE sqlLoc;
        string nombre, idSucursal, sucursal, idUsuarioGlob, IdCaja;

        //datatable de disparo
        DataTable dtDisparo;

        //indices de las columnas
        static int indTotal = 3;

        public FDisparoNube(ConSQL _sql, ConSQL _sqlLoc, string _nombre, string _idSucursal, string _sucursal, string _idUsuario, string _IdCaja)
        {
            InitializeComponent();
            sqlLoc = _sqlLoc;
            sql = _sql;
            nombre = _nombre;
            idSucursal = _idSucursal;
            sucursal = _sucursal;
            idUsuarioGlob = _idUsuario;
            IdCaja = _IdCaja;
        }

        private void fDisparoNube_Load(object sender, EventArgs e)
        {
            lblSitio.Text = sucursal;
            lblUsuario.Text = nombre;
            lblCaja_.Text = "CAJA: " + IdCaja;

            //Consultar cantidad ventas
            var hayVentas = sqlLoc.scalar(" SELECT COUNT(*) FROM PVVentas V \n" +
                "WHERE V.IdSucursal = " + idSucursal + " AND Valido = 1 \n" +
                "AND DisparadoNube = 0 AND Terminada = 1  ");
            //Consultar cantidad pagos
            var hayPagos = sqlLoc.scalar(" SELECT COUNT(*) FROM PVVentaPago P \n" +
                "JOIN PVVentas V ON P.FolioVenta = V.FolioVenta \n" +
                "WHERE V.Terminada = 1 AND P.DisparadoNube = 0");
            if(hayPagos != null)
            {
                lblCountPagos.Text = "Pagos: " + hayPagos.ToString();
            }
            //Consultar cantidad detalle
            var hayDetalle = sqlLoc.scalar(" SELECT COUNT(*) FROM PVVentasDetalle D \n" +
                "JOIN PVVentas V ON V.FolioVenta = D.FolioVenta \n" +
                "WHERE V.Terminada = 1 AND D.DisparadoNube = 0");
            if (hayDetalle != null)
            {
                lblCountDetalle.Text = "Detalle Ventas: " + hayDetalle.ToString();
            }
            //Consultar cantidad cortes
            var hayCortes = sqlLoc.scalar(" SELECT COUNT(*) FROM PVCorteCaja C \n" +
                "WHERE DisparadoNube = 0");
            if (hayCortes != null)
            {
                lblCountCortes.Text = "Cortes: " + hayCortes.ToString();
            }
            //Consultar cantidad retiros
            var hayRetiros = sqlLoc.scalar(" SELECT COUNT(*) FROM PVRetiroCaja P WHERE DisparadoNube = 0");
            if (hayRetiros != null)
            {
                lblCountRetiros.Text = "Retiros: " + hayPagos.ToString();
            }

            var hayDevoluciones = sqlLoc.scalar(" SELECT COUNT(*) FROM PVDevoluciones " +
                "WHERE DisparadoNube = 0");

            if (hayDevoluciones != null)
            {
                lblCountDevoluciones.Text = "Devoluciones: " + hayPagos.ToString();
            }

            var hayMonedero = sqlLoc.scalar(" SELECT COUNT(*) FROM PVMonederoCliente " +
                "WHERE DisparadoNube = 0");

            if (hayMonedero != null)
            {
                lblCountMonedero.Text = "Monedero: " + hayMonedero.ToString();
            }

            if (hayVentas != null)
            {
                lblCountVentas.Text = "Ventas: " + hayVentas.ToString();
                string query =
                "SELECT V.FolioVenta, C.Nombre Cliente, VD.Cantidad Productos, V.TotalVenta Total, \n" +
                "(CAST(V.FechaVenta AS DATE)) Fecha, U.Nombres Usuario, V.Terminada \n" +
                "FROM PVVentas V LEFT JOIN PVUsuarios U ON V.IdUsuarioVenta = U.Id \n" +
                "LEFT JOIN PVClientes C ON C.Id = V.Idcliente \n" +
                "LEFT JOIN ( \n" +
                "   SELECT DISTINCT VD.FolioVenta, SUM(VD.Cantidad) Cantidad \n" +
                "   FROM PVVentasDetalle VD GROUP BY VD.FolioVenta \n" +
                ") VD ON V.FolioVenta = VD.FolioVenta \n" +
                "WHERE V.IdSucursal = " + idSucursal + " AND v.DisparadoNube = 0 AND v.Terminada = 1";

                dtDisparo = sqlLoc.selec(query);
                dvgHistoria.DataSource = dtDisparo;
            }

            foreach (DataGridViewColumn col in dvgHistoria.Columns)
            {
                col.ReadOnly = true;
                //alinear total a la derecha
                if (col.Index == indTotal)
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            dvgHistoria.Select();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            ////Elimina información anterior (no necesaria)
            ////Elimina pagos
            sqlLoc.exec(" DELETE FROM PVVentaPago WHERE DisparadoNube = 1  ");

            ////Elimina detalle
            sqlLoc.exec(" DELETE FROM PVVentasDetalle WHERE DisparadoNube = 1 ");

            ////Elimina ventas
            sqlLoc.exec("DELETE FROM PVVentas WHERE DisparadoNube = 1 AND FolioCorteCaja IS NOT NULL");

            ////Elimina cortes de caja
            sqlLoc.exec(" DELETE FROM PVCorteCaja WHERE DisparadoNube = 1 ");

            ////Elimina retiros de caja
            sqlLoc.exec(" DELETE FROM PVRetiroCaja WHERE DisparadoNube = 1 AND FolioCorteCaja IS NOT NULL ");

            ////Elimina fondo de caja
            //sqlLoc.exec(" DELETE FROM PVFondoCaja WHERE DisparadoNube = 1 ");

            MessageBox.Show("Historial eliminado con éxito", "Liberador de espacio", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDisparo_Click(object sender, EventArgs e)
        {
            string ahora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            //Subir Datos locales a la nube
            ActualizarFoliosCortes();
            //Ventas
            ActualizarVentasCorte();
            SubirVentas();
            //Pagos
            SubirPagos();
            //Detalle
            SubirDetalle();
            //Retiros
            SubirRetiros();
            //Corte Caja
            SubirCortes();
            //Devoluciones
            SubirDevoluciones();
            //Monedero
            //Subir Arqueo
            SubirArqueo();


            MessageBox.Show("Sincronización realizada con éxito!", "Sincronización", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            fDisparoNube_Load(this, new EventArgs());
        }

        private void ActualizarFoliosCortes() {
            //Actualizar cortes en las ventas enviadas (Las cuales se enviaron sin uno)

            //Ventas, retiros y fondo de caja en la nube sin corte

            //----- Ventas -------
            DataTable dtVentasNube = sql.selec(" SELECT * FROM PVVentas WHERE IdSucursal=" + idSucursal + " AND (FolioCorteParcialCaja IS NULL OR FolioCorteParcialCaja = 'NULL') "); //FolioCorteParcialCaja IS NULL OR
            if (dtVentasNube.Rows.Count > 0)
            {
                string IdsVentas = "";
                foreach (DataRow dr in dtVentasNube.Rows)
                {
                    IdsVentas += "'" + dr["FolioVenta"].ToString() + "',";
                }

                if (IdsVentas.Length > 0)
                {
                    IdsVentas = IdsVentas.Substring(0, IdsVentas.Length - 1);

                    //---- Ventas locales ----
                    DataTable dtVentasLocales = sqlLoc.selec(" SELECT * FROM PVVentas WHERE IdSucursal=" + idSucursal + " AND FolioVenta IN (" + IdsVentas + ") AND (FolioCorteCaja IS NOT NULL AND FolioCorteCaja <> 'NULL') ");
                    if (dtVentasLocales.Rows.Count > 0)
                    {
                        string update = "";
                        foreach (DataRow drV in dtVentasNube.Rows)
                        {
                            var vBuscar = dtVentasLocales.Select("IdSucursal = " + idSucursal + " AND FolioVenta = '" + drV["FolioVenta"].ToString() + "'", "");

                            if (vBuscar.Count() > 0)
                            {
                                DataTable dtBuscar = new DataTable();
                                dtBuscar = vBuscar.CopyToDataTable();
                                if (dtBuscar.Rows.Count > 0)
                                {
                                    //Actualizar row
                                    update += "UPDATE TOP(1) PVVentas SET \n"
                                            //+ " IdCorteParcialCaja=" + dtBuscar.Rows[0]["IdCorteParcialCaja"].ToString() + ", \n"
                                            + " FolioCorteParcialCaja='" + dtBuscar.Rows[0]["FolioCorteParcialCaja"].ToString() + "', \n"
                                            //+ " IdCorteCaja=" + dtBuscar.Rows[0]["IdCorteCaja"].ToString() + ", \n"
                                            + " FolioCorteCaja='" + dtBuscar.Rows[0]["FolioCorteCaja"].ToString() + "' \n"
                                            + " WHERE IdSucursal=" + idSucursal + " AND FolioVenta = '" + drV["FolioVenta"].ToString() + "';";
                                }
                            }

                        }

                        if (update.Length > 0)
                        {
                            int i = sql.exec(update);
                        }
                    }
                }
            }

            pbDisparo.Value = 80;
            //----- Retiros -------
            DataTable dtRetirosNube = sql.selec(" SELECT * FROM PVRetirosCaja WHERE IdSucursal=" + idSucursal + " AND (FolioCorteCaja IS NULL OR FolioCorteCaja = 'NULL') "); //FolioCorteParcialCaja IS NULL OR
            if (dtRetirosNube.Rows.Count > 0)
            {
                string IdsRetiros = "";
                foreach (DataRow dr in dtRetirosNube.Rows)
                {
                    IdsRetiros += "'" + dr["FolioRetiro"].ToString() + "',";
                }

                if (IdsRetiros.Length > 0)
                {
                    IdsRetiros = IdsRetiros.Substring(0, IdsRetiros.Length - 1);

                    //---- Ventas Detalle locales ----
                    DataTable dtRetirosLocales = sqlLoc.selec(" SELECT * FROM PVRetiroCaja WHERE IdSucursal=" + idSucursal + " AND FolioRetiro IN (" + IdsRetiros + ") AND (FolioCorteCaja IS NOT NULL AND FolioCorteCaja <> 'NULL') ");
                    if (dtRetirosLocales.Rows.Count > 0)
                    {
                        string update = "";
                        foreach (DataRow drV in dtRetirosNube.Rows)
                        {
                            var vBuscar = dtRetirosLocales.Select("IdSucursal = " + idSucursal + " AND FolioRetiro = '" + drV["FolioRetiro"].ToString() + "'", "");
                            if (vBuscar.Count() > 0)
                            {
                                DataTable dtBuscar = new DataTable();
                                dtBuscar = vBuscar.CopyToDataTable();
                                if (dtBuscar.Rows.Count > 0)
                                {
                                    //Actualizar row
                                    update += "UPDATE TOP(1) PVRetirosCaja SET \n"
                                          //  + " IdCorteParcialCaja=" + dtBuscar.Rows[0]["IdCorteParcialCaja"].ToString() + ", \n"
                                            + " FolioCorteParcialCaja='" + dtBuscar.Rows[0]["FolioCorteParcialCaja"].ToString() + "', \n"
                                           // + " IdCorteCaja=" + dtBuscar.Rows[0]["IdCorteCaja"].ToString() + ", \n"
                                            + " FolioCorteCaja='" + dtBuscar.Rows[0]["FolioCorteCaja"].ToString() + "' \n"
                                            + " WHERE IdSucursal="+ idSucursal +" AND FolioRetiro = '" + drV["FolioRetiro"].ToString() + "';";
                                }
                            }
                        }

                        if (update.Length > 0)
                        {
                            int i = sql.exec(update);
                        }
                    }
                }


            }

            pbDisparo.Value = 90;
            //----- Fondo Caja -------
            //DataTable dtFondoCajaNube = sql.selec(" SELECT * FROM PVFondoCaja WHERE IdSucursal=" + idSucursal + " AND (IdCorteCaja IS NULL) "); //FolioCorteParcialCaja IS NULL OR
            //if (dtFondoCajaNube.Rows.Count > 0)
            //{
            //    string IdsFondoCaja = "";
            //    foreach (DataRow dr in dtFondoCajaNube.Rows)
            //    {
            //        IdsFondoCaja += dr["IdFondoCaja"].ToString() + ",";
            //    }

            //    if (IdsFondoCaja.Length > 0)
            //    {
            //        IdsFondoCaja = IdsFondoCaja.Substring(0, IdsFondoCaja.Length - 1);

            //        //---- Ventas Detalle locales ----
            //        DataTable dtFondoCajaLocales = sqlLoc.selec(" SELECT * FROM PVFondoCaja WHERE Id IN (" + IdsFondoCaja + ") AND IdCorteCaja IS NOT NULL  ");
            //        if (dtFondoCajaLocales.Rows.Count > 0)
            //        {
            //            string update = "";
            //            foreach (DataRow drV in dtFondoCajaNube.Rows)
            //            {
            //                var vBuscar = dtFondoCajaLocales.Select("IdSucursal='" + idSucursal + "' AND FolioFondoCaja = '" + drV["FolioFondoCaja"].ToString() + "'", "");
            //                if (vBuscar.Count() > 0)
            //                {
            //                    DataTable dtBuscar = new DataTable();
            //                    dtBuscar = vBuscar.CopyToDataTable();
            //                    if (dtBuscar.Rows.Count > 0)
            //                    {
            //                        //Actualizar row
            //                        update += "UPDATE TOP(1) PVFondoCaja SET \n"
            //                                + " IdCorteParcialCaja=" + dtBuscar.Rows[0]["IdCorteParcialCaja"].ToString() + ", \n"
            //                                + " FolioCorteParcialCaja='" + dtBuscar.Rows[0]["FolioCorteParcialCaja"].ToString() + "', \n"
            //                                + " IdCorteCaja=" + dtBuscar.Rows[0]["IdCorteCaja"].ToString() + ", \n"
            //                                + " FolioCorteCaja='" + dtBuscar.Rows[0]["FolioCorteCaja"].ToString() + "' \n"
            //                                + " WHERE IdSucursal='" + idSucursal + "' AND FolioFondoCaja = '" + drV["IdFondoCaja"].ToString() + "';";
            //                    }
            //                }
            //            }

            //            if (update.Length > 0)
            //            {
            //                int i = sqlP.exec(update);
            //            }
            //        }
            //    }
            //}

            pbDisparo.Value = 100;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
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

        private bool ActualizarVentasCorte()
        {
            //Buscar ventas que no se hayan enviado a la nube
            //string queryVentas = "SELECT DISTINCT FolioVenta, FolioCorteParcialCaja," +
            //    "FolioCorteCaja \n" +
            //    "FROM PVVentas WHERE DisparadoNube = 1";
            //DataTable dtVentas = sqlLoc.selec(queryVentas);

            //if (dtVentas.Rows.Count > 0)
            //{
            //    foreach (DataRow r in dtVentas.Rows)
            //    {
            //        string folioV = r["FolioVenta"].ToString();

            //        string folioCP = !string.IsNullOrEmpty(r["FolioCorteParcialCaja"].ToString()) ?
            //            "'" + r["FolioCorteParcialCaja"].ToString() + "'" : "NULL";

            //        string folioC = !string.IsNullOrEmpty(r["FolioCorteCaja"].ToString()) ?
            //            "'" + r["FolioCorteCaja"].ToString() + "'" : "NULL";

            //        string updt = "UPDATE PVVentas SET FolioCorteParcialCaja = "+ folioCP + "," +
            //            " FolioCorteCaja = " + folioC + ", FechaModifico = '" + fechaHora + "'," +
            //            " IdUsuarioModifico = " + idUsuarioGlob + " " +
            //            " WHERE FolioVenta = '" + folioV + "'";

            //        return sql.exec(updt) > 0 ? true : false;
            //    }
            //}

            return true;
        }

        private bool SubirVentas()
        {
            //Enviar la venta a la nube
            string sitio, usuario, folioV, fecha, IdCorteParcialCaja, FolioCorteParcialCaja,
                IdCorteCaja, FolioCorteCaja, CorteTerminado;

            //Buscar ventas que no se hayan enviado a la nube
            string queryVentas = "SELECT DISTINCT FolioVenta, IdSucursal, IdCliente, Factura, \n" +
                "RFCFactura, FechaFactura, UsuarioFactura, FolioFactura, TotalVenta, \n" +
                "IdCorteParcialCaja, FoliocorteParcialCaja, IdCorteCaja, FolioCorteCaja, \n" +
                "CorteTerminado, Valido, Terminada, FechaVenta, IdUsuarioVenta, NombreCliente, Pagado \n" +
                "FROM PVVentas WHERE DisparadoNube = 0 AND Terminada = 1";
            DataTable dtVentas = sqlLoc.selec(queryVentas);

            if (dtVentas.Rows.Count > 0)
            {
                foreach (DataRow r in dtVentas.Rows)
                {
                    sitio = r["IdSucursal"].ToString();
                    usuario = r["IdUsuarioVenta"].ToString();
                    folioV = r["FolioVenta"].ToString();
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

                    //meter renglon y regresar id
                    string insVenta =
                        " INSERT INTO PVVentas (FolioVenta, IdSucursal, FechaVenta, IdUsuarioVenta, \n" +
                        "Factura, RFCFactura, FechaFactura, UsuarioFactura, FolioFactura, IdCliente, \n" +
                        "CorteTerminado, Valido, FechaDisparo, IdUsuarioDisparo, IdCorteParcialCaja, \n" +
                        "FolioCorteParcialCaja, IdCorteCaja, FolioCorteCaja, TotalVenta, NombreCliente, Pagado) \n" +
                        "VALUES( \n"
                        + " '" + folioV + "', " + sitio + ","
                        + " '" + fecha + "', " + usuario + ","
                        + " '" + r["Factura"] + "', '" + r["RFCFactura"] + "', "
                        + " '" + r["FechaFactura"] + "', '" + r["UsuarioFactura"] + "', "
                        + " '" + r["FolioFactura"] + "', " + r["IdCliente"] + ", "
                        + CorteTerminado + ", '" + r["Valido"] + "',"
                        + " '" + fechaHora + "', " + idUsuarioGlob + ", " + IdCorteParcialCaja + ", "
                        + " " + FolioCorteParcialCaja + ", " + IdCorteCaja + ", "
                        + "" + FolioCorteCaja + ", " + " " + r["TotalVenta"] + "," + "'" + r["NombreCliente"] +"', '" + r["Pagado"] + "')";

                    if (sql.exec(insVenta) > 0)
                    {
                        //Actualizar BD local para marcar la venta como enviada a la nube
                        sqlLoc.exec(" UPDATE PVVentas SET DisparadoNube = 1, "
                            + "FechaDisparo='" + fechaHora + "', "
                            + "IdUsuarioDisparo=" + idUsuarioGlob 
                            + " WHERE FolioVenta = '" + folioV + "'");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool SubirPagos()
        {
            //Consultar pagos pendientes de subir a la nube
            string queryPagos = "SELECT VP.FolioVenta, VP.MontoRecibido, VP.Cambio, VP.MontoEfectivo,\n" +
                "VP.MontoTarjeta, VP.MontoTransferencia, VP.MontoMonedero, VP.MontoVales, VP.MontoCheque, \n" +
                "VP.TipoTarjeta, VP.AutorizacionTarjeta, VP.Respuesta, VP.FolioTransferencia, VP.FolioCheque, \n" +
                "VP.FechaAlta, VP.IdUsuarioAlta, VP.MontoTarjetaCredito, VP.MontoCredito, VP.idEsAbono \n" +
                "FROM PVVentaPago VP \n" +
                "JOIN PVVentas V ON VP.FolioVenta = V.FolioVenta \n" +
                "WHERE V.Terminada = 1 AND VP.DisparadoNube = 0";

            DataTable dtPagos = sqlLoc.selec(queryPagos);

            if (dtPagos.Rows.Count > 0)
            {
                foreach (DataRow r in dtPagos.Rows)
                {
                    string fecha = Convert.ToDateTime(r["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");

                    string insPagos = string.Format("INSERT INTO PVVentasPagos" +
                    "(FolioVenta, MontoRecibido, Cambio, MontoEfectivo, MontoTarjeta, MontoTransferencia," +
                    "MontoMonedero, MontoVales, TipoTarjeta, AutorizacionTarjeta, Respuesta," +
                    "FolioTransferencia, FechaAlta, IdUsuarioAlta, FechaDisparo, IdUsuarioDisparo, MontoTarjetaCredito, MontoCredito, idEsAbono, MontoCheque, FolioCheque)" +
                    "VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},'{12}',{13},'{14}',{15},{16},{17},'{18}',{19},'{20}')",
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
                    fecha, r["IdUsuarioAlta"].ToString(), fechaHora, idUsuarioGlob, r["MontoTarjetaCredito"].ToString(), r["MontoCredito"].ToString(), r["idEsAbono"].ToString(),
                    r["MontoCheque"], r["FolioCheque"].ToString());

                    if (sql.exec(insPagos) > 0)
                    {
                        //Actualizar BD local para marcar los pagos como enviados a la nube
                        sqlLoc.exec(" UPDATE PVVentaPago \n" +
                            "SET DisparadoNube = 1, FechaDisparo='" + fechaHora + "', " +
                            "IdUsuarioDisparo=" + idUsuarioGlob + " \n" +
                            "WHERE FolioVenta = '" + r["FolioVenta"].ToString() + "'");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool SubirDetalle()
        {
            //Consultar Detalle de la venta 
            //string queryDetalle = "SELECT VD.FolioVenta, VD.IdProducto, VD.Cantidad, \n" +
            //    "VD.Precio, VD.MontoDescuento, VD.Iva, VD.Ieps,VD.FechaAlta, VD.IdUsuarioAlta,\n" +
            //    "VD.EsCaja, VD.Uom \n" +
            //    "FROM PVVentasDetalle VD JOIN PVVentas V ON VD.FolioVenta = V.FolioVenta \n" +
            //    "WHERE V.Terminada = 1 AND VD.DisparadoNube = 0";

            string queryDetalle = "SELECT VD.Id, VD.FolioVenta, VD.IdProducto, VD.Cantidad, \n" +
                   " VD.Precio, VD.MontoDescuento, VD.Iva, VD.Ieps,VD.FechaAlta, VD.IdUsuarioAlta, \n" +
                   " VD.EsCaja, VD.Uom, VD.NumeroTelefonico, VD.FolioTransaccion, VD.CodigoTransaccion, VD.StatusTransaccion, \n" +
                   " VD.FechaTransaccion, VD.TransaccionID, VD.CodigoDescripcion, ISNULL(VD.MontoComision,0) MontoComision, VD.IdPresentacionProducto \n" +
                   " FROM PVVentasDetalle VD JOIN PVVentas V ON VD.FolioVenta = V.FolioVenta \n" +
                   " WHERE V.Terminada = 1 AND VD.DisparadoNube = 0";

            DataTable dtDetalle = sqlLoc.selec(queryDetalle);

            if (dtDetalle.Rows.Count > 0)
            {
                foreach (DataRow dr in dtDetalle.Rows)
                {
                    string fecha = Convert.ToDateTime(dr["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string insVentaDetalle = string.Format("INSERT INTO PVVentasDetalle \n" +
                        "(FolioVenta, IdProducto, Cantidad, Precio, Iva, Ieps, FechaAlta, " +
                        "IdUsuarioAlta, FechaDisparo, IdUsuarioDisparo, MontoDescuento,EsCaja, Uom, NumeroTelefonico, FolioTransaccion, CodigoTransaccion, StatusTransaccion, FechaTransaccion, TransaccionID, CodigoDescripcion, MontoComision, IdPresentacionProducto)\n " +
                        " VALUES({0},{1},{2},{3},{4},{5},'{6}',{7},'{8}',{9},{10},'{11}',{12},'{13}','{14}','{15}','{16}','{17}','{18}','{19}',{20},{21})",
                        !string.IsNullOrEmpty(dr["FolioVenta"].ToString())
                        ? "'" + dr["FolioVenta"].ToString() + "'" : "NULL",
                        dr["IdProducto"].ToString(), dr["Cantidad"].ToString(),
                        dr["Precio"].ToString(), dr["Iva"].ToString(),
                        dr["Ieps"].ToString(),
                        fecha, dr["IdUsuarioAlta"].ToString(), fechaHora, idUsuarioGlob,
                        dr["MontoDescuento"].ToString(), dr["EsCaja"].ToString(), dr["Uom"].ToString(),
                        dr["NumeroTelefonico"].ToString(), dr["FolioTransaccion"].ToString(), 
                        dr["CodigoTransaccion"].ToString(),
                        dr["StatusTransaccion"].ToString(), dr["FechaTransaccion"].ToString(), 
                        dr["TransaccionID"].ToString(),
                        dr["CodigoDescripcion"].ToString(), dr["MontoComision"].ToString(),
                        dr["IdPresentacionProducto"].ToString());

                 
                    if (sql.exec(insVentaDetalle) > 0)
                    {
                        //Actualizar BD local para marcar el detalle como enviado a la nube
                        sqlLoc.exec(" UPDATE PVVentasDetalle SET DisparadoNube = 1," +
                            "FechaDisparo = '" + fechaHora + "', " +
                            "IdUsuarioDisparo = " + idUsuarioGlob
                            + "WHERE FolioVenta = '" + dr["FolioVenta"].ToString() + "' AND Id="+ dr["Id"].ToString());
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void dvgHistoria_KeyDown(object sender, KeyEventArgs e)
        {
            Atajos(e);
        }

        private void Atajos(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private bool SubirRetiros()
        {
            //Consultar Retiros
            string queryRetiros = "SELECT FolioRetiro, IdUsuario, IdSucursal, FechaRetiro, Concepto, IdProveedor, " +
                "FolioCorteCaja, FolioCorteParcialCaja FROM PVRetiroCaja " +
                "WHERE DisparadoNube = 0";

            DataTable dtRetiros = sqlLoc.selec(queryRetiros);

            if (dtRetiros.Rows.Count > 0)
            {
                foreach (DataRow dr in dtRetiros.Rows) 
                {
                    string fecha = Convert.ToDateTime(dr["FechaRetiro"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string insRetiros = string.Format("INSERT INTO PVRetirosCaja \n" +
                        "(FolioRetiro, IdUsuario, IdSucursal, FechaRetiro, Concepto, " +
                        "FolioCorteCaja, FolioCorteParcialCaja, FechaDisparo, IdUsuarioDisparo, IdProveedor)\n " +
                        " VALUES({0},{1},{2},'{3}',{4},{5},{6},'{7}',{8},{9})",
                        !string.IsNullOrEmpty(dr["FolioRetiro"].ToString())
                            ? "'" + dr["FolioRetiro"].ToString() + "'" : "NULL",
                        dr["IdUsuario"].ToString(), 
                        dr["IdSucursal"].ToString(),
                        fecha, 
                        !string.IsNullOrEmpty(dr["Concepto"].ToString())
                            ? "'" + dr["Concepto"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(dr["FolioCorteCaja"].ToString())
                            ? "'" + dr["FolioCorteCaja"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(dr["FolioCorteParcialCaja"].ToString())
                            ? "'" + dr["FolioCorteParcialCaja"].ToString() + "'" : "NULL",
                        fechaHora, idUsuarioGlob, dr["IdProveedor"].ToString());

                    if (sql.exec(insRetiros) > 0)
                    {
                        //Actualizar BD local para marcar los retiros como enviado a la nube
                        sqlLoc.exec(" UPDATE PVRetiroCaja SET DisparadoNube = 1, " +
                            "FechaDisparo = '" + fechaHora + "', " +
                            "IdUsuarioDisparo = " + idUsuarioGlob
                            + "WHERE FolioRetiro = '" + dr["FolioRetiro"].ToString() + "'");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool SubirCortes()
        {
            //Consultar Cortes
            string queryCortes = "SELECT FolioCorte, FolioVentaInicial, FolioVentaFinal, \n" +
                "IdSucursal, EfectivoDeclaracion, TarjetaCredDeclaracion, TarjetaDebitoDeclaracion, \n" +
                "ValesDeclaracion, TotalCaja, FondoCaja, Efectivo, TarjetaCredito, TarjetaDebito, \n" +
                "Transferencia, Monedero, Vales, Cheques, TotalVentas, Retiros, Valido, CorteFinal, \n" +
                "IdUsuarioCorte, FechaCorte, RetirosDeclaracion FROM PVCorteCaja \n" +
                "WHERE  DisparadoNube = 0";

            DataTable dtCortes = sqlLoc.selec(queryCortes);

            if (dtCortes.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCortes.Rows) 
                {
                    string fecha = Convert.ToDateTime(dr["FechaCorte"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string insRetiros = string.Format("INSERT INTO PVCortesCaja \n" +
                        "(FolioCorte, FolioVentaInicial, FolioVentaFinal, \n" +
                        "IdSucursal, EfectivoDeclaracion, TarjetaCredDeclaracion, " +
                        "TarjetaDebitoDeclaracion, ValesDeclaracion, TotalDeclarado, FondoCaja, " +
                        "Efectivo, TarjetaCredito, TarjetaDebito, Transferencia, Monedero, " +
                        "Vales, TotalVentas, Retiros, Valido, CorteFinal, " +
                        "IdUsuarioCorte, FechaCorte, FechaDisparo, IdUsuarioDisparo, RetirosDeclaracion, cheques)\n " +
                        " VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}" +
                        ",{14},{15},{16},{17},'{18}','{19}',{20},'{21}','{22}',{23},{24},{25})",
                        !string.IsNullOrEmpty(dr["FolioCorte"].ToString())
                            ? "'" + dr["FolioCorte"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(dr["FolioVentaInicial"].ToString())
                            ? "'" + dr["FolioVentaInicial"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(dr["FolioVentaFinal"].ToString())
                            ? "'" + dr["FolioVentaFinal"].ToString() + "'" : "NULL",
                        dr["IdSucursal"].ToString(),
                        dr["EfectivoDeclaracion"].ToString(),
                        dr["TarjetaCredDeclaracion"].ToString(),
                        dr["TarjetaDebitoDeclaracion"].ToString(),
                        dr["ValesDeclaracion"].ToString(), dr["TotalCaja"].ToString(),
                        dr["FondoCaja"].ToString(), dr["Efectivo"].ToString(),
                        dr["TarjetaCredito"].ToString(),dr["TarjetaDebito"].ToString(),
                        dr["Transferencia"].ToString(),dr["Monedero"].ToString(),
                        dr["Vales"].ToString(),dr["TotalVentas"].ToString(),
                        dr["Retiros"].ToString(),
                        dr["Valido"].ToString(),dr["CorteFinal"].ToString(),
                        dr["IdUsuarioCorte"].ToString(), fecha,
                        fechaHora, idUsuarioGlob, dr["RetirosDeclaracion"].ToString(),
                        dr["Cheques"].ToString());

                    if (sql.exec(insRetiros) > 0)
                    {
                        //Actualizar BD local para marcar los retiros como enviado a la nube
                        sqlLoc.exec(" UPDATE PVCorteCaja SET DisparadoNube = 1, " +
                            "FechaDisparo = '" + fechaHora + "', " +
                            "IdUsuarioDisparo = " + idUsuarioGlob
                            + "WHERE FolioCorte = '" + dr["FolioCorte"].ToString() + "'");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool SubirArqueo()
        {
            //Consultar Cortes
            string queryCortes = "SELECT FolioCorte, FolioVentaInicial, FolioVentaFinal, \n" +
                "IdSucursal, EfectivoDeclaracion, TarjetaCredDeclaracion, TarjetaDebitoDeclaracion, \n" +
                "ValesDeclaracion, TotalCaja, FondoCaja, Efectivo, TarjetaCredito, TarjetaDebito, \n" +
                "Transferencia, Monedero, Vales, TotalVentas, Retiros, Valido, CorteFinal, \n" +
                "IdUsuarioCorte, FechaCorte, RetirosDeclaracion FROM [PVCorteCajaArqueo] \n" +
                "WHERE  DisparadoNube = 0";

            DataTable dtCortes = sqlLoc.selec(queryCortes);

            if (dtCortes.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCortes.Rows)
                {
                    string fecha = Convert.ToDateTime(dr["FechaCorte"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string insRetiros = string.Format("INSERT INTO [PVCorteCajaArqueo] \n" +
                        "(FolioCorte, FolioVentaInicial, FolioVentaFinal, \n" +
                        "IdSucursal, EfectivoDeclaracion, TarjetaCredDeclaracion, " +
                        "TarjetaDebitoDeclaracion, ValesDeclaracion, TotalDeclarado, FondoCaja, " +
                        "Efectivo, TarjetaCredito, TarjetaDebito, Transferencia, Monedero, " +
                        "Vales, TotalVentas, Retiros, Valido, CorteFinal, " +
                        "IdUsuarioCorte, FechaCorte, FechaDisparo, IdUsuarioDisparo, RetirosDeclaracion)\n " +
                        " VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}" +
                        ",{14},{15},{16},{17},'{18}','{19}',{20},'{21}','{22}',{23}, {24})",
                        !string.IsNullOrEmpty(dr["FolioCorte"].ToString())
                            ? "'" + dr["FolioCorte"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(dr["FolioVentaInicial"].ToString())
                            ? "'" + dr["FolioVentaInicial"].ToString() + "'" : "NULL",
                        !string.IsNullOrEmpty(dr["FolioVentaFinal"].ToString())
                            ? "'" + dr["FolioVentaFinal"].ToString() + "'" : "NULL",
                        dr["IdSucursal"].ToString(),
                        dr["EfectivoDeclaracion"].ToString(),
                        dr["TarjetaCredDeclaracion"].ToString(),
                        dr["TarjetaDebitoDeclaracion"].ToString(),
                        dr["ValesDeclaracion"].ToString(), dr["TotalCaja"].ToString(),
                        dr["FondoCaja"].ToString(), dr["Efectivo"].ToString(),
                        dr["TarjetaCredito"].ToString(), dr["TarjetaDebito"].ToString(),
                        dr["Transferencia"].ToString(), dr["Monedero"].ToString(),
                        dr["Vales"].ToString(), dr["TotalVentas"].ToString(),
                        dr["Retiros"].ToString(),
                        dr["Valido"].ToString(), dr["CorteFinal"].ToString(),
                        dr["IdUsuarioCorte"].ToString(), fecha,
                        fechaHora, idUsuarioGlob, dr["RetirosDeclaracion"].ToString());

                    if (sql.exec(insRetiros) > 0)
                    {
                        //Actualizar BD local para marcar los retiros como enviado a la nube
                        sqlLoc.exec(" UPDATE [PVCorteCajaArqueo] SET DisparadoNube = 1, " +
                            "FechaDisparo = '" + fechaHora + "', " +
                            "IdUsuarioDisparo = " + idUsuarioGlob
                            + "WHERE FolioCorte = '" + dr["FolioCorte"].ToString() + "'");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool SubirDevoluciones()
        {
            //Buscar devoluciones que no se hayan subido a la nube y no hayan afectado inventario 
            string queryDev = "SELECT Id, FolioVenta, IdCliente, IdProducto, CantidadOriginal, \n" +
                " CantidadDevuelta, MontoDevolucion, Activo, Motivo, Monedero, \n" +
                " Inventario, InventarioDefectuoso, FechaAlta, IdUsuarioAlta, EsCaja, Uom \n" +
                " FROM PVDevoluciones \n" +
                " WHERE DisparadoNube = 0";

            DataTable dtDevoluciones = sqlLoc.selec(queryDev);

            if(dtDevoluciones.Rows.Count > 0)
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
                    bool defectuoso = (bool)r["Defectuoso"];
                    bool incorrecto = (bool)r["Incorrecto"];
                    bool monedero = (bool)r["Monedero"];
                    string fechaAlta = Convert.ToDateTime(r["FechaAlta"]).ToString("yyyy-MM-dd HH:mm:ss");
                    string idUsuarioAlta = r["IdUsuarioAlta"].ToString();
                    string esCaja = r["EsCaja"].ToString();
                    string uom = r["Uom"].ToString();

                    if (SubirDevolucionNube(id, folioVenta, idCliente, idProducto,
                                    cantidadOriginal, cantidadDevolucion, MontoDevolucion,
                                    defectuoso, incorrecto, monedero, 0,0,
                                    fechaAlta, idUsuarioAlta,esCaja,uom))
                    {
                        //Marcar devolución como enviada a nube
                        sqlLoc.exec("UPDATE PVDevoluciones SET DisparadoNube = 1 \n" +
                            "WHERE Id = " + id);
                    }
                }
            }

            return true;
        }

        private bool SubirDevolucionNube(int id, string folioVenta, int idCliente,
            int idProducto, double cantidadOriginal, double cantidadDevolucion,
            string montoDevolucion, bool defectuoso, bool incorrecto, bool monedero,
            int inventario, int inventarioDefectuoso, string fechaAlta, string idUsuarioAlta,
            string esCaja, string uom)
        {
            string queryDev = string.Format("INSERT INTO PVDevoluciones(FolioVenta, IdCliente, IdProducto, " +
                " CantidadOriginal, CantidadDevuelta, MontoDevolucion, Defectuoso, Incorrecto," +
                " Monedero, Inventario, InventarioDefectuoso, FechaAlta, IdUsuarioAlta, " +
                " FechaDisparo, IdUsuarioDisparo, EsCaja, Uom)" +
                " VALUES('{0}',{1},{2},{3},{4},{5},'{6}','{7}','{8}',{9},{10},'{11}',{12},'{13}',{14})",
                  folioVenta, idCliente, idProducto, cantidadOriginal, cantidadDevolucion,
                  montoDevolucion, defectuoso, incorrecto, monedero, inventario, inventarioDefectuoso,
                  fechaAlta, idUsuarioAlta, fechaHora, idUsuarioGlob, esCaja, uom);
            //Subir devolución a la nube
            if (sql.exec(queryDev) > 0)
                return true;

            return false;
        }
    }
}
