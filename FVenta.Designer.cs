
using System.Windows.Forms;

namespace PVLaJoya
{
    partial class FVenta
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle89 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle90 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle91 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle92 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle93 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle94 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle95 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle96 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVenta));
            this.gbxVenta = new System.Windows.Forms.GroupBox();
            this.btnReimprimir = new System.Windows.Forms.Button();
            this.btnTerminar = new System.Windows.Forms.Button();
            this.lblEspere = new System.Windows.Forms.Label();
            this.lblIEPS = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblIEPSTit = new System.Windows.Forms.Label();
            this.lblIVATit = new System.Windows.Forms.Label();
            this.btnBuscarCodigo = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblFolio = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.dgvVenta = new System.Windows.Forms.DataGridView();
            this.colIdProducto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProducto = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResta = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSuma = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colPrecioFinal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IVA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IEPS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EsCaja = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdPresentacionVenta = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdMarca = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdLinea = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FechaAgregado = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Sku = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MontoComision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IdOrden = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtScan = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnEspera = new System.Windows.Forms.Button();
            this.gbxProductos = new System.Windows.Forms.GroupBox();
            this.lblPesoTitulo = new System.Windows.Forms.Label();
            this.lblPeso = new System.Windows.Forms.Label();
            this.lblPromo = new System.Windows.Forms.Label();
            this.lblDescProd = new System.Windows.Forms.Label();
            this.lblPrecioProd = new System.Windows.Forms.Label();
            this.pbImagen = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripLabel();
            this.lblSitio = new System.Windows.Forms.ToolStripLabel();
            this.lblCaja = new System.Windows.Forms.ToolStripLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gvPendientes = new System.Windows.Forms.DataGridView();
            this.Folio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Ticket = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cliente = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Productos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbxProductosRecargas = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRecarga = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBorrarTxtCliente = new System.Windows.Forms.Button();
            this.btnYaExisteCliente = new System.Windows.Forms.Button();
            this.txtNombreCliente = new System.Windows.Forms.TextBox();
            this.btnNoExisteCliente = new System.Windows.Forms.Button();
            this.CBCliente = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnAbrirCajon = new System.Windows.Forms.Button();
            this.btnFacturar = new System.Windows.Forms.Button();
            this.btnEnvioVentas = new System.Windows.Forms.Button();
            this.btnClientes = new System.Windows.Forms.Button();
            this.btnCreditos = new System.Windows.Forms.Button();
            this.btnConsultarPrecio = new System.Windows.Forms.Button();
            this.btnDevoluciones = new System.Windows.Forms.Button();
            this.btnCortes = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.gbxVenta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVenta)).BeginInit();
            this.panel1.SuspendLayout();
            this.gbxProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbImagen)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvPendientes)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // gbxVenta
            // 
            this.gbxVenta.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbxVenta.BackColor = System.Drawing.SystemColors.ControlLight;
            this.gbxVenta.Controls.Add(this.btnReimprimir);
            this.gbxVenta.Controls.Add(this.btnTerminar);
            this.gbxVenta.Controls.Add(this.lblEspere);
            this.gbxVenta.Controls.Add(this.lblIEPS);
            this.gbxVenta.Controls.Add(this.lblIVA);
            this.gbxVenta.Controls.Add(this.lblIEPSTit);
            this.gbxVenta.Controls.Add(this.lblIVATit);
            this.gbxVenta.Controls.Add(this.btnBuscarCodigo);
            this.gbxVenta.Controls.Add(this.btnCancelar);
            this.gbxVenta.Controls.Add(this.pictureBox1);
            this.gbxVenta.Controls.Add(this.lblFolio);
            this.gbxVenta.Controls.Add(this.lblTotal);
            this.gbxVenta.Controls.Add(this.dgvVenta);
            this.gbxVenta.Controls.Add(this.txtScan);
            this.gbxVenta.Controls.Add(this.label1);
            this.gbxVenta.Controls.Add(this.panel1);
            this.gbxVenta.Font = new System.Drawing.Font("Century Gothic", 13.8F);
            this.gbxVenta.Location = new System.Drawing.Point(12, 93);
            this.gbxVenta.Name = "gbxVenta";
            this.gbxVenta.Size = new System.Drawing.Size(948, 704);
            this.gbxVenta.TabIndex = 0;
            this.gbxVenta.TabStop = false;
            this.gbxVenta.Text = "Venta";
            // 
            // btnReimprimir
            // 
            this.btnReimprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReimprimir.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnReimprimir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReimprimir.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnReimprimir.ForeColor = System.Drawing.Color.White;
            this.btnReimprimir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReimprimir.Location = new System.Drawing.Point(773, 625);
            this.btnReimprimir.Name = "btnReimprimir";
            this.btnReimprimir.Size = new System.Drawing.Size(168, 35);
            this.btnReimprimir.TabIndex = 4;
            this.btnReimprimir.TabStop = false;
            this.btnReimprimir.Text = "Reimprimir (F7)";
            this.btnReimprimir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReimprimir.UseVisualStyleBackColor = false;
            this.btnReimprimir.Click += new System.EventHandler(this.btnReimprimir_Click);
            // 
            // btnTerminar
            // 
            this.btnTerminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTerminar.BackColor = System.Drawing.Color.Green;
            this.btnTerminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTerminar.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnTerminar.ForeColor = System.Drawing.Color.White;
            this.btnTerminar.Image = global::PVLaJoya.Properties.Resources.iconBill24x24;
            this.btnTerminar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTerminar.Location = new System.Drawing.Point(517, 627);
            this.btnTerminar.Name = "btnTerminar";
            this.btnTerminar.Size = new System.Drawing.Size(250, 71);
            this.btnTerminar.TabIndex = 3;
            this.btnTerminar.TabStop = false;
            this.btnTerminar.Text = "Pagar (F8)";
            this.btnTerminar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTerminar.UseVisualStyleBackColor = false;
            this.btnTerminar.Click += new System.EventHandler(this.btnTerminar_Click);
            this.btnTerminar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnTerminar_KeyDown);
            // 
            // lblEspere
            // 
            this.lblEspere.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEspere.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.lblEspere.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEspere.ForeColor = System.Drawing.Color.White;
            this.lblEspere.Location = new System.Drawing.Point(251, 337);
            this.lblEspere.Name = "lblEspere";
            this.lblEspere.Size = new System.Drawing.Size(516, 114);
            this.lblEspere.TabIndex = 0;
            this.lblEspere.Text = "Espere un momento...";
            this.lblEspere.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEspere.Visible = false;
            // 
            // lblIEPS
            // 
            this.lblIEPS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIEPS.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblIEPS.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblIEPS.ForeColor = System.Drawing.Color.Black;
            this.lblIEPS.Location = new System.Drawing.Point(69, 659);
            this.lblIEPS.Name = "lblIEPS";
            this.lblIEPS.Size = new System.Drawing.Size(98, 30);
            this.lblIEPS.TabIndex = 0;
            this.lblIEPS.Text = "0";
            // 
            // lblIVA
            // 
            this.lblIVA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIVA.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblIVA.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold);
            this.lblIVA.ForeColor = System.Drawing.Color.Black;
            this.lblIVA.Location = new System.Drawing.Point(69, 628);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(98, 30);
            this.lblIVA.TabIndex = 0;
            this.lblIVA.Text = "0";
            // 
            // lblIEPSTit
            // 
            this.lblIEPSTit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIEPSTit.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblIEPSTit.ForeColor = System.Drawing.Color.Black;
            this.lblIEPSTit.Location = new System.Drawing.Point(9, 659);
            this.lblIEPSTit.Name = "lblIEPSTit";
            this.lblIEPSTit.Size = new System.Drawing.Size(57, 30);
            this.lblIEPSTit.TabIndex = 0;
            this.lblIEPSTit.Text = "IEPS:";
            // 
            // lblIVATit
            // 
            this.lblIVATit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIVATit.BackColor = System.Drawing.SystemColors.ControlLight;
            this.lblIVATit.ForeColor = System.Drawing.Color.Black;
            this.lblIVATit.Location = new System.Drawing.Point(9, 628);
            this.lblIVATit.Name = "lblIVATit";
            this.lblIVATit.Size = new System.Drawing.Size(57, 30);
            this.lblIVATit.TabIndex = 0;
            this.lblIVATit.Text = "IVA:";
            // 
            // btnBuscarCodigo
            // 
            this.btnBuscarCodigo.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnBuscarCodigo.Font = new System.Drawing.Font("Century Gothic", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.btnBuscarCodigo.ForeColor = System.Drawing.Color.White;
            this.btnBuscarCodigo.Image = global::PVLaJoya.Properties.Resources.search;
            this.btnBuscarCodigo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscarCodigo.Location = new System.Drawing.Point(676, 109);
            this.btnBuscarCodigo.Name = "btnBuscarCodigo";
            this.btnBuscarCodigo.Size = new System.Drawing.Size(259, 53);
            this.btnBuscarCodigo.TabIndex = 0;
            this.btnBuscarCodigo.TabStop = false;
            this.btnBuscarCodigo.Text = "Buscar Producto  (F1)";
            this.btnBuscarCodigo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBuscarCodigo.UseVisualStyleBackColor = false;
            this.btnBuscarCodigo.Click += new System.EventHandler(this.btnBuscarCodigo_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Location = new System.Drawing.Point(773, 664);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(168, 35);
            this.btnCancelar.TabIndex = 0;
            this.btnCancelar.Text = "Cancelar (Del)";
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.btnCancelar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnCancelar_KeyDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pictureBox1.Image = global::PVLaJoya.Properties.Resources.iconBarCode2;
            this.pictureBox1.Location = new System.Drawing.Point(12, 104);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(89, 64);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // lblFolio
            // 
            this.lblFolio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFolio.AutoEllipsis = true;
            this.lblFolio.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.lblFolio.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.lblFolio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.lblFolio.Location = new System.Drawing.Point(12, 34);
            this.lblFolio.Name = "lblFolio";
            this.lblFolio.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblFolio.Size = new System.Drawing.Size(637, 63);
            this.lblFolio.TabIndex = 0;
            this.lblFolio.Text = "Folio";
            this.lblFolio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.lblTotal.Font = new System.Drawing.Font("Century Gothic", 23F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.lblTotal.ForeColor = System.Drawing.Color.White;
            this.lblTotal.Location = new System.Drawing.Point(6, 625);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(505, 70);
            this.lblTotal.TabIndex = 0;
            this.lblTotal.Text = "$-";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgvVenta
            // 
            this.dgvVenta.AllowUserToAddRows = false;
            this.dgvVenta.AllowUserToResizeRows = false;
            dataGridViewCellStyle89.Font = new System.Drawing.Font("Century Gothic", 13.8F);
            this.dgvVenta.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle89;
            this.dgvVenta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVenta.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVenta.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.dgvVenta.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvVenta.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVenta.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIdProducto,
            this.colProducto,
            this.colResta,
            this.colCantidad,
            this.colSuma,
            this.colPrecioFinal,
            this.colPrecio,
            this.colTotal,
            this.IVA,
            this.IEPS,
            this.EsCaja,
            this.Uom,
            this.IdPresentacionVenta,
            this.IdMarca,
            this.IdLinea,
            this.FechaAgregado,
            this.Sku,
            this.MontoComision,
            this.IdOrden});
            this.dgvVenta.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(49)))), ((int)(((byte)(82)))));
            this.dgvVenta.Location = new System.Drawing.Point(6, 174);
            this.dgvVenta.Name = "dgvVenta";
            this.dgvVenta.RowHeadersVisible = false;
            this.dgvVenta.RowHeadersWidth = 51;
            this.dgvVenta.RowTemplate.Height = 24;
            this.dgvVenta.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvVenta.Size = new System.Drawing.Size(936, 448);
            this.dgvVenta.TabIndex = 2;
            this.dgvVenta.TabStop = false;
            this.dgvVenta.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVenta_CellClick);
            this.dgvVenta.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVenta_CellDoubleClick);
            this.dgvVenta.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVenta_CellEndEdit);
            this.dgvVenta.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVenta_CellLeave);
            this.dgvVenta.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvVenta_CellValueChanged);
            this.dgvVenta.Enter += new System.EventHandler(this.dgvVenta_Enter);
            this.dgvVenta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvVenta_KeyDown);
            this.dgvVenta.Leave += new System.EventHandler(this.dgvVenta_Leave);
            // 
            // colIdProducto
            // 
            this.colIdProducto.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colIdProducto.Frozen = true;
            this.colIdProducto.HeaderText = "IdProducto";
            this.colIdProducto.MaxInputLength = 32770;
            this.colIdProducto.MinimumWidth = 6;
            this.colIdProducto.Name = "colIdProducto";
            this.colIdProducto.ReadOnly = true;
            this.colIdProducto.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colIdProducto.Visible = false;
            this.colIdProducto.Width = 6;
            // 
            // colProducto
            // 
            this.colProducto.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colProducto.FillWeight = 1F;
            this.colProducto.Frozen = true;
            this.colProducto.HeaderText = "Producto";
            this.colProducto.MinimumWidth = 340;
            this.colProducto.Name = "colProducto";
            this.colProducto.ReadOnly = true;
            this.colProducto.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colProducto.Width = 400;
            // 
            // colResta
            // 
            this.colResta.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle90.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle90.BackColor = System.Drawing.Color.Yellow;
            dataGridViewCellStyle90.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle90.SelectionBackColor = System.Drawing.Color.Yellow;
            dataGridViewCellStyle90.SelectionForeColor = System.Drawing.Color.Black;
            this.colResta.DefaultCellStyle = dataGridViewCellStyle90;
            this.colResta.FillWeight = 19.25134F;
            this.colResta.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colResta.Frozen = true;
            this.colResta.HeaderText = "-";
            this.colResta.MinimumWidth = 6;
            this.colResta.Name = "colResta";
            this.colResta.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colResta.Text = "-";
            this.colResta.Width = 30;
            // 
            // colCantidad
            // 
            this.colCantidad.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle91.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colCantidad.DefaultCellStyle = dataGridViewCellStyle91;
            this.colCantidad.FillWeight = 19.25134F;
            this.colCantidad.Frozen = true;
            this.colCantidad.HeaderText = "#";
            this.colCantidad.MinimumWidth = 6;
            this.colCantidad.Name = "colCantidad";
            this.colCantidad.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCantidad.Width = 150;
            // 
            // colSuma
            // 
            this.colSuma.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle92.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle92.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(105)))), ((int)(((byte)(61)))));
            dataGridViewCellStyle92.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle92.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(61)))), ((int)(((byte)(105)))), ((int)(((byte)(61)))));
            dataGridViewCellStyle92.SelectionForeColor = System.Drawing.Color.White;
            this.colSuma.DefaultCellStyle = dataGridViewCellStyle92;
            this.colSuma.FillWeight = 19.25134F;
            this.colSuma.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colSuma.Frozen = true;
            this.colSuma.HeaderText = "+";
            this.colSuma.MinimumWidth = 6;
            this.colSuma.Name = "colSuma";
            this.colSuma.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSuma.Text = "+";
            this.colSuma.Width = 30;
            // 
            // colPrecioFinal
            // 
            this.colPrecioFinal.HeaderText = "Precio";
            this.colPrecioFinal.MinimumWidth = 6;
            this.colPrecioFinal.Name = "colPrecioFinal";
            this.colPrecioFinal.ReadOnly = true;
            this.colPrecioFinal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colPrecio
            // 
            this.colPrecio.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle93.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colPrecio.DefaultCellStyle = dataGridViewCellStyle93;
            this.colPrecio.FillWeight = 19.25134F;
            this.colPrecio.HeaderText = "PrecioInicial";
            this.colPrecio.MinimumWidth = 6;
            this.colPrecio.Name = "colPrecio";
            this.colPrecio.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colPrecio.Visible = false;
            this.colPrecio.Width = 95;
            // 
            // colTotal
            // 
            this.colTotal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle94.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle = dataGridViewCellStyle94;
            this.colTotal.FillWeight = 19.25134F;
            this.colTotal.HeaderText = "Total";
            this.colTotal.MinimumWidth = 6;
            this.colTotal.Name = "colTotal";
            this.colTotal.ReadOnly = true;
            this.colTotal.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colTotal.Width = 105;
            // 
            // IVA
            // 
            this.IVA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.IVA.HeaderText = "IVA";
            this.IVA.MinimumWidth = 6;
            this.IVA.Name = "IVA";
            this.IVA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IVA.Visible = false;
            this.IVA.Width = 134;
            // 
            // IEPS
            // 
            this.IEPS.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.IEPS.HeaderText = "IEPS";
            this.IEPS.MinimumWidth = 6;
            this.IEPS.Name = "IEPS";
            this.IEPS.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IEPS.Visible = false;
            this.IEPS.Width = 134;
            // 
            // EsCaja
            // 
            this.EsCaja.HeaderText = "EsCaja";
            this.EsCaja.MinimumWidth = 6;
            this.EsCaja.Name = "EsCaja";
            this.EsCaja.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.EsCaja.Visible = false;
            // 
            // Uom
            // 
            this.Uom.HeaderText = "Uom";
            this.Uom.MinimumWidth = 6;
            this.Uom.Name = "Uom";
            this.Uom.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Uom.Visible = false;
            // 
            // IdPresentacionVenta
            // 
            this.IdPresentacionVenta.HeaderText = "IdPresentacionVenta";
            this.IdPresentacionVenta.MinimumWidth = 6;
            this.IdPresentacionVenta.Name = "IdPresentacionVenta";
            this.IdPresentacionVenta.ReadOnly = true;
            this.IdPresentacionVenta.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IdPresentacionVenta.Visible = false;
            // 
            // IdMarca
            // 
            this.IdMarca.HeaderText = "IdMarca";
            this.IdMarca.MinimumWidth = 6;
            this.IdMarca.Name = "IdMarca";
            this.IdMarca.ReadOnly = true;
            this.IdMarca.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IdMarca.Visible = false;
            // 
            // IdLinea
            // 
            this.IdLinea.HeaderText = "IdLinea";
            this.IdLinea.MinimumWidth = 6;
            this.IdLinea.Name = "IdLinea";
            this.IdLinea.ReadOnly = true;
            this.IdLinea.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.IdLinea.Visible = false;
            // 
            // FechaAgregado
            // 
            this.FechaAgregado.HeaderText = "FechaAgregado";
            this.FechaAgregado.MinimumWidth = 6;
            this.FechaAgregado.Name = "FechaAgregado";
            this.FechaAgregado.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.FechaAgregado.Visible = false;
            // 
            // Sku
            // 
            this.Sku.HeaderText = "Sku";
            this.Sku.MinimumWidth = 6;
            this.Sku.Name = "Sku";
            this.Sku.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Sku.Visible = false;
            // 
            // MontoComision
            // 
            this.MontoComision.HeaderText = "MontoComision";
            this.MontoComision.MinimumWidth = 6;
            this.MontoComision.Name = "MontoComision";
            this.MontoComision.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MontoComision.Visible = false;
            // 
            // IdOrden
            // 
            dataGridViewCellStyle95.ForeColor = System.Drawing.Color.Black;
            this.IdOrden.DefaultCellStyle = dataGridViewCellStyle95;
            this.IdOrden.HeaderText = "IdOrden";
            this.IdOrden.MinimumWidth = 6;
            this.IdOrden.Name = "IdOrden";
            this.IdOrden.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.IdOrden.Visible = false;
            // 
            // txtScan
            // 
            this.txtScan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtScan.BackColor = System.Drawing.Color.White;
            this.txtScan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScan.Font = new System.Drawing.Font("Segoe UI", 13.8F);
            this.txtScan.ForeColor = System.Drawing.Color.Black;
            this.txtScan.Location = new System.Drawing.Point(102, 118);
            this.txtScan.Name = "txtScan";
            this.txtScan.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtScan.Size = new System.Drawing.Size(568, 32);
            this.txtScan.TabIndex = 1;
            this.txtScan.TabStop = false;
            this.txtScan.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtScan_KeyDown);
            this.txtScan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.txtScan.Leave += new System.EventHandler(this.txtScan_Leave);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 16.2F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(928, 64);
            this.label1.TabIndex = 0;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.panel1.Controls.Add(this.btnEspera);
            this.panel1.Location = new System.Drawing.Point(600, 34);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(341, 63);
            this.panel1.TabIndex = 0;
            // 
            // btnEspera
            // 
            this.btnEspera.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEspera.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnEspera.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEspera.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnEspera.ForeColor = System.Drawing.Color.White;
            this.btnEspera.Image = global::PVLaJoya.Properties.Resources.iconClock24x24;
            this.btnEspera.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEspera.Location = new System.Drawing.Point(58, 11);
            this.btnEspera.Name = "btnEspera";
            this.btnEspera.Size = new System.Drawing.Size(277, 41);
            this.btnEspera.TabIndex = 0;
            this.btnEspera.TabStop = false;
            this.btnEspera.Text = "Pasar Venta a Espera (F3)";
            this.btnEspera.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEspera.UseVisualStyleBackColor = false;
            this.btnEspera.Click += new System.EventHandler(this.btnEspera_Click);
            // 
            // gbxProductos
            // 
            this.gbxProductos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProductos.BackColor = System.Drawing.Color.White;
            this.gbxProductos.Controls.Add(this.lblPesoTitulo);
            this.gbxProductos.Controls.Add(this.lblPeso);
            this.gbxProductos.Controls.Add(this.lblPromo);
            this.gbxProductos.Controls.Add(this.lblDescProd);
            this.gbxProductos.Controls.Add(this.lblPrecioProd);
            this.gbxProductos.Controls.Add(this.pbImagen);
            this.gbxProductos.Font = new System.Drawing.Font("Century Gothic", 18F);
            this.gbxProductos.Location = new System.Drawing.Point(967, 369);
            this.gbxProductos.Name = "gbxProductos";
            this.gbxProductos.Size = new System.Drawing.Size(545, 496);
            this.gbxProductos.TabIndex = 9;
            this.gbxProductos.TabStop = false;
            this.gbxProductos.Text = "Producto";
            // 
            // lblPesoTitulo
            // 
            this.lblPesoTitulo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPesoTitulo.BackColor = System.Drawing.Color.Transparent;
            this.lblPesoTitulo.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold);
            this.lblPesoTitulo.ForeColor = System.Drawing.Color.Black;
            this.lblPesoTitulo.Location = new System.Drawing.Point(283, 401);
            this.lblPesoTitulo.Name = "lblPesoTitulo";
            this.lblPesoTitulo.Size = new System.Drawing.Size(102, 41);
            this.lblPesoTitulo.TabIndex = 1;
            this.lblPesoTitulo.Text = "Peso:";
            this.lblPesoTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPeso
            // 
            this.lblPeso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPeso.BackColor = System.Drawing.Color.Transparent;
            this.lblPeso.Font = new System.Drawing.Font("Century Gothic", 20F, System.Drawing.FontStyle.Bold);
            this.lblPeso.ForeColor = System.Drawing.Color.Black;
            this.lblPeso.Location = new System.Drawing.Point(401, 402);
            this.lblPeso.Name = "lblPeso";
            this.lblPeso.Size = new System.Drawing.Size(129, 38);
            this.lblPeso.TabIndex = 17;
            this.lblPeso.Text = "--------";
            this.lblPeso.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPromo
            // 
            this.lblPromo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPromo.BackColor = System.Drawing.Color.Transparent;
            this.lblPromo.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPromo.ForeColor = System.Drawing.Color.Black;
            this.lblPromo.Location = new System.Drawing.Point(-1, 453);
            this.lblPromo.Name = "lblPromo";
            this.lblPromo.Size = new System.Drawing.Size(524, 43);
            this.lblPromo.TabIndex = 16;
            this.lblPromo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDescProd
            // 
            this.lblDescProd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDescProd.BackColor = System.Drawing.Color.Transparent;
            this.lblDescProd.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold);
            this.lblDescProd.ForeColor = System.Drawing.Color.Black;
            this.lblDescProd.Location = new System.Drawing.Point(0, 25);
            this.lblDescProd.Name = "lblDescProd";
            this.lblDescProd.Size = new System.Drawing.Size(524, 57);
            this.lblDescProd.TabIndex = 3;
            this.lblDescProd.Text = "Descripción producto";
            this.lblDescProd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrecioProd
            // 
            this.lblPrecioProd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrecioProd.BackColor = System.Drawing.Color.Transparent;
            this.lblPrecioProd.Font = new System.Drawing.Font("Century Gothic", 36F, System.Drawing.FontStyle.Bold);
            this.lblPrecioProd.ForeColor = System.Drawing.Color.Black;
            this.lblPrecioProd.Location = new System.Drawing.Point(6, 328);
            this.lblPrecioProd.Name = "lblPrecioProd";
            this.lblPrecioProd.Size = new System.Drawing.Size(524, 65);
            this.lblPrecioProd.TabIndex = 2;
            this.lblPrecioProd.Text = "$ 0.00";
            this.lblPrecioProd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPrecioProd.Click += new System.EventHandler(this.lblPrecioProd_Click);
            // 
            // pbImagen
            // 
            this.pbImagen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbImagen.BackColor = System.Drawing.Color.White;
            this.pbImagen.Location = new System.Drawing.Point(151, 85);
            this.pbImagen.Name = "pbImagen";
            this.pbImagen.Size = new System.Drawing.Size(220, 240);
            this.pbImagen.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbImagen.TabIndex = 13;
            this.pbImagen.TabStop = false;
            this.pbImagen.Click += new System.EventHandler(this.pbImagen_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuario,
            this.lblSitio,
            this.lblCaja});
            this.toolStrip1.Location = new System.Drawing.Point(0, 874);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1524, 25);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblUsuario
            // 
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(22, 22);
            this.lblUsuario.Text = "--";
            // 
            // lblSitio
            // 
            this.lblSitio.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSitio.Name = "lblSitio";
            this.lblSitio.Size = new System.Drawing.Size(22, 22);
            this.lblSitio.Text = "--";
            // 
            // lblCaja
            // 
            this.lblCaja.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblCaja.Name = "lblCaja";
            this.lblCaja.Size = new System.Drawing.Size(22, 22);
            this.lblCaja.Text = "--";
            this.lblCaja.Visible = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label4.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1524, 87);
            this.label4.TabIndex = 2;
            this.label4.Text = "Punto de Venta";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.gvPendientes);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(967, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(548, 132);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ventas Pendientes (F12)";
            // 
            // gvPendientes
            // 
            this.gvPendientes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gvPendientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvPendientes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Folio,
            this.Ticket,
            this.Cliente,
            this.Productos,
            this.Fecha});
            this.gvPendientes.GridColor = System.Drawing.SystemColors.ControlText;
            this.gvPendientes.Location = new System.Drawing.Point(3, 23);
            this.gvPendientes.Name = "gvPendientes";
            this.gvPendientes.RowHeadersVisible = false;
            this.gvPendientes.RowHeadersWidth = 51;
            this.gvPendientes.RowTemplate.Height = 24;
            this.gvPendientes.Size = new System.Drawing.Size(542, 106);
            this.gvPendientes.TabIndex = 0;
            this.gvPendientes.TabStop = false;
            this.gvPendientes.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvPendientes_CellContenDoubletClick);
            this.gvPendientes.Enter += new System.EventHandler(this.gvPendientes_Enter);
            this.gvPendientes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvPendientes_KeyDown);
            this.gvPendientes.Leave += new System.EventHandler(this.gvPendientes_Leave);
            // 
            // Folio
            // 
            dataGridViewCellStyle96.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Folio.DefaultCellStyle = dataGridViewCellStyle96;
            this.Folio.HeaderText = "Folio";
            this.Folio.MinimumWidth = 6;
            this.Folio.Name = "Folio";
            this.Folio.ReadOnly = true;
            this.Folio.Visible = false;
            this.Folio.Width = 178;
            // 
            // Ticket
            // 
            this.Ticket.HeaderText = "Folio";
            this.Ticket.MinimumWidth = 6;
            this.Ticket.Name = "Ticket";
            this.Ticket.ReadOnly = true;
            this.Ticket.Width = 125;
            // 
            // Cliente
            // 
            this.Cliente.HeaderText = "Cliente";
            this.Cliente.MinimumWidth = 6;
            this.Cliente.Name = "Cliente";
            this.Cliente.ReadOnly = true;
            this.Cliente.Width = 350;
            // 
            // Productos
            // 
            this.Productos.HeaderText = "Productos";
            this.Productos.MinimumWidth = 6;
            this.Productos.Name = "Productos";
            this.Productos.ReadOnly = true;
            this.Productos.Width = 170;
            // 
            // Fecha
            // 
            this.Fecha.HeaderText = "Fecha";
            this.Fecha.MinimumWidth = 6;
            this.Fecha.Name = "Fecha";
            this.Fecha.ReadOnly = true;
            this.Fecha.Width = 178;
            // 
            // cbxProductosRecargas
            // 
            this.cbxProductosRecargas.FormattingEnabled = true;
            this.cbxProductosRecargas.Location = new System.Drawing.Point(125, 36);
            this.cbxProductosRecargas.Name = "cbxProductosRecargas";
            this.cbxProductosRecargas.Size = new System.Drawing.Size(392, 27);
            this.cbxProductosRecargas.TabIndex = 4;
            this.cbxProductosRecargas.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(124, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Productos:";
            this.label2.Visible = false;
            // 
            // txtTelefono
            // 
            this.txtTelefono.Location = new System.Drawing.Point(523, 38);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(211, 27);
            this.txtTelefono.TabIndex = 6;
            this.txtTelefono.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(518, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Telefono";
            this.label3.Visible = false;
            // 
            // btnRecarga
            // 
            this.btnRecarga.Location = new System.Drawing.Point(740, 38);
            this.btnRecarga.Name = "btnRecarga";
            this.btnRecarga.Size = new System.Drawing.Size(112, 31);
            this.btnRecarga.TabIndex = 7;
            this.btnRecarga.Text = "Recarga";
            this.btnRecarga.UseVisualStyleBackColor = true;
            this.btnRecarga.Visible = false;
            this.btnRecarga.Click += new System.EventHandler(this.btnRecarga_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.btnBorrarTxtCliente);
            this.groupBox2.Controls.Add(this.btnYaExisteCliente);
            this.groupBox2.Controls.Add(this.txtNombreCliente);
            this.groupBox2.Controls.Add(this.btnNoExisteCliente);
            this.groupBox2.Controls.Add(this.CBCliente);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(967, 231);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(548, 120);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cliente (F6)";
            // 
            // btnBorrarTxtCliente
            // 
            this.btnBorrarTxtCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBorrarTxtCliente.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnBorrarTxtCliente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBorrarTxtCliente.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnBorrarTxtCliente.ForeColor = System.Drawing.Color.White;
            this.btnBorrarTxtCliente.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBorrarTxtCliente.Location = new System.Drawing.Point(303, 75);
            this.btnBorrarTxtCliente.Name = "btnBorrarTxtCliente";
            this.btnBorrarTxtCliente.Size = new System.Drawing.Size(86, 30);
            this.btnBorrarTxtCliente.TabIndex = 24;
            this.btnBorrarTxtCliente.TabStop = false;
            this.btnBorrarTxtCliente.Text = "Editar";
            this.btnBorrarTxtCliente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBorrarTxtCliente.UseVisualStyleBackColor = false;
            this.btnBorrarTxtCliente.Visible = false;
            this.btnBorrarTxtCliente.Click += new System.EventHandler(this.btnBorrarTxtCliente_Click);
            // 
            // btnYaExisteCliente
            // 
            this.btnYaExisteCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnYaExisteCliente.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnYaExisteCliente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnYaExisteCliente.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnYaExisteCliente.ForeColor = System.Drawing.Color.White;
            this.btnYaExisteCliente.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnYaExisteCliente.Location = new System.Drawing.Point(395, 76);
            this.btnYaExisteCliente.Name = "btnYaExisteCliente";
            this.btnYaExisteCliente.Size = new System.Drawing.Size(141, 30);
            this.btnYaExisteCliente.TabIndex = 23;
            this.btnYaExisteCliente.TabStop = false;
            this.btnYaExisteCliente.Text = "Ya Existe Cliente";
            this.btnYaExisteCliente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnYaExisteCliente.UseVisualStyleBackColor = false;
            this.btnYaExisteCliente.Visible = false;
            this.btnYaExisteCliente.Click += new System.EventHandler(this.btnYaExisteCliente_Click);
            // 
            // txtNombreCliente
            // 
            this.txtNombreCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNombreCliente.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombreCliente.Location = new System.Drawing.Point(11, 76);
            this.txtNombreCliente.Name = "txtNombreCliente";
            this.txtNombreCliente.Size = new System.Drawing.Size(286, 27);
            this.txtNombreCliente.TabIndex = 0;
            this.txtNombreCliente.TabStop = false;
            this.txtNombreCliente.Visible = false;
            this.txtNombreCliente.TextChanged += new System.EventHandler(this.txtNombreCliente_TextChanged);
            this.txtNombreCliente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNombreCliente_KeyDown);
            // 
            // btnNoExisteCliente
            // 
            this.btnNoExisteCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNoExisteCliente.AutoSize = true;
            this.btnNoExisteCliente.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnNoExisteCliente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNoExisteCliente.Font = new System.Drawing.Font("Century Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.btnNoExisteCliente.ForeColor = System.Drawing.Color.White;
            this.btnNoExisteCliente.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNoExisteCliente.Location = new System.Drawing.Point(303, 36);
            this.btnNoExisteCliente.Name = "btnNoExisteCliente";
            this.btnNoExisteCliente.Size = new System.Drawing.Size(233, 33);
            this.btnNoExisteCliente.TabIndex = 21;
            this.btnNoExisteCliente.TabStop = false;
            this.btnNoExisteCliente.Text = "No Existe Cliente";
            this.btnNoExisteCliente.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnNoExisteCliente.UseVisualStyleBackColor = false;
            this.btnNoExisteCliente.Click += new System.EventHandler(this.btnNoExisteCliente_Click);
            // 
            // CBCliente
            // 
            this.CBCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CBCliente.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CBCliente.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CBCliente.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CBCliente.FormattingEnabled = true;
            this.CBCliente.Location = new System.Drawing.Point(11, 39);
            this.CBCliente.Name = "CBCliente";
            this.CBCliente.Size = new System.Drawing.Size(286, 28);
            this.CBCliente.TabIndex = 1;
            this.CBCliente.TabStop = false;
            this.CBCliente.Text = "Seleccione un cliente";
            this.CBCliente.SelectedIndexChanged += new System.EventHandler(this.CBCliente_SelectedIndexChanged);
            this.CBCliente.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CBCliente_KeyDown);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoEllipsis = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(5, 23);
            this.label5.Name = "label5";
            this.label5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label5.Size = new System.Drawing.Size(537, 94);
            this.label5.TabIndex = 2;
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.BackColor = System.Drawing.Color.White;
            this.groupBox3.Controls.Add(this.btnAbrirCajon);
            this.groupBox3.Controls.Add(this.btnFacturar);
            this.groupBox3.Controls.Add(this.btnEnvioVentas);
            this.groupBox3.Controls.Add(this.btnClientes);
            this.groupBox3.Controls.Add(this.btnCreditos);
            this.groupBox3.Controls.Add(this.btnConsultarPrecio);
            this.groupBox3.Controls.Add(this.btnDevoluciones);
            this.groupBox3.Controls.Add(this.btnCortes);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Font = new System.Drawing.Font("Segoe UI", 10.8F);
            this.groupBox3.Location = new System.Drawing.Point(12, 803);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(949, 62);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Accesos";
            // 
            // btnAbrirCajon
            // 
            this.btnAbrirCajon.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAbrirCajon.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbrirCajon.Location = new System.Drawing.Point(806, 27);
            this.btnAbrirCajon.Name = "btnAbrirCajon";
            this.btnAbrirCajon.Size = new System.Drawing.Size(121, 23);
            this.btnAbrirCajon.TabIndex = 23;
            this.btnAbrirCajon.TabStop = false;
            this.btnAbrirCajon.Text = "Abrir Cajón (Insert)";
            this.btnAbrirCajon.UseVisualStyleBackColor = true;
            this.btnAbrirCajon.Click += new System.EventHandler(this.btnAbrirCajon_Click);
            // 
            // btnFacturar
            // 
            this.btnFacturar.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnFacturar.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFacturar.Location = new System.Drawing.Point(706, 27);
            this.btnFacturar.Name = "btnFacturar";
            this.btnFacturar.Size = new System.Drawing.Size(94, 23);
            this.btnFacturar.TabIndex = 22;
            this.btnFacturar.TabStop = false;
            this.btnFacturar.Text = "Facturar (F11)";
            this.btnFacturar.UseVisualStyleBackColor = true;
            this.btnFacturar.Click += new System.EventHandler(this.btnFacturar_Click);
            // 
            // btnEnvioVentas
            // 
            this.btnEnvioVentas.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnEnvioVentas.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnvioVentas.Location = new System.Drawing.Point(581, 27);
            this.btnEnvioVentas.Name = "btnEnvioVentas";
            this.btnEnvioVentas.Size = new System.Drawing.Size(119, 23);
            this.btnEnvioVentas.TabIndex = 5;
            this.btnEnvioVentas.TabStop = false;
            this.btnEnvioVentas.Text = "Envio Ventas (F10)";
            this.btnEnvioVentas.UseVisualStyleBackColor = true;
            this.btnEnvioVentas.Click += new System.EventHandler(this.btnEnvioVentas_Click);
            // 
            // btnClientes
            // 
            this.btnClientes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClientes.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClientes.Location = new System.Drawing.Point(376, 27);
            this.btnClientes.Name = "btnClientes";
            this.btnClientes.Size = new System.Drawing.Size(89, 23);
            this.btnClientes.TabIndex = 3;
            this.btnClientes.TabStop = false;
            this.btnClientes.Text = "Clientes (F6)";
            this.btnClientes.UseVisualStyleBackColor = true;
            this.btnClientes.Click += new System.EventHandler(this.btnClientes_Click);
            // 
            // btnCreditos
            // 
            this.btnCreditos.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCreditos.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreditos.Location = new System.Drawing.Point(280, 27);
            this.btnCreditos.Name = "btnCreditos";
            this.btnCreditos.Size = new System.Drawing.Size(91, 23);
            this.btnCreditos.TabIndex = 2;
            this.btnCreditos.TabStop = false;
            this.btnCreditos.Text = "Créditos (F5)";
            this.btnCreditos.UseVisualStyleBackColor = true;
            this.btnCreditos.Click += new System.EventHandler(this.btnCreditos_Click);
            // 
            // btnConsultarPrecio
            // 
            this.btnConsultarPrecio.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnConsultarPrecio.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConsultarPrecio.Location = new System.Drawing.Point(144, 27);
            this.btnConsultarPrecio.Name = "btnConsultarPrecio";
            this.btnConsultarPrecio.Size = new System.Drawing.Size(130, 23);
            this.btnConsultarPrecio.TabIndex = 1;
            this.btnConsultarPrecio.TabStop = false;
            this.btnConsultarPrecio.Text = "Consultar Precio (F4)";
            this.btnConsultarPrecio.UseVisualStyleBackColor = true;
            this.btnConsultarPrecio.Click += new System.EventHandler(this.btnConsultarPrecio_Click);
            // 
            // btnDevoluciones
            // 
            this.btnDevoluciones.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnDevoluciones.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevoluciones.Location = new System.Drawing.Point(8, 27);
            this.btnDevoluciones.Name = "btnDevoluciones";
            this.btnDevoluciones.Size = new System.Drawing.Size(130, 23);
            this.btnDevoluciones.TabIndex = 0;
            this.btnDevoluciones.TabStop = false;
            this.btnDevoluciones.Text = "Devoluciones (F2)";
            this.btnDevoluciones.UseVisualStyleBackColor = true;
            this.btnDevoluciones.Click += new System.EventHandler(this.btnDevoluciones_Click);
            // 
            // btnCortes
            // 
            this.btnCortes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCortes.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCortes.Location = new System.Drawing.Point(470, 27);
            this.btnCortes.Name = "btnCortes";
            this.btnCortes.Size = new System.Drawing.Size(106, 23);
            this.btnCortes.TabIndex = 4;
            this.btnCortes.TabStop = false;
            this.btnCortes.Text = "Hacer Corte (F9)";
            this.btnCortes.UseVisualStyleBackColor = true;
            this.btnCortes.Click += new System.EventHandler(this.btnCortes_Click);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoEllipsis = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(5, 23);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label6.Size = new System.Drawing.Size(938, 32);
            this.label6.TabIndex = 21;
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pbxLogo
            // 
            this.pbxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pbxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbxLogo.Image")));
            this.pbxLogo.Location = new System.Drawing.Point(0, 0);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(100, 87);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxLogo.TabIndex = 10;
            this.pbxLogo.TabStop = false;
            // 
            // FVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1524, 899);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnRecarga);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbxProductosRecargas);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.gbxProductos);
            this.Controls.Add(this.gbxVenta);
            this.Font = new System.Drawing.Font("Segoe UI", 10.8F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FVenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Venta";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fVenta_FormClosing);
            this.Load += new System.EventHandler(this.fVenta_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fVenta_KeyDown);
            this.gbxVenta.ResumeLayout(false);
            this.gbxVenta.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVenta)).EndInit();
            this.panel1.ResumeLayout(false);
            this.gbxProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbImagen)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvPendientes)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbxVenta;
        private System.Windows.Forms.GroupBox gbxProductos;
        private System.Windows.Forms.TextBox txtScan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvVenta;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblUsuario;
        private System.Windows.Forms.ToolStripLabel lblSitio;
        private System.Windows.Forms.ImageList imgLstCategorias;
        private System.Windows.Forms.ImageList imgLstProductos;
        private System.Windows.Forms.Label lblFolio;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnTerminar;
        private System.Windows.Forms.PictureBox pbxLogo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnBuscarCodigo;
        private System.Windows.Forms.ToolStripLabel lblCaja;
        private System.Windows.Forms.Label lblDescProd;
        private System.Windows.Forms.Label lblPrecioProd;
        private System.Windows.Forms.PictureBox pbImagen;
        private System.Windows.Forms.Label lblIEPSTit;
        private System.Windows.Forms.Label lblIVATit;
        private System.Windows.Forms.Label lblIEPS;
        private System.Windows.Forms.Label lblIVA;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnEspera;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView gvPendientes;
        private System.Windows.Forms.Label lblPromo;
        private System.Windows.Forms.ComboBox cbxProductosRecargas;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRecarga;
        private System.Windows.Forms.Label lblEspere;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox CBCliente;
        private Label label5;
        private Button btnNoExisteCliente;
        private Button btnYaExisteCliente;
        private TextBox txtNombreCliente;
        private Button btnBorrarTxtCliente;
        private DataGridViewTextBoxColumn Folio;
        private DataGridViewTextBoxColumn Ticket;
        private DataGridViewTextBoxColumn Cliente;
        private DataGridViewTextBoxColumn Productos;
        private DataGridViewTextBoxColumn Fecha;
        private Button btnReimprimir;
        private Label lblPeso;
        private Label lblPesoTitulo;
        private DataGridViewTextBoxColumn colIdProducto;
        private DataGridViewTextBoxColumn colProducto;
        private DataGridViewButtonColumn colResta;
        private DataGridViewTextBoxColumn colCantidad;
        private DataGridViewButtonColumn colSuma;
        private DataGridViewTextBoxColumn colPrecioFinal;
        private DataGridViewTextBoxColumn colPrecio;
        private DataGridViewTextBoxColumn colTotal;
        private DataGridViewTextBoxColumn IVA;
        private DataGridViewTextBoxColumn IEPS;
        private DataGridViewTextBoxColumn EsCaja;
        private DataGridViewTextBoxColumn Uom;
        private DataGridViewTextBoxColumn IdPresentacionVenta;
        private DataGridViewTextBoxColumn IdMarca;
        private DataGridViewTextBoxColumn IdLinea;
        private DataGridViewTextBoxColumn FechaAgregado;
        private DataGridViewTextBoxColumn Sku;
        private DataGridViewTextBoxColumn MontoComision;
        private DataGridViewTextBoxColumn IdOrden;
        private GroupBox groupBox3;
        private Label label6;
        private Button btnCortes;
        private Button btnDevoluciones;
        private Button btnConsultarPrecio;
        private Button btnCreditos;
        private Button btnClientes;
        private Button btnEnvioVentas;
        private Button btnFacturar;
        private Button btnAbrirCajon;
    }
}