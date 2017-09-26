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
    public partial class frmMasterChildLink : Form
    {
        private DB moDB;

        private List<DataField> MasterFields { get; set; }
        private List<DataField> ChildFields { get; set; }
        public string MasterField { get; set; }
        public string ChildField { get; set; }
        private bool mbOK = false;
        private string msChosenFKChildTable = "";

        public string ParentTable = "";
        
        public int DbID = 0;


        public frmMasterChildLink(DB poDB, int piDbID)
        {
            InitializeComponent();

            moDB = poDB;
            DbID = piDbID;
        }

        public string ChosenForeignKeyLinkChildTable()
        {
            return (msChosenFKChildTable);
        }

        public void SetMasterFields(List<DataField> prFlds)
        {
            MasterFields = prFlds;
            if (lstMastFlds.Items.Count > 0) lstMastFlds.Items.Clear();
            if (prFlds != null && prFlds.Count > 0)
            {
                for (int xii = 0; xii < MasterFields.Count; xii++)
                {
                    lstMastFlds.Items.Add(MasterFields[xii].Name);
                }
            }
        }

        public void SetChildFields(List<DataField> prFlds)
        {
            ChildFields = prFlds;
            if (lstChildFlds.Items.Count > 0) lstChildFlds.Items.Clear();
            if (prFlds != null && prFlds.Count > 0)
            {
                for (int xii = 0; xii < ChildFields.Count; xii++)
                {
                    lstChildFlds.Items.Add(ChildFields[xii].Name);
                }
            }
        }

        public bool OK()
        {
            return (mbOK);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstMastFlds.SelectedIndex < 0)
            {
                statusLbl.Text = "Choose a parent field";
            }
            else
            {
                if (lstChildFlds.SelectedIndex < 0)
                {
                    statusLbl.Text = "Choose a child field";
                }
                else
                {
                    if (MasterFields[lstMastFlds.SelectedIndex].Type == ChildFields[lstChildFlds.SelectedIndex].Type)
                    {
                        MasterField = lstMastFlds.Items[lstMastFlds.SelectedIndex].ToString();
                        ChildField = lstChildFlds.Items[lstChildFlds.SelectedIndex].ToString();
                        mbOK = true;
                        this.Hide();
                    }
                    else
                    {
                        statusLbl.Text = "Field types must match.";
                    }
                }
            }
        }

        private void frmMasterChildLink_DoubleClick(object sender, EventArgs e)
        {
            string xs = "";
            if (lstMastFlds.SelectedIndex >= 0) xs = MasterFields[lstMastFlds.SelectedIndex].Type.ToString();
            if (lstChildFlds.SelectedIndex >= 0)
            {
                if (xs.Length > 0) xs += ", ";
                xs = xs + ChildFields[lstChildFlds.SelectedIndex].Type.ToString();
            }
            statusLbl.Text = xs;
        }

        private void GetFieldsForTable(string psTbl)
        {
            List<DataField> xoFlds = new List<DataField>();
            string xsSQL = "SELECT * FROM [" + psTbl + "] WHERE 1 = 0";
            DataTable xoTbl = new DataTable();
            //lstChildFlds.Items.Clear();

            if (moDB.SQL(xsSQL, xoTbl))
            {
                for (int xiCol = 0; xiCol < xoTbl.Columns.Count; xiCol++)
                {
                    string xsFld = xoTbl.Columns[xiCol].ColumnName.ToString().Trim();
                    //lstChildFlds.Items.Add(xsFld);
                    DataField xoFld = new DataField();
                    xoFld.Name = xsFld;
                    xoFld.Type = xoTbl.Columns[xiCol].DataType;
                    xoFlds.Add(xoFld);
                }
            }
            xoTbl.Dispose();
            xoTbl = null;

            SetChildFields(xoFlds);
        }

        private void btnFK_Click(object sender, EventArgs e)
        {
            frmFK xoFrm = new frmFK(moDB, DbID);
            xoFrm.DbID = this.DbID;
            xoFrm.ParentTable = this.ParentTable;
            xoFrm.Text += " (with " + xoFrm.ParentTable + ")";
            xoFrm.ShowDialog();
            if (xoFrm.OK)
            {
                string xsChildTable = "", xsChildField = "", xsParentField = "";
                xoFrm.ForeignKey(out xsChildTable, out xsChildField, out xsParentField);
                GetFieldsForTable(xsChildTable);
                msChosenFKChildTable = xsChildTable;

                // select the parent/child fields here
                for (int xii = 0; xii < lstMastFlds.Items.Count; xii++)
                {
                    if (lstMastFlds.Items[xii].ToString().ToLower().Trim() == xsParentField.ToLower().Trim())
                    {
                        lstMastFlds.SelectedIndex = xii;
                        break;
                    }
                }

                for (int xii = 0; xii < lstChildFlds.Items.Count; xii++)
                {
                    if (lstChildFlds.Items[xii].ToString().ToLower().Trim() == xsChildField.ToLower().Trim())
                    {
                        lstChildFlds.SelectedIndex = xii;
                        break;
                    }
                }
            }
            try
            {
                xoFrm.Close();
                xoFrm.Dispose();
                xoFrm = null;
            }
            catch { }
        }

        private void frmMasterChildLink_FormClosing(object sender, FormClosingEventArgs e)
        {
            moDB = null;
        }
    }
}
