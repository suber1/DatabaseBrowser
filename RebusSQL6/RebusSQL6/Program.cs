using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RebusSQL6
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] psArgs = null)
        {
            bool xbMigrate = true;
            if (psArgs != null)
            {
                if (psArgs.Count() >= 1)
                {
                    for (int xii = 0; xii < psArgs.Count(); xii++)
                    {
                        if (psArgs[xii].ToUpper().IndexOf("NOMIGR") >= 0) xbMigrate = false;
                    }
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain(xbMigrate));
        }
    }
}
