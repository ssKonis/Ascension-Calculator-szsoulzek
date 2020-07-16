using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator.View
{
    public partial class CustomDatabase : Form
    {
        public CustomDatabase()
        {
            InitializeComponent();

            string szBoldWord = IOManager.DatabaseFile;
            txtInfo.Select(txtInfo.Find(szBoldWord), szBoldWord.Length);
            txtInfo.SelectionFont = new Font(txtInfo.Font, FontStyle.Bold);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string szExportFileName = IOManager.InternalDatabaseFile;
            File.WriteAllText(szExportFileName, Ascension_Calculator.Properties.Resources.database);

            MessageBox.Show("The internal database has been exported as " + szExportFileName);
        }
    }
}
