using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MissionAssistant
{
    static class Printer
    {
        #region Internal Fields
        private static double leftmost = Double.MaxValue, topmost = Double.MaxValue, rightmost = Double.MinValue, bottommost = Double.MinValue, padding = 50;
        #endregion

        private static void GetBoundaries()
        {
            leftmost = Double.MaxValue;
            topmost = Double.MaxValue;
            rightmost = Double.MinValue;
            bottommost = Double.MinValue;

            foreach (var obj in UI.drawnObjects)
            {
                var type = obj.GetType();
                if (type == typeof(MapLine))
                {
                    var item = (MapLine)obj;
                    if (item.LocalStartPoint.X < leftmost) leftmost = item.LocalStartPoint.X;
                    if (item.LocalEndPoint.X < leftmost) leftmost = item.LocalEndPoint.X;
                    if (item.LocalStartPoint.X > rightmost) rightmost = item.LocalStartPoint.X;
                    if (item.LocalEndPoint.X > rightmost) rightmost = item.LocalEndPoint.X;
                    if (item.LocalStartPoint.Y < topmost) topmost = item.LocalStartPoint.Y;
                    if (item.LocalEndPoint.Y < topmost) topmost = item.LocalEndPoint.Y;
                    if (item.LocalStartPoint.Y > bottommost) bottommost = item.LocalStartPoint.Y;
                    if (item.LocalEndPoint.Y > bottommost) bottommost = item.LocalEndPoint.Y;
                }
                else if (type == typeof(Circle))
                {
                    var item = (Circle)obj;
                    var left = item.LocalCenter.X - item.LocalRadius;
                    var top = item.LocalCenter.Y - item.LocalRadius;
                    var right = item.LocalCenter.X + item.LocalRadius;
                    var bottom = item.LocalCenter.Y + item.LocalRadius;
                    if (left < leftmost) leftmost = left;
                    if (top < topmost) topmost = top;
                    if (right > rightmost) rightmost = right;
                    if (bottom > bottommost) bottommost = bottom;
                }
                else if (type == typeof(Polygon))
                {
                    foreach (var line in ((Polygon)obj).Lines)
                    {
                        var item = line;
                        if (item.LocalStartPoint.X < leftmost) leftmost = item.LocalStartPoint.X;
                        if (item.LocalEndPoint.X < leftmost) leftmost = item.LocalEndPoint.X;
                        if (item.LocalStartPoint.X > rightmost) rightmost = item.LocalStartPoint.X;
                        if (item.LocalEndPoint.X > rightmost) rightmost = item.LocalEndPoint.X;
                        if (item.LocalStartPoint.Y < topmost) topmost = item.LocalStartPoint.Y;
                        if (item.LocalEndPoint.Y < topmost) topmost = item.LocalEndPoint.Y;
                        if (item.LocalStartPoint.Y > bottommost) bottommost = item.LocalStartPoint.Y;
                        if (item.LocalEndPoint.Y > bottommost) bottommost = item.LocalEndPoint.Y;
                    }
                }
                else if (type == typeof(Route))
                {
                    foreach (var leg in ((Route)obj).Legs)
                    {
                        var item = leg;
                        if (item.LocalStartPoint.X < leftmost) leftmost = item.LocalStartPoint.X;
                        if (item.LocalEndPoint.X < leftmost) leftmost = item.LocalEndPoint.X;
                        if (item.LocalStartPoint.X > rightmost) rightmost = item.LocalStartPoint.X;
                        if (item.LocalEndPoint.X > rightmost) rightmost = item.LocalEndPoint.X;
                        if (item.LocalStartPoint.Y < topmost) topmost = item.LocalStartPoint.Y;
                        if (item.LocalEndPoint.Y < topmost) topmost = item.LocalEndPoint.Y;
                        if (item.LocalStartPoint.Y > bottommost) bottommost = item.LocalStartPoint.Y;
                        if (item.LocalEndPoint.Y > bottommost) bottommost = item.LocalEndPoint.Y;
                    }

                    foreach (var leg in ((Route)obj).DiversionLegs)
                    {
                        var item = leg;
                        if (item.LocalStartPoint.X < leftmost) leftmost = item.LocalStartPoint.X;
                        if (item.LocalEndPoint.X < leftmost) leftmost = item.LocalEndPoint.X;
                        if (item.LocalStartPoint.X > rightmost) rightmost = item.LocalStartPoint.X;
                        if (item.LocalEndPoint.X > rightmost) rightmost = item.LocalEndPoint.X;
                        if (item.LocalStartPoint.Y < topmost) topmost = item.LocalStartPoint.Y;
                        if (item.LocalEndPoint.Y < topmost) topmost = item.LocalEndPoint.Y;
                        if (item.LocalStartPoint.Y > bottommost) bottommost = item.LocalStartPoint.Y;
                        if (item.LocalEndPoint.Y > bottommost) bottommost = item.LocalEndPoint.Y;
                    }
                }
            }
        }

        private static void SnapCanvas(Canvas canvas, string filepath)
        {
            var size = new Size(rightmost + padding, bottommost + padding);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));
            var bitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            bitmap.Render(canvas);
            PngBitmapEncoder pbe = new PngBitmapEncoder();
            pbe.Frames.Add(BitmapFrame.Create(bitmap));
            using (FileStream fs = new FileStream(filepath, FileMode.Create)) pbe.Save(fs);
            ((Grid)canvas.Parent).InvalidateMeasure();
            ((Grid)canvas.Parent).InvalidateArrange();
        }

        public static async void Print()
        {
            var pos = UI.map.Position;
            //UI.map.InitializeForBackgroundRendering(4000, 2000);
            //UI.RepositionPoints();
            GetBoundaries();
            if (leftmost == Double.MaxValue || topmost == Double.MaxValue || rightmost == Double.MinValue || bottommost == Double.MinValue) return;
            int left = (int)(leftmost - padding);
            int top = (int)(topmost - padding);
            UI.map.Offset(-left, -top);
            GetBoundaries();
            await Task.Delay(1000);
            SnapCanvas(UI.drawCanvas, @"MapDrawing.png");
            UI.map.Position = pos;
            UI.RepositionPoints();
        }
    }
}
