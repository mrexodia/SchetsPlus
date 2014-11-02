using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SchetsEditor
{
    public class SchetsControl : UserControl
    {
        public Schets schets;
        public bool verandering = false;

        public UndoList<SchetsObject> Objecten
        {
            get
            {
                return schets.objecten;
            }
            set
            {
                schets.objecten = value;
            }
        }

        public Color PenKleur { get; private set; }
        public int PenDikte { get; private set; }

        public SchetsControl()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
            this.schets = new Schets();
            this.PenDikte = 3;
            this.Paint += this.teken;
            this.Resize += this.veranderAfmeting;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        private void teken(object o, PaintEventArgs pea)
        {
            schets.Teken(pea.Graphics);
        }

        private void veranderAfmeting(object o, EventArgs ea)
        {
            schets.VeranderAfmeting(this.ClientSize);
            this.Invalidate();
        }

        public Graphics MaakBitmapGraphics()
        {
            Graphics g = schets.BitmapGraphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            return g;
        }

        public void Schoon(object o, EventArgs ea)
        {
            schets.Schoon();
            this.Invalidate();
        }

        public void Undo(object o, EventArgs ea)
        {
            Objecten.Undo();
            this.Refresh();
        }

        public void Redo(object o, EventArgs ea)
        {
            Objecten.Redo();
            this.Refresh();
        }

        public void Roteer(object o, EventArgs ea)
        {
            schets.Roteer();
            this.veranderAfmeting(o, ea);
        }

        private void veranderKleurVanNaam(string kleurNaam)
        {
            if (kleurNaam == Strings.KiesKleur)
            {
                ColorDialog cd = new ColorDialog();
                if (cd.ShowDialog(this) == DialogResult.OK)
                    PenKleur = cd.Color;
            }
            else
                PenKleur = Color.FromName(kleurNaam);
        }

        public void VeranderKleur(object obj, EventArgs ea)
        {
            veranderKleurVanNaam(((ComboBox)obj).Text);
        }

        public void VeranderKleurViaMenu(object obj, EventArgs ea)
        {
            veranderKleurVanNaam(((ToolStripMenuItem)obj).Text);
        }

        private void veranderDikteVanNaam(string dikteNaam)
        {
            try
            {
                this.PenDikte = int.Parse(dikteNaam);
            }
            catch (Exception)
            {
            }
        }

        public void VeranderDikte(object obj, EventArgs ea)
        {
            veranderDikteVanNaam(((ComboBox)obj).Text);
        }

        public void VeranderDikteViaMenu(object obj, EventArgs ea)
        {
            veranderDikteVanNaam(((ToolStripMenuItem)obj).Text);
        }
    }
}
