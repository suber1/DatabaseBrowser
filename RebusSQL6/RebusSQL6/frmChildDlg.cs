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
    public partial class frmChildDlg : Form
    {
        private DB moDB;
        private bool mbOK = false;
        public List<DataField> AvailableMasterFields;
        private string msChildTable = "";
        private List<DataField> ChildFields;

        public string ParentTable = "";
        public string ChildsParentTable = "";
        public int DbID = 0;


        public frmChildDlg(DB poDB, int poDbID)
        {
            InitializeComponent();

            moDB = poDB;
            DbID = poDbID;
        }

        private void frmChildDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            moDB = null;
        }

        public bool OK()
        {
            return (mbOK);
        }

        public List<string> ShowFields()
        {
            List<string> xsFlds = new List<string>(0);

            for (int xii = 0; xii < lstShowFlds.Items.Count; xii++)
            {
                xsFlds.Add(lstShowFlds.Items[xii].ToString());
            }

            return (xsFlds);
        }

        public string MasterField()
        {
            return (txtMasterFlds.Text.Trim());
        }

        public string ChildTable()
        {
            //return (txtManualTable.Text.Trim());
            return (msChildTable);
        }

        public string ChildField()
        {
            return (txtChildFlds.Text.Trim());
        }

        private void LoadData()
        {
            List<string> xsTbls = moDB.GetTables();
            List<string> xsTbls2 = new List<string>(0);
            string xsColms = "";

            if (xsTbls.Count >= 2)
            {
                xsColms = xsTbls[0].Trim();

                if (xsColms.IndexOf("\t") >= 0)
                {
                    string[] xsSep = { "\t" };
                    string[] xsVals = xsColms.Split(xsSep, StringSplitOptions.None);
                    int xiTableNameColumn = -1;
                    for (int xiPass = 1; xiPass <= 2; xiPass++)
                    {
                        for (int xii = 0; xii < xsVals.Count(); xii++)
                        {
                            if (xiPass == 1)
                            {
                                if (xsVals[xii].ToUpper() == "TABLE_NAME")
                                {
                                    xiTableNameColumn = xii;
                                    break;
                                }
                            }
                            else
                            {
                                string xs = xsVals[xii].ToUpper();
                                if (xs.IndexOf("NAME") >= 0)
                                {
                                    xiTableNameColumn = xii;
                                    break;
                                }
                            }
                        }
                        if (xiTableNameColumn >= 0 && xiPass < 2) break;
                    }

                    for (int xii = 1; xii < xsTbls.Count(); xii++)
                    {
                        string xsTbl = xsTbls[xii];
                        if (xiTableNameColumn >= 0)
                        {
                            xsVals = xsTbl.Split(xsSep, StringSplitOptions.None);
                            try
                            {
                                xsTbl = xsVals[xiTableNameColumn].Trim();
                            }
                            catch
                            {
                                xsTbl = "";
                            }
                        }
                        xsTbls2.Add(xsTbl);
                    }
                }
            }

            if (xsTbls2.Count > 2) xsTbls2.Sort();

            if (drpTable.Items.Count > 0) drpTable.Items.Clear();
            for (int xii = 0; xii < xsTbls2.Count; xii++)
            {
                string xsTbl = xsTbls2[xii];
                int xi = xsTbl.IndexOf("\t");
                if (xi > 0) xsTbl = xsTbl.Substring(0, xi);
                drpTable.Items.Add(xsTbl);
            }
        }

        private void AddLink()
        {
            if (AvailableMasterFields == null || AvailableMasterFields.Count == 0)
            {
                statusLbl.Text = "There are no parent fields identified.";
            }
            else
            {
                //if (lstFlds.Items.Count > 0)
                //{
                    frmMasterChildLink xoDlg = new frmMasterChildLink(moDB, DbID);
                    xoDlg.ParentTable = this.ParentTable;
                    xoDlg.SetMasterFields(this.AvailableMasterFields);
                    xoDlg.SetChildFields(this.ChildFields);
                    xoDlg.ShowDialog();
                    if (xoDlg.OK())
                    {
                        string xs = xoDlg.ChosenForeignKeyLinkChildTable().Trim();
                        if (xs.Length > 0)
                        {
                            //
                            // user chose a Foreign key link, so use that linked table
                            // as the child table in this dialog
                            //
                            xs = xs.ToLower();
                            for (int xii = 0; xii < drpTable.Items.Count; xii++)
                            {
                                if (xs == drpTable.Items[xii].ToString().Trim().ToLower())
                                {
                                    drpTable.SelectedIndex = xii;
                                    if (txtOrderByFlds.Text.Length > 0) txtOrderByFlds.Text = "";
                                    if (lstShowFlds.Items.Count > 0) lstShowFlds.Items.Clear();
                                    break;
                                }
                            }
                        }

                        xs = txtMasterFlds.Text.Trim();
                        if (xs.Length > 0) xs += ", ";
                        xs += xoDlg.MasterField.Trim();
                        txtMasterFlds.Text = xs;

                        xs = txtChildFlds.Text.Trim();
                        if (xs.Length > 0) xs += ", ";
                        xs += xoDlg.ChildField.Trim();
                        txtChildFlds.Text = xs;
                    }
                    xoDlg.Close();
                    xoDlg.Dispose();
                    xoDlg = null;
                //}
                //else
                //{
                //    statusLbl.Text = "Please first select a child table.";
                //}
            }
        }

        private void frmChildDlg_Shown(object sender, EventArgs e)
        {
            LoadData();
            statusLbl.Text = "Select the child table, the fields to show, and the parent/child field links to create the relationship.";
        }

        private void LoadFieldsForTable(string psTbl)
        {
            msChildTable = psTbl;
            string xsSQL = "SELECT * FROM [" + psTbl + "] WHERE 1 = 0";
            DataTable xoTbl = new DataTable();
            ChildFields = new List<DataField>(0);

            statusLbl.Text = "Child table: " + psTbl;

            if (moDB.SQL(xsSQL, xoTbl))
            {
                for (int xiCol = 0; xiCol < xoTbl.Columns.Count; xiCol++)
                {
                    DataField xrFld = new DataField();
                    xrFld.Name = xoTbl.Columns[xiCol].ColumnName.ToString().Trim();
                    xrFld.Type = xoTbl.Columns[xiCol].DataType;
                    ChildFields.Add(xrFld);
                }
            }
            xoTbl.Dispose();
            xoTbl = null;
            if (lstFlds.Items.Count > 0) lstFlds.Items.Clear();
            if (lstShowFlds.Items.Count > 0) lstShowFlds.Items.Clear();
            for (int xii = 0; xii < ChildFields.Count(); xii++)
            {
                lstFlds.Items.Add(ChildFields[xii].Name);
            }
        }

        private void SelectField()
        {
            if (lstFlds.SelectedIndex >= 0)
            {
                lstShowFlds.Items.Add(lstFlds.Items[lstFlds.SelectedIndex]);
            }
        }

        private void UnselectField()
        {
            if (lstShowFlds.SelectedIndex >= 0)
            {
                lstShowFlds.Items.RemoveAt(lstShowFlds.SelectedIndex);
            }
        }

        private void drpTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drpTable.SelectedIndex >= 0)
            {
                
                string xsTbl = drpTable.Items[drpTable.SelectedIndex].ToString();
                LoadFieldsForTable(xsTbl);
            }
            else
            {
                lstFlds.Items.Clear();
                lstShowFlds.Items.Clear();
            }
        }

        private void btnManualTable_Click(object sender, EventArgs e)
        {
            if (txtManualTable.Text.Trim().Length > 0) LoadFieldsForTable(txtManualTable.Text.Trim());
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (lstShowFlds.Items.Count == 0)
            {
                string xsMsg = "There are no items in the '" + label3.Text + "' to clear.";
                Global.ShowMessage(xsMsg, this.Text);
            }
            lstShowFlds.Items.Clear();
            txtOrderByFlds.Text = "";
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            SelectField();
        }

        private void SaveForeignKey()
        {
            string xsTable1 = this.ParentTable.Trim(), xsTable2 = "", xsField1 = "", xsField2 = "";
            if (xsTable1.Length == 0) xsTable1 = this.ChildsParentTable.Trim();

            if (drpTable.SelectedIndex >= 0) xsTable2 = drpTable.Items[drpTable.SelectedIndex].ToString().Trim();
            xsField1 = txtMasterFlds.Text.Trim();
            xsField2 = txtChildFlds.Text.Trim();
            if (xsField1.Length > 0)
            {
                if (xsField1.IndexOf(",") >= 0) xsField1 = "";      // not built for compound foreign keys (yet)
            }
            if (xsField2.Length > 0)
            {
                if (xsField2.IndexOf(",") >= 0) xsField1 = "";      // not built for compound foreign keys (yet)
            }

            if (xsTable1.Length > 0 && xsTable2.Length > 0 && xsField1.Length > 0 && xsField2.Length > 0)
            {
                DB xoDB = new DB();
                if (Global.OpenThisAppsDatabase(ref xoDB))
                {
                    int xiNotFoundCount = 0;
                    for (int xiPass = 1; xiPass <= 2; xiPass++)
                    {
                        string xsSQL = "SELECT * FROM ForeignKeys WHERE databaseID = " + DbID.ToString() + " AND ";
                        if (xiPass == 1)
                        {
                            xsSQL += " tableA = '" + xsTable1 + "' AND tableB = '" + xsTable2 + "'";
                        }
                        else
                        {
                            xsSQL += " tableA = '" + xsTable2 + "' AND tableB = '" + xsTable1 + "'";
                        }
                        DataTable xoTbl = new DataTable();
                        if (xoDB.SQL(xsSQL, xoTbl))
                        {
                            if (xoTbl.Rows.Count == 0) xiNotFoundCount++;
                        }
                        else
                        {
                            string xsErrMsg = moDB.Message;
                        }
                        xoTbl.Dispose();
                    }
                    if (xiNotFoundCount == 2)
                    {
                        string xsSQL = "INSERT INTO ForeignKeys (databaseID, tableA, fieldA, tableB, fieldB) VALUES (";
                        xsSQL += DbID.ToString();
                        xsSQL += ", '" + xsTable1 + "'";
                        xsSQL += ", '" + xsField1 + "'";
                        xsSQL += ", '" + xsTable2 + "'";
                        xsSQL += ", '" + xsField2 + "')";
                        if (xoDB.SQL(xsSQL))
                        {

                        }
                        else
                        {
                            string xsErrMsg = xoDB.Message;
                            Global.ShowMessage(xsErrMsg, "Save Foreign Key");
                        }
                    }
                }
                Global.CloseThisAppsDatabase(ref xoDB);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string xsErrMsg = "";

            if (lstShowFlds.Items.Count == 0)
            {
                xsErrMsg = "Please select at least one SHOW field.  Do this by selecting a child table, and choosing field(s) from it to show in the child window.";
            }
            else
            {
                if (txtMasterFlds.Text.Trim().Length == 0) xsErrMsg = "Please select at least one link between the parent and child.";
            }

            if (xsErrMsg.Length == 0)
            {
                SaveForeignKey();
                mbOK = true;
                this.Hide();
            }
            else
            {
                Global.ShowMessage(xsErrMsg, this.Text);
            }
        }

        public string OrderByClause()
        {
            string xsClause = txtOrderByFlds.Text.Trim();
            if (chkDescending.Checked) xsClause += " DESC";
            return (xsClause);
        }

        private void lstFlds_DoubleClick(object sender, EventArgs e)
        {
            SelectField();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            UnselectField();
            txtOrderByFlds.Text = "";
        }

        private void lstShowFlds_DoubleClick(object sender, EventArgs e)
        {
            //UnselectField();
            string xsFlds = txtOrderByFlds.Text.Trim();
            if (lstShowFlds.SelectedIndex >= 0)
            {
                string xsFld = lstShowFlds.Items[lstShowFlds.SelectedIndex].ToString().Trim();
                if (xsFld.Length > 0)
                {
                    if (xsFlds.Length > 0) xsFlds = xsFlds + ", ";
                    xsFlds += xsFld;
                    txtOrderByFlds.Text = xsFlds;
                }
            }
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            //if (lstFlds.Items.Count > 0)
            //{
                AddLink();
            //}
            //else
            //{
            //    statusLbl.Text = "Please first select a child table.";
            //}
        }

        private void btnClrOrderBy_Click(object sender, EventArgs e)
        {
            txtOrderByFlds.Text = "";
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string xsMsg = "From here, you are able to link another table via field(s) from the table you clicked from (the parent) to display related data.";

            xsMsg += "\r\n\nSelect the related table, choose the fields to show from such, link the tables via the <Add link...> button, and optionally, to order the data in the";
            xsMsg += " subsequent child window, double-click the 'show' fields to build the ORDER BY clause.  Click <OK> create a new SQL window (a child) tied to the parent.";

            Global.ShowMessage(xsMsg, this.Text);
        }
    }
}
