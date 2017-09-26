using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RebusData6;

namespace RebusSQL6
{
    public partial class frmFK : Form
    {
        private DB moDB;

        public string ParentTable = "";
        public string ChildTable = "";
        public bool OK = false;
        public int DbID = 0;

        private List<string> msParentField;


        public frmFK(DB poDB, int piDbID)
        {
            InitializeComponent();

            moDB = poDB;
            DbID = piDbID;
        }

        private void ItemChosen()
        {
            if (lstFKs.SelectedIndex >= 0)
            {
                OK = true;
                this.Hide();
            }
            else
            {
                statLbl.Text = "Please first select a foreign key table/field.";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ItemChosen();
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            OK = false;
            this.Hide();
        }

        private void frmFK_FormClosing(object sender, FormClosingEventArgs e)
        {
            moDB = null;
        }

        public void ForeignKey(out string psChildTable, out string psChildField, out string psParentField)
        {
            string xsChildTable = "", xsChildField = "", xsParentField = "";

            if (lstFKs.SelectedIndex >= 0)
            {
                string xs = lstFKs.Items[lstFKs.SelectedIndex].ToString();
                int xi = xs.IndexOf(".");
                if (xi > 0)
                {
                    xsChildTable = xs.Substring(0, xi);
                    xsChildField = xs.Substring(xi + 1);
                    xsParentField = msParentField[lstFKs.SelectedIndex];
                    xs = "";;
                }
            }
            psChildTable = xsChildTable;
            psChildField = xsChildField;
            psParentField = xsParentField;
        }

        private void LoadFKs()
        {
            lstFKs.Items.Clear();
            msParentField = new List<string>();
            
            DB xoDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoDB))
            {
                string xsSQL = "SELECT tableA, fieldA, tableB, fieldB FROM ForeignKeys WHERE databaseID = " + DbID.ToString() + " AND (tableA = '" + this.ParentTable + "' OR tableB = '" + this.ParentTable + "')";
                DataTable xoTbl = new DataTable();
                if (xoDB.SQL(xsSQL, xoTbl))
                {
                    string xsParentTable = this.ParentTable.Trim().ToLower();
                    if (xoTbl.Rows.Count > 0)
                    {
                        for (int xii = 0; xii < xoTbl.Rows.Count; xii++)
                        {
                            string xsChildTable = "", xsChildField = "", xsParentField = "";
                            string xsTableA = Global.NullToString(xoTbl.Rows[xii][0]).Trim().ToLower();
                            string xsFieldA = Global.NullToString(xoTbl.Rows[xii][1]).Trim().ToLower();
                            string xsTableB = Global.NullToString(xoTbl.Rows[xii][2]).Trim().ToLower();
                            string xsFieldB = Global.NullToString(xoTbl.Rows[xii][3]).Trim().ToLower();
                            if (xsParentTable == xsTableA)
                            {
                                xsChildTable = xsTableB;
                                xsChildField = xsFieldB;
                                xsParentField = xsFieldA;
                            }
                            else
                            {
                                if (xsParentTable == xsTableB)
                                {
                                    xsChildTable = xsTableA;
                                    xsChildField = xsFieldA;
                                    xsParentField = xsFieldB;
                                }
                            }
                            if (xsChildTable.Length > 0 && xsChildField.Length > 0)
                            {
                                string xs = xsChildTable + "." + xsChildField;
                                lstFKs.Items.Add(xs);
                                msParentField.Add(xsParentField);
                            }
                        }
                    }
                }
                else
                {
                    statLbl.Text = moDB.Message;
                }
                xoTbl.Dispose();
            }

            Global.CloseThisAppsDatabase(ref xoDB);
        }

        private void frmFK_Shown(object sender, EventArgs e)
        {
            LoadFKs();
        }

        private void lstFKs_DoubleClick(object sender, EventArgs e)
        {
            ItemChosen();
        }

    }

}
