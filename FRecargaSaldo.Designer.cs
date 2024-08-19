
namespace PVLaJoya
{
    partial class FRecargaSaldo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRecargaSaldo));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.pbxLogo = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.mtbNumeroTelefonico = new System.Windows.Forms.MaskedTextBox();
            this.mtbNumeroTelefonicoRepetir = new System.Windows.Forms.MaskedTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(72, 94);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 33);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ingrese número telefónico";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 19.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(184, 203);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "Repetir";
            // 
            // btnOK
            // 
            this.btnOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.btnOK.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.White;
            this.btnOK.Location = new System.Drawing.Point(166, 335);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(148, 49);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pbxLogo
            // 
            this.pbxLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.pbxLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbxLogo.Image")));
            this.pbxLogo.Location = new System.Drawing.Point(1, -3);
            this.pbxLogo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbxLogo.Name = "pbxLogo";
            this.pbxLogo.Size = new System.Drawing.Size(68, 66);
            this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxLogo.TabIndex = 22;
            this.pbxLogo.TabStop = false;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(218)))), ((int)(((byte)(234)))), ((int)(((byte)(246)))));
            this.label7.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(134)))), ((int)(((byte)(228)))));
            this.label7.Location = new System.Drawing.Point(-4, -3);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(484, 66);
            this.label7.TabIndex = 21;
            this.label7.Text = "Recargas";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mtbNumeroTelefonico
            // 
            this.mtbNumeroTelefonico.Font = new System.Drawing.Font("Century Gothic", 28.2F);
            this.mtbNumeroTelefonico.Location = new System.Drawing.Point(102, 136);
            this.mtbNumeroTelefonico.Margin = new System.Windows.Forms.Padding(2);
            this.mtbNumeroTelefonico.Name = "mtbNumeroTelefonico";
            this.mtbNumeroTelefonico.Size = new System.Drawing.Size(285, 54);
            this.mtbNumeroTelefonico.TabIndex = 23;
            this.mtbNumeroTelefonico.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.mtbNumeroTelefonico_MaskInputRejected);
            this.mtbNumeroTelefonico.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbNumeroTelefonico_KeyDown);
            // 
            // mtbNumeroTelefonicoRepetir
            // 
            this.mtbNumeroTelefonicoRepetir.Font = new System.Drawing.Font("Century Gothic", 28.2F);
            this.mtbNumeroTelefonicoRepetir.Location = new System.Drawing.Point(102, 238);
            this.mtbNumeroTelefonicoRepetir.Margin = new System.Windows.Forms.Padding(2);
            this.mtbNumeroTelefonicoRepetir.Name = "mtbNumeroTelefonicoRepetir";
            this.mtbNumeroTelefonicoRepetir.Size = new System.Drawing.Size(285, 54);
            this.mtbNumeroTelefonicoRepetir.TabIndex = 24;
            this.mtbNumeroTelefonicoRepetir.MaskInputRejected += new System.Windows.Forms.MaskInputRejectedEventHandler(this.mtbNumeroTelefonicoRepetir_MaskInputRejected);
            this.mtbNumeroTelefonicoRepetir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mtbNumeroTelefonicoRepetir_KeyDown);
            // 
            // FRecargaSaldo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 421);
            this.Controls.Add(this.mtbNumeroTelefonicoRepetir);
            this.Controls.Add(this.mtbNumeroTelefonico);
            this.Controls.Add(this.pbxLogo);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FRecargaSaldo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Recarga Saldo";
            this.Load += new System.EventHandler(this.FRecargaSaldo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pbxLogo;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.MaskedTextBox mtbNumeroTelefonico;
        private System.Windows.Forms.MaskedTextBox mtbNumeroTelefonicoRepetir;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}