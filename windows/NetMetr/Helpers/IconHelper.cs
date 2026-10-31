using System.Drawing;
using System.Drawing.Drawing2D;

namespace NetMetr.Helpers;

public static class IconHelper
{
    public static Icon CreateAppIcon()
    {
        int size = 64;
        using var bitmap = new Bitmap(size, size);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(System.Drawing.Color.Transparent);

            // Rounded background
            using var brushBg = new SolidBrush(System.Drawing.Color.FromArgb(240, 18, 24, 38));
            g.FillEllipse(brushBg, 2, 2, size - 4, size - 4);

            // Outer cyan border
            using var penRing = new Pen(System.Drawing.Color.FromArgb(200, 14, 165, 233), 3f);
            g.DrawEllipse(penRing, 3, 3, size - 6, size - 6);

            // Download arrow (Green)
            using var greenBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 16, 185, 129));
            System.Drawing.Point[] downArrow = {
                new(24, 20),
                new(24, 34),
                new(18, 34),
                new(26, 44),
                new(34, 34),
                new(28, 34),
                new(28, 20)
            };
            g.FillPolygon(greenBrush, downArrow);

            // Upload arrow (Sky blue)
            using var blueBrush = new SolidBrush(System.Drawing.Color.FromArgb(255, 56, 189, 248));
            System.Drawing.Point[] upArrow = {
                new(38, 44),
                new(38, 30),
                new(32, 30),
                new(40, 20),
                new(48, 30),
                new(42, 30),
                new(42, 44)
            };
            g.FillPolygon(blueBrush, upArrow);
        }

        var hIcon = bitmap.GetHicon();
        return Icon.FromHandle(hIcon);
    }
}
