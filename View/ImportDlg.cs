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
    public partial class ImportDlg : Form
    {
        public ImportDlg()
        {
            InitializeComponent();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            // attempt to parse
            if (Data.LoadFromString(txtText.Text, true))
            {
                Close();
            }
        }
    }
}
