using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public interface ISchetsTool
    {
        void MuisVast(SchetsControl s, Point p);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public abstract class StartpuntTool : ISchetsTool
    {
        protected Point startpunt;
        protected Brush kwast;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            startpunt = p;
        }

        public virtual void MuisLos(SchetsControl s, Point p)
        {
            kwast = new SolidBrush(s.PenKleur);
        }

        public abstract void MuisDrag(SchetsControl s, Point p);

        public abstract void Letter(SchetsControl s, char c);
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        protected TweepuntObject schetsObject;

        public static Rectangle Punten2Rechthoek(Point p1, Point p2)
        {
            return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
                                , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
                                );
        }

        public static Pen MaakPen(Brush b, int dikte)
        {
            Pen pen = new Pen(b, dikte);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            return pen;
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            base.MuisVast(s, p);
            this.Begin();
            schetsObject.kleur = s.PenKleur;
            schetsObject.dikte = 3;
            this.Bezig(s.CreateGraphics(), p, p);
            s.Objecten.Add(schetsObject);
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            s.Refresh();
            this.Bezig(s.CreateGraphics(), schetsObject.startpunt, p);
        }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
            this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
            s.Invalidate();
        }

        public override void Letter(SchetsControl s, char c)
        {
        }

        public abstract void Begin();

        public virtual void Bezig(Graphics g, Point p1, Point p2)
        {
            schetsObject.startpunt = p1;
            schetsObject.eindpunt = p2;
        }

        public virtual void Compleet(Graphics g, Point p1, Point p2)
        {
            this.Bezig(g, p1, p2);
        }
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString()
        {
            return "tekst";
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
        }

        public override void Letter(SchetsControl s, char c)
        {
            if (c > ' ' && c <= '~') //check if the character is in the ASCII range
            {
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();

                s.Objecten.Add(new TekstObject { startpunt = startpunt, kleur = s.PenKleur, font = font, tekst = tekst });

                SizeF sz = s.MaakBitmapGraphics().MeasureString(tekst, font, this.startpunt, StringFormat.GenericTypographic);
                startpunt.X += (int)sz.Width;
                s.Invalidate();
            }
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString()
        {
            return "kader";
        }

        public override void Begin()
        {
            schetsObject = new RechthoekObject();
        }
    }

    public class VolRechthoekTool : TweepuntTool
    {
        public override string ToString()
        {
            return "vlak";
        }

        public override void Begin()
        {
            schetsObject = new VolRechthoekObject();
        }
    }

    public class EllipsTool : TweepuntTool
    {
        public override string ToString()
        {
            return "ellips";
        }

        public override void Begin()
        {
            schetsObject = new EllipsObject();
        }
    }

    public class VolEllipsTool : EllipsTool
    {
        public override string ToString()
        {
            return "vullips";
        }

        public override void Begin()
        {
            schetsObject = new VolEllipsObject();
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString()
        {
            return "lijn";
        }

        public override void Begin()
        {
            schetsObject = new LijnObject();
        }
    }

    public class PenTool : LijnTool
    {
        public override string ToString()
        {
            return "pen";
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            this.MuisLos(s, p);
            this.MuisVast(s, p);
        }
    }

    public class GumTool : PenTool
    {
        public override string ToString()
        {
            return "gum";
        }
    }
}
