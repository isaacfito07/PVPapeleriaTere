using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PVLaJoya
{
    public partial class FConsultas : Form
    {
        ConSQL sqlLoc;

        public FConsultas(ConSQL _sqlLoc)
        {
            InitializeComponent();
            sqlLoc = _sqlLoc;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            gv.DataSource = null;
            DataTable dt = sqlLoc.selec(txtConsulta.Text);
            if (dt.Rows.Count > 0)
                gv.DataSource = dt;

        }

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            int i = sqlLoc.exec(txtConsulta.Text);
            if (i != 0)
                MessageBox.Show("Ejecutado");
            else
                MessageBox.Show("Verificar query");
        }

        private void fConsultas_Load(object sender, EventArgs e)
        {

        }
    }
}
