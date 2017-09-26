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
    public partial class dlgChooseSQL : Form
    {
        private struct SQLset
        {
            public string Caption;
            public string SQL;
            public string Description;
        }

        private List<SQLset> mrSQLSets;

        public dlgChooseSQL()
        {
            InitializeComponent();
            mrSQLSets = new List<SQLset>(0);
        }

        private void GetSQLsets()
        {
            mrSQLSets = new List<SQLset>(0);
            txtSQL.Text = "";
            lblDesc.Text = "";

            if (lstSQL.SelectedIndex >= 0)
            {
                DB xoAppDB = new DB();
                if (Global.OpenThisAppsDatabase(ref xoAppDB))
                {
                    string xsInfo = lstSQLinfo.Items[lstSQL.SelectedIndex].ToString();
                    List<string> xsItems = xsInfo.Split(',').ToList<string>();
                    string xsSQL_id = xsItems[0];
                    string xsSQL = "SELECT sqltext, caption, sqldescription FROM Dataset WHERE datasetID = '" + xsSQL_id + "'";
                    DataTable xoTbl = new DataTable();
                    if (xoAppDB.SQL(xsSQL, xoTbl))
                    {
                        if (xoTbl.Rows.Count > 0)
                        {
                            SQLset xrSet = new SQLset();
                            for (int xii = 0; xii < xoTbl.Rows.Count; xii++)
                            {
                                xrSet.Caption = Global.NullToString(xoTbl.Rows[xii][1]).Trim();
                                xrSet.Description = Global.NullToString(xoTbl.Rows[xii][2]).Trim();
                                xrSet.SQL = Global.NullToString(xoTbl.Rows[xii][0]).Trim();
                                mrSQLSets.Add(xrSet);
                            }
                        }
                    }
                    xoTbl.Dispose();
                    xoTbl = null;
                    xoAppDB.CloseDatabase();
                }
                xoAppDB = null;
            }

            lstInfo.Items.Clear();

            for (int xii = 0; xii < mrSQLSets.Count; xii++)
            {
                lstInfo.Items.Add(mrSQLSets[xii].Caption);
            }
        }

        private void SetControlAvails()
        {
            btnOK.Enabled = lstSQL.SelectedIndex >= 0;
            btnDel.Enabled = lstSQL.SelectedIndex >= 0;
        }

        private void SetChosen()
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void DeleteSelectedSQL()
        {
            string xsSQL = "", xsInfo = "", xsSQL_id = "";  //, xsCrLf;

            DB xoAppDB = new DB();

            if (lstSQL.SelectedIndex >= 0)
            {
                if (Global.ShowMessage("Delete '" + lstSQL.Items[lstSQL.SelectedIndex].ToString() + "'\r\n\r\nAre you sure?", "Delete Saved SQL", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    if (Global.OpenThisAppsDatabase(ref xoAppDB))
                    {
                        xsInfo = lstSQLinfo.Items[lstSQL.SelectedIndex].ToString();

                        List<string> xsItems = xsInfo.Split(',').ToList<string>();

                        xsSQL_id = xsItems[0];
                        xsSQL = "DELETE * FROM Dataset WHERE dataSetID = '" + xsSQL_id + "'";           // delete the child record(s) first
                        if (xoAppDB.SQL(xsSQL))
                        {
                            xsSQL = "DELETE * FROM Datasets WHERE dataSetID = '" + xsSQL_id + "'";      // delete the master record
                            if (xoAppDB.SQL(xsSQL))
                            {

                            }
                            else
                            {
                                Global.ShowMessage("Delete Saved SQL", "Unable to delete SQL windows description record.\r\r" + xoAppDB.Message + "\r\r" + xsSQL);
                            }
                            // finally, remove the item from each list box
                            lstSQLinfo.SelectedIndex = lstSQL.SelectedIndex;
                            lstSQL.Items.RemoveAt(lstSQL.SelectedIndex);
                            lstSQLinfo.Items.RemoveAt(lstSQLinfo.SelectedIndex);
                        }
                        else
                        {
                            Global.ShowMessage("Unable to delete SQL windows record(s).\r\r" + xoAppDB.Message + "\r\r" + xsSQL, "Delete Saved SQL");
                        }
                        Global.CloseThisAppsDatabase(ref xoAppDB);
                    }
                    else
                    {
                        Global.ShowMessage("Unable to open saved SQL database.\r\r" + xoAppDB.Message, "Delete Saved SQL");
                    }
                }
            }
            xoAppDB = null;
        }

        private void dlgChooseSQL_Shown(object sender, EventArgs e)
        {
            lblDesc.Text = "";
            SetControlAvails();
        }

        private void lstSQL_Click(object sender, EventArgs e)
        {
            SetControlAvails();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            DeleteSelectedSQL();
        }

        private void lstSQL_DoubleClick(object sender, EventArgs e)
        {
            SetChosen();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetChosen();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void lstSQL_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetSQLsets();
        }

        private void ShowCurrDataset()
        {
            if (lstInfo.SelectedIndex >= 0)
            {
                txtSQL.Text = mrSQLSets[lstInfo.SelectedIndex].SQL;
                lblDesc.Text = mrSQLSets[lstInfo.SelectedIndex].Description;
            }
            else
            {
                txtSQL.Text = "";
                lblDesc.Text = "";
            }
        }

        private void lstInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCurrDataset();
        }

    }
}
