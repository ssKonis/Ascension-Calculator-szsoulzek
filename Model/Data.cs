using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    public class Data : Eventable
    {
        #region CONSTANTS
        const byte MAX_LEVEL = 70;
        const byte MAX_AE = MAX_LEVEL;
        const byte MAX_TE = MAX_LEVEL - 9;
        #endregion

        #region TYPES
        public class ExportDataEntry
        {
            public enum LINETYPE { CLASSTAG, ABILITY, TALENT, NONE, AE, TE, LEVEL, TITLE };

            string m_szText;
            LINETYPE m_nType;
            object m_pData;

            public string Text { get { return m_szText; } }

            public LINETYPE Type { get { return m_nType; } }

            public object Data { get { return m_pData; } }

            public ExportDataEntry(string text, LINETYPE type = LINETYPE.NONE, object data = null)
            {
                m_szText = text;
                m_nType = type;
                m_pData = data;
            }
        }

        public enum INVESTMENT { AE, TE };

        class LoadData
        {
            string m_szSkillName;
            string m_szClasstag;
            int m_nTalentRanks;

            public LoadData(string szClasstag, string skillname, int ranks)
            {
                m_szClasstag = szClasstag;
                m_szSkillName = skillname;
                m_nTalentRanks = ranks;
            }

            public string SkillName { get { return m_szSkillName; } }

            public string Classtag { get { return m_szClasstag; } }

            public int TalentRanks { get { return m_nTalentRanks; } }
        }

        public class Investment
        {
            byte m_nAE;
            byte m_nTE;

            public Investment(int ae = 0, int te = 0)
            {
                AE = ae;
                TE = te;
            }

            public int AE
            {
                get { return m_nAE; }
                set { m_nAE = (byte)value; }
            }

            public int TE
            {
                get { return m_nTE; }
                set { m_nTE = (byte)value; }
            }
        }

        public enum EVENT { DATA_SELECTEDCLASSTAG, DATA_LEARNSKILL, DATA_LEVELLIMIT, DATA_AVAILABLEAE, DATA_AVAILABLETE, DATA_RESETBEGIN, DATA_RESETEND };
        #endregion

        #region MEMBERS
        // the two major lists
        static List<Skill> m_vAvailable = new List<Skill>();
        static List<Skill> m_vLearned = new List<Skill>();

        // store these for quick-speed access
        static Dictionary<string, List<Skill>> m_vTalents = new Dictionary<string, List<Skill>>(); // key is "Class_Spec"
        static Dictionary<string, List<Skill>> m_vAbilities = new Dictionary<string, List<Skill>>(); // key is "Class_Spec"

        // other shit
        static Dictionary<string, Investment> m_vInvestments = new Dictionary<string, Investment>(); // key is "Class_Spec"

        // init these
        static string m_szSelectedClasstag = "Druid_Balance";
        static byte m_nAvailableAE = MAX_AE;
        static byte m_nAvailableTE = MAX_TE;
        static byte m_nLevelLimit = MAX_LEVEL;
        static bool m_bLevelLimitCheck = true;
        static bool m_bSuspendLearnEvent = false;
        #endregion

        #region PROPERTIES
        public static List<ExportDataEntry> ExportData
        {
            get
            {
                /*
                ---Ascension Calculator---

                Required Level:\t47

                AE Spent:\t13
                TE Spent:\t0

                AE Remaining:\t7
                TE Remaining:\t19

                Druid - Balance (1 AE, 40 TE)
                    Wrath
                    Moonkin Form (1/1)

                Warlock - Destruction (5 AE, 7 TE)
                    Immolate
                */

                // prepare
                List<ExportDataEntry> vDataList = new List<ExportDataEntry>();

                // do
                vDataList.Add(new ExportDataEntry("Ascension Calculator", ExportDataEntry.LINETYPE.TITLE));
                vDataList.Add(null);
                int nReqLevel = Data.RequiredLevel;
                vDataList.Add(new ExportDataEntry(string.Format("{0,-16}{1,3}", "Required Level:", nReqLevel), ExportDataEntry.LINETYPE.LEVEL, nReqLevel));
                int nLevelLimit = Data.LevelLimit;
                vDataList.Add(new ExportDataEntry(string.Format("{0,-16}{1,3}", "Level Limit:", nLevelLimit), ExportDataEntry.LINETYPE.LEVEL, nLevelLimit));
                vDataList.Add(null);
                int nInvestmentAE = Data.GetTotalInvestments(Data.INVESTMENT.AE);
                vDataList.Add(new ExportDataEntry(string.Format("{0,-16}{1,3}", "AE Spent:", nInvestmentAE), ExportDataEntry.LINETYPE.AE, nInvestmentAE));
                int nInvestmentTE = Data.GetTotalInvestments(Data.INVESTMENT.TE);
                vDataList.Add(new ExportDataEntry(string.Format("{0,-16}{1,3}", "TE Spent:", nInvestmentTE), ExportDataEntry.LINETYPE.TE, nInvestmentTE));
                vDataList.Add(null);
                int nAvailableAE = Data.AvailableAE;
                vDataList.Add(new ExportDataEntry(string.Format("{0,-16}{1,3}", "AE Remaining:", nAvailableAE), ExportDataEntry.LINETYPE.AE, nAvailableAE));
                int nAvailableTE = Data.AvailableTE;
                vDataList.Add(new ExportDataEntry(string.Format("{0,-16}{1,3}", "TE Remaining:", nAvailableTE), ExportDataEntry.LINETYPE.TE, nAvailableTE));
                vDataList.Add(null);
                
                var vLearned = LearnedListCompiled;

                foreach (KeyValuePair<string, List<Skill>> entry in vLearned)
                {
                    string szFixedClasstag = UITools.FixClasstag(entry.Key);

                    int nInvestedClasstagAE = Data.GetInvestment(entry.Key, Data.INVESTMENT.AE);
                    int nInvestedClasstagTE = Data.GetInvestment(entry.Key, Data.INVESTMENT.TE);
                    string szClasstagInvestment = "(" + nInvestedClasstagAE + " AE, " + nInvestedClasstagTE + " TE)";

                    object[] vClasstagData = new object[3];
                    vClasstagData[0] = entry.Key;
                    vClasstagData[1] = nInvestedClasstagAE;
                    vClasstagData[2] = nInvestedClasstagTE;
                    vDataList.Add(new ExportDataEntry(szFixedClasstag + " " + szClasstagInvestment, ExportDataEntry.LINETYPE.CLASSTAG, vClasstagData));

                    List<Skill> newlist = new List<Skill>(entry.Value);
                    newlist.Sort(); // sort a copied list, not data's list

                    foreach (var sk in newlist)
                    {
                        string szSkillLine = "\t" + sk.Name;
                        ExportDataEntry.LINETYPE type = ExportDataEntry.LINETYPE.ABILITY;

                        if (sk.IsTalent) // rank time for talents only
                        {
                            szSkillLine += " (" + sk.CurrentRanks + "/" + sk.MaxRanks + ")";
                            type = ExportDataEntry.LINETYPE.TALENT;
                        }

                        vDataList.Add(new ExportDataEntry(szSkillLine, type, sk));
                    }

                    // add empty line after each classtag
                    vDataList.Add(null);
                }

                return vDataList;
            }
        }

        public static List<Skill> LearnableSkills
        {
            get
            {
                List<Skill> vResult = new List<Skill>();

                // check every skill (in the combined learned/available lists)
                // to see if it can be learned still
                List<List<Skill>> vAllSkills = new List<List<Skill>>();
                vAllSkills.Add(m_vAvailable);
                vAllSkills.Add(m_vLearned);

                foreach (var list in vAllSkills)
                {
                    foreach (var sk in list)
                    {
                        if (CanSkillBeLearned(sk))
                            vResult.Add(sk);
                    }
                }

                return vResult;
            }
        }

        public static bool LevelLimitCheck
        {
            get { return m_bLevelLimitCheck; }
            set { m_bLevelLimitCheck = value; }
        }

        public static int LevelLimit
        {
            get { return m_nLevelLimit; }

            set
            {
                if (m_nLevelLimit == value)
                    return; // but y tho

                // set caps for sanity
                if (value < 1)
                    value = 1;
                else if (value > Data.MaxLevel)
                    value = Data.MaxLevel;

                // warning checks for level limit
                if (value < m_nLevelLimit && m_bLevelLimitCheck)
                {
                    // WARNING: GOING DOWN
                    if (MessageBox.Show("Decreasing the level limit will reset all skills. Proceed?", "!!!WARNING!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        return; // halt
                }

                int nOldValue = m_nLevelLimit; // useful for people to detect decrease

                if (value < nOldValue) // see above warning
                {
                    ResetSkills();

                    // SET available ae/te based on level limit
                    AvailableAE = (byte)value;
                    AvailableTE = AvailableAE - 9;
                }
                else
                {
                    // going up... add available from difference
                    int nDifference = value - m_nLevelLimit;
                    AvailableAE += nDifference;

                    // super fucking complex formula for TE idk why
                    int nTempLevel = m_nLevelLimit + 1;
                    int nTotalAddedTE = 0;
                    for (int i = 0; i < nDifference; ++i, nTempLevel++)
                    {
                        if (nTempLevel < 10)
                            continue;

                        ++nTotalAddedTE;
                    }
                    AvailableTE += nTotalAddedTE;
                }

                // actually change
                m_nLevelLimit = (byte)value;
                                
                // event callbacks
                Invoke(EVENT.DATA_LEVELLIMIT.ToString(), nOldValue);
            }
        }

        public static string SelectedClasstag
        {
            get { return m_szSelectedClasstag; }

            set
            {
                if (m_szSelectedClasstag == value)
                    return; // but y tho

                string szOldValue = m_szSelectedClasstag;

                m_szSelectedClasstag = value;

                // event callbacks
                Invoke(EVENT.DATA_SELECTEDCLASSTAG.ToString(), szOldValue);
            }
        }

        public static int MaxLevel { get { return (int)MAX_LEVEL; } }

        public static int AvailableAE
        {
            get { return m_nAvailableAE; }

            private set
            {
                if (m_nAvailableAE == value)
                    return;

                int nOldValue = m_nAvailableAE;

                // ensure range
                if (value < 0)
                    value = 0;
                else if (value > MAX_AE)
                    value = MAX_AE;

                m_nAvailableAE = (byte)value;

                // event callbacks
                Invoke(EVENT.DATA_AVAILABLEAE.ToString(), nOldValue);
            }
        }

        public static int AvailableTE
        {
            get { return m_nAvailableTE; }

            private set
            {
                if (m_nAvailableTE == value)
                    return;

                int nOldValue = m_nAvailableTE;

                // ensure range
                if (value < 0)
                    value = 0;
                else if (value > MAX_TE)
                    value = MAX_TE;

                m_nAvailableTE = (byte)value;

                // event callbacks
                Invoke(EVENT.DATA_AVAILABLETE.ToString(), nOldValue);
            }
        }

        public static Dictionary<string, List<Skill>> TalentsList { get { return m_vTalents; } }

        public static Dictionary<string, List<Skill>> AbilitiesList { get { return m_vAbilities; } }

        public static Dictionary<string, List<Skill>> LearnedListCompiled
        {
            get
            {
                SortedDictionary<string, List<Skill>> vDictionary = new SortedDictionary<string, List<Skill>>();
                List<Skill> vGeneral = new List<Skill>(); // cuz i want general to be last
                string szGeneralClasstag = "";

                foreach (var sk in m_vLearned)
                {
                    if (sk.Classtag.Contains("General"))
                    {
                        vGeneral.Add(sk);

                        // record the exact general classtag for future purposes
                        if (szGeneralClasstag == "")
                            szGeneralClasstag = sk.Classtag;
                    }
                    else
                    {
                        if (!vDictionary.ContainsKey(sk.Classtag))
                            vDictionary.Add(sk.Classtag, new List<Skill>());

                        vDictionary[sk.Classtag].Add(sk);
                    }
                }

                Dictionary<string, List<Skill>> vReturn = new Dictionary<string, List<Skill>>(vDictionary);// make it into a "normal" dictionary so the outside world doesnt have to know ;);)

                // add the general tab to the normal one at the end so it wont get sorted
                if (szGeneralClasstag != "") // ofc IF theres any general skills
                    vReturn.Add(szGeneralClasstag, vGeneral);

                return vReturn;
            }
        }

        public static List<Skill> LearnedList { get { return m_vLearned; } }

        public static List<Skill> AvailableList { get { return m_vAvailable; } }

        public static Dictionary<string, Investment> Investments { get { return m_vInvestments; } }

        public static int RequiredLevel
        {
            get
            {
                // do default minimum later, it keeps fucking up the formulas
                int nMaxReqLevel = 0;
                int nMaxAvailableAE = LevelLimit;
                int nMaxAvailableTE = nMaxAvailableAE - 9;

                // adjust according to available ae/te as well (so they cant spam a bunch of tier 1 talents and require level 10)
                int nAESpent = nMaxAvailableAE - m_nAvailableAE;
                nMaxReqLevel = nAESpent; // 1 per level

                // 5 te spent = level 14
                if (nMaxAvailableTE > 0) // if we can even have talents, check it
                {
                    int nTESpent = nMaxAvailableTE - AvailableTE;
                    if (nTESpent > 0 && 9 + nTESpent > nMaxReqLevel)
                        nMaxReqLevel = 9 + nTESpent; // 9 because te is inclusive starting at 10
                }

                // find highest learned skill to overwrite that just in case (only going higher of course)
                foreach (var sk in m_vLearned)
                {
                    if (sk.RequiredLevel > nMaxReqLevel)
                        nMaxReqLevel = sk.RequiredLevel;
                }

                // done
                return nMaxReqLevel > 0 ? nMaxReqLevel : 1; // cap it at 1 minimum
            }
        }
        #endregion

        #region CTOR
        static Data()
        {
            // load database
            // check for custom database first
            string[] szDB;
            try
            {
                StreamReader input = new StreamReader(IOManager.DatabaseFile);
                szDB = input.ReadToEnd().Split("\n".ToCharArray());
                input.Close();
            }
            catch (Exception)
            {
                // fail silently. no custom db found. load the internal instead
                szDB = Ascension_Calculator.Properties.Resources.database.Split("\n".ToCharArray());
            }

            //Skill finalsk = null; // ??? - maybe trying to fetch a random skill to learn for the earlier bugfix attempt of empty learned list
            int nLineCount = 1;
            foreach (string szCurrLine in szDB)
            {
                if (szCurrLine == null)
                    continue; // super scared of all errors

                string szLine = szCurrLine.Trim(); // remove leading and trailing crap
                // error checking db file
                if (szLine == null)
                    continue;
                else if (szLine == "")
                    continue;
                else if (szLine.StartsWith("//"))
                    continue;

                // attempt to parse
                Skill sk;
                try
                {
                    string[] szData = szLine.Split("|".ToCharArray());

                    // make the skill
                    sk = new Skill(szData);
                }
                catch (Exception)
                {
                    // something is bad, relay that to user
                    if (MessageBox.Show("Error loading database on Line: #" + nLineCount + "\nContinue loading database?", "ERROR", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        continue;
                    else
                        break;
                }

                //finalsk = sk;
                string szKey2 = sk.Classtag;

                // add to either talent or ability lists
                if (sk.IsTalent)
                {
                    // add (and prepare) to talent map
                    if (!m_vTalents.ContainsKey(szKey2))
                        m_vTalents.Add(szKey2, new List<Skill>());

                    m_vTalents[szKey2].Add(sk);
                }                
                else
                {
                    // add (and prepare) to ability map
                    if (!m_vAbilities.ContainsKey(szKey2))
                        m_vAbilities.Add(szKey2, new List<Skill>());

                    m_vAbilities[szKey2].Add(sk);
                }

                // add this skill to the database (in the proper section)
                string szKey = sk.Classtag;
                m_vAvailable.Add(sk);

                // prepare the investments classtags
                if (!m_vInvestments.ContainsKey(szKey))
                    m_vInvestments.Add(szKey, new Investment());

                // update line counter
                nLineCount++;
            }
        }
        #endregion

        #region METHODS
        public static void GenerateRandomBuild()
        {
            if (MessageBox.Show("This will spend the remainder of your available AE and TE points in randomly chosen skills (if possible). Proceed?", "!!!WARNING!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return; // halt

            List<Skill> vSkills = Data.LearnableSkills; // get master list of everything
            if (vSkills.Count == 0)
            {
                MessageBox.Show("There were no possible skills to learn.", "Ascension Calculator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return; // can't learn anything
            }

            // ok we're good - disable events for optimization
            SuspendLearnEvent(true);

            // prepare stuff
            List<Skill> vEventSkills = new List<Skill>();
            Random rnd = new Random();

            // pick a random skill so long as there are any to pick from
            while (vSkills.Count > 0)
            {
                int nRNGIndex = rnd.Next() % vSkills.Count;
                Skill sk = vSkills[nRNGIndex];

                // try to learn this skill (try because this could be a talent that was learned previously and no longer can be)
                if (!Data.LearnSkill(sk, 1))
                {
                    // if it couldn't be learned - remove this skill... and try again obv
                    vSkills.RemoveAt(nRNGIndex); // cheeky! use the same index for a remove without the find attached to it :)
                }
                else
                {
                    // it was learned, so record that for the mass event notification (unique list, dont double add shit)
                    if (!vEventSkills.Contains(sk))
                    {
                        vEventSkills.Add(sk);
                    }
                }
            }

            // finished - notify everybody
            SuspendLearnEvent(false, vEventSkills);

            // now show the learned list to be cool
            new SkillsGeneratedDlg(vEventSkills).ShowDialog();
        }

        public static bool LearnSkill(Skill sk, int nAdjustmentValue)
        {
            if (nAdjustmentValue == 0)
                return false; // but y tho

            if (nAdjustmentValue > 0)
            {
                if (!CanSkillBeLearned(sk, (uint)nAdjustmentValue)) // going up but cant, done
                    return false;
            }

            // optimization for multiple usage
            bool bContains = m_vLearned.Contains(sk);

            // see if cant unlearn something we dont have
            if (nAdjustmentValue < 0 && !bContains)
                return false;

            if (sk.IsAbility) // make sure we're not trying to rank an ability invalidly
            {
                // cant adjust by more than 1 for abilities
                if (nAdjustmentValue != -1 && nAdjustmentValue != 1)
                    return false;
            }

            // see if this, as a talent, can be adjusted by this much
            if (!sk.AdjustRank(nAdjustmentValue) && sk.IsTalent) // will catch going down too much on learned talents
                return false; // capped out on either end, sorry

            // it happened, do adjustments
            int nAEAdjustment = sk.AECost * nAdjustmentValue;
            int nTEAdjustment = sk.TECost * nAdjustmentValue;

            // adjust currency
            AvailableAE = AvailableAE - nAEAdjustment;
            AvailableTE = AvailableTE - nTEAdjustment;

            // adjust investment
            // get the classtag of this skill
            string szClasstag = sk.Classtag;

            // update the investment by pointer
            Investment pInvestment = m_vInvestments[szClasstag];
            pInvestment.AE += nAEAdjustment;
            pInvestment.TE += nTEAdjustment;

            // is now officially "learned" or "unlearned"
            if (!bContains) // learning
            {
                m_vLearned.Add(sk);
                m_vAvailable.Remove(sk);
            }
            else //unlearning (OR LOSING SOME RANKS)
            {
                bool bRemove = false;
                if (sk.IsAbility) // if its an ability its just gone
                    bRemove = true;
                else if (sk.CurrentRanks == 0)// if its a talent AND it has 0 curr ranks, its gone
                    bRemove = true;
                if (bRemove)
                {
                    m_vLearned.Remove(sk);
                    m_vAvailable.Add(sk);
                }
            }

            if (m_bSuspendLearnEvent == false)
            {
                List<Skill> list = new List<Skill>();
                list.Add(sk); // everybody expecting a list for mass, gotta have singles too

                // pass arg as the actual skill (up to the receiver to decypher polymorphic type)
                Invoke(EVENT.DATA_LEARNSKILL.ToString(), list);
            }

            return true; // all good
        }

        public static Skill LearnSkillByName(string szClasstag, string szSkill, int nAdjustmentValue = 1)
        {
            try
            {
                // try abilities (easier than by learned vs unlearned)
                Skill sk = m_vAbilities[szClasstag].Find(x => x.Name == szSkill);

                if (sk == null)
                {
                    // try talents
                    sk = m_vTalents[szClasstag].Find(x => x.Name == szSkill);
                }

                if (sk == null)
                    return null; // idk

                if (LearnSkill(sk, nAdjustmentValue))
                    return sk;
            }
            catch (Exception) // fail silently
            {
            }

            return null;
        }

        public static bool IsSkillLearned(Skill sk)
        {
            return m_vLearned.Contains(sk);
        }

        public static bool IsSkillLearned(string szClasstag, string szSkillName)
        {
            return m_vLearned.Find(sk => sk.Name == szSkillName && sk.Classtag == szClasstag) != null;
        }

        static void SuspendLearnEvent(bool suspend, List<Skill> vSkills = null)
        {
            if (suspend)
            {
                // warn everybody for mass
                Invoke(EVENT.DATA_RESETBEGIN.ToString(), null);

                // disable mass events
                m_bSuspendLearnEvent = true;
            }
            else
            {
                // reenable events
                m_bSuspendLearnEvent = false;

                // invoke mass unlearn with the entire collection of unlearned skills
                Invoke(EVENT.DATA_LEARNSKILL.ToString(), vSkills);

                // resume actions for mass
                Invoke(EVENT.DATA_RESETEND.ToString(), null);
            }
        }

        public static void ResetSkills()
        {
            SuspendLearnEvent(true);

            // store learned skills for event arg
            List<Skill> vUnlearned = new List<Skill>(m_vLearned);

            // do something with entry.Value or entry.Key
            for (int i = m_vLearned.Count - 1; i >= 0; --i)
            {
                UnlearnSkill(m_vLearned[i]);
            }

            SuspendLearnEvent(false, vUnlearned);
        }

        public static bool UnlearnSkill(Skill sk)
        {
            if (sk.IsTalent)
                return LearnSkill(sk, -sk.CurrentRanks); // go down by how many ranks it has invested
            else
                return LearnSkill(sk, -1);
        }

        public static int GetTotalInvestments(INVESTMENT type)
        {
            int nTotal = 0;

            foreach (KeyValuePair<string, Investment> inv in m_vInvestments)
            {
                if (type == INVESTMENT.AE)
                    nTotal += inv.Value.AE;
                else if (type == INVESTMENT.TE)
                    nTotal += inv.Value.TE;
            }

            return nTotal;
        }

        public static int GetInvestment(string szClasstag, INVESTMENT type)
        {
            if (type == INVESTMENT.AE)
                return m_vInvestments[szClasstag].AE;
            else if (type == INVESTMENT.TE)
                return m_vInvestments[szClasstag].TE;

            return 0; // wat
        }

        public static bool CanSkillBeLearned(Skill sk, uint nAdjustmentValue = 1)
        {
            // manually check EXCLUSIVE SKILLS
            // dual wield (enhancement) and dual wield (general)
            if (nAdjustmentValue > 0)
            {
                if (sk.Name == "Dual Wield" && (IsSkillLearned("General_General", "Dual Wield") || IsSkillLearned("Shaman_Enhancement", "Dual Wield")))
                    return false;
            }

            // check to see if already learned and cant go up any more
            if (sk.IsAbility)
            {
                if (nAdjustmentValue > 1)
                    return false; // cant learn ability more than 1 rank

                if (IsSkillLearned(sk))
                    return false; // cant double learn lul
            }
            else if (sk.IsTalent)
            {
                // check going up
                if (sk.CurrentRanks + nAdjustmentValue > sk.MaxRanks)
                    return false;

                // check TE cost
                if (AvailableTE < sk.TECost * nAdjustmentValue)
                    return false;
            }

            // check level limit easily
            if (LevelLimit < sk.RequiredLevel)
                return false;

            // check AE cost
            if (AvailableAE < sk.AECost)
                return false;

            // guess saul goodman
            return true;
        }

        public static bool LoadFromString(string szData, bool bAskToContinue)
        {
            // prepare the storage of shit to load (because might not do anything depending on file validity)
            List<LoadData> vLoadData = new List<LoadData>();
            int nNewLevelLimit = 1;

            try
            {
                /*
                 * Ascension Calculator

                    Required Level:  59
                    Level Limit:     60

                    AE Spent:        57
                    TE Spent:        50

                    AE Remaining:     3
                    TE Remaining:     1

                    Druid - Balance (0 AE, 1 TE)
	                    Owlkin Frenzy (1/3)

                    Hunter - Beast Mastery (4 AE, 2 TE)
	                    Animal Handler (1/2)
	                    Invigoration (1/2)
	                    Scare Beast
	                    Tame Beast

                    General (2 AE, 0 TE)
	                    Parry
                 */
                // get each line
                string[] szLines = szData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string szCurrentClasstag = "";

                foreach (var line in szLines)
                {
                    if (line.Contains("Level Limit:"))
                    {
                        int nFinalSpaceIndex = line.LastIndexOf(' ');
                        nNewLevelLimit = Int32.Parse(line.Substring(nFinalSpaceIndex + 1)); // + 1 to not include the space
                    }
                    else if (line[0] == '\t') // skill line
                    {
                        string szSkillName;
                        int nRanks = 1; // assume 1 because abilities wont change this at all

                        // no skill name contains a '/' (thank fuck)
                        if (line.Contains('/')) // talent
                        {
                            int nSlashIndex = line.IndexOf('/');
                            nRanks = Int32.Parse(line[nSlashIndex - 1].ToString());
                            szSkillName = line.Substring(1, nSlashIndex - 3 - 1);
                        }
                        else
                            szSkillName = line.TrimStart("\t".ToCharArray());

                        // construct this pretend skill for later additage
                        vLoadData.Add(new LoadData(szCurrentClasstag, szSkillName, nRanks));
                    }
                    else if (line.Contains('(')) // classtag line
                    {
                        // rip the classtag
                        int nIndex = line.IndexOf(" (");
                        szCurrentClasstag = UITools.FixClasstag(line.Substring(0, nIndex), true);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("This file is corrupt. This can happen from trying to load ascension calculator files before version 2.0 or a manually modified file.",
                    "!!!ERROR!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // didnt work :(
            }

            // now, ask if they want to continue
            if (bAskToContinue)
            {
                if (MessageBox.Show("All skills will be reset to perform this operation. Proceed?", "!!!WARNING!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return false;
            }

            // attempt to learn every skill in the list now that the file i/o was successful (and they wanted to)
            // (remember skills can change names in patches. make it so it "fails" but keeps going silently)

            // kill em all, first
            ResetSkills();

            // set new level limit, without warning
            LevelLimitCheck = false;
            LevelLimit = nNewLevelLimit;
            LevelLimitCheck = true;

            SuspendLearnEvent(true);
            // check for missing skills and buil the collection of learned skills
            List<Skill> vLearning = new List<Skill>();
            bool bGood = true;

            foreach (var ld in vLoadData)
            {
                Skill sk = Data.LearnSkillByName(ld.Classtag, ld.SkillName, ld.TalentRanks);
                if (sk == null)
                    bGood = false;
                else
                    vLearning.Add(sk);
            }

            SuspendLearnEvent(false, vLearning);

            if (!bGood) // warn on skill failure
            {
                MessageBox.Show("This file contains skills that could not be found or learned properly. " +
                    "The remainder have been loaded. " +
                    "Try downloading the newest version of the calculator and manually remaking the build.",
                    "!!!WARNING!!!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return true; // sall good man
        }
        #endregion
    }
}
