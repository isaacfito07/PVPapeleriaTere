
namespace PVLaJoya
{
    partial class FMenu
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMenu));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ayudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consultasBDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configuraciónPuertosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cajaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.estadoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripLabel();
            this.lblSitio = new System.Windows.Forms.ToolStripLabel();
            this.lblCaja = new System.Windows.Forms.ToolStripLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnNuevaVenta = new System.Windows.Forms.Button();
            this.btnHistorial = new System.Windows.Forms.Button();
            this.btnDevoluciones = new System.Windows.Forms.Button();
            this.btnDisparo = new System.Windows.Forms.Button();
            this.btnArqueo = new System.Windows.Forms.Button();
            this.btnCorteCaja = new System.Windows.Forms.Button();
            this.btnCreditos = new System.Windows.Forms.Button();
            this.imgLstProductos = new System.Windows.Forms.ImageList(this.components);
            this.imgLstCategorias = new System.Windows.Forms.ImageList(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ayudaToolStripMenuItem,
            this.cajaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(14, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(961, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ayudaToolStripMenuItem
            // 
            this.ayudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.acercaDeToolStripMenuItem,
            this.consultasBDToolStripMenuItem,
            this.configuraciónPuertosToolStripMenuItem});
            this.ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            this.ayudaToolStripMenuItem.Size = new System.Drawing.Size(53, 19);
            this.ayudaToolStripMenuItem.Text = "Ayuda";
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.acercaDeToolStripMenuItem.Text = "Acerca de...";
            this.acercaDeToolStripMenuItem.Click += new System.EventHandler(this.AcercaDeToolStripMenuItem_Click);
            // 
            // consultasBDToolStripMenuItem
            // 
            this.consultasBDToolStripMenuItem.Name = "consultasBDToolStripMenuItem";
            this.consultasBDToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.consultasBDToolStripMenuItem.Text = "Consultas BD";
            this.consultasBDToolStripMenuItem.Click += new System.EventHandler(this.ConsultasBDToolStripMenuItem_Click);
            // 
            // configuraciónPuertosToolStripMenuItem
            // 
            this.configuraciónPuertosToolStripMenuItem.Name = "configuraciónPuertosToolStripMenuItem";
            this.configuraciónPuertosToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.configuraciónPuertosToolStripMenuItem.Text = "Configuración Puertos";
            this.configuraciónPuertosToolStripMenuItem.Click += new System.EventHandler(this.configuraciónPuertosToolStripMenuItem_Click);
            // 
            // cajaToolStripMenuItem
            // 
            this.cajaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.estadoToolStripMenuItem});
            this.cajaToolStripMenuItem.Name = "cajaToolStripMenuItem";
            this.cajaToolStripMenuItem.Size = new System.Drawing.Size(42, 19);
            this.cajaToolStripMenuItem.Text = "Caja";
            this.cajaToolStripMenuItem.Visible = false;
            // 
            // estadoToolStripMenuItem
            // 
            this.estadoToolStripMenuItem.Name = "estadoToolStripMenuItem";
            this.estadoToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.estadoToolStripMenuItem.Text = "Estado...";
            this.estadoToolStripMenuItem.Click += new System.EventHandler(this.EstadoToolStripMenuItem_Click);
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 480);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(961, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblUsuario
            // 
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(17, 22);
            this.lblUsuario.Text = "--";
            // 
            // lblSitio
            // 
            this.lblSitio.Name = "lblSitio";
            this.lblSitio.Size = new System.Drawing.Size(17, 22);
            this.lblSitio.Text = "--";
            // 
            // lblCaja
            // 
            this.lblCaja.Name = "lblCaja";
            this.lblCaja.Size = new System.Drawing.Size(17, 22);
            this.lblCaja.Text = "--";
            this.lblCaja.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnNuevaVenta, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHistorial, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDevoluciones, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDisparo, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnArqueo, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCorteCaja, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnCreditos, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 116);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(961, 362);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btnNuevaVenta
            // 
            this.btnNuevaVenta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNuevaVenta.AutoSize = true;
            this.btnNuevaVenta.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnNuevaVenta.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevaVenta.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnNuevaVenta.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnNuevaVenta.Image = global::PVLaJoya.Properties.Resources.iconVenta;
            this.btnNuevaVenta.Location = new System.Drawing.Point(5, 5);
            this.btnNuevaVenta.Margin = new System.Windows.Forms.Padding(5);
            this.btnNuevaVenta.Name = "btnNuevaVenta";
            this.btnNuevaVenta.Size = new System.Drawing.Size(230, 170);
            this.btnNuevaVenta.TabIndex = 0;
            this.btnNuevaVenta.Text = "Nueva Venta";
            this.btnNuevaVenta.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNuevaVenta.UseVisualStyleBackColor = false;
            this.btnNuevaVenta.Click += new System.EventHandler(this.BtnNuevaVenta_Click);
            // 
            // btnHistorial
            // 
            this.btnHistorial.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHistorial.AutoSize = true;
            this.btnHistorial.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnHistorial.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHistorial.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnHistorial.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnHistorial.Image = global::PVLaJoya.Properties.Resources.iconHistorial;
            this.btnHistorial.Location = new System.Drawing.Point(245, 5);
            this.btnHistorial.Margin = new System.Windows.Forms.Padding(5);
            this.btnHistorial.Name = "btnHistorial";
            this.btnHistorial.Size = new System.Drawing.Size(230, 170);
            this.btnHistorial.TabIndex = 1;
            this.btnHistorial.Text = "Historial";
            this.btnHistorial.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnHistorial.UseVisualStyleBackColor = false;
            this.btnHistorial.Click += new System.EventHandler(this.BtnHistorial_Click);
            // 
            // btnDevoluciones
            // 
            this.btnDevoluciones.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDevoluciones.AutoSize = true;
            this.btnDevoluciones.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDevoluciones.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDevoluciones.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnDevoluciones.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnDevoluciones.Image = global::PVLaJoya.Properties.Resources.iconBag;
            this.btnDevoluciones.Location = new System.Drawing.Point(485, 5);
            this.btnDevoluciones.Margin = new System.Windows.Forms.Padding(5);
            this.btnDevoluciones.Name = "btnDevoluciones";
            this.btnDevoluciones.Size = new System.Drawing.Size(230, 170);
            this.btnDevoluciones.TabIndex = 5;
            this.btnDevoluciones.Text = "Devoluciones";
            this.btnDevoluciones.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDevoluciones.UseVisualStyleBackColor = false;
            this.btnDevoluciones.Click += new System.EventHandler(this.btnDevoluciones_Click);
            // 
            // btnDisparo
            // 
            this.btnDisparo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisparo.AutoSize = true;
            this.btnDisparo.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnDisparo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDisparo.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnDisparo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnDisparo.Image = global::PVLaJoya.Properties.Resources.iconEnviar;
            this.btnDisparo.Location = new System.Drawing.Point(5, 185);
            this.btnDisparo.Margin = new System.Windows.Forms.Padding(5);
            this.btnDisparo.Name = "btnDisparo";
            this.btnDisparo.Size = new System.Drawing.Size(230, 172);
            this.btnDisparo.TabIndex = 2;
            this.btnDisparo.Text = "Enviar a Ventas";
            this.btnDisparo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDisparo.UseVisualStyleBackColor = false;
            this.btnDisparo.Click += new System.EventHandler(this.BtnDisparo_Click);
            // 
            // btnArqueo
            // 
            this.btnArqueo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArqueo.AutoSize = true;
            this.btnArqueo.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnArqueo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnArqueo.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnArqueo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnArqueo.Image = global::PVLaJoya.Properties.Resources.revisionArqueo;
            this.btnArqueo.Location = new System.Drawing.Point(485, 185);
            this.btnArqueo.Margin = new System.Windows.Forms.Padding(5);
            this.btnArqueo.Name = "btnArqueo";
            this.btnArqueo.Size = new System.Drawing.Size(230, 172);
            this.btnArqueo.TabIndex = 6;
            this.btnArqueo.Text = "Arqueo";
            this.btnArqueo.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnArqueo.UseVisualStyleBackColor = false;
            this.btnArqueo.Click += new System.EventHandler(this.btnArqueo_Click);
            // 
            // btnCorteCaja
            // 
            this.btnCorteCaja.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCorteCaja.AutoSize = true;
            this.btnCorteCaja.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCorteCaja.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCorteCaja.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnCorteCaja.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnCorteCaja.Image = global::PVLaJoya.Properties.Resources.iconWallet;
            this.btnCorteCaja.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCorteCaja.Location = new System.Drawing.Point(245, 185);
            this.btnCorteCaja.Margin = new System.Windows.Forms.Padding(5);
            this.btnCorteCaja.Name = "btnCorteCaja";
            this.btnCorteCaja.Size = new System.Drawing.Size(230, 172);
            this.btnCorteCaja.TabIndex = 3;
            this.btnCorteCaja.Text = "Realizar Corte";
            this.btnCorteCaja.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnCorteCaja.UseVisualStyleBackColor = false;
            this.btnCorteCaja.Click += new System.EventHandler(this.BtnCorteCaja_Click);
            // 
            // btnCreditos
            // 
            this.btnCreditos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreditos.AutoSize = true;
            this.btnCreditos.BackColor = System.Drawing.SystemColors.ControlLight;
            this.btnCreditos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreditos.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.btnCreditos.ForeColor = System.Drawing.SystemColors.InfoText;
            this.btnCreditos.Image = global::PVLaJoya.Properties.Resources.iconfinder_Wallet_33872861;
            this.btnCreditos.Location = new System.Drawing.Point(725, 5);
            this.btnCreditos.Margin = new System.Windows.Forms.Padding(5);
            this.btnCreditos.Name = "btnCreditos";
            this.btnCreditos.Size = new System.Drawing.Size(231, 170);
            this.btnCreditos.TabIndex = 7;
            this.btnCreditos.Text = "Créditos";
            this.btnCreditos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnCreditos.UseVisualStyleBackColor = false;
            this.btnCreditos.Click += new System.EventHandler(this.btnCreditos_Click);
            // 
            // imgLstProductos
            // 
            this.imgLstProductos.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgLstProductos.ImageSize = new System.Drawing.Size(16, 16);
            this.imgLstProductos.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imgLstCategorias
            // 
            this.imgLstCategorias.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imgLstCategorias.ImageSize = new System.Drawing.Size(16, 16);
            this.imgLstCategorias.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label4.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.label4.Location = new System.Drawing.Point(0, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(961, 78);
            this.label4.TabIndex = 9;
            this.label4.Text = "Menú";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbxLogo
            // 
            this.pbxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pbxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbxLogo.Image")));
            this.pbxLogo.Location = new System.Drawing.Point(0, 30);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(83, 78);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxLogo.TabIndex = 10;
            this.pbxLogo.TabStop = false;
            // 
            // FMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(961, 505);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(7, 8, 7, 8);
            this.Name = "FMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FMenu_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FMenu_FormClosed);
            this.Load += new System.EventHandler(this.FMenu_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FMenu_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblUsuario;
        private System.Windows.Forms.ToolStripLabel lblSitio;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnHistorial;
        private System.Windows.Forms.ImageList imgLstProductos;
        private System.Windows.Forms.ImageList imgLstCategorias;
        private System.Windows.Forms.Button btnDisparo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCorteCaja;
        private System.Windows.Forms.ToolStripMenuItem cajaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem estadoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem consultasBDToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel lblCaja;
        private System.Windows.Forms.Button btnDevoluciones;
        private System.Windows.Forms.Button btnArqueo;
        private System.Windows.Forms.Button btnCreditos;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        private System.Windows.Forms.Button btnNuevaVenta;
        private System.Windows.Forms.PictureBox pbxLogo;
        private System.Windows.Forms.ToolStripMenuItem configuraciónPuertosToolStripMenuItem;
    }
}