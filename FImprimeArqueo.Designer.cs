
namespace PVLaJoya
{
    partial class FImprimeArqueo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FImprimeArqueo));
            this.rpvArqueo = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rpvArqueo
            // 
            this.rpvArqueo.Location = new System.Drawing.Point(-1, 0);
            this.rpvArqueo.Margin = new System.Windows.Forms.Padding(2);
            this.rpvArqueo.Name = "rpvArqueo";
            this.rpvArqueo.ServerReport.BearerToken = null;
            this.rpvArqueo.Size = new System.Drawing.Size(378, 681);
            this.rpvArqueo.TabIndex = 1;
            // 
            // FImprimeArqueo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 625);
            this.Controls.Add(this.rpvArqueo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FImprimeArqueo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Imprime Arqueo";
            this.Load += new System.EventHandler(this.FImprimeArqueo_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rpvArqueo;
    }
}