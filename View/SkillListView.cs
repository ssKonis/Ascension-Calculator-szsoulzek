using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrightIdeasSoftware;
using System.Drawing;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;

namespace Ascension_Calculator
{
    abstract public class SkillListView : FastDataListView
    {
        #region TYPES        
        public class SortProperties
        {
            OLVColumn m_olvSortColumn;
            OLVColumn m_olvSortColumnSecondary;
            SortOrder m_SortOrder = SortOrder.Ascending;

            public OLVColumn SortColumnSecondary
            {
                get
                {
                    return m_olvSortColumnSecondary;
                }
            }

            public OLVColumn SortColumn
            {
                get
                {
                    return m_olvSortColumn;
                }

                set
                {
                    m_olvSortColumn = value;

                    // set secondary to default primary (should be name field)
                    if (this.SortColumnSecondary == null)
                        m_olvSortColumnSecondary = value;
                }
            }

            public SortOrder SortOrder
            {
                get
                {
                    return m_SortOrder;
                }

                set
                {
                    m_SortOrder = value;
                }
            }
        }

        public class SkillListGroupingStrategy : FastListGroupingStrategy
        {
            SortProperties m_pSortProperties;

            public SkillListGroupingStrategy(SortProperties props)
            {
                m_pSortProperties = props;
            }

            public override IList<OLVGroup> GetGroups(GroupingParameters parmameters)
            {
                // set parameters for what we want (lul parma)
                parmameters.SortItemsByPrimaryColumn = false;
                parmameters.PrimarySort = m_pSortProperties.SortColumn;
                parmameters.PrimarySortOrder = m_pSortProperties.SortOrder;

                // hardcore secondary sort
                parmameters.SecondarySort = m_pSortProperties.SortColumnSecondary;
                parmameters.SecondarySortOrder = SortOrder.Ascending;

                // do the real work thx
                return base.GetGroups(parmameters);
            }
        }

        class ColumnSetting
        {
            public string m_szAspectName;
            public string m_szText;
            public int m_nWidth;

            public ColumnSetting(string szAspectName, string szText, int nWidth)
            {
                m_szAspectName = szAspectName;
                m_szText = szText;
                m_nWidth = nWidth;
            }
        }

        class GroupComparer : IComparer<OLVGroup>
        {
            public int Compare(OLVGroup x, OLVGroup y)
            {
                string szLHS = x.Key as string;
                string szRHS = y.Key as string;

                if (szLHS == szRHS)
                    return 0;

                if (szLHS.Contains("General"))
                    return 1;

                if (szRHS.Contains("General"))
                    return -1;

                return szLHS.CompareTo(szRHS);
            }
        }
        #endregion

        #region MEMBERS
        SkillListGroupingStrategy grouper;
        SortProperties m_pSortProperties = new SortProperties();
        SkillToolTip m_ToolTip;
        GroupComparer m_GroupComparer = new GroupComparer();
        RowBorderDecoration m_decorRowRed = new RowBorderDecoration();
        #endregion

        #region PROPERTIES
        public bool Constant // cant change the skills within
        {
            get
            {
                return !this.IsSimpleDropSink; // mouse clicking will go off of this
            }

            set
            {
                this.IsSimpleDragSource = this.IsSimpleDropSink = !value;
            }
        }
        #endregion

        #region CTOR
        public SkillListView()
        {
            // forms settings
            Dock = System.Windows.Forms.DockStyle.Fill;

            // my settings
            m_ToolTip = new SkillToolTip(this);
            Options.RegisterEvent(Options.EVENT.OPTIONS_SHOWLISTGROUPS.ToString(), delegate (object x) { this.ShowGroups = Options.ShowListGroups; });
            Options.RegisterEvent(Data.EVENT.DATA_LEARNSKILL.ToString(),
                delegate (object x)
                {
                    if (this.SelectedItem != null)
                    {
                        List<Skill> list = x as List<Skill>;
                        foreach (var sk in list)
                        {
                            if (this.SelectedItem.RowObject == sk)
                            {
                                // unselect
                                this.SelectedItem = null;
                                break; // done obv only 1 thing can be selected (by our settings on the OLV)
                            }
                        }                        
                    }
                    UpdateFilter();
                });

            m_decorRowRed.BorderPen = UITools.Pen_Red;
            m_decorRowRed.BoundsPadding = new Size(1, 1);
            m_decorRowRed.CornerRounding = 4.0f;
            m_decorRowRed.FillBrush = new SolidBrush(Color.FromArgb(32, UITools.Red));
            Constant = false; // can change skills by default thru this listview

            // olv settings
            this.AutoGenerateColumns = false;
            this.ShowGroups = true;
            this.SortGroupItemsByPrimaryColumn = false;
            this.ModelFilter = new ModelFilter(Delegate_Filter);
            this.UseFiltering = true;
            this.ShowGroups = Options.ShowListGroups;
            this.GroupingStrategy = new SkillListGroupingStrategy(m_pSortProperties);
            this.ModelCanDrop += SkillListView_OnModelCanDrop;
            SimpleDropSink sink1 = (SimpleDropSink)DropSink;
            sink1.CanDropOnItem = false;
            sink1.CanDropOnBackground = true;
            SmallImageList = UITools.SmallIconImageList;
            GroupImageList = UITools.ClasstagsImageList;
            ShowFilterMenuOnRightClick = false;
            FullRowSelect = true;
            SelectColumnsOnRightClickBehaviour = ColumnSelectBehaviour.None;
            MultiSelect = false;
            // hot item selection
            RowBorderDecoration rbd = new RowBorderDecoration();
            rbd.BorderPen = new Pen(Color.Black, 2);
            rbd.BoundsPadding = new Size(1, 1);
            rbd.CornerRounding = 4.0f;
            rbd.FillBrush = new SolidBrush(Color.FromArgb(64, 128, 128, 128));
            // Put the decoration onto the hot item
            HotItemStyle = new HotItemStyle();
            HotItemStyle.Decoration = rbd;
            this.UseHotItem = true;
            this.ClearHotItem(); // cuz it defaults to the first item for some reason without any mouse doing
            this.HideSelection = false;
            this.MouseMove += SkillListView_OnMouseMove;
            this.MouseLeave += SkillListView_OnMouseLeave;
            FormatRow += SkillListView_OnFormatRow;
            BeforeCreatingGroups += SkillListView_OnBeforeCreatingGroups;
            AboutToCreateGroups += SkillListView_OnAboutToCreateGroups;
            this.MouseClick += SkillListView_OnMouseClick;
            this.MouseDoubleClick += SkillListView_OnMouseDoubleClick;

            // columns
            List<ColumnSetting> vColumns = new List<ColumnSetting>();
            vColumns.Add(new ColumnSetting("Name", "Name", 200));
            vColumns.Add(new ColumnSetting("Class", "Class", 80));
            vColumns.Add(new ColumnSetting("Spec", "Spec", 100));
            vColumns.Add(new ColumnSetting("RequiredLevel", "Req. Level", 90));
            vColumns.Add(new ColumnSetting("AECost", "AE", 50));
            vColumns.Add(new ColumnSetting("TECost", "TE", 50));

            foreach (var col in vColumns)
            {
                OLVColumn olvCol = GenerateColumn();
                //olvCol.AspectName = col.m_szAspectName;
                // aspectgetter way faster (doesnt use reflection for property names)
                switch (col.m_szAspectName)
                {
                    case "Name":
                        // do our sortprops copy
                        m_pSortProperties.SortColumn = olvCol;
                        // init the real one to sync it
                        this.PrimarySortColumn = olvCol;
                        this.PrimarySortOrder = SortOrder.Ascending;
                        this.SecondarySortColumn = olvCol;
                        this.SecondarySortOrder = SortOrder.Ascending;
                        olvCol.HeaderImageKey = "letter";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).Name; };
                        olvCol.ImageGetter = delegate (object sk) {
                            return (sk as Skill).IconAsString;
                        };
                        break;
                    case "Class":
                        olvCol.HeaderImageKey = "warrior";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).Class; };
                        olvCol.ImageGetter = delegate (object sk) { return (sk as Skill).Class; };
                        break;
                    case "Spec":
                        olvCol.HeaderImageKey = "ability_marksmanship";
                        olvCol.AspectGetter = delegate (object sk) { return UITools.FixSpec((sk as Skill).Spec); };
                        olvCol.ImageGetter = delegate (object sk)
                        {
                            Skill skill = sk as Skill;
                            switch (skill.Spec)
                            {
                                case "Balance":
                                    return "spell_nature_starfall";
                                case "Feral":
                                    return "ability_racial_bearform";
                                case "Restoration":
                                    if (skill.Class == "Druid")
                                        return "spell_nature_healingtouch";
                                    else // shaman
                                        return "spell_nature_magicimmunity";
                                case "BeastMastery":
                                    return "ability_hunter_beasttaming";
                                case "Marksmanship":
                                    return "ability_marksmanship";
                                case "Survival":
                                    return "ability_hunter_swiftstrike";
                                case "Arcane":
                                    return "spell_holy_magicalsentry";
                                case "Fire":
                                    return "spell_fire_flamebolt";
                                case "Frost":
                                    return "spell_frost_frostbolt02";
                                case "Holy":
                                    return "spell_holy_holybolt"; // they both use this (pal/priest)
                                case "Protection":
                                    if (skill.Class == "Paladin")
                                        return "spell_holy_devotionaura";
                                    else // warrior
                                        return "ability_warrior_defensivestance";
                                case "Retribution":
                                    return "spell_holy_auraoflight";
                                case "Discipline":
                                    return "spell_holy_wordfortitude";
                                case "Shadow":
                                    return "spell_shadow_shadowwordpain";
                                case "Assassination":
                                    return "ability_rogue_eviscerate";
                                case "Combat":
                                    return "ability_backstab";
                                case "Subtlety":
                                    return "ability_stealth";
                                case "Elemental":
                                    return "spell_nature_lightning";
                                case "Enhancement":
                                    return "spell_nature_lightningshield";
                                case "Affliction":
                                    return "spell_shadow_deathcoil";
                                case "Demonology":
                                    return "spell_shadow_metamorphosis";
                                case "Destruction":
                                    return "spell_shadow_rainoffire";
                                case "Arms":
                                    return "ability_rogue_eviscerate";
                                case "Fury":
                                    return "ability_warrior_innerrage";
                                case "General":
                                    return "ability_dualwield";
                                default:
                                    return "";
                            }
                        };
                        break;
                    case "RequiredLevel":
                        olvCol.HeaderImageKey = "level";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).RequiredLevel; };
                        break;
                    case "AECost":
                        olvCol.HeaderImageKey = "ae";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).AECost; };
                        break;
                    case "TECost":
                        olvCol.HeaderImageKey = "te";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).TECost; };
                        break;
                }
                olvCol.Text = col.m_szText;
                olvCol.Width = col.m_nWidth;
            }

            SetObjects(new List<Skill>(Data.AvailableList));
        }
        #endregion

        #region METHODS
        protected void SkillListView_OnModelCanDrop(object sender, ModelDropEventArgs e)
        {
            SimpleDropSink sink1 = (SimpleDropSink)DropSink;
            sink1.CanDropOnItem = false; // idk why something is changing it back to default (shit class, shit designer, who cares)
            sink1.CanDropOnBackground = true;

            if (e.SourceListView == this || !ModelDropCheck(e.SourceModels[0] as Skill))
            {
                // cant drop this here
                e.Effect = System.Windows.Forms.DragDropEffects.None;
                sink1.FeedbackColor = UITools.Red;
            }
            else
            {
                // can
                e.Effect = System.Windows.Forms.DragDropEffects.Link;
                sink1.FeedbackColor = UITools.Green;
            }

            e.Handled = true;
        }
                
        abstract protected bool Delegate_Filter(object x);

        abstract protected bool ModelDropCheck(Skill sk);

        private void SkillListView_OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            var hit = OlvHitTest(e.X, e.Y);
            Skill sk = hit?.Item?.RowObject as Skill;

            if (sk != null && e.Button == MouseButtons.Left)
            {
                if (!Constant) // changes allowed
                    Data.LearnSkill(sk, 1);
            }
        }

        private void SkillListView_OnMouseClick(object sender, MouseEventArgs e)
        {
            this.Focus();

            var hit = OlvHitTest(e.X, e.Y);
            Skill sk = hit?.Item?.RowObject as Skill;

            if (sk != null && e.Button == MouseButtons.Left)
            {
                Data.SelectedClasstag = sk.Classtag;
            }
        }

        protected OLVColumn GenerateColumn(int nInsertIndex = -1)
        {
            OLVColumn col = new OLVColumn();
            col.MinimumWidth = 50;
            col.GroupKeyGetter = GroupGetterDelegate;

            if (nInsertIndex >= 0 && nInsertIndex < Columns.Count)
            {
                AllColumns.Insert(nInsertIndex, col);
                Columns.Insert(nInsertIndex, col);
            }
            else
            {
                AllColumns.Add(col);
                Columns.Add(col);
            }

            return col;
        }

        object GroupGetterDelegate(object sk)
        {
            return (sk as Skill).Classtag;
        }

        void SkillListView_OnMouseLeave(object sender, EventArgs e)
        {
            m_ToolTip.Deactivate();
        }

        void SkillListView_OnMouseMove(object sender, MouseEventArgs e)
        {
            var hit = OlvHitTest(e.X, e.Y);
            Skill sk = hit?.Item?.RowObject as Skill;

            if (sk != null)
            {
                m_ToolTip.Activate(sk, e.Location);
            }
            else
            {
                m_ToolTip.Deactivate();
            }
        }
        
        private void SkillListView_OnBeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            e.Parameters.GroupComparer = m_GroupComparer;
        }

        public void UpdateFilter()
        {
            // lol wat this class is so fucking bad - it changes the selectedOBJECT (selecteditem doesnt work)
            // when the filter is udpated? wat?
            int nPriorSelectedIndex = this.SelectedIndex;
            this.UpdateFiltering();
            try
            {
                SelectedIndex = nPriorSelectedIndex;
            }
            catch (Exception) { }
        }

        void SkillListView_OnFormatRow(object sender, FormatRowEventArgs e)
        {
            const float fLightValue = 1.5f;

            Skill sk = (Skill)e.Model;

            // get the real class color
            Color clrClass = UITools.GetClassColor(sk.Class);

            // lighten it up (god damn it looks bad otherwise)
            int nR = Math.Min(255, (int)(clrClass.R * fLightValue));
            int nG = Math.Min(255, (int)(clrClass.G * fLightValue));
            int nB = Math.Min(255, (int)(clrClass.B * fLightValue));

            e.Item.BackColor = Color.FromArgb(nR, nG, nB);

            // check availability of this skill
            if (IsSkillGood(sk))
            {
                e.Item.Decoration = null;
                e.Item.ForeColor = Color.Black;
            }
            else
            {
                e.Item.Decoration = m_decorRowRed;
                e.Item.ForeColor = Color.FromArgb(128, UITools.Red);
            }
        }

        virtual protected bool IsSkillGood(Skill sk)
        {
            return Data.CanSkillBeLearned(sk);
        }

        private void SkillListView_OnAboutToCreateGroups(object sender, CreateGroupsEventArgs e)
        {
            foreach (var grp in e.Groups)
            {
                // the group key contains its classtag property
                grp.TitleImage = "classtag_" + grp.Key;
                grp.Header = ""; // image only please XD
            }
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            // get the new column to sort by
            OLVColumn col = Columns[e.Column] as OLVColumn;

            // figure out asc/desc
            if (m_pSortProperties.SortColumn != col)
            {
                m_pSortProperties.SortOrder = SortOrder.Ascending; // default to asc
            }
            else
            {
                // they clicked on the same one
                if (m_pSortProperties.SortOrder == SortOrder.Ascending)
                    m_pSortProperties.SortOrder = SortOrder.Descending;
                else
                    m_pSortProperties.SortOrder = SortOrder.Ascending;
            }

            // assign the future sort col right now
            m_pSortProperties.SortColumn = col;

            // invoke tons of shit
            base.OnColumnClick(e); // DO THIS LAST - SORTS GROUPS BEFORE NEW LIST SORT DATA GETS UPDATED (hence the sortprops above)

            if (this.SelectedIndex != -1)
                this.EnsureVisible(this.SelectedIndex);
        }
        #endregion
    }
}
