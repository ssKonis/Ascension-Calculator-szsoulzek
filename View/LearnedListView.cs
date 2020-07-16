using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;

namespace Ascension_Calculator
{
    class LearnedListView : SkillListView
    {
        #region MEMBERS
        bool m_bNoFilter = true; // to fix tooltip bug on initial list being empty
        #endregion

        #region CTOR
        public LearnedListView()
        {
            OLVColumn olvCol = GenerateColumn(1);
            olvCol.HeaderImageKey = "rankup";
            olvCol.AspectGetter = delegate (object sk) {
                Skill skill = sk as Skill;
                string szReturn = skill.CurrentRanks.ToString();
                if (skill.MaxRanks > 0)
                    szReturn += "/" + skill.MaxRanks;
                return szReturn;

            };

            olvCol.Text = "Talent Rank";
            olvCol.Width = 100;
            this.MouseClick += LearnedListView_OnMouseClick;
            this.ModelDropped += LearnedListView_OnModelDropped;

            ActivateFilter();
        }
        #endregion

        #region METHODS
        private void LearnedListView_OnModelDropped(object sender, ModelDropEventArgs e)
        {
            Data.LearnSkill(e.SourceModels[0] as Skill, 1);

            e.Handled = true;
        }

        protected override bool ModelDropCheck(Skill sk)
        {
            return Data.CanSkillBeLearned(sk);
        }

        void LearnedListView_OnMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var hit = OlvHitTest(e.X, e.Y);
            Skill sk = hit?.Item?.RowObject as Skill;

            if (sk != null && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!Constant) // changes allowed
                    Data.LearnSkill(sk, -1);
            }
        }

        void ActivateFilter()
        {
            m_bNoFilter = false;
            UpdateFilter();
        }

        override protected bool IsSkillGood(Skill sk)
        {
            return true;
        }

        override protected bool Delegate_Filter(object x)
        {
            if (m_bNoFilter)
                return true;

            Skill sk = x as Skill;

            if (Data.IsSkillLearned(sk))
                return true;

            return false;
        }
        #endregion
    }
}
