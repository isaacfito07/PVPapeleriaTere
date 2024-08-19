namespace PVLaJoya
{
    partial class FTicketDevolucion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FTicketDevolucion));
            this.rpTicket1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rpTicket1
            // 
            this.rpTicket1.Location = new System.Drawing.Point(4, 2);
            this.rpTicket1.Margin = new System.Windows.Forms.Padding(2);
            this.rpTicket1.Name = "rpTicket1";
            this.rpTicket1.ServerReport.BearerToken = null;
            this.rpTicket1.Size = new System.Drawing.Size(331, 536);
            this.rpTicket1.TabIndex = 1;
            // 
            // FTicketDevolucion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 540);
            this.Controls.Add(this.rpTicket1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FTicketDevolucion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Devolucion";
            this.Load += new System.EventHandler(this.FTicketDevolucion_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer rpTicket1;
    }
}