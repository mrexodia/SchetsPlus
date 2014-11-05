using System;
using System.Drawing;
using System.Windows.Forms;
using SchetsPlus.Properties;

namespace SchetsPlus
{
    public class Hoofdscherm : Form
    {
        MenuStrip menuStrip;

        public Hoofdscherm()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            menuStrip = new MenuStrip();
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakHelpMenu();
            this.Text = Strings.SchetsPlusTitel;
            this.IsMdiContainer = true;
            this.MainMenuStrip = menuStrip;
        }

        private void maakFileMenu()
        {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem(Strings.MenuFile);
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.FileNieuw, null, this.nieuw, Keys.Control|Keys.N));
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.FileOpenen, null, this.openen, Keys.Control | Keys.O));
            menuStrip.Items.Add(menu);
        }

        private void maakHelpMenu()
        {
            ToolStripDropDownItem menu;
            menu = new ToolStripMenuItem(Strings.MenuHelp);
            menu.DropDownItems.Add(Strings.HelpOver, null, this.about);
            menuStrip.Items.Add(menu);
        }

        private void about(object o, EventArgs ea)
        {
            MessageBox.Show(Strings.OverTekst, Strings.OverTitel, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                Filter = Strings.SchetsFilter,
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
                    MessageBox.Show(Strings.FoutOpenenTekst, Strings.FoutTitel, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Hoofdscherm));
            this.SuspendLayout();
            // 
            // Hoofdscherm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Hoofdscherm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}
