using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace Ascension_Calculator
{
    public partial class Form1 : Form
    {
        #region TYPES
        class InvestmentGraphSorter : IComparer<string>
        {
            public int Compare(string lhs, string rhs)
            {
                int nPercentIndex = lhs.IndexOf('%');
                int nTabIndex = lhs.LastIndexOf('\t');
                string szPercent = lhs.Substring(nTabIndex + 1, nPercentIndex - nTabIndex - 1);
                float fLHSPercent = float.Parse(szPercent);

                nPercentIndex = rhs.IndexOf('%');
                nTabIndex = rhs.LastIndexOf('\t');
                szPercent = rhs.Substring(nTabIndex + 1, nPercentIndex - nTabIndex - 1);
                float fRHSPercent = float.Parse(szPercent);

                if (fLHSPercent < fRHSPercent)
                    return 1;
                else if (fLHSPercent > fRHSPercent)
                    return -1;
                else
                    return 0;
            }
        }
        #endregion

        #region MEMBERS
        EZToolTip m_ToolTip;
        #endregion

        #region CTOR
        public Form1()
        {
            InitializeComponent();

            m_ToolTip = new EZToolTip(this);

            // init panels based on options
            CreatePanels();

            // init controls based on data
            OnAvailableAEChanged(null);
            OnAvailableTEChanged(null);
            OnLevelLimitChanged(null);
            lblRequiredLevel.Text = Data.RequiredLevel.ToString();

            // init form title based on version (BEFORE IOMANAGER IS MADE)
            this.Text += " (" + VersionChecker.Version + ")"; // also invokes the version checker code for free!!! sneaky little hobbitses :)

            // for file i/o
            IOManager.SetForm(this);

            // register events
            Data.RegisterEvent(Data.EVENT.DATA_AVAILABLEAE.ToString(), OnAvailableAEChanged);
            Data.RegisterEvent(Data.EVENT.DATA_AVAILABLETE.ToString(), OnAvailableTEChanged);
            Data.RegisterEvent(Data.EVENT.DATA_LEVELLIMIT.ToString(), OnLevelLimitChanged);
            Data.RegisterEvent(Data.EVENT.DATA_LEARNSKILL.ToString(), OnLearnSkill);
            Data.RegisterEvent(Data.EVENT.DATA_SELECTEDCLASSTAG.ToString(), OnClasstagChanged);
            Data.RegisterEvent(Data.EVENT.DATA_RESETBEGIN.ToString(), OnResetBegin);
            Data.RegisterEvent(Data.EVENT.DATA_RESETEND.ToString(), OnResetEnd);
            Options.RegisterEvent(Options.EVENT.OPTIONS_PANELSTALENTSABILITIES.ToString(), OnPanelsChanged);

            // init selected class button
            OnClasstagChanged(Data.SelectedClasstag);

            // init investment graphs
            UpdateInvestmentGraphs();

            // fix panel so it wont change splitter bar on maximize/minimize
            splitContainer1.FixedPanel = FixedPanel.Panel1;
        }
        #endregion

        #region METHODS
        void Reset()
        {
            // select first classtag
            SelectClasstag(pb_Druid_Balance, null);

            // reset all skills
            Data.ResetSkills();

            IOManager.ModifiedFile = false; // uhg, cuz we learn skills to reset

            // reset the level limit
            Data.LevelLimit = Data.MaxLevel;
        }

        void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!IOManager.New())
                return; // halt

            // reset everything
            Reset();
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IOManager.Open();
        }

        public void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // save it
            IOManager.SaveAs();
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close(); // handled by formclosing and 'X'
        }

        void SelectClasstag(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;

            // updated selected classtag in data
            Data.SelectedClasstag = pb.Name.TrimStart("pb_".ToCharArray());
        }

        void OnClasstagChanged(object arg)
        {
            // get the old and new
            PictureBox pdOld = Controls.Find("pb_" + (arg as string), true)[0] as PictureBox;
            PictureBox pbNew = Controls.Find("pb_" + Data.SelectedClasstag, true)[0] as PictureBox;

            // change old image
            string[] vPriorNames = pdOld.Name.Split("_".ToCharArray());
            pdOld.Image =
                Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("classtag_" +
                vPriorNames[1] + "_" + vPriorNames[2]) as Bitmap;

            // change new image
            string[] vNewNames = pbNew.Name.Split("_".ToCharArray());
            string szClasstag = vNewNames[1] + "_" + vNewNames[2];
            pbNew.Image =
                Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("classtag_" + szClasstag + "_selected") as Bitmap;
        }

        void OnLevelLimitChanged(object arg)
        {
            lblLevelLimit.Text = Data.LevelLimit.ToString();

            // update list view filters
            AvailableFilterChanged(null, null);
        }

        void OnAvailableAEChanged(object arg)
        {
            lblAvailableAE.Text = Data.AvailableAE.ToString();

            // update list view filters
            AvailableFilterChanged(null, null);            
        }

        void OnAvailableTEChanged(object arg)
        {
            lblAvailableTE.Text = Data.AvailableTE.ToString();

            // update list view filters
            AvailableFilterChanged(null, null);
        }

        void OnLearnSkill(object arg)
        {
            List<Skill> list = arg as List<Skill>;

            // update req lvl label
            lblRequiredLevel.Text = Data.RequiredLevel.ToString();

            foreach (var sk in list)
            {
                // update investment labels
                string szClassTag = sk.Classtag;
                Control lblAE = this.Controls.Find("lbl_AE_" + szClassTag, true)[0]; // should be only one control with this exact name
                Control lblTE = this.Controls.Find("lbl_TE_" + szClassTag, true)[0]; // should be only one control with this exact name

                lblAE.Text = Data.GetInvestment(szClassTag, Data.INVESTMENT.AE).ToString();
                lblTE.Text = Data.GetInvestment(szClassTag, Data.INVESTMENT.TE).ToString();

                // update label backcolor
                if (lblAE.Text != "0" || lblTE.Text != "0")
                    lblAE.Parent.BackColor = System.Drawing.SystemColors.ControlLight;
                else
                    lblAE.Parent.BackColor = System.Drawing.SystemColors.ControlDark;
            }            

            // update investment graphs
            UpdateInvestmentGraphs();

            // file is now modified
            IOManager.ModifiedFile = true;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IOManager.Exit())
                e.Cancel = true; // halt
        }

        void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new HelpDlg().ShowDialog();
            //IWin32Window win = this;
        }

        void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new OptionsDlg().ShowDialog();
        }
        
        private void AvailableFilterChanged(object sender, EventArgs e)
        {
            AvailableListView.SEARCHTYPE type;

            if (radAllWords.Checked)
                type = AvailableListView.SEARCHTYPE.WORDS_ALL;
            else if (radAnyWords.Checked)
                type = AvailableListView.SEARCHTYPE.WORDS_ANY;
            else
                type = AvailableListView.SEARCHTYPE.EXACT;

            // doing it here because this sends form1 information to the listview
            // otherwise would just have the listview register to the data event itself
            
            availableListView1.UpdateFilter(
                new AvailableListView.FilterProperties(txtSearchName.Text, txtSearchTooltip.Text, type, chkAECost.Checked, chkTECost.Checked, chkReqLevel.Checked));
        }

        private void Classtag_MouseEnter(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = Color.Black;
        }

        private void Classtag_MouseLeave(object sender, EventArgs e)
        {
            (sender as PictureBox).BackColor = System.Drawing.SystemColors.ControlLight;
        }

        private void asImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IOManager.ShareBuild(IOManager.SHARETYPE.ASIMAGE);
        }

        private void asTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IOManager.ShareBuild(IOManager.SHARETYPE.ASTEXT);
        }

        void UpdateInvestmentGraphs()
        {
            // because += their onpaint delegate isnt working for shit? weird debug issues
            List<ToolStripStatusLabel> vLabels = new List<ToolStripStatusLabel>();
            vLabels.Add(lblAEInvestmentGraph);
            vLabels.Add(lblTEInvestmentGraph);
            vLabels.Add(lblTotalInvestmentGraph);

            foreach (var lbl in vLabels)
            {
                RenderInvestmentGraph(lbl);
            }            
        }

        void RenderInvestmentGraph(ToolStripStatusLabel lbl)
        {
            Image imgOld = lbl.BackgroundImage;
            List<string> vFinalTag = new List<string>();

            // get all investments from data
            Dictionary<string, Data.Investment> investments = Data.Investments;

            // determine total variable by sender (ae/te/both)
            int nTotal;
            if (lbl.Name.Contains("AE"))
                nTotal = Data.GetTotalInvestments(Data.INVESTMENT.AE);
            else if (lbl.Name.Contains("TE"))
                nTotal = Data.GetTotalInvestments(Data.INVESTMENT.TE);
            else
                nTotal = Data.GetTotalInvestments(Data.INVESTMENT.AE) + Data.GetTotalInvestments(Data.INVESTMENT.TE);

            Rectangle rectCurrent = new Rectangle(0, 0, lbl.Width, lbl.Height);
            Bitmap bmpFinal = new Bitmap(lbl.Width, lbl.Height);
            Graphics gfx = Graphics.FromImage(bmpFinal);
            SolidBrush brClasstagColor = new SolidBrush(Color.White);

            if (nTotal > 0)
            {
                foreach (KeyValuePair<string, Data.Investment> entry in investments)
                {                    
                    int nClasstagValue;

                    if (lbl.Name.Contains("AE"))
                        nClasstagValue = entry.Value.AE;
                    else if (lbl.Name.Contains("TE"))
                        nClasstagValue = entry.Value.TE;
                    else
                        nClasstagValue = entry.Value.AE + entry.Value.TE;

                    if (nClasstagValue == 0)
                        continue; // dont change the color

                    brClasstagColor.Color = UITools.GetClassColor(entry.Key.Split("_".ToCharArray())[0]); // get color of this classtag

                    // find the % of this investment
                    float fPercent = nClasstagValue / (float)nTotal;

                    // adjust the draw rect by that %
                    rectCurrent.Width = (int)(lbl.Width * fPercent);

                    // draw it
                    gfx.FillRectangle(brClasstagColor, rectCurrent);

                    // now move over that much
                    rectCurrent.X += rectCurrent.Width;

                    // adjust tooltip based on this classtag's info
                    // string.Format("{0,-8} {1,-20} {2}", stuff)
                    //szFinalTag += UITools.FixClasstag(entry.Key) + ": \t" + ((int)(fPercent * 100)) + "%\n";
                    string szTabAmount = "\t";
                    if (entry.Key.Contains("General"))
                        szTabAmount += "\t";
                    string szPercent = string.Format("{0:#.0}", fPercent * 100) + "%";
                    vFinalTag.Add(string.Format("{0,-25}{1}{2,-6}\n", UITools.FixClasstag(entry.Key), szTabAmount, szPercent));
                }

                if (rectCurrent.X < lbl.Width) // if theres a gap left on the end, fill it
                {
                    rectCurrent.Width = lbl.Width;
                    gfx.FillRectangle(brClasstagColor, rectCurrent); // color will be the last successful classtag
                }
            }
            
            // render border
            gfx.DrawRectangle(Pens.Black, new Rectangle(0, 0, lbl.Width - 1, lbl.Height - 1)); // -1 cuz we wanna see it, not be ON the control border

            // gdi cleanup
            brClasstagColor.Dispose();
            gfx.Dispose();

            // set bg image
            lbl.BackgroundImage = bmpFinal;

            // get rid of old image
            if (imgOld != null)
                imgOld.Dispose();

            // save the "tooltip" data
            // sort the text by % first (beginning of string X.X%)
            string szFinalTag = "";
            //customerList = customerList.OrderBy(c => int.Parse(c.Code)).ToList();
            vFinalTag.Sort(new InvestmentGraphSorter());
            foreach (var szLine in vFinalTag)
            {
                szFinalTag += szLine;
            }
            lbl.Tag = szFinalTag;
        }

        private void importTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ImportDlg().ShowDialog();
        }

        private void InvestmentGraph_MouseEnter(object sender, EventArgs e)
        {
            ToolStripStatusLabel lbl = (sender as ToolStripStatusLabel);

            int nTrueX = lbl.Owner.Bounds.X + lbl.Bounds.X;
            int nTrueY = lbl.Owner.Bounds.Y + lbl.Bounds.Y;

            m_ToolTip.Activate(lbl.Tag as string, new Point(nTrueX + lbl.Bounds.Width, nTrueY));
        }

        private void InvestmentGraph_MouseLeave(object sender, EventArgs e)
        {
            m_ToolTip.Deactivate();
        }

        private void splitContainer1_Panel1_MouseClick(object sender, MouseEventArgs e)
        {
            splitContainer1.Panel1.Focus();
        }

        void OnResetBegin(object obj)
        {
            SuspendLayout();
        }

        void OnResetEnd(object obj)
        {
            ResumeLayout(true);
        }

        void OnPanelsChanged(object obj)
        {
            // clear out panels
            this.splitTalentAbility.Panel1.Controls.Clear();
            this.splitTalentAbility.Panel2.Controls.Clear();

            // reinit
            CreatePanels();
        }

        void CreatePanels()
        {
            if (Options.PanelsTalentsAbilities)
            {
                this.splitTalentAbility.Panel1.Controls.Add(new TalentPanel());
                this.splitTalentAbility.Panel2.Controls.Add(new AbilityPanel());
            }
            else
            {
                this.splitTalentAbility.Panel2.Controls.Add(new TalentPanel());
                this.splitTalentAbility.Panel1.Controls.Add(new AbilityPanel());
            }
        }

        void generateRandomBuildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data.GenerateRandomBuild();
        }
        #endregion

        private void customDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // File.WriteAllText("C:\ResourceName", Resources.ResourceName);
            View.CustomDatabase dlg = new View.CustomDatabase();

            dlg.ShowDialog();
        }
    }
}
