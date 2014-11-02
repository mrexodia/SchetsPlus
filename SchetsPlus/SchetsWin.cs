using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Drawing.Imaging;
using System.IO;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        string bestandsnaam = "";

        public SchetsWin()
        {
            ISchetsTool[] deTools = { new PenTool()         
                                    , new LijnTool()
                                    , new RechthoekTool()
                                    , new VolRechthoekTool()
                                    , new EllipsTool()
                                    , new VolEllipsTool()
                                    , new TekstTool()
                                    , new GumTool()
                                    };
            String[] deKleuren = { "Black", "Red", "Green", "Blue"
                                 , "Yellow", "Magenta", "Cyan", "White", Strings.KiesKleur 
                                 };

            this.ClientSize = new Size(700, 510);
            this.Text = Strings.NieuweSchets;
            huidigeTool = deTools[0];

            schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
            {
                vast = true;
                huidigeTool.MuisVast(schetscontrol, mea.Location);
            };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
            {
                if (vast)
                    huidigeTool.MuisDrag(schetscontrol, mea.Location);
            };
            schetscontrol.MouseUp += (object o, MouseEventArgs mea) =>
            {
                vast = false;
                huidigeTool.MuisLos(schetscontrol, mea.Location);
            };
            schetscontrol.KeyPress += (object o, KeyPressEventArgs kpea) =>
            {
                huidigeTool.Letter(schetscontrol, kpea.KeyChar);
            };
            schetscontrol.KeyDown += (object o, KeyEventArgs kea) =>
            {
                if (kea.KeyCode == Keys.Back)
                {
                    huidigeTool.Letter(schetscontrol, '\b'); //we use '\b' for backspace
                    kea.SuppressKeyPress = true;
                }
            };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakActieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakActieButtons(deKleuren);
            this.FormClosing += (object o, FormClosingEventArgs ea) =>
            {
                if (schetscontrol.verandering == true)
                {
                    DialogResult dialogResult = MessageBox.Show(Strings.WijzigingenOpslaanTekst, Strings.WijzigingenOpslaanTitel, MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        opslaan(o, ea);
                }
            };
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schetscontrol.Size = new Size(this.ClientSize.Width - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

        private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void opslaanNaam(string bestandsnaam)
        {
            try
            {
                ObjectSerializer.SerializeToCompressedFile<List<SchetsObject>>(schetscontrol.Objecten.CopyList(), bestandsnaam);
            }
            catch (Exception)
            {
                MessageBox.Show(Strings.FoutOpslaanTekst, Strings.FoutTitel, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.bestandsnaam = bestandsnaam;
            this.Text = Path.GetFileNameWithoutExtension(bestandsnaam);
            this.schetscontrol.verandering = false;
        }

        private void opslaan(object obj, EventArgs ea)
        {
            if (bestandsnaam.Length > 0)
                opslaanNaam(bestandsnaam);
            else
                opslaanAls(obj, ea);
        }

        private void opslaanAls(object obj, EventArgs ea)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures),
                Filter = Strings.SchetsFilter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                opslaanNaam(sfd.FileName);
            }
        }

        private void exporteer(object obj, EventArgs ea)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures),
                Filter = Strings.ExporterenFilter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ImageFormat f;
                switch (sfd.FilterIndex)
                {
                    case 2:
                        f = ImageFormat.Jpeg;
                        break;

                    case 3:
                        f = ImageFormat.Png;
                        break;

                    default:
                        f = ImageFormat.Bmp;
                        break;
                }

                if (!schetscontrol.schets.Exporteer(sfd.FileName, f))
                {
                    MessageBox.Show(Strings.FoutExporterenTekst, Strings.FoutTitel, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public bool LaadBestand(string bestandsnaam)
        {
            try
            {
                schetscontrol.Objecten = new UndoList<SchetsObject>(ObjectSerializer.DeserializeFromCompressedFile<List<SchetsObject>>(bestandsnaam));
            }
            catch (Exception)
            {
                return false;
            }
            this.Refresh();
            this.Text = Path.GetFileNameWithoutExtension(bestandsnaam);
            return true;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        private void maakFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem(Strings.MenuFile);
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.FileOpslaan, null, this.opslaan, Keys.Control | Keys.S));
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.FileOpslaanAls, null, this.opslaanAls, Keys.Control | Keys.Shift | Keys.S));
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.FileExporteer, null, this.exporteer, Keys.Control | Keys.E));
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.FileSluiten, null, this.afsluiten, Keys.Control | Keys.W));
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem(Strings.MenuTool);
            foreach (ISchetsTool tool in tools)
            {
                ToolStripItem item = new ToolStripMenuItem(Strings.MenuTool);
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = tool.Icoon();
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakActieMenu(String[] kleuren)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem(Strings.MenuActie);
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.ActieUndo, null, schetscontrol.Undo, Keys.Control | Keys.Z));
            menu.DropDownItems.Add(new ToolStripMenuItem(Strings.ActieRedo, null, schetscontrol.Redo, Keys.Control | Keys.Y));
            menu.DropDownItems.Add(Strings.ActieClear, null, schetscontrol.Schoon);
            menu.DropDownItems.Add(Strings.ActieRoteer, null, schetscontrol.Roteer);
            menu.DropDownItems.Add(Strings.ActieVeranderFont, null, schetscontrol.VeranderFont);
            ToolStripMenuItem submenu = new ToolStripMenuItem(Strings.ActieKiesKleur);
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
            menu.DropDownItems.Add(submenu);
            submenu = new ToolStripMenuItem(Strings.ActieKiesDikte);
            for (int i = 1; i <= 20; i++)
                submenu.DropDownItems.Add(i.ToString(), null, schetscontrol.VeranderDikteViaMenu);
            menu.DropDownItems.Add(submenu);
            menuStrip.Items.Add(menu);
        }

        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = tool.Icoon();
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0)
                    b.Select();
                t++;
            }
        }

        private void maakActieButtons(String[] kleuren)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l; ComboBox cbb;
            b = new Button();
            b.Text = Strings.ActieClear.Replace("&", "");
            b.Location = new Point(0, -1);
            b.Click += schetscontrol.Schoon;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = Strings.ActieRoteer.Replace("&", "");
            b.Location = new Point(80, -1);
            b.Click += schetscontrol.Roteer;
            paneel.Controls.Add(b);

            l = new Label();
            l.Text = Strings.LabelPenkleur;
            l.Location = new Point(165, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            cbb = new ComboBox(); cbb.Location = new Point(230, 0);
            cbb.Width = 60;
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);

            l = new Label();
            l.Text = Strings.LabelPendikte;
            l.Location = new Point(310, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            cbb = new ComboBox(); cbb.Location = new Point(l.Location.X + l.Width + 10, 0);
            cbb.Width = 50;
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderDikte;
            for (int i = 1; i <= 20; i++)
                cbb.Items.Add(i.ToString());
            cbb.SelectedIndex = 2;
            paneel.Controls.Add(cbb);

            b = new Button();
            b.Text = Strings.ActieVeranderFont.Replace("&", "");
            b.AutoSize = true;
            b.Location = new Point(cbb.Location.X + cbb.Width + 10, -1);
            b.Click += schetscontrol.VeranderFont;
            paneel.Controls.Add(b);
        }
    }
}
