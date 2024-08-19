
namespace PVLaJoya
{
    partial class FImprimeCorteParcial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FImprimeCorteParcial));
            this.rpvCorteParcial = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rpvCorteParcial
            // 
            this.rpvCorteParcial.Location = new System.Drawing.Point(-1, 0);
            this.rpvCorteParcial.Margin = new System.Windows.Forms.Padding(2);
            this.rpvCorteParcial.Name = "rpvCorteParcial";
            this.rpvCorteParcial.ServerReport.BearerToken = null;
            this.rpvCorteParcial.Size = new System.Drawing.Size(378, 681);
            this.rpvCorteParcial.TabIndex = 0;
            // 
            // FImprimeCorteParcial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 625);
            this.Controls.Add(this.rpvCorteParcial);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FImprimeCorteParcial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Corte Parcial";
            this.Load += new System.EventHandler(this.fImprimeCorteParcial_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rpvCorteParcial;
    }
}