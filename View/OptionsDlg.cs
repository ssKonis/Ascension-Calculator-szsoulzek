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
    public partial class OptionsDlg: Form
    {
        #region CTOR
        public OptionsDlg()
        {
            InitializeComponent();
            
            // set valid ranges
            numLevelLimit.Maximum = Data.MaxLevel;

            // set up controls based on global options
            chkCheckForUpdates.Checked = Options.CheckForUpdates;
            numLevelLimit.Value = Data.LevelLimit;
            if (Options.ExportBackground == "")
                radExportGradient.Checked = true;
            else if (Options.ExportBackground == "parchment")
                radExportBackgroundParchment.Checked = true;
            else if (Options.ExportBackground == "marble")
                radExportBackgroundMarble.Checked = true;
            else if (Options.ExportBackground == "rock")
                radExportBackgroundRock.Checked = true;
            pbExportGradientLeft.BackColor = Options.ExportGradientLeft;
            pbExportGradientRight.BackColor = Options.ExportGradientRight;
            if (Options.PanelsTalentsAbilities)
                radTalentsAbilities.Checked = true;
            else
                radAbilitiesTalents.Checked = true;
        }
        #endregion

        #region METHODS
        private void btnOK_Click(object sender, EventArgs e)
        {
            // send em back
            // level limit can halt due to warning, check for that
            Data.LevelLimit = (int)numLevelLimit.Value;
            if (Data.LevelLimit != (int)numLevelLimit.Value)
            {
                return; // halted by choice from user
            }
            // other options
            Options.CheckForUpdates = chkCheckForUpdates.Checked;
            Options.ExportGradientLeft = pbExportGradientLeft.BackColor;
            Options.ExportGradientRight = pbExportGradientRight.BackColor;
            if (radExportBackgroundMarble.Checked)
                Options.ExportBackground = "marble";
            else if (radExportBackgroundParchment.Checked)
                Options.ExportBackground = "parchment";
            else if (radExportBackgroundRock.Checked)
                Options.ExportBackground = "rock";
            else if (radExportGradient.Checked)
                Options.ExportBackground = "";
            Options.PanelsTalentsAbilities = radTalentsAbilities.Checked;

            // manual close to allow prevention from above
            Close();
        }
        #endregion

        private void ExportGradient_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;

            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pb.BackColor = dlg.Color;
            }
        }
    }
}
