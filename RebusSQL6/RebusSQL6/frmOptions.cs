using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RebusSQL6
{
    public partial class frmOptions : Form
    {
        public bool OK { get; set; }

        public int SelectTop { get; set; }
        public int TimeoutSecs { get; set; }


        public frmOptions()
        {
            InitializeComponent();
            SelectTop = 1000;
            TimeoutSecs = 120;
            OK = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool xbErr = false;

            try
            {
                int xi = Convert.ToInt32(txtTop.Text.Trim());
                if (xi >= 0) SelectTop = xi; else xbErr = true;
            }
            catch { xbErr = true; }

            if (xbErr)
            {
                Global.ShowMessage("Invalid TOP value.", this.Text);
            }
            else
            {
                try
                {
                    int xi = Convert.ToInt32(txtSecs.Text.Trim());
                    if (xi >= 0) TimeoutSecs = xi; else xbErr = true;
                }
                catch { xbErr = true; }
                if (xbErr)
                {
                    Global.ShowMessage("Invalid command timeout value.", this.Text);
                }
                else
                {
                    OK = true;
                    this.Hide();
                }
            }
        }

        private void frmOptions_Shown(object sender, EventArgs e)
        {
            txtTop.Text = SelectTop.ToString().Trim();
            txtSecs.Text = TimeoutSecs.ToString().Trim();
        }
    }
}
