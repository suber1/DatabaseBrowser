using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RebusData6;

namespace RebusSQL6
{
    public partial class frmManageExtProvs : Form
    {
        private List<DatabaseProvider> moExtProvs;

        private string msExternalProvidersFile = Application.StartupPath + @"\" + Global.ExternalDbProvidersFile;

        private List<string> msIntrinsicProviderNames;

        private bool mbCurrRecDirty = false;




        public frmManageExtProvs()
        {
            InitializeComponent();
            moExtProvs = new List<DatabaseProvider>(0);
            LoadIntrinsicProviderNames();
        }

        private void LoadIntrinsicProviderNames()
        {
            DB xoDB = new DB();

            msIntrinsicProviderNames = new List<string>(0);

            for (int xii = 0; xii < xoDB.Providers.Count; xii++)
            {
                msIntrinsicProviderNames.Add(xoDB.Providers[xii].Name);
            }
        }

        private void UpdateControlsAvailability()
        {
            int xii = drpProvs.SelectedIndex;

            txtOdbcConnStr.Enabled = xii >= 0;
            txtOdbcConnStrTrusted.Enabled = xii >= 0;
            txtOleDbConnStr.Enabled = xii >= 0;
            txtOleDbConnStrTrusted.Enabled = xii >= 0;
            txtSQLConnStr.Enabled = xii >= 0;
            txtSQLConnStrTrusted.Enabled = xii >= 0;
            txtFileExt.Enabled = xii >= 0;
            btnExt.Enabled = xii >= 0;

            btnSave.Enabled = mbCurrRecDirty;

            btnRemove.Enabled = xii >= 0;
        }

        private void LoadExternalProviders()
        {
            string xsErrMsg = "";
            DB xoDB = new DB();

            if (System.IO.File.Exists(msExternalProvidersFile))
            {
                xoDB.GetExternalProviders(msExternalProvidersFile, out moExtProvs, out xsErrMsg);
            }

            if (xsErrMsg.Length == 0)
            {
                //moExtProvs.Sort();
                LoadProvidersList();
            }
            else
            {
                Global.ShowMessage(xsErrMsg, "Load Additional Providers");
            }
        }

        private void LoadProvidersList()
        {
            drpProvs.Items.Clear();

            for (int xii = 0; xii < moExtProvs.Count; xii++)
            {
                drpProvs.Items.Add(moExtProvs[xii].Name);
            }
        }

        private void LoadControlsForCurrentProvider()
        {
            int xii = drpProvs.SelectedIndex;

            if (xii < 0)
            {
                txtOdbcConnStr.Text = "";
                txtOdbcConnStrTrusted.Text = "";
                txtOleDbConnStr.Text = "";
                txtOleDbConnStrTrusted.Text = "";
                txtSQLConnStr.Text = "";
                txtSQLConnStrTrusted.Text = "";
                txtFileExt.Text = "";
            }
            else
            {
                txtOdbcConnStr.Text = moExtProvs[xii].OdbcConnStr;
                txtOdbcConnStrTrusted.Text = moExtProvs[xii].OdbcConnStrTrusted;
                txtOleDbConnStr.Text = moExtProvs[xii].OleDbConnStr;
                txtOleDbConnStrTrusted.Text = moExtProvs[xii].OleDbConnStrTrusted;
                txtSQLConnStr.Text = moExtProvs[xii].DotNetConnStr;
                txtSQLConnStrTrusted.Text = moExtProvs[xii].DotNetConnStrTrusted;
                txtFileExt.Text = moExtProvs[xii].FilterExpression;
            }
            UpdateControlsAvailability();
        }

        private void txtOdbcConnStr_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void txtOdbcConnStrTrusted_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void txtOleDbConnStr_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void txtOleDbConnStrTrusted_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void txtSQLConnStr_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void txtSQLConnStrTrusted_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void txtFileExt_TextChanged(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //int xii = drpProvs.SelectedIndex;

            DatabaseProvider xoProv = new DatabaseProvider();

            xoProv.DotNetConnStr = txtSQLConnStr.Text.Trim();
            xoProv.DotNetConnStrTrusted = txtSQLConnStrTrusted.Text.Trim();
            xoProv.OleDbConnStr = txtOleDbConnStr.Text.Trim();
            xoProv.OleDbConnStrTrusted = txtOleDbConnStrTrusted.Text.Trim();
            xoProv.OdbcConnStr = txtOleDbConnStr.Text.Trim();
            xoProv.OdbcConnStrTrusted = txtOleDbConnStrTrusted.Text.Trim();
            xoProv.FilterExpression = txtFileExt.Text.Trim();

            moExtProvs[drpProvs.SelectedIndex] = xoProv;

            SaveToFile();
        }

        private bool SaveToFile()
        {
            string xsErrMsg = "";

            StreamWriter xoWrtr = null;

            try
            {
                xoWrtr = new StreamWriter(msExternalProvidersFile);
                xoWrtr.WriteLine("<RebusDatabaseProviders>");
                for (int xii = 0; xii < moExtProvs.Count; xii++)
                {
                    xoWrtr.WriteLine("");
                    xoWrtr.WriteLine("   <Provider>");
                    xoWrtr.WriteLine("");
                    xoWrtr.WriteLine("      <Name='" + moExtProvs[xii].Name + "' />");
                    xoWrtr.WriteLine("      <OdbcConnStr='" + moExtProvs[xii].OdbcConnStr + "' />");
                    xoWrtr.WriteLine("      <OdbcConnStrTrusted='" + moExtProvs[xii].OdbcConnStrTrusted + "' />");
                    xoWrtr.WriteLine("      <OleDbConnStr='" + moExtProvs[xii].OleDbConnStr + "' />");
                    xoWrtr.WriteLine("      <OleDbConnStrTrusted='" + moExtProvs[xii].OleDbConnStrTrusted + "' />");
                    xoWrtr.WriteLine("      <DotNetConnStr='" + moExtProvs[xii].DotNetConnStr + "' />");
                    xoWrtr.WriteLine("      <DotNetConnStrTrusted='" + moExtProvs[xii].DotNetConnStrTrusted + "' />");
                    xoWrtr.WriteLine("      <FileExt='" + moExtProvs[xii].FilterExpression + "' />");
                    xoWrtr.WriteLine("");
                    xoWrtr.WriteLine("   </Provider>");
                }
                xoWrtr.WriteLine("");
                xoWrtr.WriteLine("</RebusDatabaseProviders>");
                mbCurrRecDirty = false;
            }
            catch (Exception xoExc)
            {
                xsErrMsg = xoExc.Message;
            }
            if (xoWrtr != null)
            {
                xoWrtr.Close();
                xoWrtr.Dispose();
            }

            if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "Save Additional Providers");

            UpdateControlsAvailability();

            return (xsErrMsg.Length == 0);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string xsNewName = "", xsErrMsg = "";
            bool xbContinue = true;

            if (mbCurrRecDirty)
            {
                DialogResult xeYesNoCancel = Global.ShowMessage("Save current changes?", "Add New Provider", MessageBoxButtons.YesNoCancel);
                switch (xeYesNoCancel)
                {
                    case System.Windows.Forms.DialogResult.Yes:
                        if (!SaveToFile()) xbContinue = false;
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        break;
                    default:
                        xbContinue = false;
                        break;
                }
            }

            if (xbContinue)
            {
                DialogResult xe = Global.InputBox("Add New Provider", "Enter a unique name for the new provider: ", ref xsNewName);
                if (xe == System.Windows.Forms.DialogResult.OK)
                {
                    xsNewName = xsNewName.Trim();
                    if (xsNewName.Length > 0)
                    {
                        for (int xii = 0; xii < moExtProvs.Count; xii++)
                        {
                            if (xsNewName.ToUpper() == moExtProvs[xii].Name.Trim().ToUpper())
                            {
                                xsErrMsg = "Name is NOT unique.";
                                break;
                            }
                        }
                        if (xsErrMsg.Length == 0)
                        {
                            for (int xii = 0; xii < msIntrinsicProviderNames.Count; xii++)
                            {
                                if (xsNewName.ToUpper() == msIntrinsicProviderNames[xii].Trim().ToUpper())
                                {
                                    xsErrMsg = "Name is NOT unique (intrinsic).";
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        xsErrMsg = "Name cannot be blank.";
                    }
                    if (xsErrMsg.Length == 0)
                    {
                        DatabaseProvider xoProv = new DatabaseProvider();
                        xoProv.Name = xsNewName;
                        xoProv.OdbcConnStr = "";
                        xoProv.OdbcConnStrTrusted = "";
                        xoProv.OleDbConnStr = "";
                        xoProv.OleDbConnStrTrusted = "";
                        xoProv.DotNetConnStr = "";
                        xoProv.DotNetConnStrTrusted = "";
                        xoProv.DotNetConnStrTrusted = "";
                        xoProv.FilterExpression = "";
                        moExtProvs.Add(xoProv);
                        if (SaveToFile())
                        {
                            LoadProvidersList();
                            drpProvs.SelectedIndex = drpProvs.Items.Count - 1;
                            LoadControlsForCurrentProvider();
                            UpdateControlsAvailability();
                            txtOdbcConnStr.Focus();
                        }
                        else
                        {
                            moExtProvs.Remove(xoProv);
                        }
                    }
                }

                if (xsErrMsg.Length > 0) Global.ShowMessage(xsErrMsg, "Add New Provider");
            }
        }

        private void drpProvs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mbCurrRecDirty)
            {
                if (Global.ShowMessage("Save current changes?", "Save Changes", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveToFile();
                }
            }
            LoadControlsForCurrentProvider();
            //up
        }

        private void frmManageExtProvs_Load(object sender, EventArgs e)
        {
            LoadExternalProviders();
        }

        private void frmManageExtProvs_Shown(object sender, EventArgs e)
        {
            UpdateControlsAvailability();
        }

        private void btnExt_Click(object sender, EventArgs e)
        {
            //"Access files (*.mdb)|*.mdb|All files (*.*)|*.*";
            string xsDesc = "", xsExt = "";

            if (Global.InputBox("File Extension", "Enter a BRIEF description of type file: ", ref xsDesc) == System.Windows.Forms.DialogResult.OK)
            {
                if (Global.InputBox("File Extension", "Enter the file extension (no period): ", ref xsExt) == System.Windows.Forms.DialogResult.OK)
                {
                    txtFileExt.Text = xsDesc.Trim() + " files (*." + xsExt + ")|*." + xsExt + "|All files (*.*)|*.*";
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (Global.ShowMessage("Remove this provider?", "Remove Provider", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                int xii = drpProvs.SelectedIndex;
                moExtProvs.RemoveAt(xii);

                if (SaveToFile())
                {
                    LoadProvidersList();
                    if (moExtProvs.Count > 0)
                    {
                        xii = xii - 1;
                        if (xii + 1 > moExtProvs.Count) xii = xii - 1;
                        drpProvs.SelectedIndex = xii;
                        LoadControlsForCurrentProvider();
                        //UpdateControlsAvailability();
                        txtOdbcConnStr.Focus();
                    }
                }
                UpdateControlsAvailability();
            }
        }

    }
}
