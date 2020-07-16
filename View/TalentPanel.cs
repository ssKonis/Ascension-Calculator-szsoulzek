using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    public class TalentPanel : SkillPanel
    {
        #region MEMBERS
        Font m_ftRank = new Font("Verdana", 10.0f, FontStyle.Regular); // optimization
        #endregion

        #region CTOR
        public TalentPanel()
        {
            // handle .base members
            // limit the talent panel to 4 across and 11 down
            // 0 -> 3 (4)
            // 0 -> 10 (11)
            Centered = true;
            NumGridX = 4;
            NumGridY = 11;
        }
        #endregion

        #region METHODS
        override protected Bitmap GetPanelBackgroundImage()
        {
            return Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("background_" + Data.SelectedClasstag) as Bitmap;
        }

        override protected void LoadGridData()
        {
            // load talent grid data
            // talent pointers will be loaded by .base
            string[] szGraph = Ascension_Calculator.Properties.Resources.talent_graph.Split("\n".ToCharArray());
            string szCurrClasstag = "";

            Dictionary<string, List<Skill>> vTalents = Data.TalentsList;
            // name|tooltip|icon file name|level requirement|AE cost|TE cost|class|spec|max talent ranks|

            foreach (string szLine in szGraph)
            {
                string[] szData = szLine.TrimStart("\t \n".ToCharArray()).Split("|".ToCharArray());

                if (szData.Length == 3) // this is a classspec section header
                {
                    szCurrClasstag = szData[0] + "_" + szData[1];
                }
                else
                {
                    // graph out this line
                    //  skill name = [0]
                    //  x = [1]
                    //  y = [2]
                    // Starlight Wrath|0|1|

                    // with custom databases, these talent keys may not exist so check
                    if (!vTalents.ContainsKey(szCurrClasstag))
                        continue;

                    Skill sk = vTalents[szCurrClasstag].Find(x => x.Name == szData[0]);

                    if (sk == null)
                        continue; // wat

                    // set this talentui's graph position
                    // create a ui for this talent
                    AddSkillToGraph(sk, Int32.Parse(szData[1]), Int32.Parse(szData[2]));
                }
            }
        }

        override protected SKILL_COLOR GetSkillColor(Skill sk)
        {
            if (sk.IsAbility)
                return SKILL_COLOR.GREY; // wat

            if (sk.CurrentRanks == sk.MaxRanks)
                return SKILL_COLOR.YELLOW;
            else if (sk.CurrentRanks > 0)
                return SKILL_COLOR.GREEN;
            else if (Data.CanSkillBeLearned(sk))
                return SKILL_COLOR.GREY;
            else
                return SKILL_COLOR.RED;
        }

        override protected bool My_OnPaint(Graphics gfx)
        {
            // do default
            if (!base.My_OnPaint(gfx)) // checks for classtag validity for us
                return false; // halt

            // prepare to draw, sir
            Bitmap bmpRankBG = Ascension_Calculator.Properties.Resources.background_rank;
            Point ptRank = new Point();

            // draw our talent ranks on all talents
            List<SkillUI> vGridData = GetGridData(Data.SelectedClasstag);
            int nIconSize = IconSize;
            foreach (var sUI in vGridData)
            {
                Point ptIcon = GetScrolledGridLocation(sUI.XGrid, sUI.YGrid);
                // draw the rank background
                ptRank.X = ptIcon.X + nIconSize - (bmpRankBG.Width >> 1);
                ptRank.Y = ptIcon.Y + nIconSize - (bmpRankBG.Height >> 1);
                gfx.DrawImage(bmpRankBG, ptRank);

                // draw the rank value (0 pts = normal, 1+ but not max = green, max = yellow)
                Brush brColor;
                switch (GetSkillColor(sUI.Skill))
                {
                    case SKILL_COLOR.GREEN:
                        brColor = UITools.Brush_Green;
                        break;
                    case SKILL_COLOR.GREY:
                        brColor = UITools.Brush_Grey;
                        break;
                    case SKILL_COLOR.YELLOW:
                        brColor = UITools.Brush_Yellow;
                        break;
                    case SKILL_COLOR.RED:
                        brColor = UITools.Brush_Red;
                        break;
                    default:
                        brColor = Brushes.White;
                        break;
                }

                // string, font, brush
                gfx.DrawString(sUI.Skill.CurrentRanks.ToString(), m_ftRank, brColor, ptRank.X + 5, ptRank.Y + 3);
            }

            // done
            return true;
        }
        #endregion
    }
}
