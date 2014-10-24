using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace SchetsEditor
{
    [DataContract, KnownType(typeof(StartpuntObject)), KnownType(typeof(PenObject))]
    public abstract class SchetsObject
    {
        [DataMember]
        public Color kleur;
        [DataMember]
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
        public abstract bool Geklikt(SchetsControl s, Point p);
    }

    [DataContract]
    public class PenObject : SchetsObject
    {
        [DataMember]
        public List<LijnObject> lijnen = new List<LijnObject>();

        public override void Teken(Graphics g)
        {
            foreach (LijnObject lijn in lijnen)
                lijn.Teken(g);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            foreach (LijnObject lijn in lijnen)
                if (lijn.Geklikt(s, p))
                    return true;
            return false;
        }
    }

    [DataContract, KnownType(typeof(TweepuntObject)), KnownType(typeof(TekstObject))]
    public abstract class StartpuntObject : SchetsObject
    {
        [DataMember]
        public Point startpunt;
    }

    [DataContract, KnownType(typeof(FontStyle)), KnownType(typeof(GraphicsUnit))]
    public class TekstObject : StartpuntObject
    {
        [DataMember]
        public Font font;
        [DataMember]
        public string tekst;

        private Size getSize(Graphics g)
        {
            return Size.Round(g.MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic));
        }

        public override void Teken(Graphics g)
        {
            g.DrawString(tekst, font, this.MaakBrush(), this.startpunt, StringFormat.GenericTypographic);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            return TweepuntObject.GekliktInRechthoek(new Rectangle(this.startpunt, getSize(s.MaakBitmapGraphics())), p);
        }
    }

    [DataContract, KnownType(typeof(LijnObject)),
    KnownType(typeof(VolRechthoekObject)), KnownType(typeof(RechthoekObject)),
    KnownType(typeof(VolEllipsObject)), KnownType(typeof(EllipsObject))]
    public abstract class TweepuntObject : StartpuntObject
    {
        [DataMember]
        public Point eindpunt;

        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                                , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                                );
        }

        public static bool GekliktInRechthoek(Rectangle box, Point p)
        {
            return (p.X >= box.Left && p.X <= box.Right) && (p.Y >= box.Top && p.Y <= box.Bottom);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            return GekliktInRechthoek(Punten2Rechthoek(this.startpunt, this.eindpunt), p);
        }
    }

    public class LijnObject : TweepuntObject
    {
        //Formule van: http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
        public double DistanceToLine(Point p)
        {
            double x1 = this.startpunt.X;
            double y1 = this.startpunt.Y;
            double x2 = this.eindpunt.X;
            double y2 = this.eindpunt.Y;
            double dx = x2 - x1;
            double dy = y2 - y1;
            double d = Math.Abs(dy * p.X - dx * p.Y - x1 * y2 + x2 * y1) / Math.Sqrt(dx * dx + dy * dy);
            return d;
        }

        public override void Teken(Graphics g)
        {
            g.DrawLine(this.MaakPen(), this.startpunt, this.eindpunt);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            if (!base.Geklikt(s, p))
                return false;
            return DistanceToLine(p) < 2;
        }
    }

    public class VolRechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillRectangle(this.MaakBrush(), TweepuntObject.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class RechthoekObject : VolRechthoekObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawRectangle(this.MaakPen(), TweepuntObject.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class VolEllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillEllipse(this.MaakBrush(), TweepuntObject.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }

    public class EllipsObject : VolEllipsObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawEllipse(this.MaakPen(), TweepuntObject.Punten2Rechthoek(this.startpunt, this.eindpunt));
        }
    }
}
