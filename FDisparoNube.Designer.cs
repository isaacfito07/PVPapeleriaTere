
namespace PVLaJoya
{
    partial class FDisparoNube
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FDisparoNube));
            this.dvgHistoria = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.btnDisparo = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripLabel();
            this.lblSitio = new System.Windows.Forms.ToolStripLabel();
            this.lblCaja_ = new System.Windows.Forms.ToolStripLabel();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.pbDisparo = new System.Windows.Forms.ProgressBar();
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCountPagos = new System.Windows.Forms.Label();
            this.lblCountVentas = new System.Windows.Forms.Label();
            this.lblCountCortes = new System.Windows.Forms.Label();
            this.lblCountDetalle = new System.Windows.Forms.Label();
            this.lblCountRetiros = new System.Windows.Forms.Label();
            this.lblCountDevoluciones = new System.Windows.Forms.Label();
            this.lblCountMonedero = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dvgHistoria)).BeginInit();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // dvgHistoria
            // 
            this.dvgHistoria.AllowUserToAddRows = false;
            this.dvgHistoria.AllowUserToDeleteRows = false;
            this.dvgHistoria.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dvgHistoria.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dvgHistoria.BackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.dvgHistoria.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvgHistoria.GridColor = System.Drawing.Color.RoyalBlue;
            this.dvgHistoria.Location = new System.Drawing.Point(-2, 236);
            this.dvgHistoria.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dvgHistoria.Name = "dvgHistoria";
            this.dvgHistoria.RowHeadersWidth = 51;
            this.dvgHistoria.RowTemplate.Height = 24;
            this.dvgHistoria.Size = new System.Drawing.Size(1199, 417);
            this.dvgHistoria.TabIndex = 21;
            this.dvgHistoria.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dvgHistoria_KeyDown);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label4.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1197, 87);
            this.label4.TabIndex = 19;
            this.label4.Text = "Enviar a nube";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDisparo
            // 
            this.btnDisparo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.btnDisparo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDisparo.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisparo.ForeColor = System.Drawing.Color.White;
            this.btnDisparo.Location = new System.Drawing.Point(7, 118);
            this.btnDisparo.Name = "btnDisparo";
            this.btnDisparo.Size = new System.Drawing.Size(156, 47);
            this.btnDisparo.TabIndex = 26;
            this.btnDisparo.Text = "Disparo";
            this.btnDisparo.UseVisualStyleBackColor = false;
            this.btnDisparo.Click += new System.EventHandler(this.btnDisparo_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuario,
            this.lblSitio,
            this.lblCaja_});
            this.toolStrip1.Location = new System.Drawing.Point(0, 657);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1197, 25);
            this.toolStrip1.TabIndex = 27;
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
            // lblCaja_
            // 
            this.lblCaja_.Name = "lblCaja_";
            this.lblCaja_.Size = new System.Drawing.Size(17, 22);
            this.lblCaja_.Text = "--";
            // 
            // btnEliminar
            // 
            this.btnEliminar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEliminar.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(961, 118);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(193, 47);
            this.btnEliminar.TabIndex = 28;
            this.btnEliminar.Text = "Eliminar Historial";
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Visible = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // pbDisparo
            // 
            this.pbDisparo.Location = new System.Drawing.Point(184, 128);
            this.pbDisparo.Name = "pbDisparo";
            this.pbDisparo.Size = new System.Drawing.Size(970, 24);
            this.pbDisparo.TabIndex = 29;
            // 
            // pbxLogo
            // 
            this.pbxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pbxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbxLogo.Image")));
            this.pbxLogo.Location = new System.Drawing.Point(7, -1);
            this.pbxLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(86, 87);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxLogo.TabIndex = 20;
            this.pbxLogo.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(13, 170);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 25);
            this.label1.TabIndex = 30;
            this.label1.Text = "Datos Pendientes: ";
            // 
            // lblCountPagos
            // 
            this.lblCountPagos.AutoSize = true;
            this.lblCountPagos.Location = new System.Drawing.Point(351, 200);
            this.lblCountPagos.Name = "lblCountPagos";
            this.lblCountPagos.Size = new System.Drawing.Size(58, 21);
            this.lblCountPagos.TabIndex = 31;
            this.lblCountPagos.Text = "Pagos:";
            // 
            // lblCountVentas
            // 
            this.lblCountVentas.AutoSize = true;
            this.lblCountVentas.Location = new System.Drawing.Point(13, 200);
            this.lblCountVentas.Name = "lblCountVentas";
            this.lblCountVentas.Size = new System.Drawing.Size(66, 21);
            this.lblCountVentas.TabIndex = 32;
            this.lblCountVentas.Text = "Ventas: ";
            // 
            // lblCountCortes
            // 
            this.lblCountCortes.AutoSize = true;
            this.lblCountCortes.Location = new System.Drawing.Point(1040, 200);
            this.lblCountCortes.Name = "lblCountCortes";
            this.lblCountCortes.Size = new System.Drawing.Size(66, 21);
            this.lblCountCortes.TabIndex = 33;
            this.lblCountCortes.Text = "Cortes: ";
            // 
            // lblCountDetalle
            // 
            this.lblCountDetalle.AutoSize = true;
            this.lblCountDetalle.Location = new System.Drawing.Point(190, 200);
            this.lblCountDetalle.Name = "lblCountDetalle";
            this.lblCountDetalle.Size = new System.Drawing.Size(65, 21);
            this.lblCountDetalle.TabIndex = 34;
            this.lblCountDetalle.Text = "Detalle:";
            // 
            // lblCountRetiros
            // 
            this.lblCountRetiros.AutoSize = true;
            this.lblCountRetiros.Location = new System.Drawing.Point(887, 200);
            this.lblCountRetiros.Name = "lblCountRetiros";
            this.lblCountRetiros.Size = new System.Drawing.Size(70, 21);
            this.lblCountRetiros.TabIndex = 35;
            this.lblCountRetiros.Text = "Retiros: ";
            // 
            // lblCountDevoluciones
            // 
            this.lblCountDevoluciones.AutoSize = true;
            this.lblCountDevoluciones.Location = new System.Drawing.Point(494, 200);
            this.lblCountDevoluciones.Name = "lblCountDevoluciones";
            this.lblCountDevoluciones.Size = new System.Drawing.Size(112, 21);
            this.lblCountDevoluciones.TabIndex = 37;
            this.lblCountDevoluciones.Text = "Devoluciones:";
            // 
            // lblCountMonedero
            // 
            this.lblCountMonedero.AutoSize = true;
            this.lblCountMonedero.Location = new System.Drawing.Point(703, 201);
            this.lblCountMonedero.Name = "lblCountMonedero";
            this.lblCountMonedero.Size = new System.Drawing.Size(92, 21);
            this.lblCountMonedero.TabIndex = 36;
            this.lblCountMonedero.Text = "Monedero:";
            // 
            // FDisparoNube
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(1197, 682);
            this.Controls.Add(this.lblCountDevoluciones);
            this.Controls.Add(this.lblCountMonedero);
            this.Controls.Add(this.lblCountRetiros);
            this.Controls.Add(this.lblCountDetalle);
            this.Controls.Add(this.lblCountCortes);
            this.Controls.Add(this.lblCountVentas);
            this.Controls.Add(this.lblCountPagos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbDisparo);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnDisparo);
            this.Controls.Add(this.dvgHistoria);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.label4);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FDisparoNube";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Disparo a Nube";
            this.Load += new System.EventHandler(this.fDisparoNube_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dvgHistoria)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dvgHistoria;
        private System.Windows.Forms.PictureBox pbxLogo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnDisparo;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblUsuario;
        private System.Windows.Forms.ToolStripLabel lblSitio;
        private System.Windows.Forms.ToolStripLabel lblCaja_;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.ProgressBar pbDisparo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCountPagos;
        private System.Windows.Forms.Label lblCountVentas;
        private System.Windows.Forms.Label lblCountCortes;
        private System.Windows.Forms.Label lblCountDetalle;
        private System.Windows.Forms.Label lblCountRetiros;
        private System.Windows.Forms.Label lblCountDevoluciones;
        private System.Windows.Forms.Label lblCountMonedero;
    }
}