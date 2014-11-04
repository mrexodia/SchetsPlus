using System;
using System.Windows.Forms;

namespace SchetsPlus
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
