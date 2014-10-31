using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Drawing.Imaging;

namespace SchetsEditor
{
    public class Schets
    {
        private Bitmap bitmap;
        public List<SchetsObject> objecten = new List<SchetsObject>();

        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }

        public bool Exporteer(string bestandsnaam, ImageFormat f)
        {
            try
            {
                bitmap.Save(bestandsnaam, f);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public Graphics BitmapGraphics
        {
            get
            {
                return Graphics.FromImage(bitmap);
            }
        }

        public void VeranderAfmeting(Size sz)
        {
            if (sz.Width > bitmap.Size.Width || sz.Height > bitmap.Size.Height)
            {
                bitmap = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width),
                                    Math.Max(sz.Height, bitmap.Size.Height));
            }
        }

        public void Teken(Graphics gr)
        {
            BitmapGraphics.Clear(Color.White);
            foreach (SchetsObject s in objecten)
                s.Teken(BitmapGraphics);
            gr.DrawImage(bitmap, 0, 0);
        }

        public void Schoon()
        {
            objecten.Clear();
        }

        public void Roteer()
        {
            foreach (SchetsObject s in objecten)
                s.Roteer(bitmap.Size);
        }
    }
}
