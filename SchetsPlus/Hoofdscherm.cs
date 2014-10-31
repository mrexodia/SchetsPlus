using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {
            this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = "SchetsPlus";
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }

        private void maakFileMenu()
        {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("File");
            menu.DropDownItems.Add("Nieuw", null, this.nieuw);
            menu.DropDownItems.Add("Openen", null, this.openen);
            menuStrip.Items.Add(menu);
        }

        private void maakHelpMenu()
        {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem("Help");
            menu.DropDownItems.Add("Over \"SchetsPlus\"", null, this.about);
            menuStrip.Items.Add(menu);
        }

        private void about(object o, EventArgs ea)
        {
            MessageBox.Show("SchetsPlus versie 1.0\n(c) UU Informatica 2014"
                           , "Over \"SchetsPlus\""
                           , MessageBoxButtons.OK
                           , MessageBoxIcon.Information
                           );
        }

        private void nieuw(object sender, EventArgs e)
        {
            SchetsWin s = new SchetsWin();
            s.MdiParent = this;
            s.Show();
        }

        private void openen(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures),
                Filter = "Schets Files (*.schets)|*.schets",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SchetsWin s = new SchetsWin();
                s.LaadBestand(ofd.FileName);
                if (s.LaadBestand(ofd.FileName))
                {
                    s.MdiParent = this;
                    s.Show();
                }
                else
                    MessageBox.Show("Er is een fout opgetreden bij het openen van het bestand!", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
