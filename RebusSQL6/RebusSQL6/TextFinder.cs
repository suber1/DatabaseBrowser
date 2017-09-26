using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RebusTextFinder
{
    public partial class TextFinder
    {
        private const string mcsNoMatchMsg = " not found.";

        private bool DialogActive { get; set; }

        private string msMessage;

        private RichTextBox moLinkedTxtBox = null;

        private List<int> miIndices { get; set; }

        private bool MatchCase { get; set; }
        private bool NeedsIndexing { get; set; }

        private string FindText { get; set; }

        private int miCurrentIndex = -1;

        private string msLastTextIndexed = "";
        private string msLastFindText = "";
        private bool mbLastMatchCase = false;

        private Form moFindDlgInternal;



        //
        // constructor
        //
        public TextFinder()
        {
            miIndices = new List<int>(0);
            msMessage = "";
            DialogActive = false;
            MatchCase = false;
            NeedsIndexing = true;
            FindText = "";
            mbLastMatchCase = MatchCase;
            moFindDlgInternal = null;
        }

        //
        // destructor
        //
        ~TextFinder()
        {
            if (moLinkedTxtBox != null) moLinkedTxtBox = null;
            if (moFindDlgInternal != null) moFindDlgInternal.Dispose();
        }

        private void IndexText()
        {
            string xsText = moLinkedTxtBox.Text;
            string xsWord = FindText;
            if (!MatchCase)
            {
                xsText = xsText.ToUpper();
                xsWord = xsWord.ToUpper();
            }
            miIndices = new List<int>(0);
            if (xsWord.Length > 0)
            {
                if (xsText.Length > 0)
                {
                    int xiPos = xsText.IndexOf(xsWord, 0);
                    while (xiPos >= 0)
                    {
                        miIndices.Add(xiPos);
                        xiPos = xsText.IndexOf(xsWord, xiPos + xsWord.Length);
                    }
                }
            }

            miCurrentIndex = -1;        // reset

            NeedsIndexing = false;
            msLastTextIndexed = moLinkedTxtBox.Text;
            msLastFindText = FindText;
            mbLastMatchCase = MatchCase;

            return;
        }

        private void CheckForReindex()
        {
            bool xbReindex = false;

            if (MatchCase != mbLastMatchCase)
            {
                xbReindex = true;
            }
            else
            {
                if (msLastFindText != FindText)
                {
                    xbReindex = true;
                }
                else
                {
                    if (moLinkedTxtBox.Text.Length != msLastTextIndexed.Length)
                    {
                        xbReindex = true;
                    }
                    else
                    {
                        xbReindex = (moLinkedTxtBox.Text != msLastTextIndexed);
                    }
                }
            }
            if (xbReindex) IndexText();
        }

        private RichTextBox LinkedTextBox()     // internal
        {
            return (moLinkedTxtBox);
        }

        private int FindNext()                  // internal
        {
            string xsMsg = "";
            int xiPos = -1;

            CheckForReindex();

            if (miIndices.Count > 0)
            {
                if (miCurrentIndex < 0)
                {
                    miCurrentIndex = 0;
                }
                else
                {
                    if (miCurrentIndex == miIndices.Count - 1)
                    {
                        miCurrentIndex = 0;
                    }
                    else
                    {
                        miCurrentIndex++;
                    }
                }
                xiPos = miIndices[miCurrentIndex];
            }
            else
            {
                xsMsg = "'" + FindText + "' " + mcsNoMatchMsg;
            }
            msMessage = xsMsg;

            return (xiPos);
        }

        private int FindPrevious()      // internal
        {
            string xsMsg = "";
            int xiPos = -1;

            CheckForReindex();

            if (miIndices.Count > 0)
            {
                if (miCurrentIndex < 0)
                {
                    miCurrentIndex = 0;
                }
                else
                {
                    if (miCurrentIndex == 0)
                    {
                        miCurrentIndex = miIndices.Count - 1;
                    }
                    else
                    {
                        miCurrentIndex = miCurrentIndex - 1;
                    }
                }
                xiPos = miIndices[miCurrentIndex];
            }
            else
            {
                xsMsg = "'" + FindText + "' " + mcsNoMatchMsg;
            }

            //psMsg = xsMsg;
            msMessage = xsMsg;

            return (xiPos);
        }

        private void NextOrPrev(object sender, bool pbNext = true)
        {
            string xsMsg = "";

            if (msLastTextIndexed != moLinkedTxtBox.Text)
            {
                msLastTextIndexed = moLinkedTxtBox.Text;
                NeedsIndexing = true;
            }
            if (FindText.Length == 0)
            {
                xsMsg = "No 'find' text specified.";
            }
            else
            {
                if (NeedsIndexing)
                {
                    IndexText();
                }
                try
                {
                    int xiPos = 0;
                    if (pbNext) xiPos = this.FindNext(); else xiPos = this.FindPrevious();
                    if (xiPos >= 0)
                    {
                        moLinkedTxtBox.SelectionStart = xiPos;
                        moLinkedTxtBox.SelectionLength = FindText.Length;
                        //moLinkedTxtBox.Focus();  // no need, with HideSelection as false

                        if (moLinkedTxtBox.HideSelection == true) moLinkedTxtBox.HideSelection = false;
                        //if (moLinkedTxtBox.HideSelection) moLinkedTxtBox.Focus();
                        moLinkedTxtBox.Focus();
                    }
                    else
                    {
                        xsMsg = "'" + FindText + "' " + mcsNoMatchMsg;
                    }
                }
                catch (Exception xoExc)
                {
                    xsMsg = xoExc.Message;
                }
            }
            if (xsMsg.Length > 0) msMessage = xsMsg;
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            string xsMsg = "";

            if (e.Control && e.KeyCode == Keys.F)       // Ctrl+F (FIND)
            {
                if (!DialogActive)
                {
                    // here, show the associated FIND DIALOG window (always on top)
                    ShowFindDialog();
                    moLinkedTxtBox = (RichTextBox)sender;
                    DialogActive = true;
                }
                e.Handled = true;
            }
            else
            {
                if (e.Shift && e.KeyCode == Keys.F3)        // find PREV
                {
                    // search back
                    if (moLinkedTxtBox != null)
                    {
                        NextOrPrev(sender, false);
                        e.Handled = true;
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.F3)               // find NEXT
                    {
                        // search forward
                        if (moLinkedTxtBox != null)
                        {
                            NextOrPrev(sender);
                            e.Handled = true;
                        }
                    }
                }

            }
            if (xsMsg.Length > 0)
            {
                msMessage = xsMsg;
            }
        }

        public string GetMessage()
        {
            string xsMsg = msMessage;
            msMessage = "";
            return (xsMsg);
        }

        private void TextFindDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            GetFindTextAndCaseMatchFromDlg();
            this.DialogActive = false;
        }

        private void GetFindTextAndCaseMatchFromDlg()  //(out string psFindText, out bool pbMatchCase)
        {
            for (int xii = 0; xii < moFindDlgInternal.Controls.Count; xii++)
            {
                Control xoCtrl = moFindDlgInternal.Controls[xii];
                string xs = xoCtrl.Name.Trim().ToUpper();
                if (xs == "TXTWHAT")
                {
                    FindText = xoCtrl.Text;
                }
                else
                {
                    if (xs == "CHKMATCHCASE")
                    {
                        CheckBox xoChk = (CheckBox)xoCtrl;
                        MatchCase = xoChk.Checked;
                        xoChk = null;
                    }
                }
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            GetFindTextAndCaseMatchFromDlg();  //(out xsFindText, out xbMatchCase);

            int xiPos = FindNext();
            if (xiPos >= 0)
            {
                LinkedTextBox().SelectionStart = xiPos;
                LinkedTextBox().SelectionLength = FindText.Length;
            }
            else
            {
                string xs = GetMessage();
                if (xs.Length > 0) MessageBox.Show(xs);
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            GetFindTextAndCaseMatchFromDlg();

            int xiPos = FindPrevious();
            if (xiPos >= 0)
            {
                LinkedTextBox().SelectionStart = xiPos;
                LinkedTextBox().SelectionLength = FindText.Length;
            }
            else
            {
                string xs = GetMessage();
                if (xs.Length > 0) MessageBox.Show(xs);
            }
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            DialogActive = false;
            moFindDlgInternal.Close();
            moFindDlgInternal.Dispose();
        }

        private void ShowFindDialog()
        {
            if (moFindDlgInternal != null) moFindDlgInternal.Dispose();

            moFindDlgInternal = new Form();

            moFindDlgInternal.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            moFindDlgInternal.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            moFindDlgInternal.ClientSize = new System.Drawing.Size(400, 130);
            moFindDlgInternal.Font = new System.Drawing.Font("Lucida Sans", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            moFindDlgInternal.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            moFindDlgInternal.Name = "TextFindDlg";
            moFindDlgInternal.ShowInTaskbar = false;
            moFindDlgInternal.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            moFindDlgInternal.Text = "Find";
            moFindDlgInternal.TopMost = true;
            moFindDlgInternal.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextFindDlg_FormClosing);

            Label lblWhat = new System.Windows.Forms.Label();
            TextBox txtWhat = new System.Windows.Forms.TextBox();
            CheckBox chkMatchCase = new System.Windows.Forms.CheckBox();
            Button btnNext = new System.Windows.Forms.Button();
            Button btnPrev = new System.Windows.Forms.Button();
            Button btnCanc = new System.Windows.Forms.Button();

            // 
            // lblWhat
            //
            lblWhat.Location = new System.Drawing.Point(13, 22);
            lblWhat.Name = "lblWhat";
            lblWhat.Size = new System.Drawing.Size(95, 20);
            lblWhat.TabIndex = 0;
            lblWhat.Text = "Find what:";
            lblWhat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            moFindDlgInternal.Controls.Add(lblWhat);

            // 
            // txtWhat
            // 
            txtWhat.Location = new System.Drawing.Point(114, 22);
            txtWhat.Name = "txtWhat";
            txtWhat.Size = new System.Drawing.Size(195, 22);
            txtWhat.TabIndex = 1;
            txtWhat.Text = FindText;
            //txtWhat.TextChanged += new System.EventHandler(this.txtWhat_TextChanged);
            moFindDlgInternal.Controls.Add(txtWhat);

            // 
            // chkMatchCase
            // 
            chkMatchCase.AutoSize = true;
            chkMatchCase.Location = new System.Drawing.Point(114, 61);
            chkMatchCase.Name = "chkMatchCase";
            chkMatchCase.Size = new System.Drawing.Size(94, 19);
            chkMatchCase.TabIndex = 2;
            chkMatchCase.Text = "Match case";
            chkMatchCase.UseVisualStyleBackColor = true;
            chkMatchCase.Checked = MatchCase;
            moFindDlgInternal.Controls.Add(chkMatchCase);

            // 
            // btnNext
            // 
            btnNext.Location = new System.Drawing.Point(325, 21);
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(102, 25);
            btnNext.TabIndex = 3;
            btnNext.Text = "Find Next";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += new System.EventHandler(this.btnNext_Click);
            moFindDlgInternal.Controls.Add(btnNext);

            // 
            // btnPrev
            // 
            btnPrev.Location = new System.Drawing.Point(325, 58);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new System.Drawing.Size(102, 25);
            btnPrev.TabIndex = 4;
            btnPrev.Text = "Find Previous";
            btnPrev.UseVisualStyleBackColor = true;
            btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            moFindDlgInternal.Controls.Add(btnPrev);

            // 
            // btnCanc
            // 
            btnCanc.Location = new System.Drawing.Point(325, 95);
            btnCanc.Name = "btnCanc";
            btnCanc.Size = new System.Drawing.Size(102, 25);
            btnCanc.TabIndex = 5;
            btnCanc.Text = "Close";
            btnCanc.UseVisualStyleBackColor = true;
            btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            moFindDlgInternal.Controls.Add(btnCanc);

            moFindDlgInternal.Show();

        }

    }
}
