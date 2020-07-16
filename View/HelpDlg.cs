using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    public partial class HelpDlg : Form
    {
        #region METHODS
        public HelpDlg()
        {
            InitializeComponent();

            richTextBox1.Text = "SIDE PANEL: Left-click a spec to go to its layout panel." +
                "\n\nTOP PANELS: Left-click a skill to learn it (or add points in a talent). Hold Ctrl to max out a talent." +
                "\nRight-click a skill to unlearn it (or remove points in a talent). Hold Ctrl to remove all points in a talent." +
                "\n\nBOTTOM LISTS: Left-click a skill to go to its layout panel." +
                "\nLearn or unlearn a skill by left-click dragging and dropping it between the two lists." +
                "\nDouble left-click a skill to learn it (or add points in a talent)." +
                "\nRight-click a skill to unlearn it (or remove points in a talent)." +
                "\n\nHover over the investment graphs on the bottom of the window to see detailed investment percentages." +
                "\n\nPaypal donations accepted at: szsoulzek@gmail.com" +
                "\n\nVisit the official Ascension forums for calculator version updates as well as additional FAQs and tips for Ascension here:" +
                "\n" + VersionChecker.FAQURL;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=N6Z28DCL2G7CW");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(VersionChecker.FAQURL);
        }
        #endregion
    }
}
