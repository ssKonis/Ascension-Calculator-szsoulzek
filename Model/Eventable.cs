using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ascension_Calculator
{
    abstract public class Eventable
    {
        #region TYPES
        public delegate void AscEvent(object arg);
        #endregion

        #region MEMBERS
        static Dictionary<string, AscEvent> m_vEvents = new Dictionary<string, AscEvent>();
        #endregion

        #region METHODS
        static public void RegisterEvent(string szEvent, AscEvent callback)
        {
            if (!m_vEvents.ContainsKey(szEvent))
            {
                // add the new callback
                m_vEvents.Add(szEvent, new AscEvent(callback));
            }
            else
            {
                // append the callback to this list
                m_vEvents[szEvent] += callback;
            }
        }

        static protected void Invoke(string szEvent, object arg)
        {
            if (!m_vEvents.ContainsKey(szEvent)) // no such event to invoke
            {
                return; // sanity
            }
            
            // invoke it, with the built-in if not null ?.
            m_vEvents[szEvent]?.Invoke(arg);
        }
        #endregion
    }
}
