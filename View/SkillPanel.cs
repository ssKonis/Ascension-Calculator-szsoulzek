using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Input;

namespace Ascension_Calculator
{
    public abstract class SkillPanel : GFXPanel
    {
        #region TYPES
        protected class SkillUI
        {
            Skill m_Skill;
            int m_nXGrid;
            int m_nYGrid;

            public Skill Skill { get { return m_Skill; } }

            public int XGrid
            {
                get { return m_nXGrid; }
                set { m_nXGrid = value; }
            }

            public int YGrid
            {
                get { return m_nYGrid; }
                set { m_nYGrid = value; }
            }

            public SkillUI(Skill sk, int nGridX, int nGridY)
            {
                m_Skill = sk;
                m_nXGrid = nGridX;
                m_nYGrid = nGridY;
            }
        }

        public enum SKILL_COLOR { GREY, GREEN, YELLOW, RED };
        #endregion

        #region MEMBERS
        Dictionary<string, List<SkillUI>> m_vGridData = new Dictionary<string, List<SkillUI>>(); // key is "Class_Spec"
        SkillToolTip m_ToolTip;
        SkillUI m_SelectedSkillUI;

        int m_nIconSpacing = 12;
        int m_nIconSize = 46;
        int m_nIconBorderSize = 8; // usually divided by 2 everywhere cuz each side, this number is for total added area

        int m_nNumGridX;
        int m_nNumGridY;

        bool m_bCentered;
        #endregion

        #region PROPERTIES
        protected int IconSize { get { return m_nIconSize; } }

        virtual protected int NumGridX
        {
            get { return m_nNumGridX; }

            set
            {
                if (m_nNumGridX == value)
                    return;

                m_nNumGridX = value;

                UpdateAutoScrollMinSize();
            }
        }

        protected int NumGridY
        {
            get { return m_nNumGridY; }

            set
            {
                if (m_nNumGridY == value)
                    return;

                m_nNumGridY = value;

                UpdateAutoScrollMinSize();
            }
        }

        public bool Centered
        {
            get { return m_bCentered; }

            set
            {
                if (m_bCentered == value)
                    return;

                m_bCentered = value;

                UpdateAutoScrollMinSize();
            }
        }

        public int IconSpacing
        {
            get { return m_nIconSpacing; }

            set
            {
                if (m_nIconSpacing == value)
                    return;

                m_nIconSpacing = value;

                UpdateAutoScrollMinSize();
            }
        }

        SkillToolTip ToolTip
        {
            get { return m_ToolTip; }
            set { m_ToolTip = value; }
        }

        protected SkillUI SelectedSkillUI
        {
            get
            {
                return m_SelectedSkillUI;
            }

            set
            {
                if (m_SelectedSkillUI == value)
                    return; // wat

                m_SelectedSkillUI = value;
                Invalidate(); // refresh ofc
            }
        }
        #endregion

        #region CTOR
        protected SkillPanel()
        {
            // init control settings
            this.BackColor = Color.Black;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;

            // load grid data (to be defined by children)
            LoadGridData();

            // set up tooltip
            ToolTip = new SkillToolTip(this); // let children do this for their specific types of tooltip

            // register events
            Data.RegisterEvent(Data.EVENT.DATA_LEVELLIMIT.ToString(), OnLevelLimitChanged);
            Data.RegisterEvent(Data.EVENT.DATA_SELECTEDCLASSTAG.ToString(), OnClasstagChanged);
            Data.RegisterEvent(Data.EVENT.DATA_LEARNSKILL.ToString(), OnLearnSkill);
            Options.RegisterEvent(Options.EVENT.OPTIONS_DISPLAYPANELBACKGROUNDS.ToString(), OnDisplayPanelBackgroundsChanged);
            this.MouseClick += OnMouseClick;
            this.MouseMove += OnMouseMove;
            this.Paint += OnPaint;
            this.MouseLeave += OnMouseLeave;
            this.MouseClick += SkillPanel_OnMouseClick;

            // invoke it once for starting value (fuck ordering in form1)
            //OnClasstagChanged(null); // not needed?
            UpdatePanelBackground(); // bgs only update when classtags changes.. or never... or on options change only. set it here for everybody
        }
        #endregion

        #region METHODS
        private void SkillPanel_OnMouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();
        }

        protected void ModifySkillGridLoc(string szClasstag, int nListIndex, int nNewGridX, int nNewGridY)
        {
            SkillUI skUI = m_vGridData[szClasstag][nListIndex]; // optimization
            skUI.XGrid = nNewGridX;
            skUI.YGrid = nNewGridY;

            RefreshSkillPanel(); // efficiency because it doesnt happen in the above
        }

        void UpdateAutoScrollMinSize()
        {
            Size sz = GetGridSize();

            if (this.AutoScrollMinSize == sz)
                return;

            this.AutoScrollMinSize = sz;

            RefreshSkillPanel();
        }   
        
        void OnLevelLimitChanged(object arg)
        {
            RefreshSkillPanel();
        }

        void UpdatePanelBackground()
        {
            if (Options.DisplayPanelBackgrounds == false)
                this.BackgroundImage = null; // remove the background image
            else
                this.BackgroundImage = GetPanelBackgroundImage();
            
            // refresh is guaranteed because if/else
        }

        abstract protected Bitmap GetPanelBackgroundImage();

        void OnDisplayPanelBackgroundsChanged(object arg)
        {
            UpdatePanelBackground();
        }

        void OnMouseLeave(object sender, EventArgs e)
        {
            // this event fires when the cursor is over the TOOLTIP (which can show up over an icon)
            // check to see if the cursor's screen coords converted to client coords is NOT over an icon
            // only if then, deactivate

            // UPDATE: fuck that noise. transparent window style on the tooltip's createparams fixes all that sheeeittt
            //if (GetSkillOnMouse(this.PointToClient(System.Windows.Forms.Cursor.Position)) == null)
            ToolTip.Deactivate();

            // cancel highlighted icon
            SelectedSkillUI = null;
        }

        abstract protected void LoadGridData();

        abstract protected SKILL_COLOR GetSkillColor(Skill sk); // they draw their colors completely differently

        virtual protected bool My_OnPaint(Graphics gfx) // cuz need return type for children to proceed or not
        {
            // if theres no graph data for the current spec (which can happen with talents or in future server changes)
            if (!m_vGridData.ContainsKey(Data.SelectedClasstag))
                return false; // do not proceed with drawing!

            // noice - but not needed thanks to our super functions which handle scrolling already with locations
            // e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);

            List<SkillUI> pGraph = m_vGridData[Data.SelectedClasstag];

            Rectangle rectIcon = new Rectangle(); // optimization
            rectIcon.Width = rectIcon.Height = m_nIconSize;
            Rectangle rectBorder = new Rectangle();
            rectBorder.Width = rectBorder.Height = m_nIconSize + m_nIconBorderSize;

            foreach (var sUI in pGraph)
            {
                // draw icon
                Point ptGridLoc = GetScrolledGridLocation(sUI.XGrid, sUI.YGrid);
                rectIcon.X = ptGridLoc.X;
                rectIcon.Y = ptGridLoc.Y;
                gfx.DrawImage(sUI.Skill.IconAsBitmap, rectIcon);

                // draw border (different size)
                rectBorder.X = rectIcon.X - (m_nIconBorderSize >> 1);
                rectBorder.Y = rectIcon.Y - (m_nIconBorderSize >> 1);

                // find border image (0 pts = normal, 1+ but not max = green, max = yellow)
                Bitmap bmpBorder = null;
                switch (GetSkillColor(sUI.Skill))
                {
                    case SKILL_COLOR.GREEN:
                        bmpBorder = Ascension_Calculator.Properties.Resources.hue_green;
                        break;
                    case SKILL_COLOR.YELLOW:
                        bmpBorder = Ascension_Calculator.Properties.Resources.hue_yellow;
                        break;
                    case SKILL_COLOR.RED:
                        bmpBorder = Ascension_Calculator.Properties.Resources.hue_red;
                        break;
                }

                // draw selection hue maybe
                if (SelectedSkillUI == sUI)
                    gfx.DrawImage(Ascension_Calculator.Properties.Resources.hue_highlight, rectBorder);

                // draw border hue
                if (bmpBorder != null)
                    gfx.DrawImage(bmpBorder, rectBorder);

                // draw border image
                gfx.DrawImage(Ascension_Calculator.Properties.Resources.icon_border, rectBorder);
            }

            // done, proceed with children rendering
            return true;
        }

        void OnPaint(object sender, PaintEventArgs e)
        {
            My_OnPaint(e.Graphics); // for the return type
        }

        SkillUI GetSkillOnMouse(Point ptMouseLocation)
        {
            // if theres no graph data for the current spec (which can happen with talents or in future server changes)
            if (!m_vGridData.ContainsKey(Data.SelectedClasstag))
                return null;

            // check each skillui's bounding area for this click location (NOW WITH BORDER DETECTION!!!)
            Rectangle rect = new Rectangle();
            rect.Width = rect.Height = m_nIconSize + (m_nIconBorderSize >> 1);
            Point ptLoc = new Point(); // optimization

            foreach (var sk in m_vGridData[Data.SelectedClasstag])
            {
                ptLoc = GetScrolledGridLocation(sk.XGrid, sk.YGrid);

                // fix rect
                rect.X = ptLoc.X;
                rect.Y = ptLoc.Y;

                if (rect.Contains(ptMouseLocation))
                {
                    // got 'em
                    return sk;
                }
            }

            // not found in this location
            return null;
        }

        void OnMouseClick(object sender, MouseEventArgs e)
        {
            SkillUI sUI = GetSkillOnMouse(e.Location); // lovely function already accounts for scrolling
            if (sUI == null)
                return; // they clicked nowhere

            // we found the ui they clicked on
            int nAdjustment = 0;
            bool bMax = false;
            if (Control.ModifierKeys == Keys.Control && sUI.Skill.IsTalent) // cant "max" abilities
                bMax = true;
            if (e.Button == MouseButtons.Left) // go up 1 rank
            {
                nAdjustment = 1;
                if (bMax)
                {
                    // figure out how many ranks we can max out on
                    int nMaxCounter = 0;
                    while (Data.CanSkillBeLearned(sUI.Skill, (uint)(nAdjustment + nMaxCounter)))
                        ++nMaxCounter;
                    // become that amount
                    nAdjustment = nMaxCounter;
                }
            }
            else if (e.Button == MouseButtons.Right) // go down 1 rank
            {
                nAdjustment = -1;
                if (bMax)
                    nAdjustment *= sUI.Skill.CurrentRanks; // go down by X ranks where X is how many learned so far if ctrl is down
            }

            // change this skill's points
            Data.LearnSkill(sUI.Skill, nAdjustment);
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            SkillUI sUI = GetSkillOnMouse(e.Location); // lovely function already accounts for scrolling
            if (sUI == null)
            {
                SelectedSkillUI = null;
                ToolTip.Deactivate();
                return; // they moused nothing
            }

            // the mouse is now officially hovering over a skill
            Point ptSkillLoc = GetScrolledGridLocation(sUI.XGrid, sUI.YGrid);
            ptSkillLoc.X += m_nIconSize;

            ToolTip.Activate(sUI.Skill, ptSkillLoc);
            // dont redraw for no reason
            if (SelectedSkillUI != sUI)
            {
                SelectedSkillUI = sUI;
            }
        }

        virtual protected Point GetScrolledGridLocation(int nXCoord, int nYCoord)
        {
            Size gridSize = GetGridSize();

            // include borders
            int nTrueIconSize = m_nIconSize + (m_nIconBorderSize >> 1);

            // only center along each axis if panel is bigger than the grid size
            int nXOffset = 0;
            int nYOffset = 0;

            // and only center if we're supposed to
            if (Centered)
            {
                if (this.Width > gridSize.Width)
                {
                    nXOffset = (this.Width >> 1) - (((nTrueIconSize * m_nNumGridX) + (IconSpacing * (m_nNumGridX - 1))) >> 1);
                }

                if (this.Height > gridSize.Height)
                {
                    nYOffset = (this.Height >> 1) - (((nTrueIconSize * m_nNumGridY) + (IconSpacing * (m_nNumGridY - 1))) >> 1);
                }
            }

            // adjust for scrolling
            nXOffset += this.AutoScrollPosition.X;
            nYOffset += this.AutoScrollPosition.Y;

            return new Point(nXCoord * (IconSpacing + nTrueIconSize) + nXOffset, nYCoord * (IconSpacing + nTrueIconSize) + nYOffset);
        }

        void OnLearnSkill(object arg)
        {
            //if (Data.SelectedClasstag == (arg as Skill).Classtag)
                RefreshSkillPanel(); // do it anyway because we might have cross-classtag interaction (like dual wield stuff)
        }

        virtual protected void OnClasstagChanged(object arg)
        {
            UpdatePanelBackground();
        }

        protected void RefreshSkillPanel()
        {
            // do the panel
            this.Invalidate();

            // do the tooltip
            ToolTip.Refresh();
        }

        virtual protected Size GetGridSize()
        {
            int nGridWidth = (m_nIconSize + (m_nIconBorderSize >> 1)) * m_nNumGridX + IconSpacing * (m_nNumGridX - 1);
            int nGridHeight = (m_nIconSize + (m_nIconBorderSize >> 1)) * m_nNumGridY + IconSpacing * (m_nNumGridY - 1);

            return new Size(nGridWidth + 10, nGridHeight + 10); // + for padding
        }

        protected void AddSkillToGraph(Skill sk, int nGridX, int nGridY)
        {
            string szClasstag = sk.Classtag; // optimization, its a string operation

            if (!m_vGridData.ContainsKey(szClasstag))
                m_vGridData.Add(szClasstag, new List<SkillUI>());

            m_vGridData[szClasstag].Add(new SkillUI(sk, nGridX, nGridY));
        }

        protected List<SkillUI> GetGridData(string szClasstag)
        {
            return m_vGridData[szClasstag];
        }
        #endregion
    }
}
