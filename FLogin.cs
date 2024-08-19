using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FLogin : Form
    {
        public ConSQL sqlP, sqlLoc;
        public string nombreUsuario, sucursal, idSucursal, usuario, idUsuario, numCaja = "0";

        static string dirProductos = @"C:\ImgPuntoVenta\Productos\";

        public FLogin()
        {
            InitializeComponent();

            sqlP = new ConSQL(ConfigurationManager.ConnectionStrings["conCenor"].ConnectionString);
            sqlLoc = new ConSQL(ConfigurationManager.ConnectionStrings["PVLaJoya"].ConnectionString);

            //crear directorios de imagenes
            if (!Directory.Exists(@"C:\ImgPuntoVenta\"))
            {
                Directory.CreateDirectory(@"C:\ImgPuntoVenta\");
            }

            if (!Directory.Exists(dirProductos))
            {
                Directory.CreateDirectory(dirProductos);
            }

            this.lblVersion.Text = String.Format("Versión {0}", AssemblyVersion);
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            BuscaSucursalesPorUsuario();
        }

        private void BuscaSucursalesPorUsuario() {
            string query = " SELECT S.Id, S.Nombre FROM PVCat_UsuarioSucursal SU \n"
                            + " LEFT JOIN PVSucursales S ON SU.IdSucursal = S.Id\n"
                            + " LEFT JOIN PVUsuarios U ON SU.IdUsuario = U.Id\n"
                            + " WHERE U.Usuario='"+ txtUsuario.Text.Trim() +"'";

            sqlLoc.llenaCombo(cbxSitio, sqlLoc.selec(query), "Id", "Nombre");
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BuscaSucursalesPorUsuario();
            }
            
        }

        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            BuscaSucursalesPorUsuario();
        }

        private void txtUsuario_Click(object sender, EventArgs e)
        {
            BuscaSucursalesPorUsuario();
        }

        private void FLogin_Load(object sender, EventArgs e)
        {
            //Llenar combo sucursales
            //string query = "SELECT Id, Nombre FROM PVSucursales ORDER BY Nombre";
            //sqlLoc.llenaCombo(cbxSitio, sqlLoc.selec(query), "Id", "Nombre");
        }

        private void BtnEntrar_Click(object sender, EventArgs e)
        {
            FEspera espera = new FEspera();
            espera.Text = "INICIANDO...";
            espera.Show();

            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                MessageBox.Show("Escribe tu nombre de usuario", "Falta Usuario", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                goto fin;
            }
            
            if (cbxSitio.SelectedIndex == -1 || string.IsNullOrEmpty(cbxSitio.SelectedValue.ToString()))
            {
                MessageBox.Show("Selecciona una tienda", "Falta Tienda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                goto fin;
            }

            if (string.IsNullOrEmpty(txtContra.Text))
            {
                MessageBox.Show("Escribe tu contraseña", "Falta contraseña", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                goto fin;
            }

            //Revisa que exista el usuario y contraseña
            string query = "SELECT *  " +
                    "FROM PVUsuarios WHERE Usuario = '" + txtUsuario.Text.Trim() + "' ";
            DataTable dtUsuario = sqlLoc.selec(query);
            //Usuario existe
            if (dtUsuario.Rows.Count > 0)
            {
                DataRow r = dtUsuario.Rows[0];
                string hoy = DateTime.Now.ToString("yyyy-MM-dd");
                string contrasenaUsuario = r["Contrasena"].ToString().Trim();
                //Verificar contraseña
                if (txtContra.Text.Trim() == contrasenaUsuario)
                {
                    //Revisar si el usuario tiene permiso para entrar al punto de venta
                    if (Convert.ToBoolean(r["PuntoVenta"]))
                    {
                        usuario = txtUsuario.Text.Trim();
                        nombreUsuario = r["Nombres"].ToString();
                        idUsuario = r["Id"].ToString();
                        sucursal = cbxSitio.Text;
                        idSucursal = cbxSitio.SelectedValue.ToString();
                        /*Verificar si la caja tiene fondo
                        var fCaja = sqlLoc.scalar(" SELECT TOP(1) Id FROM PVFondoCaja WHERE IdSucursal = '" + idSucursal + "'\n"
                                + " AND IdUsuario='" + idUsuario + "' AND FolioCorteParcialCaja IS NULL AND FolioCorteCaja IS NULL ORDER BY Id DESC");
                        
                        if (fCaja != null)
                        {
                            FMenu menu = new FMenu(sqlP, sqlLoc, nombreUsuario, idSucursal, sucursal, idUsuario, numCaja);
                            this.Hide();
                            menu.ShowDialog();
                        }
                        else
                        {
                            FCajaFondo fondoCaja = new FCajaFondo(sqlP, sqlLoc, nombreUsuario, 
                                idSucursal, sucursal, idUsuario, numCaja);
                            this.Hide();
                            fondoCaja.ShowDialog();
                        }*/

                        if (CheckForInternetConnection())
                        {
                            //Sincronizar inventario
                            DataTable dtCopiar = sqlP.selec("SELECT IdProducto, SUM(Cantidad) Cantidad, IdSucursal, EsCaja, Localizacion, \n"
                                                + " CantidadPresentacion FROM PV_Inventario \n" // WHERE Activo=1
                                                + " GROUP BY IdProducto, IdSucursal, EsCaja, Localizacion, CantidadPresentacion");
                            sqlLoc.exec(" TRUNCATE TABLE PVInventario ");
                            sqlLoc.copiaBulto(dtCopiar, "PVInventario");
                        }
                       

                        FMenu menu = new FMenu(sqlP, sqlLoc, nombreUsuario, idSucursal, sucursal, idUsuario, numCaja);
                        this.Hide();
                        espera.Close();
                        menu.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("No tienes el permiso para acceder a punto de venta, solicitalo a Dirección", "Sin permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto fin;
                    }
                }
                else
                {
                    MessageBox.Show("Contraseña incorrecta", "Contraseña incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    goto fin;
                }
            }
            else
            {
                MessageBox.Show("No existe el usuario " + txtUsuario.Text.Trim(), "No existe usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
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

        private void BtnSicronizar_Click(object sender, EventArgs e)
        {
            gbxLogin.Enabled = false;

            FEspera espera = new FEspera();
            espera.Text = "DESCARGANDO CATALOGOS";
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
                dtCopiar = sqlP.selec(query);
                sqlLoc.exec(" TRUNCATE TABLE PVClientes ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVClientes");
                

                string queryTipoCliente = "SELECT id, TipoCliente, Porcentaje, 1 AS Activo, FechaAlta, idUsuarioAlta, FechaModificado, idUsuarioModificado \n" +
                    "FROM Cat_TipoCliente WHERE Activo = 1";
                dtCopiar = sqlP.selec(queryTipoCliente);
                sqlLoc.exec(" TRUNCATE TABLE PVTipoCliente ");
                if (dtCopiar.Rows.Count > 0)
                {
                    bool d = InsertTable("PVTipoCliente", dtCopiar);
                }

                //Monedero Cliente
                string queryMonedero = "SELECT FolioVenta, IdCliente, Monto, Valido, \n" +
                    "FechaAlta, IdUsuarioAlta, 1 DisparadoNube \n" +
                    "FROM PVMonederoCliente WHERE Valido = 1";
                dtCopiar = sqlP.selec(queryMonedero);
                sqlLoc.exec(" TRUNCATE TABLE PVMonederoCliente ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVMonederoCliente");

                //Devoluciones Cliente
                dtCopiar = sqlP.selec("SELECT Id, FolioVenta, IdCliente, IdProducto, \n" +
                    " CantidadOriginal, CantidadDevuelta, MontoDevolucion, Activo, \n" +
                    " Motivo, Monedero, Inventario, InventarioDefectuoso, \n" +
                    " FechaAlta, IdUsuarioAlta, 1 DisparadoNube, EsCaja, Uom \n" +
                    " FROM PVDevoluciones");
                sqlLoc.exec(" TRUNCATE TABLE PVDevoluciones ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVDevoluciones");

                //Cat_Usuarios
                dtCopiar = sqlP.selec(" SELECT Id, Nombres, Usuario, 'ledsco' Correo, Contrasena, " +
                    " PuntoVenta, CancelarVenta, CorteCaja, EstadoCaja, CorreccionesCaja, " +
                    " RetiroCaja, Descuentos, Arqueo, Devoluciones, ModificarPrecioVenta " +
                    " FROM PV_Cat_Usuarios WHERE Activo = 1 ");
                sqlLoc.exec(" TRUNCATE TABLE PVUsuarios ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVUsuarios");

                //Cat_Sucursales
                dtCopiar = sqlP.selec(" SELECT Id, Nombre, ISNULL(Telefono,'') Telefono, Calle, NumInterior, NumExterior, " +
                    "Colonia, CP, Poblacion, IdUsuarioGerente " +
                    "FROM PV_Cat_Sucursales WHERE Activo = 1 ");
                var x = sqlLoc.exec(" TRUNCATE TABLE PVSucursales ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVSucursales");
                
                //Cat_Productos
                dtCopiar = sqlP.selec("SELECT P.Id, P.Clave, P.Descripcion, M.Marca, P.Presentacion, \n" +
                    "C.Categoria, L.Descripcion Linea, ISNULL(CE.Iva, 0) Iva, ISNULL(CE.Ieps, 0) Ieps, \n" +
                    "P.ClaveSAT, P.ClaveUnidad, P.Foto, '' CodigoBarras, P.IdMarca, P.IdLinea, ISNULL(P.Pesaje,0) Pesaje \n" +
                    "FROM PV_Cat_Productos P LEFT JOIN PV_Cat_Marcas M ON M.Id = P.IdMarca \n" +
                    "LEFT JOIN PV_Cat_Categorias C ON P.IdCategoria = C.Id \n" +
                    "LEFT JOIN PV_Cat_LineaProd L ON P.IdLinea = L.Id \n" +
                    "LEFT JOIN PV_Cat_ClavesEsquema CE ON CE.Clave = P.ClaveEsquema");
                sqlLoc.exec(" TRUNCATE TABLE PVProductos ");
                sqlLoc.copiaBulto(dtCopiar, "PVProductos");

                //Codigos de barra / precios
                dtCopiar = sqlP.selec("SELECT P.Id, P.IdProducto, P.IdPresentacion, Pres.Presentacion, Uom, CodigoBarras, \n" +
                    "Precio, P.VerPV, P.IdProductoIndividual, P.sku \n" +
                    "FROM PV_Cat_PresentacionesVenta_Productos P \n" +
                    "LEFT JOIN PV_Cat_PresentacionesAlmacen Pres ON P.IdPresentacion = Pres.Id \n" +
                    "WHERE P.Activo = 1");
                sqlLoc.exec(" TRUNCATE TABLE PVPresentacionesVentaProd ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPresentacionesVentaProd");

                // Precios de Nueva Version
                string queryPrecios = "SELECT id, idproducto, IdPresentacionVenta, \n" +
                    "ultimoCosto, costoponderado, idSucursal, \n" +
                    "General, Talleres, Distribuidores, \n" +
                    "GeneralAnterior, TalleresAnterior, DistribuidoresAnterior, \n" +
                    "UltimoCostoAnterior, Activo, fechamodificado, \n" +
                    "idusuariomodificado, FechaModificadoCambio \n" +
                    "FROM PV_CAT_PRECIOS WHERE Activo = 1";
                dtCopiar = sqlP.selec(queryPrecios);
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
                dtCopiar = sqlP.selec(queryPromo);
                sqlLoc.exec(" TRUNCATE TABLE PVPromociones ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPromociones");

                //Promociones Detalle
                dtCopiar = sqlP.selec("SELECT PD.Id, PD.IdPromocion, PD.IdProducto IdPresProducto, \n" +
                    "PD.Precio, PD.IdMarca, PD.IdLinea, PD.Cantidad, PD.PrecioPromocion, \n" +
                    "PD.Descuento, PD.IdProductoRegalo, PD.CantidadRegalo \n" +
                    "FROM PVPromocionesDetalle PD \n" +
                    "LEFT JOIN PVPromociones P ON PD.IdPromocion = P.Id \n" +
                    "WHERE P.Estatus = 2");
                sqlLoc.exec(" TRUNCATE TABLE PVPromocionesDetalle ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVPromocionesDetalle");

                //Inventario
                dtCopiar = sqlP.selec("SELECT IdProducto, SUM(Cantidad) Cantidad, IdSucursal, ISNULL(EsCaja, 0) AS EsCaja, Localizacion, \n"
                                    + " CantidadPresentacion FROM Inventario WHERE Activo=1 \n"
                                    + " GROUP BY IdProducto, IdSucursal, EsCaja, Localizacion, CantidadPresentacion");
                sqlLoc.exec(" TRUNCATE TABLE PVInventario ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVInventario");

                //Usuarios sucrusales
                dtCopiar = sqlP.selec(" SELECT Id, IdUsuario, IdSucursal FROM PV_Cat_UsuarioSucursal WHERE Activo=1 ");
                sqlLoc.exec(" TRUNCATE TABLE PVCat_UsuarioSucursal ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVCat_UsuarioSucursal");


                //Proveedores
                dtCopiar = sqlP.selec(" SELECT Id,Clave, Nombre, RFC, Telefono, Activo FROM PV_Cat_Proveedores ");
                sqlLoc.exec(" TRUNCATE TABLE PVProveedores ");
                cuantos += sqlLoc.copiaBulto(dtCopiar, "PVProveedores");

                if (cuantos > 0)
                {
                    MessageBox.Show("Catalogos descargados con exito!", "Descarga", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //Llenar combo sucursales
                BuscaSucursalesPorUsuario();
                //string querySuc = "SELECT Id, Nombre FROM PVSucursales ORDER BY Nombre";
                //sqlLoc.llenaCombo(cbxSitio, sqlLoc.selec(querySuc), "Id", "Nombre");

                espera.Close();
                espera.Dispose();
            }
            catch {
                goto fin;
            }
        fin:;
            gbxLogin.Enabled = true;
            espera.Close();
            espera.Dispose();
        }

        private void BtnImagenes_Click(object sender, EventArgs e)
        {
            //imagenes
            if (!CheckForInternetConnection())
            {
                MessageBox.Show("No hay conexion a internet, revisa tu conexión", "Sin internet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                goto fin;
            }

            DialogResult respuesta = MessageBox.Show("El proceso de descarga de imagenes puede ser tardado, " +
                "dependiendo su tamaño y cantidad de productos, ¿Quieres continuar?", "Confirmación", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (respuesta == DialogResult.OK)
            {
                gbxLogin.Enabled = false;
                FEspera espera = new FEspera();
                espera.Text = " DESCARGANDO IMAGENES... ";
                espera.Show();

                string queryProductos =
                    " SELECT Id, Foto FROM PV_Cat_Productos " +
                    " WHERE Activo = 1 AND Foto IS NOT NULL AND Foto != '' ";

                //sqlP = new ConSQL(cadeP);
                DataTable dtProductos = sqlP.selec(queryProductos);

                PictureBox pbxImg = new PictureBox();

                foreach (DataRow dr in dtProductos.Rows)
                {
                    var img = dr["Foto"].ToString();
                    if (!File.Exists(dirProductos + dr["Foto"].ToString()))
                    {
                        try
                        {
                            string rutaImagen = "https://terepapeleria.ledsco.com.mx/FotosProductos/" + dr["Foto"].ToString();
                            pbxImg.Load(rutaImagen);
                            pbxImg.Image.Save(dirProductos + dr["Foto"].ToString());
                        }
                        catch { }
                    }
                }

                MessageBox.Show(" Imagenes descargadas con exito! ", "Descarga", MessageBoxButtons.OK, MessageBoxIcon.Information);

                gbxLogin.Enabled = true;
                espera.Close();
                espera.Dispose();
            }
            else
            {
                MessageBox.Show("Descarga cancelada", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        fin:;
        }

        public bool InsertTable(string TableName, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                int e = 0;
                string values = "(";
                string queryInsertTable = "INSERT INTO "+TableName + " (";
                foreach (DataColumn column in dt.Columns)
                {
                    e++;
                    object dato = objetoInsert(dr[column.ColumnName]);
                    if (e == dt.Columns.Count)
                    {
                        queryInsertTable += column.ColumnName + ") VALUES ";
                        values += dato + ")";
                    }
                    else
                    {
                        queryInsertTable += column.ColumnName + ",";
                        values += dato + ",";
                    }
                }
                queryInsertTable = queryInsertTable + values;
                sqlLoc.exec(queryInsertTable);
            }
            return true;
        }

        public object objetoInsert(object val)
        {
            if (val.GetType() == typeof(string))
            {
                val = (string)"'"+val+"'";
            }
            else if (val.GetType() == typeof(int))
            {
                val = (int)val;
            }
            else if (val.GetType() == typeof(DateTime))
            {
                val = (DateTime)val;
            }
            else
            {
                val = "''";
            }
            return val;
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
