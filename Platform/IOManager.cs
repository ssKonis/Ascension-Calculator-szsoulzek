using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Ascension_Calculator
{
    public class IOManager
    {
        #region CONSTANTS
        const string DEFAULTFILENAME = "Untitled" + FILEEXTTYPE;
        const string FILEEXTTYPE = ".asc2";
        const string FILEFILTER = "Ascension Calculator 2 Files (*" + FILEEXTTYPE + ") | *" + FILEEXTTYPE;
        const string FILE_DATABASE = "database.txt";
        const string FILE_INTERNAL_DATABASE = "internal_database.txt";
        #endregion

        #region TYPES
        public enum SHARETYPE { ASIMAGE, ASTEXT };        
        #endregion

        #region MEMBERS
        static string m_szCurrentFileName = DEFAULTFILENAME;
        static string m_szOriginalTitle;
        static Form1 m_pMainForm;
        static Font m_ftExportFont = new Font("Verdana", 9.5f, FontStyle.Bold); // optimization
        #endregion

        #region PROPERTIES
        static public string DatabaseFile
        {
            get { return FILE_DATABASE; }
        }

        static public string InternalDatabaseFile
        {
            get { return FILE_INTERNAL_DATABASE; }
        }

        static public bool ModifiedFile
        {
            private get { return CurrentFileName.EndsWith("*"); }

            set
            {
                if (ModifiedFile == value)
                    return;

                if (value) // make it modified
                {
                    if (!CurrentFileName.EndsWith("*")) // append the * if its not alrdy there
                        CurrentFileName += "*";
                }
                else // make it not modified
                {
                    if (CurrentFileName.EndsWith("*")) // if its modified, take off the *
                        CurrentFileName = CurrentFileName.Remove(CurrentFileName.Length - 1);
                }                
            }
        }

        public static string CurrentFileName
        {
            get
            {
                return m_szCurrentFileName;
            }

            set
            {
                m_szCurrentFileName = value;
                m_pMainForm.Text = m_szOriginalTitle + " - " + m_szCurrentFileName;
            }
        }
        #endregion
        
        #region METHODS
        static public object GetOption(string option)
        {
            return Properties.Settings.Default[option];
        }

        static public void SetOption(string option, object value)
        {
            Properties.Settings.Default[option] = value;
            Properties.Settings.Default.Save();
        }

        static public void SetForm(Form1 mainForm)
        {
            // to access the text at any time
            m_pMainForm = mainForm;

            m_szOriginalTitle = m_pMainForm.Text;

            CurrentFileName = DEFAULTFILENAME; // invoke shit

            ModifiedFile = false;
        }

        static public bool New()
        {
            if (!HandleModifiedFile())
                return false; // halt

            // update file name back to default
            CurrentFileName = DEFAULTFILENAME;

            // no longer modified
            ModifiedFile = false;

            return true; // proceed
        }

        static public void Open()
        {
            if (!HandleModifiedFile())
                return; // halt

            // attempt to open a file
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = FILEEXTTYPE;
            dlg.Filter = FILEFILTER;
            if (dlg.ShowDialog() != DialogResult.OK)
                return; // halt open

            // load file info
            StreamReader fileIn = new StreamReader(dlg.FileName);

            string szFileData = fileIn.ReadToEnd();

            // done reading
            fileIn.Close();

            // update file name
            CurrentFileName = dlg.FileName;

            // actually load these skills
            Data.LoadFromString(szFileData, false);

            // no longer modified
            ModifiedFile = false;
        }

        static public void SaveAs()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = FILEEXTTYPE;
            dlg.Filter = FILEFILTER;
            if (dlg.ShowDialog() != DialogResult.OK)
                return; // halt save

            StreamWriter fileOut = new StreamWriter(dlg.FileName);

            fileOut.Write(GetExportString());

            // done writing
            fileOut.Close();

            // update file name
            CurrentFileName = dlg.FileName;

            // no longer modified
            ModifiedFile = false;
        }

        static public bool Exit()
        {
            if (!HandleModifiedFile())
                return false; // halt

            return true;
        }

        static private bool HandleModifiedFile()
        {
            if (!ModifiedFile)
                return true; // nothing to do

            DialogResult result = MessageBox.Show("Do you want to save changes to " +
                CurrentFileName.Remove(CurrentFileName.Length - 1) + "?", "Confirmation", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Yes)
            {
                //save it
                m_pMainForm.saveAsToolStripMenuItem_Click(null, null);

                // done
                return true;
            }
            else if (result == DialogResult.No)
            {
                // done
                return true;
            }
            else
            {
                return false; // cancel last function
            }
        }

        static public void ShareBuild(SHARETYPE type)
        {
            if (type == SHARETYPE.ASIMAGE)
                ExportAsImage();
            else if (type == SHARETYPE.ASTEXT)
                ExportAsText();
        }

        static void ExportAsImage()
        {
            // build the display string list
            List<Data.ExportDataEntry> vExportData = Data.ExportData;

            // build the shared line height between calculation (here) and drawing (below)
            const int EXPORTPADDING = 5;
            int nTextHeight = 0;
            using (Bitmap bmpTemp = new Bitmap(1, 1))
            {
                using (Graphics gfxTemp = Graphics.FromImage(bmpTemp))
                    nTextHeight = (int)gfxTemp.MeasureString("OK", m_ftExportFont).Height; // just get a line height with this font
            }
            Dictionary<Data.ExportDataEntry.LINETYPE, int> vHeights = new Dictionary<Data.ExportDataEntry.LINETYPE, int>();
            vHeights.Add(Data.ExportDataEntry.LINETYPE.NONE, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.ABILITY, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.AE, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.LEVEL, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.TALENT, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.TE, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.TITLE, nTextHeight);
            vHeights.Add(Data.ExportDataEntry.LINETYPE.CLASSTAG, Properties.Resources.classtag_Druid_Balance.Height + EXPORTPADDING);

            // calculate total height based on the heights list and the data
            int nTotalHeight = 0;
            foreach (var entry in vExportData)
            {
                if (entry == null) // default line height on a fake newline
                {
                    nTotalHeight += vHeights[Data.ExportDataEntry.LINETYPE.NONE];
                    continue;
                }

                // real height for this line
                nTotalHeight += vHeights[entry.Type];
            }

            // create bitmap storage
            // dont use line height, just size of the classtag height
            Bitmap bmpFinal = new Bitmap(355, nTotalHeight, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            // create a gfx from the bitmap for rendering
            Graphics gfx = Graphics.FromImage(bmpFinal);

            // graphics settings
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // render
            // background
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF rectfBounds = bmpFinal.GetBounds(ref unit);

            Brush brBackground;
            // background gradient?
            if (Options.ExportBackground == "")
            {
                brBackground = new LinearGradientBrush(rectfBounds, Options.ExportGradientLeft, Options.ExportGradientRight, LinearGradientMode.Horizontal);
            }
            else // background image
            {
                brBackground = new TextureBrush(Ascension_Calculator.Properties.Resources.ResourceManager.GetObject("tiled_" + Options.ExportBackground) as Bitmap, WrapMode.Tile);
            }
            gfx.FillRectangle(brBackground, rectfBounds);
            brBackground.Dispose(); // done with background

            using (Pen p = new Pen(Color.Black, 3.0f))
                gfx.DrawRectangle(p, 0, 0, rectfBounds.Width, rectfBounds.Height);

            // string lines
            PointF ptLocation = new PointF(EXPORTPADDING, EXPORTPADDING);
            foreach (var data in vExportData)
            {
                if (data == null)
                {
                    ptLocation.Y += vHeights[Data.ExportDataEntry.LINETYPE.NONE];
                    continue; // dont handle this line, only "newline" it
                }

                Bitmap bmpShared = null; // should get set
                PointF ptTemp = new PointF(ptLocation.X, ptLocation.Y);
                string szText = data.Text;
                Brush brForeColor = Brushes.White;
                switch (data.Type)
                {
                    case Data.ExportDataEntry.LINETYPE.CLASSTAG:
                        // draw classtag image
                        object[] arr = (object[])data.Data;
                        Bitmap bmpClasstag = UITools.GetClasstagImage((string)arr[0]) as Bitmap;
                        gfx.DrawImage(bmpClasstag, ptTemp);
                        ptTemp.X += bmpClasstag.Width + EXPORTPADDING; // move over for next element
                        // draw ae investment
                        Bitmap bmpAE = UITools.GetInvestmentImage(Data.INVESTMENT.AE) as Bitmap;
                        ptTemp.Y += (bmpClasstag.Height >> 1) - (bmpAE.Height >> 1);
                        gfx.DrawImage(bmpAE, ptTemp);
                        ptTemp.X += bmpAE.Width + EXPORTPADDING;
                        string szAEInvestment = ((int)arr[1]).ToString();
                        UITools.DrawShadowString(gfx, m_ftExportFont, Brushes.DeepSkyBlue, szAEInvestment, (int)ptTemp.X, (int)ptTemp.Y);
                        ptTemp.X += gfx.MeasureString(szAEInvestment, m_ftExportFont).Width + EXPORTPADDING;
                        // draw te investment
                        Bitmap bmpTE = UITools.GetInvestmentImage(Data.INVESTMENT.TE) as Bitmap;
                        gfx.DrawImage(bmpTE, ptTemp);
                        ptTemp.X += bmpTE.Width + EXPORTPADDING;
                        string szTEInvestment = ((int)arr[2]).ToString();
                        UITools.DrawShadowString(gfx, m_ftExportFont, Brushes.Violet, szTEInvestment, (int)ptTemp.X, (int)ptTemp.Y);
                        // go down
                        ptLocation.Y += vHeights[Data.ExportDataEntry.LINETYPE.CLASSTAG];
                        break;
                    case Data.ExportDataEntry.LINETYPE.ABILITY:
                        Skill sk = data.Data as Skill;
                        // indent without \t
                        ptTemp.X += 20;
                        // store icon to draw
                        bmpShared = UITools.SmallIconImageList.Images[sk.IconAsString] as Bitmap;
                        // cut off tabs
                        szText = szText.Replace("\t", "");
                        // change to class color
                        brForeColor = new SolidBrush(UITools.GetClassColor(sk.Class));
                        goto case Data.ExportDataEntry.LINETYPE.NONE; // noice
                    case Data.ExportDataEntry.LINETYPE.TALENT:
                        // nothing to do lul, switch gotos too goog
                        goto case Data.ExportDataEntry.LINETYPE.ABILITY;
                    case Data.ExportDataEntry.LINETYPE.AE:
                        bmpShared = UITools.GetInvestmentImage(Data.INVESTMENT.AE) as Bitmap;
                        // change color
                        brForeColor = Brushes.DeepSkyBlue;
                        goto case Data.ExportDataEntry.LINETYPE.NONE; // noice
                    case Data.ExportDataEntry.LINETYPE.TE:
                        bmpShared = UITools.GetInvestmentImage(Data.INVESTMENT.TE) as Bitmap;
                        // change color
                        brForeColor = Brushes.Violet;
                        goto case Data.ExportDataEntry.LINETYPE.NONE; // noice
                    case Data.ExportDataEntry.LINETYPE.LEVEL:
                        bmpShared = UITools.SmallIconImageList.Images["level"] as Bitmap;
                        // change color
                        brForeColor = Brushes.Gold;
                        goto case Data.ExportDataEntry.LINETYPE.NONE; // noice
                    case Data.ExportDataEntry.LINETYPE.TITLE:
                        goto case Data.ExportDataEntry.LINETYPE.NONE; // noice
                    case Data.ExportDataEntry.LINETYPE.NONE:
                        // draw icon
                        if (bmpShared != null)
                        {
                            gfx.DrawImage(bmpShared, ptTemp);
                            // go over
                            ptTemp.X += bmpShared.Width + EXPORTPADDING;
                        }
                        // draw remainder string
                        UITools.DrawShadowString(gfx, m_ftExportFont, brForeColor, szText, (int)ptTemp.X, (int)ptTemp.Y);
                        // go down
                        ptLocation.Y += vHeights[Data.ExportDataEntry.LINETYPE.NONE];
                        break;
                }              

                // reset point x
                ptLocation.X = EXPORTPADDING;
            }
            
            // done
            gfx.Flush();
            gfx.Dispose();

            try
            {
                // done, copy to clipboard lul
                //Clipboard.SetText(szFinal, TextDataFormat.Text);
                Clipboard.SetImage(bmpFinal);
            }
            catch (Exception) { }

            MessageBox.Show("Copied build to clipboard as image!", "Share Build");
        }

        static string GetExportString()
        {
            List<Data.ExportDataEntry> vDataList = Data.ExportData;

            string szFinal = "";

            foreach (var data in vDataList)
            {
                if (data != null)
                {
                    // add this line
                    szFinal += data.Text;
                }                

                // add a newline per line always
                szFinal += "\r\n";
            }

            return szFinal;
        }

        static void ExportAsText()
        {
            string szFinal = GetExportString();

            try
            {
                // done, copy to clipboard lul
                Clipboard.SetText(szFinal, TextDataFormat.Text);
            }
            catch (Exception) { }

            MessageBox.Show("Copied build to clipboard as text!", "Share Build");
        }
        #endregion
    }
}
