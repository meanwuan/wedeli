using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeDeLi1.style
{
    public static class imageborder
    {
        public static void BogocPictureBox(PictureBox pictureBox, int radius)
        {
            if (pictureBox.Image == null) return;

            Image image = pictureBox.Image;
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

            pictureBox.Image = roundedImage;

            // Bo góc vùng hiển thị PictureBox
            System.Drawing.Drawing2D.GraphicsPath roundedRegion = new GraphicsPath();
            roundedRegion.StartFigure();
            roundedRegion.AddArc(0, 0, radius * 2, radius * 2, 180, 90);
            roundedRegion.AddArc(pictureBox.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90);
            roundedRegion.AddArc(pictureBox.Width - radius * 2, pictureBox.Height - radius * 2, radius * 2, radius * 2, 0, 90);
            roundedRegion.AddArc(0, pictureBox.Height - radius * 2, radius * 2, radius * 2, 90, 90);
            roundedRegion.CloseFigure();

            pictureBox.Region = new Region(roundedRegion);
        }
    }
}
