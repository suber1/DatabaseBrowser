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
    public partial class frmPickOne : Form
    {
        private bool mbOK = false;


        public frmPickOne()
        {
            InitializeComponent();
        }


        public bool OK()
        {
            this.Hide();
            return (mbOK);
        }

        public void SetTitlePromptList(string psTitle, string psPrompt, List<string> psChoices)
        {
            this.Text = psTitle;
            lblPrompt.Text = psPrompt;
            drp.Items.Clear();
            if (psChoices != null)
            {
                for (int xii = 0; xii < psChoices.Count; xii++)
                {
                    drp.Items.Add(psChoices[xii]);
                }
            }
        }

        public int SelectedIndex()
        {
            return (drp.SelectedIndex);
        }

        private void frmPickOne_Load(object sender, EventArgs e)
        {

        }

        private void UpdateControlsAvailability()
        {
            btnOK.Enabled = drp.SelectedIndex >= 0;
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            mbOK = false;
            this.Hide();
        }

        private void frmPickOne_Shown(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void drp_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            mbOK = true;
            this.Hide();
        }
    }
}
