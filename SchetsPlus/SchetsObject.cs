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
        public Color kleur = Color.Black;
        [DataMember]
        public int dikte = 0;

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
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.FromArgb(~this.kleur.ToArgb()));
            this.dikte += 4;
            this.Teken(g);
            this.dikte -= 4;
            g.Flush();

            return bmp.GetPixel(p.X, p.Y).ToArgb() == this.kleur.ToArgb();
        }

        public abstract void Teken(Graphics g);
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
    }

    [DataContract, KnownType(typeof(LijnObject)),
    KnownType(typeof(VolRechthoekObject)), KnownType(typeof(RechthoekObject)),
    KnownType(typeof(VolEllipsObject)), KnownType(typeof(EllipsObject))]
    public abstract class TweepuntObject : StartpuntObject
    {
        [DataMember]
        public Point eindpunt;

        public Rectangle Rechthoek
        {
            get
            {
                return Punten2Rechthoek(this.startpunt, this.eindpunt);
            }
        }

        public Rectangle BoundingBox
        {
            get
            {
                Rectangle r = this.Rechthoek;
                r.X -= this.dikte / 2;
                r.Y -= this.dikte / 2;
                r.Width += this.dikte;
                r.Height += this.dikte;
                return r;
            }
        }

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
    }

    public class LijnObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawLine(this.MaakPen(), this.startpunt, this.eindpunt);
        }
    }

    public class VolRechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillRectangle(this.MaakBrush(), this.Rechthoek);
        }
    }

    public class RechthoekObject : VolRechthoekObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawRectangle(this.MaakPen(), this.Rechthoek);
        }
    }

    public class VolEllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillEllipse(this.MaakBrush(), this.Rechthoek);
        }
    }

    public class EllipsObject : VolEllipsObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawEllipse(this.MaakPen(), this.Rechthoek);
        }
    }
}
