using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    public class EZToolTip : ToolTip
    {
        #region MEMBERS
        Control m_Control;
        #endregion

        #region PROPERTIES
        protected Control Control { get { return m_Control; } }
        #endregion

        #region CTOR
        public EZToolTip(Control ctrl)
        {
            Deactivate(); // dont show initially ofc
            this.UseFading = false;

            m_Control = ctrl;
            this.SetToolTip(ctrl, "UNUSED");
        }
        #endregion

        #region METHODS
        public void Activate(string szText, Point ptLoc)
        {
            // show it
            this.Active = true;
            IWin32Window win = m_Control;
            this.Show(szText, win, ptLoc);
        }

        public void Deactivate()
        {
            // hide it
            this.Active = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // so the tooltip wont steal mouse event control from the panel (like clicking n shit)
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED (double buffering)
                return cp;
            }
        }
        #endregion
    }
}
