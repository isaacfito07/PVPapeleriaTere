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
    public partial class FCliente : Form
    {
        string rfc = "";
        ConSQL sqlLoc, sql; //ConSQLCE
        string IdUsuario = "";
        int IdVenta = 0;
        int idCliente = 0;
        string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

        public FCliente(ConSQL _sqlLocal, ConSQL _sql, string _IdUsuario)
        {
            InitializeComponent();

            sqlLoc = _sqlLocal;
            sql = _sql;
            IdUsuario = _IdUsuario;
        }

        private void FCliente_Load(object sender, EventArgs e)
        {
            LlenarClientes();
        }

        private void LlenarClientes()
        {
            string concatBusqueda = "";
            if (txtBusqueda.Text.Trim() != "") {
                concatBusqueda = " WHERE CONCAT(Nombre, ' - ' , Telefono , '') LIKE '%"+ txtBusqueda.Text.Trim() +"%'";
            }

            sqlLoc.llenaCombo(cbxCliente, sqlLoc.selec("SELECT 0 Id, '' Nombre UNION ALL SELECT Id, " +
                "CONCAT(Nombre, ' - ' , Telefono , '') Nombre " +
                "FROM PVClientes "+ concatBusqueda +" ORDER BY Nombre "), "Id", "Nombre");
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            LlenarClientes();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnAgregaCliente_Click(object sender, EventArgs e)
        {
            btnAgregaCliente.Enabled = false;
            //Verificar conxion
            if (CheckForInternetConnection())
            {
                //Si hay conexión actualizar el catalogo antes de agregar nuevo usuario
                ActualizarCatalogo();
            }

            //Generar clave random
            string clave = AsignarClave();
            while (ExisteClave(clave))
            {
                clave = AsignarClave();
            }


            if (CamposRequeridos())
            {
                var estado = cbxEstados.Text;
                
                //Guardar nuevo cliente en nube
                string query = string.Format("INSERT INTO PV_Cat_Clientes(Clave, Nombre, Telefono, "
                    + "RFC, Correo, Calle, NumInterior, NumExterior, CP, Municipio, "
                    + "Estado, FechaAlta, IdUsuarioAlta, "
                    + "UsoCFDI, DiasCredito,LimiteCredito, Activo) \n"
                    + "   OUTPUT INSERTED.Id \n"
                    + "VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'"
                    + ",{12},{13},{14},{15},{16})",
                    clave, txtNombreCliente.Text.Trim(),
                    txtTelefono.Text.Trim(), txtRFC.Text.Trim(),
                    txtCorreo.Text.Trim(), txtDomicilio.Text.Trim(), txtNumInt.Text.Trim(),
                    txtNumExt.Text.Trim(), txtCP.Text.Trim(), txtCiudad.Text.Trim(),
                    estado.ToString(), fechaHora, IdUsuario,0,0,0,1);

                var id = sql.scalar(query);
                if (id != null)
                {
                    idCliente = (int)id;
                    ActualizarCatalogo();
                    cbxCliente.SelectedValue = idCliente;
                    btnOK_Click(sender, e);
                    MessageBox.Show("Cliente registrado con éxito",
                       "Atención", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                   // this.Close();


                }
                else
                {
                    MessageBox.Show("Ocurrio un error al guardar el cliente, verifica la información e intentalo de nuevo",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                }
            }
            else
            {
                MessageBox.Show("Ocurrio un error al guardar el cliente, verifica la información e intentalo de nuevo",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }

            btnAgregaCliente.Enabled = true;
        }

        private string AsignarClave()
        {
            Random rdm = new Random();
            int value = rdm.Next(1000);
            string clave = value.ToString("000");
            
            return clave;
        }

        private bool CamposRequeridos()
        {
            if (string.IsNullOrEmpty(txtNombreCliente.Text.Trim()) || string.IsNullOrEmpty(txtTelefono.Text.Trim()))
                return false;
            else
                return true;
        }

        private bool ExisteClave(string clave)
        {
            var existe = sql.scalar("SELECT Clave FROM PVClientes WHERE Clave = '" + clave + "'");
            return existe == null ? false : true;
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (ActualizarCatalogo())
            {
                MessageBox.Show("Se descargó el catalogo de clientes", "Descarga",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool ActualizarCatalogo()
        {
            //Refrescar catalogo clientes
            string query = " SELECT Id, Clave, Nombre, RFC, Calle, NumInterior, " +
                "NumExterior, Colonia, CP, Telefono, Poblacion, Municipio, Estado, " +
                "CONVERT(int, ISNULL(UsoCFDI, 0)) UsoCFDI, RegimenFiscal, " +
                "DiasCredito, LimiteCredito, FechaAlta, IdUsuarioAlta " +
                "FROM PV_Cat_Clientes WHERE Activo = 1 ";
            DataTable dtClientes = sql.selec(query);
            sqlLoc.exec(" TRUNCATE TABLE PVClientes ");

            if (sqlLoc.copiaBulto(dtClientes, "PVClientes") > 0)
            {
                LlenarClientes();
                return true;
            }

            return false;
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

        public int IdCliente
        {
            get
            {
                if (cbxCliente.SelectedValue != null)
                {
                    int id = 0;
                    if (int.TryParse(cbxCliente.SelectedValue.ToString(), out id))
                        return int.Parse(cbxCliente.SelectedValue.ToString());
                }
                return 0;
            }
        }

       
        public string NombreCliente
        {
            get
            {
                string nombre = "";
                if (cbxCliente.SelectedValue != null && cbxCliente.SelectedValue.ToString() != "0")
                    nombre = cbxCliente.Text;

                return nombre;
            }
        }
    }
}
