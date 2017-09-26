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
    public partial class frmChooseDB : Form
    {

        private List<LoadedDb> mrLoadedDbs;
        private bool mbOK = false;
        private long miSelectedDbID = 0;
        private bool mbDbsRemoved = false;
        private bool mbDbListChanged = false;



        public frmChooseDB()
        {
            InitializeComponent();
            mrLoadedDbs = new List<LoadedDb>(0);
            mbOK = false;
            miSelectedDbID = 0;
            mbDbsRemoved = false;
        }




        public bool OK()
        {
            return (mbOK);
        }

        public long SelectedDbID()
        {
            return (miSelectedDbID);
        }

        public bool DbsRemoved()
        {
            return (mbDbsRemoved);
        }

        public bool DbListChanged()
        {
            return (mbDbListChanged);
        }

        private void UpdateControlsAvailability()
        {
            btnOpen.Enabled = lstDBs.SelectedIndex >= 0;
            btnRemove.Enabled = lstDBs.SelectedIndex >= 0;
            btnChgDesc.Enabled = lstDBs.SelectedIndex >= 0;
        }

        public void LoadDBs(List<LoadedDb> prDBs)
        {
            //mrLoadedDbs = new List<LoadedDb>(0);
            //if (prDBs != null)
            //{
            //    if (prDBs.Count > 0)
            //    {
            //        for (int xii = 0; xii < prDBs.Count; xii++)
            //        {
            //            mrLoadedDbs.Add(prDBs[xii]);
            //        }
            //    }
            //}
            LoadList();
            LoadDbList();
        }

        private void LoadDbList()
        {
            lstDBs.Items.Clear();
            if (mrLoadedDbs != null)
            {
                if (mrLoadedDbs.Count > 0)
                {
                    for (int xii = 0; xii < mrLoadedDbs.Count; xii++)
                    {
                        lstDBs.Items.Add(GetListLineItem(mrLoadedDbs[xii]));
                    }
                }
            }
        }

        private string GetListLineItem(LoadedDb poDbToOpen)
        {
            string xs = poDbToOpen.Description.Trim();
            if (xs.Length == 0) xs = poDbToOpen.ConnStrOverride.TrimEnd();

            if (xs.Length == 0) xs = poDbToOpen.Database.TrimEnd();
            xs = xs.Replace('\r', ' ');
            xs = xs.Replace('\n', ' ');

            return (xs);
        }

        private void DatabaseChosen()
        {
            if (lstDBs.SelectedIndex >= 0)
            {
                mbOK = true;
                miSelectedDbID = mrLoadedDbs[lstDBs.SelectedIndex].ID;
                this.Hide();
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            DatabaseChosen();
        }

        private void frmChooseDB_Shown(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private bool GetDbToOpen(ref DatabaseToOpen prDbToOpen, out string psErrMsg)
        {
            string xsErrMsg = "";

            if (lstDBs.SelectedIndex < 0)
            {
                for (int xii = 0; xii < mrLoadedDbs.Count; xii++)
                {
                    if (mrLoadedDbs[xii].Database.Trim().ToUpper() == prDbToOpen.Database.Trim().ToUpper())
                    {
                        lstDBs.SelectedIndex = xii;
                        break;
                    }
                }
            }

            if (lstDBs.SelectedIndex >= 0)
            {
                DB xoAppDB = new DB();
                int xiID = mrLoadedDbs[lstDBs.SelectedIndex].ID;

                if (Global.OpenThisAppsDatabase(ref xoAppDB))
                {
                    DataTable xoTbl = new DataTable();
                    xoTbl.Reset();
                    string xsSQL = "SELECT connType, providerName, source, [database], userID, connStr FROM [database] WHERE ID = " + xiID.ToString();
                    if (xoAppDB.SQL(xsSQL, xoTbl))
                    {
                        if (xoTbl.Rows.Count > 0)
                        {
                            prDbToOpen.UniqueID = xiID;

                            DatabaseProvider xrProv = new DatabaseProvider();

                            string xsProv = null2str(xoTbl.Rows[0][1].ToString()).TrimEnd().ToUpper();

                            if (xsProv.Length > 0)
                            {
                                for (int xii = 0; xii < xoAppDB.Providers.Count; xii++)
                                {
                                    if (xsProv == xoAppDB.Providers[xii].Name.TrimEnd().ToUpper())
                                    {
                                        xrProv = xoAppDB.Providers[xii];
                                        break;
                                    }
                                }
                            }
                            xrProv.ConnectionStringOverride = xoTbl.Rows[0][5].ToString().TrimEnd();
                            prDbToOpen.Provider = xrProv;

                            prDbToOpen.ConnectionType = (ConnectivityType)xoAppDB.null2int(xoTbl.Rows[0][0]);

                            prDbToOpen.DataSource = xoTbl.Rows[0][2].ToString().TrimEnd();
                            prDbToOpen.Database = xoTbl.Rows[0][3].ToString().TrimEnd();
                            prDbToOpen.UserID = xoTbl.Rows[0][4].ToString().TrimEnd();
                            //bool xbNeedPswd = false;
                            //if (xoTbl.Rows[0][5] != null) xbNeedPswd = Convert.ToBoolean(xoTbl.Rows[0][5]);
                            //prDbToOpen.NeedPassword = true;
                        }
                        else
                        {
                            xsErrMsg = "Database connection record not found.\r\n(SQL: " + xsSQL + ")  (ID: " + xiID.ToString();
                        }
                    }
                    else
                    {
                        xsErrMsg = xoAppDB.Message + "\r\n(SQL: " + xsSQL + ")";
                    }
                    ReleaseTable(ref xoTbl);
                    Global.CloseThisAppsDatabase(ref xoAppDB);
                }
                else
                {
                    xsErrMsg = xoAppDB.Message;
                }
            }

            psErrMsg = xsErrMsg;

            return (xsErrMsg.Length == 0);
        }

        private string null2str(object po)
        {
            string xs = "";

            if (po != null) xs = po.ToString();

            return (xs);
        }

        public void RemoveTheDbFromList()
        {
            const string xcsCaption = "Remove DB From List";
            string xsErrMsg = "";
            int xiOrgIdx = lstDBs.SelectedIndex;

            DatabaseToOpen xoDbToOpen = new DatabaseToOpen();
            if (GetDbToOpen(ref xoDbToOpen, out xsErrMsg))
            {
                DB xoAppDB = new DB();
                if (Global.OpenThisAppsDatabase(ref xoAppDB))
                {
                    mbDbsRemoved = true;
                    mbDbListChanged = true;
                    DataTable xoTbl = new DataTable();
                    string xsSQL = "SELECT dataSetID FROM Datasets WHERE dbID = " + xoDbToOpen.UniqueID.ToString();
                    if (xoAppDB.SQL(xsSQL, xoTbl))
                    {
                        bool xbCont = true;
                        //int xiSets = Convert.ToInt32(xoTbl.Rows[0][0]);
                        int xiSets = xoTbl.Rows.Count;
                        if (xiSets >= 1)
                        {
                            if (Global.ShowMessage("There are " + xiSets.ToString() + " dataset(s) associated to this database which will also be removed.  Would you like to continue?", xcsCaption, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                            {
                                xbCont = false;
                            }
                        }
                        if (xbCont)
                        {
                            if (xiSets > 0)
                            {
                                for (int xiSet = 1; xiSet <= xiSets; xiSet++)
                                {
                                    string xsDsID = xoTbl.Rows[xiSet - 1][0].ToString().TrimEnd();
                                    // children
                                    xsSQL = "DELETE * FROM [dataset] WHERE dataSetID = '" + xsDsID + "'";
                                    if (!xoAppDB.SQL(xsSQL))
                                    {
                                        xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                                        xbCont = false;
                                        break;
                                    }
                                    // parent
                                    xsSQL = "DELETE * FROM [datasets] WHERE dataSetID = '" + xsDsID + "'";
                                    if (!xoAppDB.SQL(xsSQL))
                                    {
                                        xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                                        xbCont = false;
                                        break;
                                    }
                                }
                            }
                            if (xbCont)
                            {
                                xsSQL = "DELETE * FROM [Database] WHERE [ID] = " + xoDbToOpen.UniqueID.ToString();
                                if (xoAppDB.SQL(xsSQL))
                                {
                                    lstDBs.Items.RemoveAt(xiOrgIdx);
                                    mrLoadedDbs.RemoveAt(xiOrgIdx);
                                    UpdateControlsAvailability();
                                }
                                else
                                {
                                    xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                                }
                            }
                        }
                    }
                    ReleaseTable(ref xoTbl);
                }
                else
                {
                    xsErrMsg = xoAppDB.Message;
                }
                Global.CloseThisAppsDatabase(ref xoAppDB);
            }
            else
            {
                xsErrMsg = "Unable to locate internal database ID.";
            }

            UpdateControlsAvailability();
            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, xcsCaption);
        }

        private void ReleaseTable(ref DataTable poTbl)
        {
            if (poTbl != null)
            {
                try
                {
                    poTbl.Dispose();
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                poTbl = null;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            string xsErrMsg = "";
            const string xcsCaption = "Remove DB From List";

            int xiOrgIdx = lstDBs.SelectedIndex;

            if (xiOrgIdx >= 0)
            {
                string xs = lstDBs.Items[lstDBs.SelectedIndex].ToString();
                if (Global.ShowMessage("Remove '" + xs + "' from the list?", xcsCaption, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    RemoveTheDbFromList();
                }
            }
            UpdateControlsAvailability();
            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, xcsCaption);
        }

        private void lstDBs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCurrInfo();
            UpdateControlsAvailability();
        }

        private void ShowCurrInfo()
        {
            if (lstDBs.SelectedIndex >= 0)
            {
                string xsDS = mrLoadedDbs[lstDBs.SelectedIndex].DataSource.Trim();
                if (xsDS.Length > 0) xsDS = "[" + xsDS + "] ";
                lblDB.Text = xsDS + mrLoadedDbs[lstDBs.SelectedIndex].Database.Trim() + "  (ID: " + mrLoadedDbs[lstDBs.SelectedIndex].ID.ToString().Trim() + ")";

                string xs = mrLoadedDbs[lstDBs.SelectedIndex].ConnStrOverride;
                xs = xs.Replace('\r', ' ');
                xs = xs.Replace('\n', ' ');
                lblConnStr.Text = xs;
            }
            else
            {
                lblDB.Text = "";
                lblConnStr.Text = "";
            }
        }

        private void lstDBs_DoubleClick(object sender, EventArgs e)
        {
            DatabaseChosen();
        }

        private void frmChooseDB_Load(object sender, EventArgs e)
        {
            lblConnStr.Text = "";
            lblDB.Text = "";
        }

        private void btnChgDesc_Click(object sender, EventArgs e)
        {
            string xsErrMsg = "";
            if (lstDBs.SelectedIndex >= 0)
            {
                string xsDesc = mrLoadedDbs[lstDBs.SelectedIndex].Description.Trim();
                if (Global.InputBox("Change Description", "Enter new description:", ref xsDesc) == System.Windows.Forms.DialogResult.OK)
                {
                    xsDesc = xsDesc.Replace("'", "").Trim();
                    DB xoAppDB = new DB();
                    if (Global.OpenThisAppsDatabase(ref xoAppDB))
                    {
                        int xiID = mrLoadedDbs[lstDBs.SelectedIndex].ID;
                        string xsSQL = "UPDATE [Database] SET [description] = '" + xsDesc + "' WHERE [ID] = " + xiID.ToString();
                        if (xoAppDB.SQL(xsSQL))
                        {
                            mbDbListChanged = true;
                            xoAppDB.CloseDatabase();
                            int xii = lstDBs.SelectedIndex;
                            LoadDBs(mrLoadedDbs);
                            if (xii >= 0 && xii < lstDBs.Items.Count) lstDBs.SelectedIndex = xii;
                        }
                        else
                        {
                            xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                            xoAppDB.CloseDatabase();
                        }
                        xoAppDB.CloseDatabase();
                    }
                    else
                    {
                        xsErrMsg = xoAppDB.Message;
                    }
                }
            }
            if (xsErrMsg.Length > 0)
            {
                Global.ShowMessage(xsErrMsg, "Change Description");
            }
        }

        private void LoadList()
        {
            string xsErrMsg = "";

            DB xoAppDB = new DB();

            if (Global.OpenThisAppsDatabase(ref xoAppDB))
            {
                //string xsSQL = "SELECT [ID], [database], [description], connStr, [source] FROM [database] ORDER BY [description]";
                string xsSQL = "SELECT [ID], [database], [description], connStr, [source] FROM [database] ORDER BY IIF(ISNULL([description]) OR LEN(TRIM([description])) = 0, IIF(ISNULL([database]) OR LEN(TRIM([database])) = 0, [connStr], [database]), [description])";
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    mrLoadedDbs = new List<LoadedDb>(0);
                    if (xoTbl.Rows.Count > 0)
                    {
                        for (int xii = 0; xii < xoTbl.Rows.Count; xii++)
                        {
                            LoadedDb xoDB = new LoadedDb();
                            xoDB.ID = Convert.ToInt16(xoTbl.Rows[xii][0]);
                            xoDB.Description = null2str(xoTbl.Rows[xii][2]);
                            xoDB.Database = null2str(xoTbl.Rows[xii][1]);
                            xoDB.ConnStrOverride = null2str(xoTbl.Rows[xii][3]);
                            xoDB.DataSource = null2str(xoTbl.Rows[xii][4]);
                            mrLoadedDbs.Add(xoDB);
                        }
                    }
                }
                else
                {
                    xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                }
                ReleaseTable(ref xoTbl);
            }

            if (xsErrMsg.Length > 0)
            {
                Global.ShowMessage(xsErrMsg, "Load Database List");
            }
        }

        private void lblDB_DoubleClick(object sender, EventArgs e)
        {
            Global.ShowMessage(lblDB.Text);
        }

    }
}
