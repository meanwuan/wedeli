using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeDeLi1.style
{
    public static class imageborder
    {
        public static Bitmap bogocima(Image image, int radius)
        {
            int width = image.Width;
            int height = image.Height;

            Bitmap roundedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(roundedImage))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    int diameter = radius * 2;

                    path.StartFigure();
                    path.AddArc(0, 0, diameter, diameter, 180, 90); // top-left
                    path.AddArc(width - diameter, 0, diameter, diameter, 270, 90); // top-right
                    path.AddArc(width - diameter, height - diameter, diameter, diameter, 0, 90); // bottom-right
                    path.AddArc(0, height - diameter, diameter, diameter, 90, 90); // bottom-left
                    path.CloseFigure();

                    using (Brush brush = new TextureBrush(image))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            return roundedImage;
        }
    }
}
