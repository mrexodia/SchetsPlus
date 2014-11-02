﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using SchetsEditor.Properties;

namespace SchetsEditor
{

    public interface ISchetsTool
    {
        Image Icoon();
        void MuisVast(SchetsControl s, Point p);
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

        public void MuisVast(SchetsControl s, Point p)
        {
            for (int i = s.Objecten.Count - 1; i >= 0; i--)
            {
                if (s.Objecten[i].Geklikt(s, p))
                {
                    s.Objecten.RemoveAt(i);
                    s.verandering = true;
                    s.Refresh();
                    break;
                }
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

    public abstract class SchetsTool : ISchetsTool
    {
        protected SchetsObject obj;

        public virtual Image Icoon()
        {
            return null;
        }

        public virtual void MuisVast(SchetsControl s, Point p)
        {
            obj.kleur = s.PenKleur;
            s.Objecten.Add(obj);
            s.verandering = true;
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

        public override void MuisVast(SchetsControl s, Point p)
        {
            startpunt = p;
            obj = new PenObject { dikte = s.PenDikte };
            base.MuisVast(s, p);
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
        public override void MuisVast(SchetsControl s, Point p)
        {
            ((StartpuntObject)obj).startpunt = p;
            base.MuisVast(s, p);
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

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new TekstObject { font = s.TekstFont };
            base.MuisVast(s, p);
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

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new LijnObject { dikte = s.PenDikte };
            base.MuisVast(s, p);
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

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new RechthoekObject { dikte = s.PenDikte };
            base.MuisVast(s, p);
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
            return Strings.ToolEllipsTekst;
        }

        public override Image Icoon()
        {
            return Resources.ellips;
        }

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new EllipsObject { dikte = s.PenDikte };
            base.MuisVast(s, p);
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

        public override void MuisVast(SchetsControl s, Point p)
        {
            obj = new VolEllipsObject();
            base.MuisVast(s, p);
        }
    }
}
