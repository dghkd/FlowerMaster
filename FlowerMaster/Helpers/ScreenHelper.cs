using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace FlowerMaster.Helpers
{
    public class ScreenHelper
    {
        public static IEnumerable<ScreenHelper> AllScreens()
        {
            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new ScreenHelper(screen);
            }
        }

        public static ScreenHelper GetScreenFrom(Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            ScreenHelper wpfScreen = new ScreenHelper(screen);
            return wpfScreen;
        }

        public static ScreenHelper GetScreenFrom(System.Windows.Point point)
        {
            int x = (int)Math.Round(point.X);
            int y = (int)Math.Round(point.Y);

            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
            ScreenHelper wpfScreen = new ScreenHelper(screen);

            return wpfScreen;
        }

        public static ScreenHelper Primary
        {
            get { return new ScreenHelper(System.Windows.Forms.Screen.PrimaryScreen); }
        }

        /// <summary>
        /// 擷取螢幕畫面
        /// <para>此方法會依螢幕畫面擷取，所以會擷取到其他前景視窗</para>
        /// </summary>
        /// <param name="x">螢幕座標x點</param>
        /// <param name="y">螢幕座標y點</param>
        /// <param name="width">擷取寬度</param>
        /// <param name="height">擷取高度</param>
        /// <returns></returns>
        public static Bitmap CaptureScreen(int x, int y, int width, int height)
        {
            Matrix transformToDevice;
            using (var source = new HwndSource(new HwndSourceParameters()))
                transformToDevice = source.CompositionTarget.TransformToDevice;
            System.Drawing.Size size = new System.Drawing.Size(Convert.ToInt32(width * transformToDevice.M11),
                                                                Convert.ToInt32(height * transformToDevice.M22));

            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(new System.Drawing.Point(x, y), new System.Drawing.Point(0, 0), size, CopyPixelOperation.SourceCopy);
                g.Dispose();
            }

            return bmp;
        }

        private readonly Screen screen;

        internal ScreenHelper(System.Windows.Forms.Screen screen)
        {
            this.screen = screen;
        }

        public Rect DeviceBounds
        {
            get { return this.GetRect(this.screen.Bounds); }
        }

        public Rect WorkingArea
        {
            get { return this.GetRect(this.screen.WorkingArea); }
        }

        private Rect GetRect(System.Drawing.Rectangle value)
        {
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        public string DeviceName
        {
            get { return this.screen.DeviceName; }
        }
    }
}