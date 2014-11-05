using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SchetsPlus.Properties;

namespace SchetsPlus
{

    public interface ISchetsTool
    {
        Image Icoon();
        void MuisVast(SchetsControl s, Point p, MouseButtons b);
        void MuisDrag(SchetsControl s, Point p);
        void MuisLos(SchetsControl s, Point p);
        void Letter(SchetsControl s, char c);
    }

    public class GumTool : ISchetsTool
    {
        public override string ToString()
        {
            return Strings.ToolGumTekst;
        }

        public Image Icoon()
        {
            return Resources.gum;
        }

        public void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            int index = SchetsTool.GekliktObject(s, p);
            if(index != SchetsTool.GeenObject)
            {
                s.Objecten.RemoveAt(index);
                s.Refresh();
            }
        }

        public void MuisDrag(SchetsControl s, Point p)
        {
        }

        public void MuisLos(SchetsControl s, Point p)
        {
        }

        public void Letter(SchetsControl s, char c)
        {
        }
    }

    public class PipetTool : ISchetsTool
    {
        public override string ToString()
        {
            return Strings.ToolPipetTekst;
        }

        public Image Icoon()
        {
            return Resources.pipet;
        }

        public void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            s.PenKleur = s.schets.bitmap.GetPixel(p.X, p.Y);
        }

        public void MuisDrag(SchetsControl s, Point p)
        {
        }

        public void MuisLos(SchetsControl s, Point p)
        {
        }

        public void Letter(SchetsControl s, char c)
        {
        }
    }

    public class LayerTool : ISchetsTool
    {
        public override string ToString()
        {
            return Strings.ToolLayerTekst;
        }

        public Image Icoon()
        {
            return Resources.lagen;
        }

        public void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
        }

        public void MuisDrag(SchetsControl s, Point p)
        {
        }

        public void MuisLos(SchetsControl s, Point p)
        {
        }

        public void Letter(SchetsControl s, char c)
        {
        }
    }

    public abstract class SchetsTool : ISchetsTool
    {
        public const int GeenObject = -1;
        protected SchetsObject obj;

        public virtual Image Icoon()
        {
            return null;
        }

        public static int GekliktObject(SchetsControl s, Point p)
        {
            for (int i = s.Objecten.Count - 1; i >= 0; i--)
            {
                if (s.Objecten[i].Geklikt(s, p))
                {
                    return i;
                }
            }
            return GeenObject;
        }

        public virtual void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj.kleur = s.PenKleur;
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

    public class PenTool : SchetsTool
    {
        private Point startpunt;

        public override string ToString()
        {
            return Strings.ToolPenTekst;
        }

        public override Image Icoon()
        {
            return Resources.pen;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            startpunt = p;
            obj = new PenObject { dikte = s.PenDikte };
            base.MuisVast(s, p, b);
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            PenObject obj = (PenObject)this.obj;
            obj.lijnen.Add(new LijnObject { kleur = obj.kleur, dikte = obj.dikte, startpunt = this.startpunt, eindpunt = p });
            startpunt = p;
            base.MuisDrag(s, p);
        }
    }

    public abstract class StartpuntTool : SchetsTool
    {
        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            ((StartpuntObject)obj).startpunt = p;
            base.MuisVast(s, p, b);
        }
    }

    public class TekstTool : StartpuntTool
    {
        private static string backspace(string s)
        {
            if (s.Length == 0)
                return s;
            return s.Remove(s.Length - 1);
        }

        public override string ToString()
        {
            return Strings.ToolTekstTekst;
        }

        public override Image Icoon()
        {
            return Resources.tekst;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj = new TekstObject { font = s.TekstFont };
            base.MuisVast(s, p, b);
        }

        public override void Letter(SchetsControl s, char c)
        {
            TekstObject obj = (TekstObject)this.obj;
            if (!Char.IsControl(c))
                obj.tekst += c.ToString();
            else if (c == '\b') //backspace
                obj.tekst = backspace(obj.tekst);
            base.Letter(s, c);
        }
    }

    public abstract class TweepuntTool : StartpuntTool
    {
        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            ((TweepuntObject)obj).eindpunt = p;
            base.MuisVast(s, p, b);
        }

        public override void MuisDrag(SchetsControl s, Point p)
        {
            ((TweepuntObject)obj).eindpunt = p;
            base.MuisDrag(s, p);
        }
    }

    public class LijnTool : TweepuntTool
    {
        public override string ToString()
        {
            return Strings.ToolLijnTekst;
        }

        public override Image Icoon()
        {
            return Resources.lijn;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj = new LijnObject { dikte = s.PenDikte };
            base.MuisVast(s, p, b);
        }
    }

    public class RechthoekTool : TweepuntTool
    {
        public override string ToString()
        {
            return Strings.ToolKaderTekst;
        }

        public override Image Icoon()
        {
            return Resources.kader;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj = new RechthoekObject { dikte = s.PenDikte };
            base.MuisVast(s, p, b);
        }
    }

    public class VolRechthoekTool : TweepuntTool
    {
        public override string ToString()
        {
            return Strings.ToolVlakTekst;
        }

        public override Image Icoon()
        {
            return Resources.vlak;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj = new VolRechthoekObject();
            base.MuisVast(s, p, b);
        }
    }

    public class EllipsTool : TweepuntTool
    {
        public override string ToString()
        {
            return Strings.ToolEllipsTekst;
        }

        public override Image Icoon()
        {
            return Resources.ellips;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj = new EllipsObject { dikte = s.PenDikte };
            base.MuisVast(s, p, b);
        }
    }

    public class VolEllipsTool : TweepuntTool
    {
        public override string ToString()
        {
            return Strings.ToolVullipsTekst;
        }

        public override Image Icoon()
        {
            return Resources.vullips;
        }

        public override void MuisVast(SchetsControl s, Point p, MouseButtons b)
        {
            obj = new VolEllipsObject();
            base.MuisVast(s, p, b);
        }
    }
}
