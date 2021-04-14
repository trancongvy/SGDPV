using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
namespace AutoUpdate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
             string path ;
             if (args.Length == 0)
             {
                 path = @"https://www.phanmemsgd.com/Content/Update/"; 
             }
             else
             {
                 path = args[0].Trim();
             }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(path));
        }
    }
}
