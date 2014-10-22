using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.Serialization;

namespace SchetsEditor
{
    public interface ISchetsObject
    {
        void Teken(Graphics g);
    }

    public abstract class StartpuntObject : ISchetsObject
    {
        public Point startpunt;
        public Brush kwast;

        public abstract void Teken(Graphics g);
    }

    public class TekstObject : StartpuntObject
    {
        public Font font;
        public string tekst;

        public override void Teken(Graphics g)
        {
            g.DrawString(tekst, font, kwast, this.startpunt, StringFormat.GenericTypographic);
        }
    }

    public abstract class TweepuntObject : StartpuntObject
    {
        public Point eindpunt;
        public Pen pen;
    }
}
