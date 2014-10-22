using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SchetsEditor
{
    public abstract class SchetsTool
    {
        protected SchetsObject obj;

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            obj.kleur = s.PenKleur;
            obj.dikte = 3;
            s.Objecten.Add(obj);
        }

        public virtual void MuisDrag(SchetsControl s, Point p)
        {
            s.Refresh();
        }

        public virtual void MuisLos(SchetsControl s, Point p)
        {
            s.Invalidate();
        }

        public virtual void Letter(SchetsControl s, char c)
        {
            s.Refresh();
        }
    }

    public abstract class StartpuntTool : SchetsTool
    {
        public override void MuisVast(SchetsControl s, Point p)
        {
            ((StartpuntObject)obj).startpunt = p;
            base.MuisVast(s, p);
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            base.MuisDrag(s, p);
        }

        public override void MuisLos(SchetsControl s, Point p)
        {
            base.MuisLos(s, p);
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public override void MuisVast(SchetsControl s, Point p)
        {
            ((TweepuntObject)obj).eindpunt = p;
            base.MuisVast(s, p);
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            ((TweepuntObject)obj).eindpunt = p;
            base.MuisDrag(s, p);
        }
    }

    public class TekstTool : StartpuntTool
    {
        public override string ToString()
        {
            return "tekst";
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new TekstObject();
            base.MuisVast(s, p);
        }

        public override void Letter(SchetsControl s, char c)
        {
            if (c > ' ' && c <= '~') //check if the character is in the ASCII range
            {
                Font font = new Font("Tahoma", 40);
                string tekst = c.ToString();

                s.Objecten.Add(new TekstObject { startpunt = ((StartpuntObject)obj).startpunt, kleur = s.PenKleur, font = font, tekst = tekst });

                SizeF sz = s.MaakBitmapGraphics().MeasureString(tekst, font, ((StartpuntObject)obj).startpunt, StringFormat.GenericTypographic);
                ((StartpuntObject)obj).startpunt.X += (int)sz.Width;
            }
            base.Letter(s, c);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString()
        {
            return "kader";
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new RechthoekObject();
            base.MuisVast(s, p);
        }
    }

    public class VolRechthoekTool : TweepuntTool
    {
        public override string ToString()
        {
            return "vlak";
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new VolRechthoekObject();
            base.MuisVast(s, p);
        }
    }

    public class EllipsTool : TweepuntTool
    {
        public override string ToString()
        {
            return "ellips";
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new EllipsObject();
            base.MuisVast(s, p);
        }
    }

    public class VolEllipsTool : TweepuntTool
    {
        public override string ToString()
        {
            return "vullips";
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new VolEllipsObject();
            base.MuisVast(s, p);
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString()
        {
            return "lijn";
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new LijnObject();
            base.MuisVast(s, p);
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
            base.MuisDrag(s, p);
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
