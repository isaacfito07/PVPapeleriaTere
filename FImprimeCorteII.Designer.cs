
namespace PVLaJoya
{
    partial class FImprimeCorteII
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FImprimeCorteII));
            this.rpvCorte = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rpvCorte
            // 
            this.rpvCorte.Location = new System.Drawing.Point(0, -1);
            this.rpvCorte.Margin = new System.Windows.Forms.Padding(2);
            this.rpvCorte.Name = "rpvCorte";
            this.rpvCorte.ServerReport.BearerToken = null;
            this.rpvCorte.Size = new System.Drawing.Size(366, 718);
            this.rpvCorte.TabIndex = 0;
            // 
            // FImprimeCorteII
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 718);
            this.Controls.Add(this.rpvCorte);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FImprimeCorteII";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Imprime Corte";
            this.Load += new System.EventHandler(this.fImprimeCorteII_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rpvCorte;
    }
}