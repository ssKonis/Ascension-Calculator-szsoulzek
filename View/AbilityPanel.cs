using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Ascension_Calculator
{
    public class AbilityPanel : SkillPanel
    {
        #region CONSTANTS
        const int GRID_WIDTH_MINIMUM = 5;
        const int PADDING_WIDTH = 35;
        const int PADDING_HEIGHT = 60;

        const int NAME_TRUNC_LIMIT = 14;
        #endregion

        #region MEMBERS
        Font m_ftText = new Font("Verdana", 7.5f, FontStyle.Bold); // optimization
        #endregion

        #region CTOR
        public AbilityPanel()
        {
            IconSpacing = 55;
            
            Scroll += Panel_Scroll;
            Layout += OnPanelLayoutChanged; // this will calculate our GRIDX/Y for us

            // invoke it to set our grid size
            OnPanelLayoutChanged(null, null);
        }
        #endregion

        #region METHODS
        override protected Bitmap GetPanelBackgroundImage()
        {
            return Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("background_General_General") as Bitmap;
        }

        private void OnPanelLayoutChanged(object sender, LayoutEventArgs e)
        {
            // calculate how many skills can fit on the X axis using the panel's new size, iconsize, and iconspacing (including our custom PADDING)
            int nPanelWidth = Width - PADDING_WIDTH;
            int nPanelHeight = Height - PADDING_HEIGHT;

            int nNewGridX = nPanelWidth / (IconSize + IconSpacing);

            // cap it minimum
            if (nNewGridX < GRID_WIDTH_MINIMUM)
                nNewGridX = GRID_WIDTH_MINIMUM;

            // figure out how many rows we need to fit all the selected class tag's abilities
            if (!Data.AbilitiesList.ContainsKey(Data.SelectedClasstag)) // this classtag doesnt exist. show nothing
                return;

            List<Skill> vAbilities = Data.AbilitiesList[Data.SelectedClasstag]; // optimization, who knows how keys are accessed
                        
            int nNewGridY = (vAbilities.Count / nNewGridX) + 1;

            // dont add one for evenly divisible counts
            if (vAbilities.Count % nNewGridX == 0)
            {
                --nNewGridY;
            }

            // fix every skill in the current classtag
            for (int i = 0; i < vAbilities.Count; ++i)
            {
                ModifySkillGridLoc(Data.SelectedClasstag, i, i % nNewGridX, i / nNewGridX);
            }

            // actually set it
            NumGridX = nNewGridX;
            NumGridY = nNewGridY;

            // show off our fine work
            //Refresh(); // grid modifiers include this obviously
        }

        private void Panel_Scroll(object sender, ScrollEventArgs e)
        {
            RefreshSkillPanel();
        }

        override protected void LoadGridData()
        {
            // init skill grid data
            // skill pointers will be loaded by .base
            Dictionary<string, List<Skill>> vAbilities = Data.AbilitiesList;

            foreach (var vList in vAbilities.Values)
            {
                List<Skill> vSortedList = new List<Skill>(vList); // copy cuz we're gonna sort it
                // sort it by required level
                vSortedList.Sort(new Skill.CompareByRequiredLevel());

                foreach (var pAbility in vSortedList)
                {
                    // create a ui for this ability
                    AddSkillToGraph(pAbility, 0, 0); // gonna be recalibrated from the panel layout event
                }
            }
        }

        override protected void OnClasstagChanged(object arg)
        {
            // update the grid by num skills
            OnPanelLayoutChanged(null, null);

            //Refresh(); // it includes a refresh - duh
        }

        override protected SKILL_COLOR GetSkillColor(Skill sk)
        {
            if (sk.IsTalent)
                return SKILL_COLOR.GREY; // wat

            if (Data.IsSkillLearned(sk))
                return SKILL_COLOR.GREEN;
            else if (Data.CanSkillBeLearned(sk))
                return SKILL_COLOR.GREY;
            else
                return SKILL_COLOR.RED;
        }

        override protected Point GetScrolledGridLocation(int nXCoord, int nYCoord)
        {
            Point ptFixed = base.GetScrolledGridLocation(nXCoord, nYCoord);

            ptFixed.X += PADDING_WIDTH; // padding
            ptFixed.Y += (PADDING_HEIGHT >> 1); // padding

            return ptFixed;
        }

        override protected Size GetGridSize()
        {
            Size sizeFixed = base.GetGridSize();

            sizeFixed.Width += (PADDING_WIDTH << 1); // to make right side of right-most icon have more room from screen edge
            sizeFixed.Height += PADDING_HEIGHT;

            return sizeFixed; // + for padding
        }

        override protected bool My_OnPaint(Graphics gfx)
        {
            // do default
            if (!base.My_OnPaint(gfx)) // checks for classtag validity for us
                return false; // halt

            // prepare to draw, sir
            Point ptLoc = new Point();
            string[] szLines = new string[3];

            // draw all our abilites for this classpec
            List<SkillUI> vGridData = GetGridData(Data.SelectedClasstag);

            int nIconSize = IconSize;
            
            foreach (var sUI in vGridData)
            {
                Point ptIcon = GetScrolledGridLocation(sUI.XGrid, sUI.YGrid);

                // prepare the text(s) to be shown
                string szSkillName = sUI.Skill.Name;

                // truncate too long names
                if (szSkillName.Length > NAME_TRUNC_LIMIT + 3)
                {
                    szSkillName = szSkillName.Remove(NAME_TRUNC_LIMIT);
                    szSkillName += "...";
                }

                szLines[0] = szSkillName;
                szLines[1] = "Level " + sUI.Skill.RequiredLevel;
                szLines[2] = sUI.Skill.AECost + " AE";

                ptLoc.Y = ptIcon.Y + nIconSize - 8;
                // position each line (by row)
                foreach (var szLine in szLines)
                {
                    SizeF sizeString = gfx.MeasureString(szLine, m_ftText);
                    ptLoc.X = ptIcon.X + (nIconSize >> 1) - ((int)(sizeString.Width) >> 1);
                    ptLoc.Y += (int)sizeString.Height - 1;

                    // draw text (WITH SHADOW!) (based on learned color)
                    Brush brush = Brushes.White;
                    if (Data.IsSkillLearned(sUI.Skill))
                        brush = UITools.Brush_Green;
                    else if (!Data.CanSkillBeLearned(sUI.Skill))
                        brush = UITools.Brush_Red;
                                        
                    UITools.DrawShadowString(gfx, m_ftText, brush, szLine, ptLoc.X, ptLoc.Y);
                }
            }

            // done
            return true;
        }
        #endregion
    }
}
