using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Drawing.Imaging;

namespace SchetsEditor
{
    [DataContract]
    public class SchetsData
    {
        [DataMember]
        public int rotate = 0;

        [DataMember]
        public List<SchetsObject> objecten = new List<SchetsObject>();
    }

    public class Schets
    {
        private Bitmap bitmap;
        public SchetsData data;

        public Schets()
        {
            bitmap = new Bitmap(1, 1);
            data = new SchetsData();
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
            foreach (SchetsObject schetsObject in data.objecten)
                schetsObject.Teken(BitmapGraphics);
            for (int i = 0; i < data.rotate; i++)
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            gr.DrawImage(bitmap, 0, 0);
        }

        public void Schoon()
        {
            data = new SchetsData();
        }

        public void Roteer()
        {
            data.rotate = data.rotate < 3 ? data.rotate + 1 : 0;
        }

        public Point RotatePoint(Point p)
        {
            Point m = new Point(bitmap.Width / 2, bitmap.Height / 2);
            p = new Point(p.X - m.X, p.Y - m.Y);

            double cosine = Math.Cos(-0.5 * Math.PI * data.rotate);
            double sine = Math.Sin(-0.5 * Math.PI * data.rotate);
            p = new Point((int)(p.X * cosine - p.Y * sine),
                          (int)(p.X * sine + p.Y * cosine));

            return new Point(p.X + m.X, p.Y + m.Y);
        }
    }
}
