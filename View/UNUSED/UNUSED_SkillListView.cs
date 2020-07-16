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

namespace Ascension_Calculator
{
    /*public abstract class SkillListView : FastObjectListView
    {
        #region TYPES
        abstract protected bool Delegate_Filter(object x);

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
        SortProperties m_pSortProperties = new SortProperties();
        SkillToolTip m_ToolTip = new AbilityToolTip();
        #endregion

        #region CTOR
        public SkillListView()
        {
            // my stuff
            m_ToolTip.SetControl(this);

            // register events
            Data.RegisterEvent(Data.EVENT.LEARNSKILL.ToString(),
                delegate (object sk)
                {
                    if (this.SelectedItem != null)
                    {
                        if (this.SelectedItem.RowObject == sk)
                        {
                            // unselect
                            this.SelectedItem = null;
                        }
                    }
                    UpdateFilter();
                });
            Options.RegisterEvent(Options.EVENT.SHOWLISTGROUPS.ToString(),
                delegate (object x)
                {
                    this.ShowGroups = Options.ShowListGroups;
                    this.RebuildColumns();
                    UpdateFilter();
                    if (this.SelectedIndex != -1)
                        this.EnsureVisible(this.SelectedIndex);
                });

            // init column settings
            List<ColumnSetting> vColumns = new List<ColumnSetting>();
            vColumns.Add(new ColumnSetting("Skill_Name", "Name", 200));
            vColumns.Add(new ColumnSetting("Skill_RequiredLevel", "Req. Level", 100));
            vColumns.Add(new ColumnSetting("Skill_AECost", "AE", 100));
            vColumns.Add(new ColumnSetting("Talent_TECost", "TE", 100));

            foreach (var col in vColumns)
            {
                OLVColumn olvCol = GenerateColumn();
                //olvCol.AspectName = col.m_szAspectName;
                // V way faster!!! also can manually handle the TECost issue of availability!!!
                switch (col.m_szAspectName)
                {
                    case "Skill_Name":
                        // do our sortprops copy
                        m_pSortProperties.SortColumn = olvCol;
                        // init the real one to sync it
                        this.PrimarySortColumn = olvCol;
                        this.PrimarySortOrder = SortOrder.Ascending;
                        olvCol.HeaderImageKey = "letter";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).Name; };
                        break;
                    case "Skill_RequiredLevel":
                        olvCol.HeaderImageKey = "level";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).RequiredLevel; };
                        break;
                    case "Skill_AECost":
                        olvCol.HeaderImageKey = "ae";
                        olvCol.AspectGetter = delegate (object sk) { return (sk as Skill).AECost; };
                        break;
                    case "Talent_TECost":
                        olvCol.HeaderImageKey = "te";
                        olvCol.AspectGetter = delegate (object sk)
                        {
                            if (sk is Talent)
                                return (sk as Talent).Talent_TECost;
                            else
                                return null;
                        };
                        break;
                }
                olvCol.Text = col.m_szText;
                olvCol.Width = col.m_nWidth;
            }

            // init object list view itself settings
            AboutToCreateGroups += SkillListView_AboutToCreateGroups;
            SmallImageList = UITools.SmallIconImageList;
            GroupImageList = UITools.ClasstagsImageList;
            ShowFilterMenuOnRightClick = false;
            CellEditUseWholeCell = false;
            Cursor = System.Windows.Forms.Cursors.Default;
            Location = new System.Drawing.Point(3, 16);
            Size = new System.Drawing.Size(730, 279);
            UseCompatibleStateImageBehavior = false;
            View = System.Windows.Forms.View.Details;
            UseFiltering = true;
            ModelFilter = new ModelFilter(Delegate_Filter);
            FormatRow += OnFormatRow;
            BeforeCreatingGroups += SkillListView_BeforeCreatingGroups;
            FullRowSelect = true;
            SelectColumnsOnRightClickBehaviour = ColumnSelectBehaviour.None;
            MultiSelect = false;
            this.MouseEnter += OnMouseEnter;

            // hot item selection
            // Make the decoration
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
            this.MouseMove += SkillListView_MouseMove;
            this.MouseLeave += SkillListView_MouseLeave;

            // groups
            this.ShowGroups = Options.ShowListGroups;
            this.GroupingStrategy = new SkillListGroupingStrategy(m_pSortProperties);

            // init column images (on the name column only)
            AllColumns[0].ImageGetter = delegate (object sk) {
                return (sk as Skill).IconAsString;
            };
        }
        #endregion

        #region METHODS
        private void SkillListView_MouseLeave(object sender, EventArgs e)
        {
            m_ToolTip.Deactivate();
        }

        private void SkillListView_MouseMove(object sender, MouseEventArgs e)
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

        private void OnMouseEnter(object sender, EventArgs e)
        {
            this.Focus();
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            if (SelectedItem != null)
            {
                Data.SelectedClasstag = (SelectedItem.RowObject as Skill).Classtag;
            }
        }
        
        object GroupGetterDelegate(object sk)
        {
            return (sk as Skill).Classtag;
        }

        protected OLVColumn GenerateColumn(int nInsertIndex = -1)
        {
            OLVColumn col = new OLVColumn();
            col.MinimumWidth = 50;
            col.GroupKeyGetter = GroupGetterDelegate;

            if (nInsertIndex >= 0 && nInsertIndex < Columns.Count)
            {
                AllColumns.Insert(1, col);
                Columns.Insert(1, col);
            }
            else
            {
                AllColumns.Add(col);
                Columns.Add(col);
            }

            return col;
        }
        
        private void SkillListView_BeforeCreatingGroups(object sender, CreateGroupsEventArgs e)
        {
            e.Parameters.GroupComparer = new GroupComparer();
        }

        void OnFormatRow(object sender, FormatRowEventArgs e)
        {
            const float fLightValue = 1.5f;

            Skill sk = (Skill)e.Model;

            // get the real class color
            Color clrClass = UITools.GetClassColor(sk.Class);

            // lighten it up (god damn it looks bad)
            int nR = Math.Min(255, (int)(clrClass.R * fLightValue));
            int nG = Math.Min(255, (int)(clrClass.G * fLightValue));
            int nB = Math.Min(255, (int)(clrClass.B * fLightValue));

            e.Item.BackColor = Color.FromArgb(nR, nG, nB);
        }

        public void UpdateFilter()
        {
            // lol wat this class is so fucking bad - it changes the selectedOBJECT (selecteditem doesnt work)
            // when the filter is udpated? wat?
            object item = this.SelectedObject;
            // TODO
            this.UpdateFiltering(); // ... is this the crash? the way this reapplys the filtering? (NOPE) - dont use imodelfilter on virtuallists!!!
            //ModelFilter = ModelFilter; // reapply filtering normally
            if (item != null)
            {
                this.SelectedObject = item;
                //EnsureModelVisible(item);
            }
        }
                
        private void SkillListView_AboutToCreateGroups(object sender, CreateGroupsEventArgs e)
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
    }*/
}
