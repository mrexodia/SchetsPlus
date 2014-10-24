﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace SchetsEditor
{
    [DataContract, KnownType(typeof(PenObject)), KnownType(typeof(StartpuntObject))]
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

            g.Clear(Color.FromArgb(~kleur.ToArgb()));
            dikte += 4;
            Teken(g);
            dikte -= 4;
            g.Flush();

            return bmp.GetPixel(p.X, p.Y).ToArgb() == kleur.ToArgb();
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

    [DataContract, KnownType(typeof(TekstObject)), KnownType(typeof(TweepuntObject))]
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
    }

    public class LijnObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawLine(MaakPen(), startpunt, eindpunt);
        }
    }

    public class RechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawRectangle(MaakPen(), Rechthoek);
        }
    }

    public class VolRechthoekObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillRectangle(MaakBrush(), Rechthoek);
        }
    }

    public class EllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.DrawEllipse(MaakPen(), Rechthoek);
        }
    }

    public class VolEllipsObject : TweepuntObject
    {
        public override void Teken(Graphics g)
        {
            g.FillEllipse(MaakBrush(), Rechthoek);
        }
    }
}
