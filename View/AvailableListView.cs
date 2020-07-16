using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;

namespace Ascension_Calculator
{
    class AvailableListView : SkillListView
    {
        #region TYPES
        public enum SEARCHTYPE { WORDS_ANY, WORDS_ALL, EXACT };

        public class FilterProperties
        {
            string m_szNameFilter;
            string m_szTooltipFilter;
            SEARCHTYPE m_nTooltipSearchType;
            bool m_bAEFilter;
            bool m_bTEFilter;
            bool m_bLevelFilter;

            public string NameFilter { get { return m_szNameFilter; } }

            public string TooltipFilter { get { return m_szTooltipFilter; } }

            public SEARCHTYPE TooltipSearchType { get { return m_nTooltipSearchType; } }

            public bool AEFilter { get { return m_bAEFilter; } }

            public bool TEFilter { get { return m_bTEFilter; } }

            public bool LevelFilter { get { return m_bLevelFilter; } }

            public FilterProperties(string name, string tooltip, SEARCHTYPE type, bool ae, bool te, bool level)
            {
                m_szNameFilter = name;
                m_szTooltipFilter = tooltip;
                m_nTooltipSearchType = type;
                m_bAEFilter = ae;
                m_bTEFilter = te;
                m_bLevelFilter = level;
            }
        }
        #endregion

        #region MEMBERS
        FilterProperties m_FilterProperties = new FilterProperties("", "", SEARCHTYPE.WORDS_ANY, true, true, true);
        #endregion

        #region CTOR
        public AvailableListView()
        {
            this.ModelDropped += AvailableListView_OnModelDropped;
        }
        #endregion

        #region METHODS
        private void AvailableListView_OnModelDropped(object sender, ModelDropEventArgs e)
        {
            Data.UnlearnSkill(e.SourceModels[0] as Skill);

            e.Handled = true;
        }

        protected override bool ModelDropCheck(Skill sk)
        {
            return true; // can always drop any skill here (if enforced by skill data only)
        }

        override protected bool Delegate_Filter(object x)
        {
            Skill sk = x as Skill;

            // dont show learned skills in the available list
            if (Data.IsSkillLearned(sk))
                return false;

            // check level limits
            if (m_FilterProperties.LevelFilter && sk.RequiredLevel > Data.LevelLimit)
                return false;

            // check ae cost
            if (m_FilterProperties.AEFilter && sk.AECost > Data.AvailableAE)
                return false;

            // check te cost
            if (m_FilterProperties.TEFilter && sk.TECost > Data.AvailableTE)
                return false;

            // check text filters
            if (m_FilterProperties.NameFilter != string.Empty)
            {
                if (sk.Name.ToLower().Contains(m_FilterProperties.NameFilter) == false) // doesnt have the filter name we're looking for if we have one
                    return false;
            }

            if (m_FilterProperties.TooltipFilter != string.Empty)
            {
                string szSkillTTLowered = sk.Text.ToLower(); // optimization, do it only once conditionally now

                if (m_FilterProperties.TooltipSearchType == SEARCHTYPE.EXACT)
                {
                    if (szSkillTTLowered.Contains(m_FilterProperties.TooltipFilter) == false) // doesnt have the filter tooltip we're looking for if we have one
                        return false;
                }
                else // search by words by space
                {
                    string[] szWords = m_FilterProperties.TooltipFilter.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    // check each word
                    bool bGood = false;
                    foreach (var word in szWords)
                    {
                        if (szSkillTTLowered.Contains(word))
                        {
                            bGood = true; // ANY words is safe
                        }
                        else if (m_FilterProperties.TooltipSearchType == SEARCHTYPE.WORDS_ALL)
                        {
                            return false; // didnt contain a word during type ALL words
                        }
                    }

                    if (bGood == false) // no words found, bad result for any search type
                        return false;
                }
            }

            // passed all tests, show it
            return true;
        }

        public void UpdateFilter(FilterProperties props)
        {
            // capture form1 controls info
            m_FilterProperties = props;

            // invoke the filter
            base.UpdateFilter();
        }
        #endregion
    }
}
