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
    }
}
