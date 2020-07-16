using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Ascension_Calculator
{   
    public class Skill : IComparable, INotifyPropertyChanged
    {
        #region TYPES
        public class CompareByRequiredLevel : IComparer<Skill>
        {
            public int Compare(Skill lhs, Skill rhs)
            {
                if (lhs.RequiredLevel < rhs.RequiredLevel)
                    return -1;
                else if (lhs.RequiredLevel > rhs.RequiredLevel)
                    return 1;

                return lhs.CompareTo(rhs);
            }
        }
        #endregion

        #region MEMBERS
        string m_szIcon;
        string m_szName;
        string m_szText;
        byte m_nReqLevel;
        byte m_nAECost;
        string m_szClass;
        string m_szSpec;
        byte m_nTECost;
        byte m_nMaxRanks;
        byte m_nCurrRanks;
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region PROPERTIES
        public string Name { get { return m_szName; } }

        public string Text { get { return m_szText; } }

        public int RequiredLevel { get { return m_nReqLevel; } }

        public int AECost { get { return m_nAECost; } }

        public string Class { get { return m_szClass; } }

        public string Spec { get { return m_szSpec; } }

        public string Classtag { get { return Class + "_" + Spec; } }

        public Bitmap IconAsBitmap { get { return Ascension_Calculator.Properties.Resources.ResourceManager.GetObject(m_szIcon) as Bitmap; } }

        public string IconAsString { get { return m_szIcon; } }

        public int TECost { get { return m_nTECost; } }

        public int MaxRanks { get { return m_nMaxRanks; } }

        public int CurrentRanks { get { return m_nCurrRanks; } }

        public bool IsTalent { get { return m_nTECost != 0; } }
        
        public bool IsAbility { get { return m_nTECost == 0; } }
        #endregion

        #region CTOR
        public Skill(string[] szData)
        {
            //Wrath|Costs Mana\t30 yd range\n1.5 sec cast\nCauses Nature damage to the target.|spell_nature_abolishmagic|1|2|0|Druid|Balance|0|

            // name
            m_szName = szData[0];

            // tooltip
            // remove all "\\" from the tooltip text by turning them into just "\"
            m_szText = szData[1].Replace("\\t", "\t").Replace("\\n", "\n");

            // icon
            m_szIcon = szData[2];

            // required level
            m_nReqLevel = Byte.Parse(szData[3]);

            // ae cost
            m_nAECost = Byte.Parse(szData[4]);

            // te cost
            m_nTECost = Byte.Parse(szData[5]);

            // class
            m_szClass = szData[6];

            // spec
            m_szSpec = szData[7];

            // max ranks
            m_nMaxRanks = Byte.Parse(szData[8]);
        }
        #endregion

        #region METHODS
        public int CompareTo(object obj) // for list.sort
        {
            return Name.CompareTo((obj as Skill).Name);
        }

        public bool AdjustRank(int nAdjustmentValue)
        {
            if (nAdjustmentValue == 0 || IsAbility)
                return false; // wat

            int nFinalRank = CurrentRanks + nAdjustmentValue;
            if (nFinalRank < 0 || nFinalRank > MaxRanks)
                return false; // no can do

            // do it
            m_nCurrRanks += (byte)nAdjustmentValue;

            NotifyPropertyChanged("CurrentRanks"); // hmmm, a skill only ever changes by ranks (learned unlearned list are in data and arent props of my model)

            return true;
        }

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
