using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;

namespace Ascension_Calculator
{
    /*public class LearnedListView : SkillListView
    {
        #region CTOR
        public LearnedListView()
        {
            OLVColumn olvCol = GenerateColumn(1);
            olvCol.HeaderImageKey = "rankup";
            olvCol.AspectGetter = delegate (object sk) {
                if (sk is Talent)
                    return (sk as Talent).Talent_CurrentRanks;
                else
                    return null;
            };

            olvCol.Text = "Talent Rank";
            olvCol.Width = 100;
        }
        #endregion

        #region METHODS
        override protected bool Delegate_Filter(object x)
        {
            return Data.IsSkillLearned(x as Skill);
        }
        #endregion
    }*/
}
