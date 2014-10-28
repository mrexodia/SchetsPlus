using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

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
                Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
                                         , Math.Max(sz.Height, bitmap.Size.Height)
                                         );
                Graphics gr = Graphics.FromImage(nieuw);
                gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
                gr.DrawImage(bitmap, 0, 0);
                bitmap = nieuw;
            }
        }

        public void Teken(Graphics gr)
        {
            Schoon();
            foreach (SchetsObject schetsObject in data.objecten)
                schetsObject.Teken(BitmapGraphics);
            for (int i = 0; i < data.rotate; i++)
                bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            gr.DrawImage(bitmap, 0, 0);
        }

        public void Schoon()
        {
            Graphics gr = Graphics.FromImage(bitmap);
            gr.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
        }

        public void Roteer()
        {
            data.rotate = data.rotate < 3 ? data.rotate + 1 : 0;
        }
    }
}
