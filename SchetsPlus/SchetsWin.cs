using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.Drawing.Imaging;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
        ResourceManager resourcemanager = new ResourceManager("SchetsEditor.Properties.Resources", Assembly.GetExecutingAssembly());
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
                                 , "Yellow", "Magenta", "Cyan", "White", "Anders..." 
                                 };

            this.ClientSize = new Size(700, 510);
            huidigeTool = deTools[0];
            this.FormClosing += (object o, FormClosingEventArgs ea) =>
            {
                if (schetscontrol.verandering == true)
                {
                    DialogResult dialogResult = MessageBox.Show("Wil je de veranderingen opslaan?", "Opslaan?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        opslaan(o, ea);
                    }
                }
            };
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
            this.maakAktieMenu(deKleuren);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren);
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
                ObjectSerializer.SerializeToCompressedFile<List<SchetsObject>>(schetscontrol.Objecten, bestandsnaam);
            }
            catch (Exception)
            {
                MessageBox.Show("Er is een fout opgetreden bij het opslaan!", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.bestandsnaam = bestandsnaam;
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
                Filter = "Schets Files (*.schets)|*.schets",
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
                Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png",
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
                    MessageBox.Show("Er is een fout opgetreden bij het exporteren!", "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public bool LaadBestand(string bestandsnaam)
        {
            try
            {
                schetscontrol.Objecten = ObjectSerializer.DeserializeFromCompressedFile<List<SchetsObject>>(bestandsnaam);
            }
            catch (Exception)
            {
                return false;
            }
            this.Refresh();
            this.bestandsnaam = bestandsnaam;
            return true;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }

        private void maakFileMenu()
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
            menu.DropDownItems.Add("Opslaan", null, this.opslaan);
            menu.DropDownItems.Add("Opslaan als", null, this.opslaanAls);
            menu.DropDownItems.Add("Exporteer", null, this.exporteer);
            menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
            menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {
                ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren)
        {
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon);
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer);
            ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
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
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0)
                    b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren)
        {
            paneel = new Panel();
            paneel.Size = new Size(600, 24);
            this.Controls.Add(paneel);

            Button b; Label l; ComboBox cbb;
            b = new Button();
            b.Text = "Clear";
            b.Location = new Point(0, 0);
            b.Click += schetscontrol.Schoon;
            paneel.Controls.Add(b);

            b = new Button();
            b.Text = "Rotate";
            b.Location = new Point(80, 0);
            b.Click += schetscontrol.Roteer;
            paneel.Controls.Add(b);

            l = new Label();
            l.Text = "Penkleur:";
            l.Location = new Point(180, 3);
            l.AutoSize = true;
            paneel.Controls.Add(l);

            cbb = new ComboBox(); cbb.Location = new Point(240, 0);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList;
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);
        }
    }
}
