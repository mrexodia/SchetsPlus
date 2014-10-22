using System;
using System.Windows.Forms;

namespace SchetsEditor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new Hoofdscherm());
        }
    }
}
