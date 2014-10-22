using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace SchetsEditor
{
    public abstract class SchetsObject
    {
        public Color kleur;
        public int dikte;

        public Brush MaakBrush()
        {
            return new SolidBrush(kleur);
        }

        public Pen MaakPen()
        {
            Pen pen = new Pen(MaakBrush(), dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }

        public abstract void Teken(Graphics g);
    }

    public abstract class StartpuntObject : SchetsObject
    {
        public Point startpunt;
    }

    public abstract class TweepuntObject : StartpuntObject
    {
        public Point eindpunt;
    }

    public class TekstObject : StartpuntObject
    {
        public Font font;
        public string tekst;

        public override void Teken(Graphics g)
        {
            g.DrawString(tekst, font, this.MaakBrush(), this.startpunt, StringFormat.GenericTypographic);
        }
    }

    public class RechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawRectangle(this.MaakPen(), TweepuntTool.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class VolRechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillRectangle(this.MaakBrush(), TweepuntTool.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class EllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawEllipse(this.MaakPen(), TweepuntTool.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class VolEllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillEllipse(this.MaakBrush(), TweepuntTool.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class LijnObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawLine(this.MaakPen(), this.startpunt, this.eindpunt);
        }
    }
}
