using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Ascension_Calculator
{
    public class SkillToolTip : EZToolTip
    {
        #region TYPES
        class TextLine
        {
            public Brush m_Brush = Brushes.White;
            public string m_szText = "";

            public TextLine() { }

            public TextLine(string szText, Brush brush)
            {
                m_szText = szText;
                m_Brush = brush;
            }
        }
        #endregion

        #region PROPERTIES
        Font Font { get { return m_ftText; } }

        Skill Skill
        {
            get { return m_Skill; }

            set
            {
                if (value == null || m_Skill == value)
                    return; // can happen

                m_Skill = value;

                // adjust rectbounds height based on this skill's information
                // (must be defined by child cuz its different per each type)
                using (var gfx = Graphics.FromHwnd(IntPtr.Zero))
                {
                    int nTotalHeight = 0;
                    List<TextLine> vLines = GetDisplayText();

                    foreach (var txtLine in vLines)
                    {
                        nTotalHeight += (int)gfx.MeasureString(txtLine.m_szText, Font, new Size(Bounds.Width - (TextPadding << 1), Int32.MaxValue)).Height + (TextPadding << 1);
                    }

                    m_rectBounds.Height = nTotalHeight;
                }
            }
        }

        Size Bounds { get { return new Size(m_rectBounds.Width, m_rectBounds.Height); } }

        int TextPadding { get { return m_nTextPadding; } }
        #endregion

        #region MEMBERS
        Skill m_Skill;
        Rectangle m_rectBounds = new Rectangle(0, 0, 300, 200);

        Font m_ftText = new Font("Verdana", 8.5f, FontStyle.Bold);
        int m_nTextPadding = 5;
        #endregion

        #region CTOR
        public SkillToolTip(Control ctl) : base(ctl)
        {
            // event registration
            Data.RegisterEvent(Data.EVENT.DATA_AVAILABLEAE.ToString(), OnAvailableAETEChanged);
            Data.RegisterEvent(Data.EVENT.DATA_AVAILABLETE.ToString(), OnAvailableAETEChanged);
            this.OwnerDraw = true;
            this.Popup += new PopupEventHandler(this.OnPopup);
            this.Draw += new DrawToolTipEventHandler(OnDraw);
        }
        #endregion

        #region METHODS
        public void Refresh()
        {
            if (!this.Active)
                return; // nothing to show
                        
            Activate("UNUSED", new Point(m_rectBounds.X, m_rectBounds.Y));
        }

        public void Activate(Skill sk, Point ptTopRight)
        {
            if (this.Active && sk == Skill)
                return; // do nothing, already shown

            // capture the info to display
            Skill = sk;

            // and reposition
            //m_rectBounds.X = ptTopRight.X + 5; // padding cuz of mouse on icon also on tooltip bug
            //m_rectBounds.Y = ptTopRight.Y - m_rectBounds.Height - 5;
            ptTopRight = new Point(ptTopRight.X + 5, ptTopRight.Y - m_rectBounds.Height - 5);

            Point ptScreen = Control.PointToScreen(new Point(ptTopRight.X, ptTopRight.Y));

            // fix position to not be off-screen (only gotta check for top/right cuz of positioning from icon...)
            // compare against ALL screens (monitor extensions) max width and minimum height
            int nMaxWidth = 0;
            Screen[] screens = Screen.AllScreens;
            foreach (var scr in screens)
            {
                int nScreenX = scr.Bounds.X + scr.Bounds.Width;
                if (nScreenX > nMaxWidth)
                    nMaxWidth = nScreenX;
            }
            // Screen.PrimaryScreen
            if (ptScreen.X + m_rectBounds.Width > nMaxWidth)
            {
                ptScreen.X = nMaxWidth - m_rectBounds.Width;
            }
            else if (ptScreen.X < 0)
            {
                ptScreen.X = 0;
            }
            //if (ptScreen.Y + m_rectBounds.Height > scr.Bounds.Height) // cant happen? cuz tooltips show up to the top right of the fucking mouse
            if (ptScreen.Y < 0)
            {
                ptScreen.Y = 0;
            }
            
            Point ptFixed = Control.PointToClient(ptScreen);
            m_rectBounds.X = ptFixed.X;
            m_rectBounds.Y = ptFixed.Y;

            // show it
            Activate("UNUSED", new Point(m_rectBounds.X, m_rectBounds.Y));
        }
                
        void OnPopup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = new Size(m_rectBounds.Width, m_rectBounds.Height);
        }

        void OnAvailableAETEChanged(object args)
        {
            Refresh(); // to color the costs
        }
        
        void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            // background
            e.Graphics.FillRectangle(Brushes.Black, e.Bounds);

            // border (based on dynamic availability ofc!)
            Pen penBorder;

            if (Data.IsSkillLearned(Skill))
            {
                if (Skill.CurrentRanks == Skill.MaxRanks && Skill.IsTalent)
                    penBorder = UITools.Pen_Yellow;
                else
                    penBorder = UITools.Pen_Green;
            }
            else if (Data.CanSkillBeLearned(Skill))
                penBorder = UITools.Pen_Grey;
            else
                penBorder = UITools.Pen_Red;

            e.Graphics.DrawRectangle(penBorder, 0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1); // -1 on sizes because of border issues

            // text
            RectangleF rectfLoc = new RectangleF(TextPadding, TextPadding, e.Bounds.Width - TextPadding, e.Bounds.Height - TextPadding);
            List<TextLine> vLines = GetDisplayText();

            foreach (var txtLine in vLines)
            {
                e.Graphics.DrawString(txtLine.m_szText, Font, txtLine.m_Brush, rectfLoc);
                rectfLoc.Y += (int)((e.Graphics.MeasureString(txtLine.m_szText, Font, new Size(Bounds.Width - (TextPadding << 1), Int32.MaxValue)).Height + (TextPadding << 1)) * 0.6f); // idk why scruntch
            }

            // ae cost
            Bitmap bmpAE = Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("ae") as Bitmap;
            Point ptCosts = new Point(TextPadding, Bounds.Height - TextPadding - bmpAE.Height); // bottom left, indented

            // if it has a cost, render it
            if (Skill.AECost > 0)
            {
                e.Graphics.DrawImage(bmpAE, ptCosts); // the small essence image
                ptCosts.X += bmpAE.Width + 5; // nudge for text

                Brush brAECost;
                // set AE cost color based on availability
                if (Skill.AECost > Data.AvailableAE)
                    brAECost = UITools.Brush_Red;
                else
                    brAECost = Brushes.White;
                e.Graphics.DrawString(Skill.AECost.ToString(), Font, brAECost, ptCosts); // cost value

                ptCosts.X += 20; // nudge for te
            }

            // te cost
            Bitmap bmpTE = Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("te") as Bitmap;

            // draw te cost
            if (Skill.TECost > 0)
            {
                e.Graphics.DrawImage(bmpTE, ptCosts); // the small essence image
                ptCosts.X += bmpTE.Width + 5; // nudge for text and te cost

                Brush brTECost;
                // set AE cost color based on availability
                if (Skill.TECost > Data.AvailableTE)
                    brTECost = UITools.Brush_Red;
                else
                    brTECost = Brushes.White;

                e.Graphics.DrawString(Skill.TECost.ToString(), Font, brTECost, ptCosts); // cost value
            }
        }

        List<TextLine> GetDisplayText()
        {
            if (Skill == null)
                return null; // wat

            // prepare the text to return
            List<TextLine> vLines = new List<TextLine>();

            vLines.Add(new TextLine(Skill.Name, Brushes.White));

            Brush brReqLvl;
            if (Skill.RequiredLevel > Data.LevelLimit) // make req lvl line red if not met
                brReqLvl = UITools.Brush_Red;
            else
                brReqLvl = Brushes.White; // otherwise its good
            vLines.Add(new TextLine("Requires Level " + Skill.RequiredLevel, brReqLvl));
            vLines.Add(new TextLine(Skill.Text, UITools.Brush_Yellow));

            // talent ranks
            if (Skill.IsTalent)
            {
                // inject rank at [1] right after name
                // set rank color
                Brush brush = UITools.Brush_Grey;
                if (Skill.CurrentRanks == Skill.MaxRanks)
                    brush = UITools.Brush_Yellow;
                else if (Skill.CurrentRanks > 0)
                    brush = UITools.Brush_Green;

                vLines.Insert(1, new TextLine("Rank " + Skill.CurrentRanks + "/" + Skill.MaxRanks, brush));
            }

            return vLines;
        }
        #endregion
    }
}
