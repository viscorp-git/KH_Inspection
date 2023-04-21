using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KH_Inspection
{
    public partial class frm_Loader : Form
    {
        public frm_Loader()
        {
            InitializeComponent();
        }

        public void Init_Progress(int max_Value)
        {
            lb_Text.Text = "Load.....";
            pgB_Loading.Maximum = max_Value;
            pgB_Loading.Value = 0;

            // progressBar1.Value = value;
        }

        public void UpdateProgress(string txt, int value)
        {
            lb_Text.Text = txt;

            pgB_Loading.Value = value;

        }

        private void frm_Loader_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ocp_progress.Stop();
        }
    }
}
