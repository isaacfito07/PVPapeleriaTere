using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace PVLaJoya
{
    public partial class FReferenciaServicio : Form
    {
        ConSQL sqlLoc;
        string sku = "";

        public FReferenciaServicio(ConSQL _sqlLoc, string _sku)
        {
            sqlLoc = _sqlLoc;
            InitializeComponent();
            sku = _sku;
        }

        private void FReferenciaServicio_Load(object sender, EventArgs e)
        {
            if (sku.Contains("UBER")) {
                mtbComision.Text = "$0.00";
            }

            // mtbMonto.Mask = "$#,###,###.00";
            mtbMonto.MaskInputRejected += new MaskInputRejectedEventHandler(mtbMonto_MaskInputRejected);
            mtbMonto.KeyDown += new KeyEventHandler(mtbMonto_KeyDown);

            //mtbNumeroReferencia.Select(0, mtbNumeroReferencia.Text.Length);
            mtbNumeroReferencia.SelectAll();
            mtbNumeroReferencia.Focus();

            
        }


        //TELEFONO
        private void mtbMonto_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            if (mtbMonto.MaskFull)
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("Sólo puedes ingresar números (0-9) en este campo.", mtbMonto, 0, -20, 5000);
            }
            else if (e.Position == mtbMonto.Mask.Length)
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("Sólo puedes ingresar números (0-9) en este campo.", mtbMonto, 0, -20, 5000);
            }
            else
            {
                toolTip1.ToolTipTitle = "Datos incorrectos";
                toolTip1.Show("Sólo puedes ingresar números (0-9) en este campo.", mtbMonto, 0, -20, 5000);
            }
        }

        private void mtbNumeroTelefonico_KeyDown(object sender, KeyEventArgs e)
        {
            // The balloon tip is visible for five seconds; if the user types any data before it disappears, collapse it ourselves.
            toolTip1.Hide(mtbMonto);
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            double monto = 0;
            double.TryParse(mtbMonto.Text.Replace("$", "").Replace(",", ""), out monto);

            bool hayError = false;

            if (ValidaReferencia()) {
                if ((sku.Contains("NETFLIX") && monto < 300) || (sku.Contains("NETFLIX") && monto > 3000)) {
                    MessageBox.Show("El monto es inválido. Solo puede pagar un mínimo de $300.00 y hasta un máximo de $3,000.00. Favor de verificar",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                    hayError = true;
                }
                else if ((sku.Contains("UBER") && monto > 1500))
                {
                    MessageBox.Show("El monto es inválido. No puede pagar una cantidad mayor a $1,500.00. Favor de verificar",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                    hayError = true;

                }
                else if ((sku.Contains("GASNAT") && monto > 10000)|| (sku.Contains("ECOGAS") && monto > 10000) || (sku.Contains("TELMEX") && monto > 10000)
                     || (sku.Contains("DISH") && monto > 10000) || (sku.Contains("SKY") && monto > 10000) || (sku.Contains("IZI") && monto > 10000) || (sku.Contains("CREDHIPOTECA") && monto > 10000))
                {
                    MessageBox.Show("El monto es inválido. No puede pagar una cantidad mayor a $10,000.00. Favor de verificar",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                    hayError = true;

                }
                else if ((sku.Contains("S3LUZCFEMXNV") && monto > 20000)) //Vencidos
                {
                    MessageBox.Show("El monto es inválido. No puede pagar una cantidad mayor a $20,000.00. Favor de verificar",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                    hayError = true;

                }
                else if ((sku.Contains("LUZCFEONLINEMXN") && monto > 26000)) //Online
                {
                    MessageBox.Show("El monto es inválido. No puede pagar una cantidad mayor a $26,000.00. Favor de verificar",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                    hayError = true;

                }
                else if ((sku.Contains("S3LUZCFEMXN") && monto > 29000)) //Vigentes
                {
                    MessageBox.Show("El monto es inválido. No puede pagar una cantidad mayor a $29,000.00. Favor de verificar",
                              "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button2);
                    hayError = true;

                }
                else
                    this.Close();

                if (hayError)
                    mtbMonto.Text = "";
            }
            else
            {
                MessageBox.Show("Las referencias no coinciden o el monto es inválido. Favor de verificar",
                          "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button2);
            }
        }

        private bool ValidaReferencia()
        {
            bool validado = false;

            if ((mtbNumeroReferencia.Text == mtbNumeroReferenciaRepetir.Text) && mtbMonto.Text.Trim() != "")
                validado = true;

            return validado;
        }

        private void mtbMonto_Validating(object sender, CancelEventArgs e)
        {
            string value;
            NumberStyles style;
            CultureInfo culture;
            decimal currency;

            string input = mtbMonto.Text.Trim();
            if (!input.StartsWith("$"))
            {
                mtbMonto.Text = "$" + mtbMonto.Text;
            }

             value = mtbMonto.Text;
            style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            culture = CultureInfo.CreateSpecificCulture("en-US");
            if (!Decimal.TryParse(value, style, culture, out currency))
            {
                MessageBox.Show("Por favor, ingrese un monto válido.", "Monto inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // prevent the textbox from losing focus
                e.Cancel = true;
            }
        }

        private void mtbMonto_Validated(object sender, EventArgs e)
        {
            string input = mtbMonto.Text.Trim();
            if (input.StartsWith("$"))
            {
                string temp = input.Replace("$", "");
                string specifier = "C";
                CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                mtbMonto.Text = Decimal.Parse(temp).ToString(specifier, culture);
            }
            else {
                mtbMonto.Text = "$"+ mtbMonto.Text;

                if (input.StartsWith("$"))
                {
                    string temp = input.Replace("$", "");
                    string specifier = "C";
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
                    mtbMonto.Text = Decimal.Parse(temp).ToString(specifier, culture);
                }
            }

            if (mtbMonto.Text.Trim() != "") {
                var CantidadConLetra = sqlLoc.scalar("SELECT .dbo.CantidadConLetraMoneda("+ mtbMonto.Text.Trim().Replace("$", "").Replace(",","") +")");
                if (CantidadConLetra != null)
                    lblCantLetra.Text = CantidadConLetra.ToString();
                else
                    lblCantLetra.Text = "";

            }
        }

        private void mtbMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            //string value;
            //NumberStyles style;
            //CultureInfo culture;
            //decimal currency;


            //value = mtbMonto.Text;
            //style = NumberStyles.Number | NumberStyles.AllowCurrencySymbol;
            //culture = CultureInfo.CreateSpecificCulture("en-US");

        }

        private void mtbNumeroReferencia_KeyPress(object sender, KeyPressEventArgs e)
        {
           
        }

        private void mtbNumeroReferencia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mtbNumeroReferenciaRepetir.Select(0, mtbNumeroReferenciaRepetir.Text.Length);
                mtbNumeroReferenciaRepetir.Focus();
            }
        }


        private void mtbNumeroReferenciaRepetir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                mtbMonto.Select(0, mtbMonto.Text.Length);
                mtbMonto.Focus();
            }
        }

        private void mtbMonto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK.Focus();
            }
        }

        public string ReferenciaServicio
        {
            get
            {
                return mtbNumeroReferencia.Text;
            }
        }

        public string MontoServicio
        {
            get
            {
                return mtbMonto.Text;
            }
        }

        public string MontoComision
        {
            get {
                return mtbComision.Text.Replace("$","").Replace(",","");
            }
        }

       
    }


}
