
namespace PVLaJoya
{
    partial class FCajaFondo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FCajaFondo));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblUsuario = new System.Windows.Forms.ToolStripLabel();
            this.lblSitio = new System.Windows.Forms.ToolStripLabel();
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.lblTit = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nudFondo = new System.Windows.Forms.NumericUpDown();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFondo)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblUsuario,
            this.lblSitio});
            this.toolStrip1.Location = new System.Drawing.Point(0, 162);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(382, 25);
            this.toolStrip1.TabIndex = 46;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblUsuario
            // 
            this.lblUsuario.Font = new System.Drawing.Font("Century Gothic", 9F);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(16, 22);
            this.lblUsuario.Text = "--";
            // 
            // lblSitio
            // 
            this.lblSitio.Font = new System.Drawing.Font("Century Gothic", 9F);
            this.lblSitio.Name = "lblSitio";
            this.lblSitio.Size = new System.Drawing.Size(16, 22);
            this.lblSitio.Text = "--";
            // 
            // pbxLogo
            // 
            this.pbxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pbxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbxLogo.Image")));
            this.pbxLogo.Location = new System.Drawing.Point(1, 0);
            this.pbxLogo.Margin = new System.Windows.Forms.Padding(2);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(62, 63);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxLogo.TabIndex = 45;
            this.pbxLogo.TabStop = false;
            // 
            // lblTit
            // 
            this.lblTit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.lblTit.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold);
            this.lblTit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.lblTit.Location = new System.Drawing.Point(1, 0);
            this.lblTit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTit.Name = "lblTit";
            this.lblTit.Size = new System.Drawing.Size(381, 63);
            this.lblTit.TabIndex = 44;
            this.lblTit.Text = "Fondo de Caja";
            this.lblTit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(220, 127);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(2);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 33);
            this.btnGuardar.TabIndex = 49;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 16.2F);
            this.label1.Location = new System.Drawing.Point(39, 89);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 25);
            this.label1.TabIndex = 48;
            this.label1.Text = "Cantidad:";
            // 
            // nudFondo
            // 
            this.nudFondo.DecimalPlaces = 2;
            this.nudFondo.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Bold);
            this.nudFondo.Location = new System.Drawing.Point(169, 83);
            this.nudFondo.Margin = new System.Windows.Forms.Padding(2);
            this.nudFondo.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.nudFondo.Name = "nudFondo";
            this.nudFondo.Size = new System.Drawing.Size(161, 40);
            this.nudFondo.TabIndex = 47;
            this.nudFondo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudFondo.Enter += new System.EventHandler(this.nudFondo_Enter);
            // 
            // FCajaFondo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(382, 187);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudFondo);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.lblTit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FCajaFondo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fondo de Caja";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fCajaFondo_FormClosing);
            this.Load += new System.EventHandler(this.fCajaFondo_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FCajaFondo_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFondo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblUsuario;
        private System.Windows.Forms.ToolStripLabel lblSitio;
        private System.Windows.Forms.PictureBox pbxLogo;
        private System.Windows.Forms.Label lblTit;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudFondo;
    }
}