using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileViewer
{
    /// <summary>
    /// main class
    /// </summary>
    internal static class MainClass
    {
        /// <summary>
        /// file to view
        /// </summary>
        internal static string filePathAndName = string.Empty;

        /// <summary>
        /// show error and save to clipboard
        /// </summary>
        /// <param name="er"></param>
        internal static void ShowError(Exception er)
        {
            if(MessageBox.Show(er.ToString(), 
                "Save exception to clipboard?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error, 
                MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Clipboard.Clear();
                Clipboard.SetText(er.ToString());
                //
            }
        }
    }
}
