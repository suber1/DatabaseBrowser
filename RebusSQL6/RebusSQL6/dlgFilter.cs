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
    public partial class dlgFilter : Form
    {
        public bool OK { get; set; }
        public string MatchText { get { return txt.Text.Trim(); } set { txt.Text = value.Trim(); /*Avails();*/ } }



        public dlgFilter(string psContains = "")
        {
            InitializeComponent();
            OK = false;
            MatchText = psContains;
            Avails();
        }

        private void dlgFilter_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Avails()
        {
            btnOK.Enabled = (MatchText.Length > 0);
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            Avails();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OK = true;
            this.Hide();
        }
    }
}
