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
    public partial class frmBaseMDI : Form
    {
        public MDIType FormType { get { return meFormType; } set { meFormType = value; } }
        private MDIType meFormType;

        public int InternalWindowID { get; set; }

        public frmBaseMDI()
        {
            InitializeComponent();
            meFormType = MDIType.Unknown;
        }

        private void frmBaseMDI_Enter(object sender, EventArgs e)
        {
            frmMain xoMDIParent = (frmMain)this.MdiParent;
            xoMDIParent.RefreshChildWindowList();
        }

        private void frmBaseMDI_Activated(object sender, EventArgs e)
        {
            frmMain xoParent = (frmMain)this.MdiParent;
            xoParent.ChildWindowFocused();
            xoParent = null;
        }

        private void frmBaseMDI_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmMain xoMDIParent = (frmMain)this.MdiParent;
            //xoMDIParent.RefreshChildWindowList();
            xoMDIParent.NeedRefresh();
        }
    }
}
