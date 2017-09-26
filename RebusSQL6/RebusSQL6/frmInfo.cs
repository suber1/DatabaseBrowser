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
    public partial class frmInfo : Form
    {
        public frmInfo(string psInfoText = "")
        {
            InitializeComponent();
            txtInfo.Text = psInfoText;
        }
    }
}
