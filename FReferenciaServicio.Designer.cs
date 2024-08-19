namespace PVLaJoya
{
    partial class FReferenciaServicio
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FReferenciaServicio));
            this.mtbNumeroReferenciaRepetir = new System.Windows.Forms.MaskedTextBox();
            this.mtbNumeroReferencia = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.mtbMonto = new System.Windows.Forms.MaskedTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCantLetra = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mtbComision = new System.Windows.Forms.MaskedTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // mtbNumeroReferenciaRepetir
            // 
            this.mtbNumeroReferenciaRepetir.Font = new System.Drawing.Font("Century Gothic", 28.2F);
            this.mtbNumeroReferenciaRepetir.Location = new System.Drawing.Point(400, 115);
            this.mtbNumeroReferenciaRepetir.Margin = new System.Windows.Forms.Padding(2);
            this.mtbNumeroReferenciaRepetir.Name = "mtbNumeroReferenciaRepetir";
            this.mtbNumeroReferenciaRepetir.Size = new System.Drawing.Size(380, 54);
            this.mtbNumeroReferenciaRepetir.TabIndex = 31;
            this.mtbNumeroReferenciaRepetir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbNumeroReferenciaRepetir_KeyDown);
            // 
            // mtbNumeroReferencia
            // 
            this.mtbNumeroReferencia.Font = new System.Drawing.Font("Century Gothic", 28.2F);
            this.mtbNumeroReferencia.Location = new System.Drawing.Point(9, 115);
            this.mtbNumeroReferencia.Margin = new System.Windows.Forms.Padding(2);
            this.mtbNumeroReferencia.Name = "mtbNumeroReferencia";
            this.mtbNumeroReferencia.Size = new System.Drawing.Size(380, 54);
            this.mtbNumeroReferencia.TabIndex = 30;
            this.mtbNumeroReferencia.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbNumeroReferencia_KeyDown);
            this.mtbNumeroReferencia.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtbNumeroReferencia_KeyPress);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label7.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.label7.Location = new System.Drawing.Point(-2, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(791, 66);
            this.label7.TabIndex = 28;
            this.label7.Text = "Servicios";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.btnOK.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(314, 206);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(148, 49);
            this.btnOK.TabIndex = 27;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(394, 80);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(251, 33);
            this.label2.TabIndex = 26;
            this.label2.Text = "Repetir referencia";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 80);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 33);
            this.label1.TabIndex = 25;
            this.label1.Text = "Referencia";
            // 
            // pbxLogo
            // 
            this.pbxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pbxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbxLogo.Image")));
            this.pbxLogo.Location = new System.Drawing.Point(4, 0);
            this.pbxLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(68, 66);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxLogo.TabIndex = 29;
            this.pbxLogo.TabStop = false;
            // 
            // mtbMonto
            // 
            this.mtbMonto.Font = new System.Drawing.Font("Century Gothic", 28.2F);
            this.mtbMonto.Location = new System.Drawing.Point(9, 206);
            this.mtbMonto.Margin = new System.Windows.Forms.Padding(2);
            this.mtbMonto.Name = "mtbMonto";
            this.mtbMonto.Size = new System.Drawing.Size(285, 54);
            this.mtbMonto.TabIndex = 32;
            this.mtbMonto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbMonto_KeyDown);
            this.mtbMonto.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtbMonto_KeyPress);
            this.mtbMonto.Validating += new System.ComponentModel.CancelEventHandler(this.mtbMonto_Validating);
            this.mtbMonto.Validated += new System.EventHandler(this.mtbMonto_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 172);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 33);
            this.label3.TabIndex = 33;
            this.label3.Text = "Monto";
            // 
            // lblCantLetra
            // 
            this.lblCantLetra.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCantLetra.AutoSize = true;
            this.lblCantLetra.Font = new System.Drawing.Font("Century Gothic", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantLetra.Location = new System.Drawing.Point(9, 275);
            this.lblCantLetra.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCantLetra.Name = "lblCantLetra";
            this.lblCantLetra.Size = new System.Drawing.Size(23, 25);
            this.lblCantLetra.TabIndex = 34;
            this.lblCantLetra.Text = "_";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 16F);
            this.label4.Location = new System.Drawing.Point(9, 336);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(255, 25);
            this.label4.TabIndex = 35;
            this.label4.Text = "Cargo por transacción";
            // 
            // mtbComision
            // 
            this.mtbComision.Font = new System.Drawing.Font("Century Gothic", 16F);
            this.mtbComision.Location = new System.Drawing.Point(314, 334);
            this.mtbComision.Margin = new System.Windows.Forms.Padding(2);
            this.mtbComision.Name = "mtbComision";
            this.mtbComision.ReadOnly = true;
            this.mtbComision.Size = new System.Drawing.Size(110, 34);
            this.mtbComision.TabIndex = 36;
            this.mtbComision.Text = "$8.00";
            this.mtbComision.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FReferenciaServicio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 384);
            this.Controls.Add(this.mtbComision);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblCantLetra);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.mtbMonto);
            this.Controls.Add(this.mtbNumeroReferenciaRepetir);
            this.Controls.Add(this.mtbNumeroReferencia);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FReferenciaServicio";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Servicios";
            this.Load += new System.EventHandler(this.FReferenciaServicio_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MaskedTextBox mtbNumeroReferenciaRepetir;
        private System.Windows.Forms.MaskedTextBox mtbNumeroReferencia;
        private System.Windows.Forms.PictureBox pbxLogo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MaskedTextBox mtbMonto;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblCantLetra;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MaskedTextBox mtbComision;
    }
}