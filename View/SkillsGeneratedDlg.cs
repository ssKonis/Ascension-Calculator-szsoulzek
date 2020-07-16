using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    public partial class SkillsGeneratedDlg : Form
    {
        public SkillsGeneratedDlg(List<Skill> vSkills)
        {
            InitializeComponent();

            LearnedListView llv = new LearnedListView();

            // reconfigure
            llv.SetObjects(vSkills);
            llv.Constant = true;

            gfxPanel.Controls.Add(llv);
        }
    }
}
