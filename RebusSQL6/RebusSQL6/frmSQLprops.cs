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
    public partial class frmSQLprops : Form
    {
        private bool mbOK;
        public bool OK { get { return mbOK; } set { mbOK = value; } }

        public frmSQLprops()
        {
            InitializeComponent();
            mbOK = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            mbOK = true;
            this.Close();
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            mbOK = false;
            this.Close();
        }
    }
}
