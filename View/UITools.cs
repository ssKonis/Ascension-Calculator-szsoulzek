using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Resources;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;

namespace Ascension_Calculator
{
    public class UITools
    {
        #region MEMBERS
        static SolidBrush m_brGrey;
        static SolidBrush m_brGreen;
        static SolidBrush m_brYellow;
        static SolidBrush m_brRed;

        static Pen m_pGrey;
        static Pen m_pGreen;
        static Pen m_pRed;
        static Pen m_pYellow;

        static System.Windows.Forms.ImageList m_ilSmallIcons = new System.Windows.Forms.ImageList();
        static System.Windows.Forms.ImageList m_ilClasstags = new System.Windows.Forms.ImageList();
        #endregion

        #region PROPERTIES
        static public SolidBrush Brush_Grey { get { return m_brGrey; } }

        static public SolidBrush Brush_Green { get { return m_brGreen; } }

        static public SolidBrush Brush_Yellow { get { return m_brYellow; } }

        static public SolidBrush Brush_Red { get { return m_brRed; } }

        static public Pen Pen_Red { get { return m_pRed; } }

        static public Pen Pen_Grey { get { return m_pGrey; } }

        static public Pen Pen_Green { get { return m_pGreen; } }

        static public Pen Pen_Yellow { get { return m_pYellow; } }

        public static ImageList SmallIconImageList { get { return m_ilSmallIcons; } }

        public static ImageList ClasstagsImageList { get { return m_ilClasstags; } }

        public static Color Grey { get { return Color.FromArgb(128, 128, 128); } }
        public static Color Green { get { return Color.FromArgb(23, 249, 16); } }
        public static Color Yellow { get { return Color.FromArgb(231, 186, 0); } }
        public static Color Red { get { return Color.FromArgb(255, 64, 64); } }
        #endregion

        #region CTOR
        static UITools()
        {
            // load brushes
            m_brGrey = new SolidBrush(Grey);
            m_brGreen = new SolidBrush(Green);
            m_brYellow = new SolidBrush(Yellow);
            m_brRed = new SolidBrush(Red);

            // load pens
            m_pGrey = new Pen(Grey);
            m_pGreen = new Pen(Green);
            m_pRed = new Pen(Red);
            m_pYellow = new Pen(Yellow);
                        
            // load images into image lists (classtags and smallicon)
            bool bWatchForSize = true; // to only set the il classtag size once instead of multiple times
            ResourceSet rsrcSet = Ascension_Calculator.Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false);
            foreach (DictionaryEntry entry in rsrcSet)
            {
                string name = entry.Key as string;
                Image img = entry.Value as Image;
                if (img != null)
                {
                    // add to icon list
                    SmallIconImageList.Images.Add(name, img);

                    // maybe add to classtag list
                    if (name.Contains("classtag"))
                    {
                        ClasstagsImageList.Images.Add(name, img);

                        if (bWatchForSize)
                        {
                            ClasstagsImageList.ImageSize = img.Size;
                            bWatchForSize = false; // got 'em
                        }
                    }
                }
            }
        }
        #endregion

        #region METHODS
        static public Image GetClasstagImage(string szClasstag)
        {
            return Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("classtag_" + szClasstag) as Bitmap;
        }

        static public Image GetInvestmentImage(Data.INVESTMENT type)
        {
            return Ascension_Calculator.Properties.Resources.ResourceManager.GetObject(type.ToString().ToLower()) as Bitmap;
        }

        static public string FixClasstag(string szClasstag, bool bUnfix = false)
        {
            if (!bUnfix) // fix it
            {
                szClasstag = szClasstag.Replace("_", " - ");
                szClasstag = szClasstag.Replace("General - General", "General");
                szClasstag = FixSpec(szClasstag);
            }
            else // unfix it
            {
                szClasstag = szClasstag.Replace(" - ", "_");
                szClasstag = szClasstag.Replace("General", "General_General");
                szClasstag = FixSpec(szClasstag, true);
            }

            return szClasstag;
        }

        static public string FixSpec(string szSpec, bool bUnfix = false)
        {
            if (!bUnfix)
                return szSpec.Replace("BeastMastery", "Beast Mastery");
            else
                return szSpec.Replace("Beast Mastery", "BeastMastery");
        }

        static public void DrawShadowString(Graphics gfx, Font font, Brush brFore, string szString, int x, int y)
        {
            PointF pfOffset = new PointF(x - 2, y + 2);
            gfx.DrawString(szString, font, Brushes.Black, pfOffset); // shadow
            gfx.DrawString(szString, font, brFore, new Point(x, y)); // actual
        }

        static public Color GetClassColor(string szClass)
        {
            /*
             * Death Knight	196	31	59	0.77	0.12	0.23	#C41F3B	Red †
                Demon Hunter	163	48	201	0.64	0.19	0.79	#A330C9	Dark Magenta
                Druid	255	125	10	1.00	0.49	0.04	#FF7D0A	Orange
                Hunter	171	212	115	0.67	0.83	0.45	#ABD473	Green
                Mage	105	204	240	0.41	0.80	0.94	#69CCF0	Light Blue
                Monk	0	255	150	0.00	1.00	0.59	#00FF96	Spring Green
                Paladin	245	140	186	0.96	0.55	0.73	#F58CBA	Pink
                Priest	255	255	255	1.00	1.00	1.00	#FFFFFF	White*
                Rogue	255	245	105	1.00	0.96	0.41	#FFF569	Yellow*
                Shaman	0	112	222	0.00	0.44	0.87	#0070DE	Blue
                Warlock	148	130	201	0.58	0.51	0.79	#9482C9	Purple
                Warrior	199	156	110	0.78	0.61	0.43	#C79C6E	Tan
             */

            switch (szClass)
            {
                case "Druid":
                    return Color.FromArgb(255, 125, 10);
                case "Hunter":
                    return Color.FromArgb(171, 212, 115);
                case "Mage":
                    return Color.FromArgb(105, 204, 240);
                case "Paladin":
                    return Color.FromArgb(245, 140, 186);
                case "Priest":
                    return Color.White;
                case "Rogue":
                    return Color.FromArgb(255, 245, 105);
                case "Shaman":
                    return Color.FromArgb(0, 112, 222);
                case "Warlock":
                    return Color.FromArgb(148, 130, 201);
                case "Warrior":
                    return Color.FromArgb(199, 156, 110);
                case "General":
                    return Color.FromArgb(112, 112, 112);
            }

            return Color.White; // wat
        }
        #endregion
    }
}
