using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace FileViewer
{
    public partial class ReadDatabaseForm : Form
    {
        /// <summary>
        /// this is where result of query will go
        /// </summary>
        internal byte[] ResultBytes = null;
        public ReadDatabaseForm()
        {
            InitializeComponent();
        }

        private void ReadDatabaseForm_Load(object sender, EventArgs e)
        {

        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            using (Wve.HourglassCursor waitCursor = new Wve.HourglassCursor())
            {
                try
                {
                    SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder();
                    scsb.DataSource = textBoxServer.Text.Trim();
                    scsb.InitialCatalog = textBoxDatabase.Text.Trim();
                    scsb.UserID = textBoxLogin.Text.Trim();
                    scsb.Password = textBoxPassword.Text.Trim();
                    using (SqlConnection cn = new SqlConnection())
                    {
                        cn.ConnectionString = scsb.ToString();
                        SqlCommand cmd = new SqlCommand(textBoxSqlQuery.Text.Trim(), cn);
                        cn.Open();
                        object o = cmd.ExecuteScalar();
                        if((o!= null) && (o != DBNull.Value))
                        {
                            if (o is byte[])
                            {
                                ResultBytes = (Byte[])o;
                                this.DialogResult = DialogResult.OK;
                            }
                            else if (o is String)
                            {
                                ResultBytes = UnicodeEncoding.Unicode.GetBytes((string)o);
                                this.DialogResult = DialogResult.OK;
                            }
                            else
                            {
                                throw new Exception("Don't know how to read this data.  Only know byte[] and string.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Sorry, got null result.");
                        }
                    }
                }
                catch (Exception er)
                {
                    MainClass.ShowError(er);
                }
            }
        }

        private void buttonLoadSettings_Click(object sender, EventArgs e)
        {
            using (Wve.HourglassCursor waitCursor = new Wve.HourglassCursor())
            {
                try
                {
                    if(openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(openFileDialog1.FileName))
                        {
                            using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                            {
                                char delimiter = '|';
                                string s = sr.ReadToEnd();
                                string[] ss = s.Split(delimiter);
                                if (ss.Length > 4)
                                {
                                    textBoxServer.Text = ss[0];
                                    textBoxDatabase.Text = ss[1];
                                    textBoxLogin.Text = ss[2];
                                    textBoxPassword.Text = ss[3];
                                    textBoxSqlQuery.Text = ss[4];
                                }
                                else
                                {
                                    MessageBox.Show("unrecognized format for file " +
                                        openFileDialog1.FileName);
                                }
                            }//from using streamreader
                        }//from if file exists
                    }//from if ok to open file
                }
                catch (Exception er)
                {
                    MainClass.ShowError(er);
                }
            }
        }

        private void buttonSaveSettings_Click(object sender, EventArgs e)
        {
            using (Wve.HourglassCursor waitCursor = new Wve.HourglassCursor())
            {
                try
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        char delimiter = '|';
                        StringBuilder sb = new StringBuilder();
                        sb.Append(textBoxServer.Text.Trim());
                        sb.Append(delimiter);
                        sb.Append(textBoxDatabase.Text.Trim());
                        sb.Append(delimiter);
                        sb.Append(textBoxLogin.Text);
                        sb.Append(delimiter);
                        sb.Append(textBoxPassword.Text);
                        sb.Append(delimiter);
                        sb.Append(textBoxSqlQuery.Text);
                        using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                        {
                            sw.Write(sb.ToString());
                            sw.Flush();
                        }
                    }//from if ok to save
                }
                catch (Exception er)
                {
                    MainClass.ShowError(er);
                }
            }
        }
    }
}
