using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FBuscadorCodigos : Form
    {
        ConSQL sqlLoc, sql;
        string idProd = "";
        string idPres = "";
        string cantidad = "1";
        string idSucursal = "";
        int idTipoCliente = 0;
        BindingSource bindingSource = null;

        public FBuscadorCodigos(ConSQL _sqlLoc, ConSQL _sql , string _idSucursal, int _idTipoCliente = 1)
        {
            InitializeComponent();
            bindingSource = new BindingSource();

            sqlLoc = _sqlLoc;
            sql = _sql;
            idSucursal = _idSucursal;
            idTipoCliente = _idTipoCliente;
        }

        private void fBuscadorCodigos_Load(object sender, EventArgs e)
        {
            //Carga productos
            cargaProductos();
            txtProducto.Select();
        }

        public void cargaProductos(string producto = "")
        {
            //string query = "SELECT DISTINCT P.Id, \n" 
            //    + " CONCAT(P.Descripcion, ' ', P.Marca, ' ', \n"
            //    + " P.Presentacion) Producto, \n" 
            //    + " CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación', \n" 
            //    + " Pres.CodigoBarras, Pres.Uom, \n"
            //    + " ROUND((ISNULL " 
            //    + "    (Pres.Precio + (" 
            //    + "       (Pres.Precio * P.Iva) + " 
            //    + "       (Pres.Precio * p.Ieps)" 
            //    + "     ), " 
            //    + "0)),2) Precio, P.Foto, Pres.Id IdPresentacionVenta, \n"
            //    + " P.IdMarca, P.IdLinea, ISNULL(I.Existencia,0) Existencia \n"
            //    + " FROM PVProductos P \n"
            //    + " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id \n"
            //    + " LEFT JOIN (SELECT IdProducto, SUM(Cantidad) Existencia FROM PVInventario WHERE IdSucursal="+ idSucursal + " GROUP BY IdProducto) I ON P.Id = I.IdProducto\n";


            string query = "SELECT \n"
	                       + "     DISTINCT P.Id, CONCAT(ISNULL(P2.Descripcion, P.Descripcion), ' ', P.Marca, ' ', P.Presentacion) Producto,  \n"
                           + "     CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación',  Pres.CodigoBarras, Pres.Uom, \n"
                           + "     ROUND((ISNULL\n"
                           + "     (Pres.Precio + ( \n"
                           + "         (Pres.Precio * P.Iva) + \n"
                           + "         (Pres.Precio * p.Ieps)\n"
                           + "     ),\n"
                           + "     0)),2) Precio, P.Foto, Pres.Id IdPresentacionVenta, P.IdMarca, P.IdLinea, \n"
                           + "     ISNULL(IC.Existencia,0) Existencia, ISNULL(Pres.VerPV,0) VerPV\n"
                           + " FROM PVProductos P \n"
                           + " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id \n"
                           + " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n"
                           + " LEFT JOIN \n"
                           + " ( \n"
                           + "     SELECT \n"
                           + "         idProducto, idSucursal, EsCaja, SUM(cantidad) Existencia \n"
                           + "     FROM PVInventario I \n"
                           + "     WHERE I.IdSucursal= "+ idSucursal + "\n"
                           + "     GROUP BY idProducto, idSucursal, EsCaja \n"
                           + " )IC ON IC.IdProducto = ISNULL(Pres.IdProductoIndividual, P.id) AND \n"
                           + " CASE WHEN Pres.Uom = 1 THEN 0 ELSE 1 END = EsCaja WHERE  ISNULL(Pres.VerPV,0)=1";

            //Nueva version
            query = "SELECT \n"
                           + "     DISTINCT P.Id, CONCAT(ISNULL(P2.Descripcion, P.Descripcion), ' ', P.Marca, ' ', P.Presentacion) Producto,  \n"
                           + "     CONCAT(Pres.Presentacion, ' (', Pres.Uom, ')') 'Presentación',  Pres.CodigoBarras, Pres.Uom, \n"
                           + "     PVP.Precio, \n"
                           + "P.Foto, Pres.Id IdPresentacionVenta, P.IdMarca, P.IdLinea, \n"
                           + "     ISNULL(IC.Existencia,0) Existencia, ISNULL(Pres.VerPV,0) VerPV\n"
                           + " FROM PVProductos P \n"
                           + " LEFT JOIN (\n" +
                                "SELECT DISTINCT Pre.idproducto, Pre.idSucursal, Pre.IdPresentacionVenta," +
                                "(CASE Cli.id WHEN 1 THEN ROUND((ISNULL(Pre.General + ((Pre.General * P2.Iva) + (Pre.General * p2.Ieps)),0)),2) \n"
                                + " WHEN 2 THEN ROUND((ISNULL(Pre.Talleres + ((Pre.Talleres * P2.Iva) + (Pre.Talleres * p2.Ieps)),0)),2) \n"
                                + " WHEN 3 THEN ROUND((ISNULL(Pre.Distribuidores + ((Pre.Distribuidores * P2.Iva) + (Pre.Distribuidores * p2.Ieps)),0)),2) END) " +
                                "AS Precio FROM PVPrecios Pre INNER JOIN PVProductos P2 ON P2.id = Pre.idproducto INNER JOIN PVTipoCliente Cli ON Cli.id = " + idTipoCliente + ") \n" +
                                "PVP ON PVP.idproducto = P.Id"
                           + " LEFT JOIN PVPresentacionesVentaProd Pres ON Pres.IdProducto = P.Id AND Pres.Id = PVP.IdPresentacionVenta \n"
                           + " LEFT JOIN PVProductos P2 ON Pres.IdProductoIndividual = P2.Id \n"
                           + " LEFT JOIN \n"
                           + " ( \n"
                           + "     SELECT \n"
                           + "         idProducto, idSucursal, EsCaja, SUM(cantidad) Existencia \n"
                           + "     FROM PVInventario I \n"
                           + "     WHERE I.IdSucursal= " + idSucursal + "\n"
                           + "     GROUP BY idProducto, idSucursal, EsCaja \n"
                           + " )IC ON IC.IdProducto = ISNULL(Pres.IdProductoIndividual, P.id) AND \n"
                           + " CASE WHEN Pres.Uom = 1 THEN 0 ELSE 1 END = EsCaja WHERE  ISNULL(Pres.VerPV,0)=1 AND PVP.idSucursal = " +idSucursal;

            if (!string.IsNullOrEmpty(producto))
            {
                query += " AND (CONCAT(P.Descripcion, ' ', P.Marca, ' ',  P.Presentacion) like '%" + producto + "%' OR \n"
                    + " CONCAT(P2.Descripcion, ' ', P.Marca, ' ',  P.Presentacion) like '%" + producto + "%') ";
            }
            
            DataTable dtProductos = sqlLoc.selec(query);
            bindingSource.DataSource = dtProductos;

            dgvProductos.DataSource = null;

            if (dtProductos.Rows.Count > 0)
            {
                dgvProductos.DataSource = dtProductos;
                dgvProductos.Columns["Id"].Visible = false;
                dgvProductos.Columns["IdPresentacionVenta"].Visible = false;
                dgvProductos.Columns["Uom"].Visible = false;
                dgvProductos.Columns["CodigoBarras"].Visible = false;
                dgvProductos.Columns["Foto"].Visible = false;
                dgvProductos.Columns["IdMarca"].Visible = false;
                dgvProductos.Columns["IdLinea"].Visible = false;
                dgvProductos.Columns["Precio"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProductos.Columns["Producto"].Width = 700;
                dgvProductos.Columns["Precio"].Width = 120;
                dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "c";
                dgvProductos.Columns["Precio"].DefaultCellStyle.Font = new System.Drawing.Font("Arial", 14F, FontStyle.Bold);
                dgvProductos.Columns["Presentación"].Width = 160;
                dgvProductos.Columns["Existencia"].Width = 100;
                dgvProductos.Columns["Existencia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvProductos.Columns["VerPV"].Visible = false;

                lblCantProd.Text = dtProductos.Rows.Count.ToString() + " Productos";
            }
        }

        private void txtProducto_TextChanged(object sender, EventArgs e)
        {
            string searchValue = txtProducto.Text.Trim();

            if (string.IsNullOrEmpty(searchValue))
            {
                bindingSource.RemoveFilter();
            }
            else
            {
                // Filtrar por la columna "Nombre"
                bindingSource.Filter = $"[Producto] LIKE '%{searchValue}%'";
            }
        }

        private void txtProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //buscar por codigo de producto
                if (txtProducto.Text.Trim() != "")
                {
                    cargaProductos(txtProducto.Text.Trim());
                }
            }
            if (e.KeyCode == Keys.F2 || e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.F9)
            {
                btnSicronizar_Click(sender, e);
            }
        }

        private void dgvProductos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                idProd = (string)dgvProductos.Rows[e.RowIndex].Cells["Id"].Value.ToString();
                cantidad = (string)dgvProductos.Rows[e.RowIndex].Cells["Uom"].Value.ToString();
                idPres = (string)dgvProductos.Rows[e.RowIndex].Cells["IdPresentacionVenta"].Value.ToString();
                this.Close();
            }
        }

        private void dgvProductos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.Close();
            }
        }

        public string IdProducto
        {
            get
            {
                return idProd;
            }
        }

        public string Cantidad
        {
            get
            {
                return cantidad;
            }
        }

        public string IdPresentacionVenta
        {
            get
            {
                return idPres;
            }
        }
        

        private void dgvProductos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.Enter)
            {
                // Se obtiene la fila y la columna actual
                int rowIndex = dgvProductos.CurrentCell.RowIndex;
                int columnIndex = dgvProductos.CurrentCell.ColumnIndex;

                // Se crea un nuevo objeto DataGridViewCellEventArgs con los índices actuales
                DataGridViewCellEventArgs cellEventArgs = new DataGridViewCellEventArgs(columnIndex, rowIndex);

                dgvProductos_CellContentDoubleClick(sender, cellEventArgs);//Se cambia el this.close
                e.Handled = true;
            }
        }

        private void fBuscadorCodigos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
                this.Close();
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void btnSicronizar_Click(object sender, EventArgs e)
        {
            FEspera espera = new FEspera();
            espera.Text = "ACTUALIZANDO CATALOGOS";
            espera.Show();

            try
            {
                if (!CheckForInternetConnection())
                {
                    MessageBox.Show("No hay conexion a internet, revisa tu conexión", "Sin internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    goto fin;
                }

                //intenta copiar los usuarios y catalogos de la nube a la local
                DataTable dtCopiar;

                int cuantos = 0;

                //Cat_Clientes
                string query = " SELECT Id, Clave, Nombre, RFC, Calle, NumInterior, " +
                    "NumExterior, Colonia, CP, Telefono, Poblacion, Municipio, Estado, " +
                    "UsoCFDI, RegimenFiscal, " +
                    "DiasCredito, LimiteCredito, FechaAlta, IdUsuarioAlta, ISNULL(TieneDescuento,0) TieneDescuento, ISNULL(PorcentajeDescuento,0) PorcentajeDescuento, idTipoCliente " +
                    "FROM PV_Cat_Clientes WHERE Activo = 1 ";
                dtCopiar = sql.selec(query);
                sqlLoc.exec(" TRUNCATE TABLE PVClientes ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVClientes");

                //Monedero Cliente
                string queryMonedero = "SELECT FolioVenta, IdCliente, Monto, Valido, \n" +
                    "FechaAlta, IdUsuarioAlta, 1 DisparadoNube \n" +
                    "FROM PVMonederoCliente WHERE Valido = 1";
                dtCopiar = sql.selec(queryMonedero);
                sqlLoc.exec(" TRUNCATE TABLE PVMonederoCliente ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVMonederoCliente");

                //Devoluciones Cliente
                dtCopiar = sql.selec("SELECT Id, FolioVenta, IdCliente, IdProducto, \n" +
                    " CantidadOriginal, CantidadDevuelta, MontoDevolucion, Activo, \n" +
                    " Motivo, Monedero, Inventario, InventarioDefectuoso, \n" +
                    " FechaAlta, IdUsuarioAlta, 1 DisparadoNube, EsCaja, Uom \n" +
                    " FROM PVDevoluciones");
                sqlLoc.exec(" TRUNCATE TABLE PVDevoluciones ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVDevoluciones");

                //Cat_Usuarios
                dtCopiar = sql.selec(" SELECT Id, Nombres, Usuario, 'ledsco' Correo, Contrasena, " +
                    " PuntoVenta, CancelarVenta, CorteCaja, EstadoCaja, CorreccionesCaja, " +
                    " RetiroCaja, Descuentos, Devoluciones, ModificarPrecioVenta " +
                    " FROM PV_Cat_Usuarios WHERE Activo = 1 ");
                sqlLoc.exec(" TRUNCATE TABLE PVUsuarios ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVUsuarios");

                //Cat_Sucursales
                dtCopiar = sql.selec(" SELECT Id, Nombre, ISNULL(Telefono,'') Telefono, Calle, NumInterior, NumExterior, " +
                    "Colonia, CP, Poblacion, IdUsuarioGerente " +
                    "FROM PV_Cat_Sucursales WHERE Activo = 1 ");
                var x = sqlLoc.exec(" TRUNCATE TABLE PVSucursales ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVSucursales");

                ////Cat_Productos
                //dtCopiar = sql.selec("SELECT P.Id, P.Clave, P.Descripcion, M.Marca, P.Presentacion, \n" +
                //    "C.Categoria, L.Descripcion Linea, ISNULL(CE.Iva, 0) Iva, ISNULL(CE.Ieps, 0) Ieps, \n" +
                //    "P.ClaveSAT, P.ClaveUnidad, P.Foto, '' CodigoBarras, P.IdMarca, P.IdLinea \n" +
                //    "FROM PV_Cat_Productos P LEFT JOIN PV_Cat_Marcas M ON M.Id = P.IdMarca \n" +
                //    "LEFT JOIN PV_Cat_Categorias C ON P.IdCategoria = C.Id \n" +
                //    "LEFT JOIN PV_Cat_LineaProd L ON P.IdLinea = L.Id \n" +
                //    "LEFT JOIN PV_Cat_ClavesEsquema CE ON CE.Clave = P.ClaveEsquema");
                //sqlLoc.exec(" TRUNCATE TABLE PVProductos ");
                //sqlLoc.copiaBulto(dtCopiar, "PVProductos");




                //Cat_Productos
                dtCopiar = sql.selec("SELECT P.Id, P.Clave, P.Descripcion, M.Marca, P.Presentacion, \n" +
                    "C.Categoria, L.Descripcion Linea, ISNULL(CE.Iva, 0) Iva, ISNULL(CE.Ieps, 0) Ieps, \n" +
                    "P.ClaveSAT, P.ClaveUnidad, P.Foto, '' CodigoBarras, P.IdMarca, P.IdLinea, ISNULL(P.Pesaje,0) Pesaje \n" +
                    "FROM PV_Cat_Productos P LEFT JOIN PV_Cat_Marcas M ON M.Id = P.IdMarca \n" +
                    "LEFT JOIN PV_Cat_Categorias C ON P.IdCategoria = C.Id \n" +
                    "LEFT JOIN PV_Cat_LineaProd L ON P.IdLinea = L.Id \n" +
                    "LEFT JOIN PV_Cat_ClavesEsquema CE ON CE.Clave = P.ClaveEsquema");
                sqlLoc.exec(" TRUNCATE TABLE PVProductos ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVProductos");

                //Codigos de barra / precios
                //dtCopiar = sqlP.selec("SELECT P.Id, P.IdProducto, P.IdPresentacion, Pres.Presentacion, Uom, CodigoBarras, \n" +
                //    "Precio, P.VerPV, P.IdProductoIndividual, P.sku \n" +
                //    "FROM PV_Cat_PresentacionesVenta_Productos P \n" +
                //    "LEFT JOIN PV_Cat_PresentacionesAlmacen Pres ON P.IdPresentacion = Pres.Id \n" +
                //    "WHERE P.Activo = 1");
                //sqlLoc.exec(" TRUNCATE TABLE PVPresentacionesVentaProd ");
                //cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPresentacionesVentaProd");


                ////Codigos de barra / precios
                //dtCopiar = sql.selec("SELECT P.Id, P.IdProducto, P.IdPresentacion, Pres.Presentacion, Uom, CodigoBarras, Precio, P.VerPV, P.IdProductoIndividual, P.sku \n" +
                //    "FROM PV_Cat_PresentacionesVenta_Productos P \n" +
                //    "LEFT JOIN PV_Cat_PresentacionesAlmacen Pres ON P.IdPresentacion = Pres.Id \n" +
                //    "WHERE P.Activo = 1");
                //sqlLoc.exec(" TRUNCATE TABLE PVPresentacionesVentaProd ");
                //cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPresentacionesVentaProd");



                //Codigos de barra / precios
                dtCopiar = sql.selec("SELECT P.Id, P.IdProducto, P.IdPresentacion, Pres.Presentacion, Uom, CodigoBarras, \n" +
                    "Precio, P.VerPV, P.IdProductoIndividual, P.sku \n" +
                    "FROM PV_Cat_PresentacionesVenta_Productos P \n" +
                    "LEFT JOIN PV_Cat_PresentacionesAlmacen Pres ON P.IdPresentacion = Pres.Id \n" +
                    "WHERE P.Activo = 1");
                sqlLoc.exec(" TRUNCATE TABLE PVPresentacionesVentaProd ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPresentacionesVentaProd");

                string queryPrecios = "SELECT id, idproducto, IdPresentacionVenta, \n" +
                    "ultimoCosto, costoponderado, idSucursal, \n" +
                    "General, Talleres, Distribuidores, \n" +
                    "GeneralAnterior, TalleresAnterior, DistribuidoresAnterior, \n" +
                    "UltimoCostoAnterior, Activo, fechamodificado, \n" +
                    "idusuariomodificado, FechaModificadoCambio \n" +
                    "FROM PV_CAT_PRECIOS WHERE Activo = 1";
                dtCopiar = sql.selec(queryPrecios);
                sqlLoc.exec(" TRUNCATE TABLE PVPrecios ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPrecios");

                //Promociones
                string queryPromo = "SELECT P.Id, P.IdSucursal, P.TipoPromocion, P.Estatus, \n" +
                    "P.FechaInicio, P.FechaFin, P.Descripcion, D.IdProducto, D.Precio, D.IdMarca, \n" +
                    "D.IdLinea, D.Cantidad, D.PrecioPromocion, D.Descuento, D.IdProductoRegalo, \n" +
                    "D.CantidadRegalo, P.Combinado, P.Multiplo, P.Individual \n" +
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
                    "WHERE Estatus = 2 AND P.Valida=1";
                dtCopiar = sql.selec(queryPromo);
                sqlLoc.exec(" TRUNCATE TABLE PVPromociones ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPromociones");

                //Promociones Detalle
                dtCopiar = sql.selec("SELECT PD.Id, PD.IdPromocion, PD.IdProducto IdPresProducto, \n" +
                    "PD.Precio, PD.IdMarca, PD.IdLinea, PD.Cantidad, PD.PrecioPromocion, \n" +
                    "PD.Descuento, PD.IdProductoRegalo, PD.CantidadRegalo \n" +
                    "FROM PVPromocionesDetalle PD \n" +
                    "LEFT JOIN PVPromociones P ON PD.IdPromocion = P.Id \n" +
                    "WHERE P.Estatus = 2");
                sqlLoc.exec(" TRUNCATE TABLE PVPromocionesDetalle ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPromocionesDetalle");

                //Inventario
                dtCopiar = sql.selec("SELECT IdProducto, SUM(Cantidad) Cantidad, IdSucursal, ISNULL(EsCaja, 0) AS EsCaja, Localizacion, \n"
                                    + " CantidadPresentacion FROM Inventario WHERE Activo=1 \n"
                                    + " GROUP BY IdProducto, IdSucursal, EsCaja, Localizacion, CantidadPresentacion");
                sqlLoc.exec(" TRUNCATE TABLE PVInventario ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVInventario");

                //Usuarios sucrusales
                dtCopiar = sql.selec(" SELECT Id, IdUsuario, IdSucursal FROM PV_Cat_UsuarioSucursal WHERE Activo=1 ");
                sqlLoc.exec(" TRUNCATE TABLE PVCat_UsuarioSucursal ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVCat_UsuarioSucursal");


                //Proveedores
                dtCopiar = sql.selec(" SELECT Id,Clave, Nombre, RFC, Telefono, Activo FROM PV_Cat_Proveedores ");
                sqlLoc.exec(" TRUNCATE TABLE PVProveedores ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVProveedores");

                if (cuantos > 0)
                {
                    MessageBox.Show("Catalogos descargados con exito!", "Descarga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (cuantos > 0)
                {
                    MessageBox.Show("Catalogos descargados con éxito!", "Descarga", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //Carga productos
                    cargaProductos();
                    txtProducto.Select();
                }
           
                espera.Close();
                espera.Dispose();
            }
            catch
            {
                goto fin;
            }
        fin:;
            espera.Close();
            espera.Dispose();
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

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            ////Cat_Productos
            //DataTable dtProd = sql.selec("SELECT P.Id, P.Clave, P.Descripcion, M.Marca, P.Presentacion, \n" +
            //    "C.Categoria, L.Descripcion Linea, ISNULL(CE.Iva, 0) Iva, ISNULL(CE.Ieps, 0) Ieps, \n" +
            //    "P.ClaveSAT, P.ClaveUnidad, P.Foto, '' CodigoBarras \n" +
            //    "FROM PV_Cat_Productos P LEFT JOIN PV_Cat_Marcas M ON M.Id = P.IdMarca \n" +
            //    "LEFT JOIN PV_Cat_Categorias C ON P.IdCategoria = C.Id \n" +
            //    "LEFT JOIN PV_Cat_LineaProd L ON P.IdLinea = L.Id \n" +
            //    "LEFT JOIN PV_Cat_ClavesEsquema CE ON CE.Clave = P.ClaveEsquema");
            //sqlLoc.exec(" TRUNCATE TABLE PVProductos ");
            //sqlLoc.copiaBulto(dtProd, "PVProductos");
            
            ////Cat_Precios
            //DataTable dtPrecios = sql.selec(" SELECT Id, IdProducto, PrecioPonderado Precio \n" +
            //    "FROM PV_Cat_Precios WHERE Activo = 1  ");
            //sqlLoc.exec(" TRUNCATE TABLE PVPrecios ");
            //sqlLoc.copiaBulto(dtPrecios, "PVPrecios");

            //cargaProductos();
        }


        
    }
}
