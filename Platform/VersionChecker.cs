using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ascension_Calculator
{
    class VersionChecker
    {
        #region CONSTANTS
        static string m_szVersion = "2.5";
        static string m_szFAQURL = "http://www.google.com"; // default to this i guess...

        static string TAG_VERSION = "_ASCVERSION_";
        static string TAG_DOWNLOADLINK = "_ASCDOWNLOAD_";
        static string TAG_FAQ = "_ASCFAQ_";
        #endregion

        #region PROPERTIES
        public static string Version { get { return m_szVersion; } }
        public static string FAQURL { get { return m_szFAQURL; } }
        #endregion

        #region CTOR
        static VersionChecker()
        {            
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadStringCompleted += OnDownloadStringCompleted;
                    wc.DownloadStringAsync(new Uri("http://textuploader.com/dka8k"));
                }
            }
            catch (Exception)
            {
                return; // fail silently
            }
        }
        #endregion

        #region METHODS
        static string RipStringBetween(string szHTML, string szStartUID, string szEndUID, int nStartIndex)
        {
            int nTempIndex = szHTML.IndexOf(szStartUID, nStartIndex);
            if (nTempIndex == -1)
                return null; // not found
            nTempIndex += szStartUID.Length;
            return szHTML.Substring(nTempIndex, szHTML.IndexOf(szEndUID, nTempIndex) - nTempIndex);
        }
        
        private static void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // safety assign the faqurl around trycatch
            string szFAQURL = m_szFAQURL; // default it to the member defined at top of class

            try
            {
                // check forum post vs our local version
                // recover page contents
                string szHTML = e.Result;

                // extract the gravy
                int nStartIndex = szHTML.LastIndexOf(TAG_VERSION);
                string szNewVersion = RipStringBetween(szHTML, TAG_VERSION, "\r\n", nStartIndex);
                szFAQURL = RipStringBetween(szHTML, TAG_FAQ, "\r\n", nStartIndex);

                // handle the version (maybe)
                if (Options.CheckForUpdates)
                {
                    if (szNewVersion.CompareTo(m_szVersion) > 0)
                    {
                        if (MessageBox.Show("A new version is available (" + szNewVersion + "). Would you like to download it?", "New Version", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // extract the download link
                            string szDownloadLink = RipStringBetween(szHTML, TAG_DOWNLOADLINK, "\r\n", nStartIndex);
                            Process.Start(szDownloadLink);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return; // fail silently
            }

            // store the real faqurl
            m_szFAQURL = szFAQURL;
        }
        #endregion
    }
}
