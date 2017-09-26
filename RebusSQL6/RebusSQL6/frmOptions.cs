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

        public int Top { get; set; }


        public frmOptions()
        {
            InitializeComponent();
            Top = 1000;
            OK = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            bool xbErr = false;

            try
            {
                int xi = Convert.ToInt32(txtTop.Text.Trim());
                if (xi >= 0) Top = xi; else xbErr = true;
            }
            catch { xbErr = true; }

            if (xbErr)
            {
                Global.ShowMessage("Invalid TOP value.", this.Text);
            }
            else
            {
                OK = true;
                this.Hide();
            }
        }

        private void frmOptions_Shown(object sender, EventArgs e)
        {
            txtTop.Text = Top.ToString().Trim();
        }
    }
}
