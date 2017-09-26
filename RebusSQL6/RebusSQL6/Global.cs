using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RebusData6;

namespace RebusSQL6
{
    public static class Global
    {
        public const string ThisAppsDatabaseFile = "RebusSQL6.mdb";
        public const string ExternalDbProvidersFile = "RebusDatabaseProviders.txt";
        public const string EmbeddedSingleQt = "(~)";

        //public string ThisAppsDatabaseConnStr = "";
        //public bool SecondaryConnTried = false;
        public static bool ThisAppsDatabaseUseODBC = false;

        public static List<DB> goDB;

        public static string gsAppDb = "";

        public static char gcc251 = Convert.ToChar(251);
        public static char gcc252 = Convert.ToChar(252);
        public static char gccQT = Convert.ToChar(34);

        public static int SelectTop = 1000;

        public static string NullToString(object po)
        {
            string xs = "";
            try
            {
                if (po != null)
                {
                    xs = po.ToString();
                }
            }
            catch { }
            return (xs);
        }

        public static bool RetrieveColumnWidths(List<string> psCaption, out List<int> piWidth, out string psErrMsg)
        {
            string xsSection = "ColumnSize";
            string xsErrMsg = "";
            string xsValue = "";
            List<int> xiWidth = new List<int>();


            DB xoAppDB = new DB();
            if (OpenThisAppsDatabase(ref xoAppDB))
            {
                DataTable xoTbl = new DataTable();
                for (int xiCol = 0; xiCol < psCaption.Count; xiCol ++)
                {
                    int xiWdt = 0;
                    string xsEntry = psCaption[xiCol];
                    string xsUser = Environment.UserName;
                    string xsWhere = "WHERE [UserInfo] = '" + xsUser + "' AND [Section] = '" + xsSection + "' AND [Entry] = '" + xsEntry + "'";
                    string xsSQL = "SELECT [value] FROM [Customs] " + xsWhere;
                
                    if (xoAppDB.SQL(xsSQL, xoTbl))
                    {
                        if (xoTbl.Rows.Count > 0)
                        {
                            xsValue = xoTbl.Rows[0][0].ToString().TrimEnd();
                            try
                            {
                                xiWdt = Convert.ToInt32(xsValue);
                            }
                            catch
                            {
                                xiWdt = 0;
                            }
                        }
                    }
                    else
                    {
                        xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                    }
                    xiWidth.Add(xiWdt);
                }
                try
                {
                    if (xoTbl != null)
                    {
                        xoTbl.Dispose();
                    }
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                xoTbl = null;
            }
            else
            {
                xsErrMsg = xoAppDB.Message;
            }
            CloseThisAppsDatabase(ref xoAppDB);

            piWidth = xiWidth;
            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        public static bool StoreColumnWidth(string psCaption, int piWidth, out string psErrMsg)
        {
            string xsSection = "ColumnSize";
            string xsEntry = psCaption;
            string xsValue = piWidth.ToString();
            string xsErrMsg = "";

            DB xoAppDB = new DB();
            if (OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsUser = Environment.UserName;
                string xsWhere = "WHERE [UserInfo] = '" + xsUser + "' AND [Section] = '" + xsSection + "' AND [Entry] = '" + xsEntry + "'";
                string xsSQL = "SELECT [value] FROM [Customs] " + xsWhere;
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        xsSQL = "UPDATE [Customs] SET [Value] = '" + xsValue + "' " + xsWhere;
                    }
                    else
                    {
                        xsSQL = "INSERT INTO [Customs] ([UserInfo], [Section], [Entry], [Value]) VALUES (";
                        xsSQL = xsSQL + "'" + xsUser + "', ";
                        xsSQL = xsSQL + "'" + xsSection + "', ";
                        xsSQL = xsSQL + "'" + xsEntry + "', ";
                        xsSQL = xsSQL + "'" + xsValue + "')";
                    }
                    if (!xoAppDB.SQL(xsSQL)) xsErrMsg = xoAppDB.Message;
                }
                else
                {
                    xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                }
                try
                {
                    if (xoTbl != null)
                    {
                        xoTbl.Dispose();
                    }
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                xoTbl = null;
            }
            else
            {
                xsErrMsg = xoAppDB.Message;
            }

            CloseThisAppsDatabase(ref xoAppDB);

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);

        }

        public static bool StoreSetting(string psSection, string psEntry, string psValue, out string psErrMsg)
        {
            string xsErrMsg = "";

            DB xoAppDB = new DB();
            if (OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsUser = Environment.UserName;
                string xsWhere = "WHERE [UserInfo] = '" + xsUser + "' AND [Section] = '" + psSection + "' AND [Entry] = '" + psEntry + "'";
                string xsSQL = "SELECT [value] FROM [Customs] " + xsWhere;
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        xsSQL = "UPDATE [Customs] SET [Value] = '" + psValue + "' " + xsWhere;
                    }
                    else
                    {
                        xsSQL = "INSERT INTO [Customs] ([UserInfo], [Section], [Entry], [Value]) VALUES (";
                        xsSQL = xsSQL + "'" + xsUser + "', ";
                        xsSQL = xsSQL + "'" + psSection + "', ";
                        xsSQL = xsSQL + "'" + psEntry + "', ";
                        xsSQL = xsSQL + "'" + psValue + "')";
                    }
                    if (!xoAppDB.SQL(xsSQL)) xsErrMsg = xoAppDB.Message;
                }
                else
                {
                    xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                }
                try
                {
                    if (xoTbl != null)
                    {
                        xoTbl.Dispose();
                    }
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                xoTbl = null;
            }
            else
            {
                xsErrMsg = xoAppDB.Message;
            }
            CloseThisAppsDatabase(ref xoAppDB);

            psErrMsg = xsErrMsg;
            return (xsErrMsg.Length == 0);
        }

        public static bool RetrieveSetting(string psSection, string psEntry, out string psValue, out string psErrMsg)
        {
            string xsErrMsg = "";
            string xsValue = "";

            DB xoAppDB = new DB();
            if (OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsUser = Environment.UserName;
                string xsWhere = "WHERE [UserInfo] = '" + xsUser + "' AND [Section] = '" + psSection + "' AND [Entry] = '" + psEntry + "'";
                string xsSQL = "SELECT [value] FROM [Customs] " + xsWhere;
                DataTable xoTbl = new DataTable();
                if (xoAppDB.SQL(xsSQL, xoTbl))
                {
                    if (xoTbl.Rows.Count > 0)
                    {
                        xsValue = xoTbl.Rows[0][0].ToString().TrimEnd();
                    }
                }
                else
                {
                    xsErrMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                }
                try
                {
                    if (xoTbl != null)
                    {
                        xoTbl.Dispose();
                    }
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                xoTbl = null;
            }
            else
            {
                xsErrMsg = xoAppDB.Message;
            }
            CloseThisAppsDatabase(ref xoAppDB);

            psErrMsg = xsErrMsg;
            psValue = xsValue;
            return (xsErrMsg.Length == 0);
        }

        public static DialogResult ShowMessage(string psMsg, string psTitle = "", MessageBoxButtons poBtns = MessageBoxButtons.OK)
        {
            DialogResult xeDialogResult;
            string xsTitle = psTitle.Trim();

            if (xsTitle.Length == 0) xsTitle = Application.ProductName;

            //xeDialogResult = MessageBox.Show(psMsg, xsTitle, poBtns);
            xeDialogResult = MsgDialog.Show(psMsg, xsTitle, poBtns);

            return (xeDialogResult);
        }

        public static int ThisAppsDatabaseProviderIdx()
        {
            int xiIdx = -1;
            DB xoDB = new DB();

            for (int xii = 0; xii < xoDB.Providers.Count; xii++)
            {

            }

            return (xiIdx);
        }

        public static bool OpenThisAppsDatabase(ref DB poDB)
        {
            string xsErrMsg = "";
            poDB = new DB();

            DatabaseToOpen xoDbToOpen = new DatabaseToOpen();
            xoDbToOpen.Provider = poDB.Providers[0];
            if (ThisAppsDatabaseUseODBC)
            {
                xoDbToOpen.ConnectionType = ConnectivityType.ODBC;
            }
            else
            {
                xoDbToOpen.ConnectionType = ConnectivityType.OleDB;
                // test-force fail, to go to ODBC attempt
                //xoDbToOpen.ConnectionType = ConnectivityType.DotNet;
            }
            xoDbToOpen.Database = Global.gsAppDb;

            if (poDB.OpenDatabase(xoDbToOpen))
            {

            }
            else
            {
                xsErrMsg = poDB.Message;
                xoDbToOpen.ConnectionType = ConnectivityType.ODBC;
                if (poDB.OpenDatabase(xoDbToOpen))
                {
                    // Jet 4.0 OLE not present, but we can use ODBC instead
                    xsErrMsg = "";
                    ThisAppsDatabaseUseODBC = true;
                }
            }

            return (xsErrMsg.Length == 0);
        }

        public static void CloseThisAppsDatabase(ref DB poDB)
        {
            if (poDB != null)
            {
                try
                {
                    poDB.CloseDatabase();
                }
                catch (Exception xoExc)
                {
                    string xs = xoExc.Message;
                }
                poDB = null;
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        public static string iifs(bool pb, string ps1, string ps2)
        {
            string xsRet = ps2;
            if (pb) xsRet = ps1;
            return (xsRet);
        }

        public static void DropHistory(int piHistType, string psHistValue)
        {
            DB xoAppDB = new DB();
            if (Global.OpenThisAppsDatabase(ref xoAppDB))
            {
                string xsMsg = "";
                string xsSQL = "DELETE * FROM [History] WHERE histType = " + piHistType.ToString() + " AND histValue = '" + psHistValue + "'";
                if (xoAppDB.SQL(xsSQL))
                {
                    if (xoAppDB.LastNumberOfRecordsActioned > 0)
                    {
                        xsMsg = "\"" + psHistValue + "\" removed from history.";
                    }
                    else
                    {
                        xsMsg = "No history found.  (SQL: " + xsSQL + ")";
                    }
                }
                else
                {
                    xsMsg = xoAppDB.Message + "  (SQL: " + xsSQL + ")";
                }
                ShowMessage(xsMsg, "Drop History");
            }
            xoAppDB.CloseDatabase();
            xoAppDB = null;
        }

    }           // end class Global

    public static partial class MsgDialog
    {
        //public MsgDialog()
        //{

        //}

        public static DialogResult Show(string psMsg, string psCaption = "", MessageBoxButtons piBtns = MessageBoxButtons.OK, MessageBoxIcon piIcon = MessageBoxIcon.None)
        {
            DialogResult xiResult;

            const int xciCaptionBarHeight = 38;
            const int xciBtnWidth = 70;
            const int xciBtnHeight = 28;
            const int xciBtnsBetween = 12;
            const int xciBtnsRightMargin = 12;
            const int xciBtnsBottomMargin = 8;
            const int xciTextHorzMargin = 12;
            const int xciTextVertMargin = 12;
            const int xciMinDlgWidth = 320;
            const int xciMinDlgHeight = 200;
            const int xciIconMargin = 10;

            int xiDlgWidth = xciMinDlgWidth;
            int xiDlgHeight = xciMinDlgHeight;
            int xiCancBtn = 0;
            int xiIconWidth = 0;
            int xiForIcon = 0;
            bool xbAddVertScrollBar = false;
            bool xbWidthOK = false;

            List<string> xsBtns = new List<string>(0);

            try
            {
                PictureBox xoPic = null;
                Bitmap xoBmp = null;

                if (piIcon != MessageBoxIcon.None)
                {

                    if (piIcon == MessageBoxIcon.Information)
                    {
                        xoBmp = SystemIcons.Information.ToBitmap();
                    }
                    else
                    {
                        if (piIcon == MessageBoxIcon.Question)
                        {
                            xoBmp = SystemIcons.Question.ToBitmap();
                        }
                        else
                        {
                            if (piIcon == MessageBoxIcon.Exclamation)
                            {
                                xoBmp = SystemIcons.Exclamation.ToBitmap();
                            }
                            else
                            {
                                if (piIcon == MessageBoxIcon.Error)
                                {
                                    xoBmp = SystemIcons.Error.ToBitmap();
                                }
                                else
                                {
                                    if (piIcon == MessageBoxIcon.Warning)
                                    {
                                        xoBmp = SystemIcons.Warning.ToBitmap();
                                    }
                                    else
                                    {
                                        if (piIcon == MessageBoxIcon.Stop || piIcon == MessageBoxIcon.Hand)
                                        {
                                            xoBmp = SystemIcons.Hand.ToBitmap();
                                        }
                                        else
                                        {
                                            if (piIcon == MessageBoxIcon.Asterisk)
                                            {
                                                xoBmp = SystemIcons.Asterisk.ToBitmap();
                                            }
                                            else
                                            {
                                                xoBmp = SystemIcons.Information.ToBitmap();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    xoPic = new PictureBox();
                    xoPic.Image = xoBmp;
                    xiIconWidth = xoPic.Image.Width;
                    xiForIcon = xiIconWidth + xciIconMargin;
                }

                switch (piBtns)
                {
                    case MessageBoxButtons.OK:
                        xsBtns.Add("OK");
                        break;
                    case MessageBoxButtons.YesNo:
                        xsBtns.Add("Yes");
                        xsBtns.Add("No");
                        xiCancBtn = 1;
                        break;
                    case MessageBoxButtons.OKCancel:
                        xsBtns.Add("OK");
                        xsBtns.Add("Cancel");
                        xiCancBtn = 1;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        xsBtns.Add("Yes");
                        xsBtns.Add("No");
                        xsBtns.Add("Cancel");
                        xiCancBtn = 2;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        xsBtns.Add("Retry");
                        xsBtns.Add("Cancel");
                        xiCancBtn = 1;
                        break;
                    case MessageBoxButtons.AbortRetryIgnore:
                        xsBtns.Add("Abort");
                        xsBtns.Add("Retry");
                        xsBtns.Add("Ignore");
                        xiCancBtn = 2;
                        break;
                    default:
                        break;
                }

                Form xoDlg = new Form();
                xoDlg.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                xoDlg.StartPosition = FormStartPosition.CenterParent;
                xoDlg.MinimizeBox = false;
                xoDlg.MaximizeBox = false;
                xoDlg.ShowInTaskbar = false;
                xoDlg.Text = psCaption;

                // message to be shown in a read-only in a text box (so that it is scrollable, if necessary, and copyable)
                TextBox xoTxt = new TextBox();
                xoTxt.ReadOnly = true;
                xoTxt.Multiline = true;
                xoTxt.TabStop = false;
                xoTxt.Text = psMsg;
                xoTxt.BorderStyle = BorderStyle.None;
                xoTxt.WordWrap = true;

                if (psMsg.Length > 0)
                {
                    while (true)
                    {
                        xoTxt.Width = xiDlgWidth - (xciTextHorzMargin * 2) - 16 - xiForIcon;
                        xoTxt.Height = xiDlgHeight - xciCaptionBarHeight - (xciTextVertMargin * 2) - xciBtnHeight - xciBtnsBottomMargin;

                        Rectangle xrRect = Screen.PrimaryScreen.WorkingArea;
                        if (xiDlgWidth >= xrRect.Width * 50 / 100 && xiDlgHeight >= xrRect.Height * 75 / 100)           // we'll max out at 50% of screen width and 75% of screen height, and let the text box vertical scroll bar take it from there
                        {
                            xbAddVertScrollBar = true;
                            break;
                        }
                        else
                        {
                            // see if we need to expand the size of the dialog box based on the length of the text
                            Point xiLast = xoTxt.GetPositionFromCharIndex(xoTxt.Text.Length - 1);
                            SizeF xzLast;

                            using (Graphics xoGr = Graphics.FromHwnd(xoTxt.Handle))
                            {
                                xzLast = xoGr.MeasureString(psMsg, xoTxt.Font);
                            }

                            if (xiLast.X + (int)xzLast.Width < xoTxt.ClientSize.Width) xbWidthOK = true;
                            if (xiLast.Y >= xoTxt.ClientSize.Height)
                            {
                                // expand in 5% increments until it fits (or we max out above)
                                bool xbExpandWidth = true;
                                if (xbWidthOK || xiDlgWidth >= xrRect.Width * 50 / 100) xbExpandWidth = false;
                                if (xbExpandWidth || xiDlgHeight < xrRect.Height * 75 / 100)
                                {
                                    if (xbExpandWidth)
                                    {
                                        if (xiDlgWidth < xrRect.Width * 50 / 100) xiDlgWidth = xiDlgWidth + (xrRect.Width * 5 / 100);
                                    }
                                    if (xiDlgHeight < xrRect.Height * 75 / 100) xiDlgHeight = xiDlgHeight + (xrRect.Height * 5 / 100);
                                }
                                else
                                {
                                    xbAddVertScrollBar = true;
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                }

                // now we can set the dialog size
                xoDlg.Width = xiDlgWidth;
                xoDlg.Height = xiDlgHeight;

                // and position the text box within it
                xoTxt.Left = xciTextHorzMargin + xiForIcon;
                xoTxt.Top = xciTextVertMargin;
                if (xbAddVertScrollBar) xoTxt.ScrollBars = ScrollBars.Vertical;

                // and place it on the dialog (always 1st...see AcceptButton/CancelButton where referencing subsequent controls)
                xoDlg.Controls.Add(xoTxt);

                // button(s)
                int xiX = xoDlg.Width - xciBtnsRightMargin - (xsBtns.Count * (xciBtnsBetween + xciBtnWidth));
                Button xoBtn = null;
                for (int xiBtn = 0; xiBtn < xsBtns.Count; xiBtn++)
                {
                    xoBtn = new Button();
                    xoBtn.Text = xsBtns[xiBtn];
                    xoBtn.Width = xciBtnWidth;
                    xoBtn.Height = xciBtnHeight;
                    xoBtn.Left = xiX;    // xoDlg.Width - xoBtn.Width - 12 - 8;
                    xoBtn.Top = xoDlg.Height - xoBtn.Height - xciBtnsBottomMargin - xciCaptionBarHeight;
                    xoBtn.Click += new System.EventHandler(ShowMessage_Button_Click);
                    xoDlg.Controls.Add(xoBtn);
                    xiX = xiX + xoBtn.Width + xciBtnsBetween;
                }
                

                // place the icon, if specified (do as the LAST control...because of .Controls[] references below)
                if (xiIconWidth > 0)
                {
                    xoPic.Left = xciIconMargin;
                    xoPic.Top = (xoTxt.Height / 2) + xoTxt.Top - (xoPic.Height / 2);
                    xoDlg.Controls.Add(xoPic);
                }

                // show
                xoDlg.AcceptButton = (Button)xoDlg.Controls[1];
                xoDlg.CancelButton = (Button)xoDlg.Controls[xiCancBtn + 1];
                //throw new Exception("Testing the fail safe...");
                xoDlg.ShowDialog();
                xiResult = xoDlg.DialogResult;

                // cleanup
                xoBtn.Dispose();
                xoTxt.Dispose();
                xoTxt = null;
                if (xoPic != null)
                {
                    xoPic.Dispose();
                    xoPic = null;
                }
                if (xoBmp != null)
                {
                    xoBmp.Dispose();
                    xoBmp = null;
                }

                //MessageBox.Show(xoDlg.DialogResult.ToString());
                xoDlg.Close();
                xoDlg.Dispose();
                xoDlg = null;
            }
            catch (Exception xoExc)
            {
                // fail safe
                xiResult = MessageBox.Show(psMsg, psCaption, piBtns);
                MessageBox.Show(xoExc.Message, "ShowMessage Internal Error");
            }

            return (xiResult);
        }

        private static void ShowMessage_Button_Click(object sender, EventArgs e)
        {
            Button xoBtn = (Button)sender;
            Form xoDlg = (Form)xoBtn.Parent;

            DialogResult xiResult;
            string xsBtn = xoBtn.Text.ToUpper();
            switch (xsBtn)
            {
                case "OK":
                    xiResult = System.Windows.Forms.DialogResult.OK;
                    break;
                case "YES":
                    xiResult = System.Windows.Forms.DialogResult.Yes;
                    break;
                case "NO":
                    xiResult = System.Windows.Forms.DialogResult.No;
                    break;
                case "CANCEL":
                    xiResult = System.Windows.Forms.DialogResult.Cancel;
                    break;
                case "NONE":
                    xiResult = System.Windows.Forms.DialogResult.None;
                    break;
                case "ABORT":
                    xiResult = System.Windows.Forms.DialogResult.Abort;
                    break;
                case "RETRY":
                    xiResult = System.Windows.Forms.DialogResult.Retry;
                    break;
                case "IGNORE":
                    xiResult = System.Windows.Forms.DialogResult.Ignore;
                    break;
                default:
                    xiResult = System.Windows.Forms.DialogResult.OK;
                    break;
            }

            // store the result
            xoDlg.DialogResult = xiResult;
            xoDlg.Hide();

            // cleanup
            xoDlg = null;
            xoBtn = null;
        }
    }


}               // end namespace
