using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace SchetsPlus
{
    [DataContract, KnownType(typeof(PenObject)), KnownType(typeof(StartpuntObject))]
    public abstract class SchetsObject
    {
        [DataMember]
        public Color kleur = Color.Black;

        [DataMember]
        public int dikte = 0;

        public const int Correctie = 2;

        public SchetsObject Copy()
        {
            return (SchetsObject)this.MemberwiseClone();
        }

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

        public virtual bool Geklikt(SchetsControl s, Point p)
        {
            Bitmap bmp = new Bitmap(s.ClientSize.Width, s.ClientSize.Height);
            bmp.MakeTransparent(Color.Transparent);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Transparent);
            dikte += SchetsObject.Correctie * 2;
            Teken(g);
            dikte -= SchetsObject.Correctie * 2;
            g.Flush();

            return bmp.GetPixel(p.X, p.Y).A > 0; //achtergrond heeft Color.A = 0
        }

        public static Point RotatePoint(Point p, Size size)
        {
            Point m = new Point(size.Width / 2, size.Height / 2);
            p = new Point(p.X - m.X, p.Y - m.Y);

            double cosine = Math.Cos(-0.5 * Math.PI * 1);
            double sine = Math.Sin(-0.5 * Math.PI * 1);
            p = new Point((int)(p.X * cosine - p.Y * sine),
                          (int)(p.X * sine + p.Y * cosine));

            return new Point(p.X + m.X, p.Y + m.Y);
        }

        public abstract void Teken(Graphics g);
        public abstract void Roteer(Size size);
        public abstract void Beweeg(int dx, int dy);
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

        public override void Roteer(Size size)
        {
            foreach (LijnObject lijn in lijnen)
                lijn.Roteer(size);
        }

        public override void Beweeg(int dx, int dy)
        {
            foreach (LijnObject lijn in lijnen)
                lijn.Beweeg(dx, dy);
        }
    }

    [DataContract, KnownType(typeof(TekstObject)), KnownType(typeof(TweepuntObject))]
    public abstract class StartpuntObject : SchetsObject
    {
        [DataMember]
        public Point startpunt;

        public override void Roteer(Size size)
        {
            startpunt = SchetsObject.RotatePoint(startpunt, size);
        }

        public override void Beweeg(int dx, int dy)
        {
            startpunt.X += dx;
            startpunt.Y += dy;
        }
    }

    [DataContract, KnownType(typeof(FontStyle)), KnownType(typeof(GraphicsUnit))]
    public class TekstObject : StartpuntObject
    {
        [DataMember]
        public Font font;

        [DataMember]
        public string tekst;

        public override void Teken(Graphics g)
        {
            g.DrawString(tekst, font, MaakBrush(), startpunt, StringFormat.GenericTypographic);
        }
    }

    [DataContract, KnownType(typeof(LijnObject)),
    KnownType(typeof(RechthoekObject)), KnownType(typeof(VolRechthoekObject)),
    KnownType(typeof(EllipsObject)), KnownType(typeof(VolEllipsObject))]
    public abstract class TweepuntObject : StartpuntObject
    {
        [DataMember]
        public Point eindpunt;

        public Rectangle Rechthoek
        {
            get
            {
                return new Rectangle(new Point(Math.Min(startpunt.X, eindpunt.X), Math.Min(startpunt.Y, eindpunt.Y)),
                                    new Size(Math.Abs(startpunt.X - eindpunt.X), Math.Abs(startpunt.Y - eindpunt.Y)));
            }
        }

        public Rectangle OuterBox
        {
            get
            {
                Rectangle r = Rechthoek;
                if (dikte > 0)
                {
                    dikte += SchetsObject.Correctie * 2;
                    r.X -= dikte / 2;
                    r.Y -= dikte / 2;
                    r.Width += dikte;
                    r.Height += dikte;
                    dikte -= SchetsObject.Correctie * 2;
                }
                return r;
            }
        }

        public Rectangle InnerBox
        {
            get
            {
                Rectangle r = Rechthoek;
                if (dikte > 0)
                {
                    dikte += SchetsObject.Correctie * 2;
                    r.X += dikte / 2;
                    r.Y += dikte / 2;
                    r.Width -= dikte;
                    r.Height -= dikte;
                    dikte -= SchetsObject.Correctie * 2;
                }
                return r;
            }
        }

        public static bool GekliktInRechthoek(Rectangle box, Point p)
        {
            return (p.X >= box.Left && p.X <= box.Right) && (p.Y >= box.Top && p.Y <= box.Bottom);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            return GekliktInRechthoek(OuterBox, p);
        }

        public override void Roteer(Size size)
        {
            base.Roteer(size);
            eindpunt = SchetsObject.RotatePoint(eindpunt, size);
        }

        public override void Beweeg(int dx, int dy)
        {
            base.Beweeg(dx, dy);
            eindpunt.X += dx;
            eindpunt.Y += dy;
        }
    }

    public class LijnObject : TweepuntObject
    {
        public double AfstandTotLijn(Point p)
        {
            double x1 = startpunt.X;
            double y1 = startpunt.Y;
            double x2 = eindpunt.X;
            double y2 = eindpunt.Y;
            double dx = x2 - x1;
            double dy = y2 - y1;
            //Formule van: http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line#Line_defined_by_two_points
            double d = Math.Abs(dy * p.X - dx * p.Y - x1 * y2 + x2 * y1) / Math.Sqrt(dx * dx + dy * dy);
            return d - dikte / 2;
        }

        public override void Teken(Graphics g)
        {
            g.DrawLine(MaakPen(), startpunt, eindpunt);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            if (!base.Geklikt(s, p))
                return false;
            return AfstandTotLijn(p) < SchetsObject.Correctie;
        }
    }

    public class RechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawRectangle(MaakPen(), Rechthoek);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            if (!base.Geklikt(s, p))
                return false;
            return !GekliktInRechthoek(InnerBox, p);
        }
    }

    public class VolRechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillRectangle(MaakBrush(), Rechthoek);
        }
    }

    public class VolEllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillEllipse(MaakBrush(), Rechthoek);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            if (!base.Geklikt(s, p))
                return false;
            Rectangle r = OuterBox;
            Size sz = new Size(r.Width / 2, r.Height / 2);
            p = new Point(p.X - r.X - sz.Width, p.Y - r.Y - sz.Height);
            return EllipsObject.Formule(p, sz) <= 1;
        }
    }

    public class EllipsObject : VolEllipsObject
    {
        public static double Formule(Point p, Size sz)
        {
            //Formule van: http://www.analyzemath.com/calculus/Integrals/area_ellipse.html
            return (double)(p.X * p.X) / (double)(sz.Width * sz.Width) + (double)(p.Y * p.Y) / (double)(sz.Height * sz.Height);
        }

        public override void Teken(Graphics g)
        {
            g.DrawEllipse(MaakPen(), Rechthoek);
        }

        public override bool Geklikt(SchetsControl s, Point p)
        {
            if (!base.Geklikt(s, p))
                return false;
            Rectangle r = InnerBox;
            Size sz = new Size(r.Width / 2, r.Height / 2);
            p = new Point(p.X - r.X - sz.Width, p.Y - r.Y - sz.Height);
            return EllipsObject.Formule(p, sz) >= 1;
        }
    }
}
