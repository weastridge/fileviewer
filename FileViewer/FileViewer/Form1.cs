using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace FileViewer
{
    public partial class FormMain : Form
    {
        
        public FormMain()
        {
            InitializeComponent();
        }

        internal class _encodingType
        {
            /// <summary>
            /// the type of string encoding
            /// </summary>
            public  Encoding encodingType = null;
            /// <summary>
            /// name of encoding or not spec if null
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return encodingType == null ? 
                    "not spec" : 
                    encodingType.EncodingName;
            }
            public _encodingType(Encoding e)
            {
                encodingType = e;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //load combob box
                comboBoxEncoding.Items.Clear();
                comboBoxEncoding.Items.Add(new _encodingType(null));
                comboBoxEncoding.Items.Add(new _encodingType(Encoding.ASCII));
                comboBoxEncoding.Items.Add(new _encodingType(Encoding.BigEndianUnicode));
                comboBoxEncoding.Items.Add(new _encodingType(Encoding.Unicode));
                comboBoxEncoding.Items.Add(new _encodingType(Encoding.UTF32));
                comboBoxEncoding.Items.Add(new _encodingType(Encoding.UTF7));
                comboBoxEncoding.Items.Add(new _encodingType(Encoding.UTF8));
                comboBoxEncoding.SelectedIndex = 0;

                //initialize status bar
                statusStrip1.Items.Clear();
                statusStrip1.Items.Add(".");
                //sizing required to avoid problems with long text
                statusStrip1.Items[0].TextAlign = ContentAlignment.MiddleLeft;
                statusStrip1.LayoutStyle = ToolStripLayoutStyle.Flow;
            }
            catch (Exception er)
            {
                MainClass.ShowError(er);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chooseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Wve.HourglassCursor waitCursor = new Wve.HourglassCursor())
            {
                try
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        MainClass.filePathAndName = openFileDialog1.FileName;
                        this.Text = "View File: " + MainClass.filePathAndName;
                        statusStrip1.Items[0].Text = MainClass.filePathAndName;
                        resetTabPages();
                    }
                }
                catch (Exception er)
                {
                    MainClass.ShowError(er);
                }
            }//from using wait cursor
        }

        //reset all the tab pages to empty
        private void resetTabPages()
        {
            textBoxHex.Clear();
            textBoxText.Clear();
        }

        /// <summary>
        /// show status strip contents in text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusStrip1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Wve.TextViewer.ShowText("status: ", 
                    statusStrip1.Items[0].Text);
            }
            catch (Exception er)
            {
                MainClass.ShowError(er);
            }
        }

        /// <summary>
        /// load current file into currently active tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            using (Wve.HourglassCursor waitCursor = new Wve.HourglassCursor())
            {
                try
                {
                    if(string.IsNullOrEmpty(MainClass.filePathAndName))
                    {
                        MessageBox.Show("Please choose file to load.");
                    }
                    else if (File.Exists(MainClass.filePathAndName))
                    {
                        StringBuilder sb = new StringBuilder();
                        using (FileStream fs = new FileStream(MainClass.filePathAndName, FileMode.Open))
                        {
                            if (tabControl1.SelectedTab == tabPageHex)
                            {
                                textBoxHex.Clear();
                                byte[] bytes = new byte[fs.Length];
                                fs.Read(bytes, 0, bytes.Length);
                                string s = Wve.WveTools.BytesToHex(bytes, " ");
                                textBoxHex.Text = s;
                            }//from if hex page
                            else if(tabControl1.SelectedTab == tabPageText)
                            {
                                Encoding enc = ((_encodingType)(comboBoxEncoding.SelectedItem)).encodingType;
                                if(enc == null)
                                {
                                    using (StreamReader sr = new StreamReader(fs))
                                    {
                                        textBoxText.Text = sr.ReadToEnd();
                                    }
                                }
                                else
                                {
                                    using (StreamReader sr = new StreamReader(fs, enc))
                                    {
                                        textBoxText.Text = sr.ReadToEnd();
                                    }
                                }
                            }//from if text page
                        }//from using filestream
                        //show details
                        FileInfo fi = new FileInfo(MainClass.filePathAndName);
                        statusStrip1.Items[0].Text = fi.ToString() +
                            "   length: " + fi.Length;
                    }//from if file exists
                    else
                    {
                        MessageBox.Show("file " + MainClass.filePathAndName + " not found.");
                    }
                }
                catch (Exception er)
                {
                    MainClass.ShowError(er);
                }
            }//from using wait cursor
        }

        private void buttonChooseFile_Click(object sender, EventArgs e)
        {
            chooseToolStripMenuItem_Click(sender, e);
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            using (Wve.HourglassCursor waitCursor = new Wve.HourglassCursor())
            {
                try
                {
                    //if text
                    if (tabControl1.SelectedTab == tabPageText)
                    {
                        //start with beginning if no text is selected or 
                        // with next character if some already selected
                        int startPoint = textBoxText.SelectionLength == 0 ?
                            0 : textBoxText.SelectionStart + 1;
                        if (textBoxText.Text.ToUpper().IndexOf(textBoxFind.Text.ToUpper(),
                            startPoint) > -1)
                        {
                            textBoxText.Select(textBoxText.Text.ToUpper().IndexOf(textBoxFind.Text.ToUpper(),
                                startPoint), textBoxFind.Text.Length);
                            textBoxText.ScrollToCaret();
                        }
                        else
                        {
                            MessageBox.Show("Reached end of text.");
                            //reset to beginning in case want to search from top again
                            if (textBoxText.Text.Length > 0)
                            {
                                textBoxText.Select(0, 0);
                                textBoxText.ScrollToCaret();
                            }
                        }
                    }//from if tabPageXML
                }
                catch (Exception er)
                {
                    MainClass.ShowError(er);
                }
            }
        }
    }
}
