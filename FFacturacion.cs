using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FFacturacion : Form
    {
        string rfc = "";
        ConSQL sqlLoc, sql;
        string IdUsuario = "", folioVenta;
        string fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");


        public FFacturacion(ConSQL _sqlLocal, ConSQL _sql, string _IdUsuario, string _folioVenta)
        {
            InitializeComponent();

            sqlLoc = _sqlLocal;
            sql = _sql;
            IdUsuario = _IdUsuario;
            folioVenta = _folioVenta;
        }

        private void fFacturacion_Load(object sender, EventArgs e)
        {
            llenarClientes();
        }

        private void llenarClientes()
        {
            sqlLoc.llenaCombo(cbxCliente, 
                sqlLoc.selec(" SELECT RFC, CONCAT(Nombre, ' - ', RFC) Cliente " +
                "FROM PVClientes ORDER BY Nombre "), "RFC", "Cliente");
        }

        /*private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                sqlLoc.llenaCombo(cbxCliente, 
                    sqlLoc.selec(" SELECT RFC, CONCAT(Nombre, ' - ', RFC) Cliente " +
                    "FROM PVClientes " +
                    "WHERE (Nombre LIKE '%" + txtCliente.Text.Trim() + "%' " +
                    "OR RFC LIKE '%" + txtCliente.Text.Trim() + "%') ORDER BY Cliente "), 
                    "RFC", "Cliente");
            }
        }*/

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbxCliente.SelectedValue != null)
            {
                cbPublicoGeneral.Checked = false;
                rfc = cbxCliente.SelectedValue.ToString();

                //Carga datos del cliente seleccionado
                DataTable dtRFC = sqlLoc.selec(" SELECT * FROM PVClientes WHERE RFC = '" + rfc + "' ");
                if (dtRFC.Rows.Count > 0)
                {
                    txtNombreCliente.Text = dtRFC.Rows[0]["Nombre"].ToString();
                    txtRFC.Text = dtRFC.Rows[0]["RFC"].ToString();
                    txtCorreo.Text = dtRFC.Rows[0]["Correo"].ToString();
                    txtCalle.Text = dtRFC.Rows[0]["Calle"].ToString();
                    txtNumInt.Text = dtRFC.Rows[0]["NumInterior"].ToString();
                    txtNumExt.Text = dtRFC.Rows[0]["NumExterior"].ToString();
                    txtColonia.Text = dtRFC.Rows[0]["Colonia"].ToString();
                    txtCP.Text = dtRFC.Rows[0]["CP"].ToString();
                    txtMunicipio.Text = dtRFC.Rows[0]["Municipio"].ToString();
                    cbxEstados.Text = dtRFC.Rows[0]["Estado"].ToString();
                    txtTelefono.Text = dtRFC.Rows[0]["Telefono"].ToString();
                }

                //this.Close();
            }
            else
            {
                MessageBox.Show("Selecciona un cliente", "Espera", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
        }

        private void fFacturacion_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (rfc == "")
            {
                DialogResult dr = MessageBox.Show("No se seleccionó cliente para facturar, ¿continuar sin factura?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        public string RFCFactura
        {
            get
            {
                return rfc;
            }
        }

        private void txtRFC_Leave(object sender, EventArgs e)
        {
            if (isRFC(txtRFC.Text.Trim()))
            {

                var existe = sqlLoc.scalar(" SELECT RFC " +
                    "FROM PVClientes WHERE RFC = '" + txtRFC.Text.Trim() + "' ");
                if (existe != null)
                {
                    MessageBox.Show("Este RFC ya esta registrado", "Espera", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                    cbxCliente.SelectedValue = txtRFC.Text.Trim();
                    btnOK_Click(sender, e);
                    // txtRFC.Text = "";
                }

            }
        }

        private void btnAgregaCliente_Click(object sender, EventArgs e)
        {
            if (cbPublicoGeneral.Checked)
            {
                txtRFC.Text = "X0X0X0X0X0";
            }
            else
            {
                if (txtNombreCliente.Text.Trim() == "")
                {
                    txtNombreCliente.Focus();
                    MessageBox.Show("Escribe el nombre del cliente", "Falta", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1); txtNombreCliente.Focus();
                    goto fin;
                }

                if (!isRFC(txtRFC.Text.Trim()))
                {
                    txtRFC.Focus();
                    MessageBox.Show("Escribe el RFC del cliente", "Falta", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                if (!isCorreo(txtCorreo.Text.Trim()))
                {
                    txtCorreo.Focus();
                    MessageBox.Show("Escribe el correo del cliente", "Falta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                if (txtCalle.Text.Trim() == "")
                {
                    txtCalle.Focus();
                    MessageBox.Show("Escribe el domicilio del cliente", "Falta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                if (txtCP.Text.Trim() == "")
                {
                    txtCP.Focus();
                    MessageBox.Show("Escribe el codigo postal del cliente", "Falta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                if (txtMunicipio.Text.Trim() == "")
                {
                    txtMunicipio.Focus();
                    MessageBox.Show("Escribe la ciudad del cliente", "Falta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

                if (cbxEstados.Text.Trim() == "")
                {
                    cbxEstados.Focus();
                    // MessageBox.Show("Selecciona el estado del cliente", "Falta", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                    goto fin;
                }

            }

            //Verifica si el rfc está registrado, si sí, solo hay que actualizar.
            var existe = sqlLoc.scalar(" SELECT RFC FROM PVClientes " +
                "WHERE RFC = '" + txtRFC.Text.Trim() + "' ");
            if (existe != null)
            {
                if (cbPublicoGeneral.Checked == false)
                {
                    string upd =
                    " UPDATE PVClientes SET  " +
                    " Nombre = '" + txtNombreCliente.Text.Trim() + "', " +
                    " RFC = '" + txtRFC.Text.Trim() + "', " +
                    " Calle = '" + txtRFC.Text.Trim() + "'," +
                    " NumInterior = '" + txtNumInt.Text.Trim() + "'," +
                    " NumExterior = '" + txtNumExt.Text.Trim() + "'," +
                    " Municipio = '" + txtMunicipio.Text.Trim() + "', " +
                    " Estado = '" + cbxEstados.Text.Trim() + "', " +
                    " CP = '" + txtCP.Text.Trim() + "', " +
                    " Telefono = '" + txtTelefono.Text.Trim() + "', " +
                    " IdUsuarioAlta = " + IdUsuario + ",  " +
                    " FechaAlta = '" + fechaHora + "', " +
                    " Correo = '" + txtCorreo.Text.Trim() + "' " +
                    " WHERE RFC = '" + txtRFC.Text.Trim() + "' ";

                    if (sqlLoc.exec(upd) > 0)
                    {
                        llenarClientes();

                        cbxCliente.SelectedValue = txtRFC.Text;
                        
                        if (cbxCliente.SelectedValue != null)
                        {
                            rfc = cbxCliente.SelectedValue.ToString();
                            this.Close();
                        }
                    }
                }
            }
            else
            {

                string ins =
                " INSERT INTO PVClientes " +
                " (Nombre, RFC, Calle, NumInterior, NumExterior, Municipio, Estado, " +
                " CP, Telefono, IdUsuarioAlta, FechaAlta, Correo) " +
                " VALUES " +
                " ( " +
                " '" + txtNombreCliente.Text.Trim() + "', " +
                " '" + txtRFC.Text.Trim() + "', " +
                " '" + txtCalle.Text.Trim() + "', " +
                " '" + txtNumInt.Text.Trim() + "', " +
                " '" + txtNumExt.Text.Trim() + "', " +
                " '" + txtMunicipio.Text.Trim() + "', " +
                " '" + cbxEstados.Text.Trim() + "', " +
                " '" + txtCP.Text.Trim() + "', " +
                " '" + txtTelefono.Text.Trim() + "', " +
                " " + IdUsuario + ", " +
                " '" + fechaHora + "', " +
                " '" + txtCorreo.Text.Trim() + "' " +
                " ) ";

                if (sqlLoc.exec(ins) > 0)
                {
                    llenarClientes();

                    cbxCliente.SelectedValue = txtRFC.Text;

                    MessageBox.Show("Cliente agregado con éxito.", "Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    if (cbxCliente.SelectedValue != null)
                    {
                        rfc = cbxCliente.SelectedValue.ToString();
                        this.Close();
                    }
                }
            }

            //Actualiza venta con el el rfc del cliente
            int y = sqlLoc.exec(" UPDATE TOP(1) PVVentas " +
                "SET RFCFactura= '" + txtRFC.Text.Trim() + "', " +
                "FechaFactura= '" + fechaHora + "', " +
                "UsuarioFactura= '" + IdUsuario + "' WHERE FolioVenta = " + folioVenta);
            if (y != 0)
            {
                MessageBox.Show("Factura generada con éxito", "Factura", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }

            fin:;

        }

        private static bool isRFC(string strIn)
        {
            bool invalid = false;
            bool regreso;
            invalid = false;
            if (string.IsNullOrEmpty(strIn))
                return false;

            if (invalid)
                return false;

            try
            {
                regreso = Regex.IsMatch(strIn,
                      @"^([A-ZÑ\x26]{3,4}([0-9]{2})(0[1-9]|1[0-2])(0[1-9]|1[0-9]|2[0-9]|3[0-1]))((-)?([A-Z\d]{3}))?$",
                      RegexOptions.IgnoreCase);

                return regreso;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            //Refrescar catalogo clientes
            //Cat_Clientes
            string query = " SELECT Id, Clave, Nombre, RFC, Calle, NumInterior, " +
                "NumExterior, Colonia, CP, Poblacion, Municipio, Estado, " +
                "CONVERT(int, ISNULL(UsoCFDI, 0)) UsoCFDI, RegimenFiscal, " +
                "DiasCredito, LimiteCredito, FechaAlta, IdUsuarioAlta, Telefono, Correo " +
                "FROM PV_Cat_Clientes WHERE Activo = 1 ";
            DataTable dtClientes = sql.selec(query);
            sqlLoc.exec(" TRUNCATE TABLE PVClientes ");

            if (sqlLoc.copiaBulto(dtClientes, "PVClientes") > 0)
            {
                MessageBox.Show("Se descargó el catalogo de clientes", "Descarga",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                llenarClientes();
            }
        }

        private static bool isCorreo(string strIn)
        {

            if (strIn == "")
            {
                strIn = "-";
            }

            try
            {
                MailAddress m = new MailAddress(strIn);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
