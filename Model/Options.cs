using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    public class Options : Eventable
    {
        #region TYPES
        public enum EVENT { OPTIONS_DISPLAYPANELBACKGROUNDS, OPTIONS_CHECKFORUPDATES, OPTIONS_SHOWLISTGROUPS,
            OPTIONS_EXPORTGRADIENTLEFT, OPTIONS_EXPORTGRADIENTRIGHT, OPTIONS_EXPORTBACKGROUND, OPTIONS_PANELSTALENTSABILITIES
        };
        #endregion

        #region PROPERTIES
        public static bool PanelsTalentsAbilities
        {
            get
            {
                return (bool)IOManager.GetOption(EVENT.OPTIONS_PANELSTALENTSABILITIES.ToString());
            }

            set
            {
                bool bOldValue = (bool)IOManager.GetOption(EVENT.OPTIONS_PANELSTALENTSABILITIES.ToString());

                if (bOldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_PANELSTALENTSABILITIES.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_PANELSTALENTSABILITIES.ToString(), bOldValue);
            }
        }

        public static Color ExportGradientRight
        {
            get
            {
                return (Color)IOManager.GetOption(EVENT.OPTIONS_EXPORTGRADIENTRIGHT.ToString());
            }

            set
            {
                Color OldValue = (Color)IOManager.GetOption(EVENT.OPTIONS_EXPORTGRADIENTRIGHT.ToString());

                if (OldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_EXPORTGRADIENTRIGHT.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_EXPORTGRADIENTRIGHT.ToString(), OldValue);
            }
        }

        public static Color ExportGradientLeft
        {
            get
            {
                return (Color)IOManager.GetOption(EVENT.OPTIONS_EXPORTGRADIENTLEFT.ToString());
            }

            set
            {
                Color OldValue = (Color)IOManager.GetOption(EVENT.OPTIONS_EXPORTGRADIENTLEFT.ToString());

                if (OldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_EXPORTGRADIENTLEFT.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_EXPORTGRADIENTLEFT.ToString(), OldValue);
            }
        }

        public static string ExportBackground
        {
            get
            {
                return (string)IOManager.GetOption(EVENT.OPTIONS_EXPORTBACKGROUND.ToString());
            }

            set
            {
                string OldValue = (string)IOManager.GetOption(EVENT.OPTIONS_EXPORTBACKGROUND.ToString());

                if (OldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_EXPORTBACKGROUND.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_EXPORTBACKGROUND.ToString(), OldValue);
            }
        }

        public static bool DisplayPanelBackgrounds
        {
            get
            {
                return (bool)IOManager.GetOption(EVENT.OPTIONS_DISPLAYPANELBACKGROUNDS.ToString());
            }

            set
            {
                bool bOldValue = (bool)IOManager.GetOption(EVENT.OPTIONS_DISPLAYPANELBACKGROUNDS.ToString());

                if (bOldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_DISPLAYPANELBACKGROUNDS.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_DISPLAYPANELBACKGROUNDS.ToString(), bOldValue);
            }
        }

        public static bool CheckForUpdates
        {
            get
            {
                return (bool)IOManager.GetOption(EVENT.OPTIONS_CHECKFORUPDATES.ToString());
            }

            set
            {
                bool bOldValue = (bool)IOManager.GetOption(EVENT.OPTIONS_CHECKFORUPDATES.ToString());

                if (bOldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_CHECKFORUPDATES.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_CHECKFORUPDATES.ToString(), bOldValue);
            }
        }

        public static bool ShowListGroups
        {
            get
            {
                return false;
                // just dont ever show groups - causes that fucking crash....
                //return (bool)IOManager.GetOption(EVENT.SHOWLISTGROUPS.ToString());
            }

            set
            {
                bool bOldValue = (bool)IOManager.GetOption(EVENT.OPTIONS_SHOWLISTGROUPS.ToString());

                if (bOldValue == value)
                    return; // no change

                // change it
                IOManager.SetOption(EVENT.OPTIONS_SHOWLISTGROUPS.ToString(), value);

                // event callbacks
                Invoke(EVENT.OPTIONS_SHOWLISTGROUPS.ToString(), bOldValue);
            }
        }
        #endregion
    }
}
