using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace FlowerMaster
{
    /// <summary>
    /// WebBrowserOverlay.xaml 的互動邏輯
    /// </summary>
    public partial class WebBrowserOverlay : Window
    {
        #region Win32 Helper

        private static class Win32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }

                public POINT(Point pt)
                {
                    X = Convert.ToInt32(pt.X);
                    Y = Convert.ToInt32(pt.Y);
                }
            };

            [DllImport("user32.dll")]
            internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

            [DllImport("user32.dll")]
            internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        };

        #endregion Win32 Helper

        #region Private Member

        private FrameworkElement _placementTarget;
        private WebBrowser _web;

        #endregion Private Member

        #region Constructor

        public WebBrowserOverlay(WebBrowser web, FrameworkElement placementTarget)
        {
            InitializeComponent();
            _web = web;
            GD_Root.Children.Add(_web);

            _placementTarget = placementTarget;
            Window owner = Window.GetWindow(placementTarget);
            Debug.Assert(owner != null);

            //owner.SizeChanged += delegate { OnSizeLocationChanged(); };
            owner.LocationChanged += delegate { OnSizeLocationChanged(); };
            _placementTarget.SizeChanged += delegate { OnSizeLocationChanged(); };

            if (owner.IsVisible)
            {
                Owner = owner;
                Show();
            }
            else
            {
                owner.IsVisibleChanged += delegate
                {
                    if (owner.IsVisible)
                    {
                        Owner = owner;
                        Show();
                    }
                };
            }
        }

        #endregion Constructor

        #region Window Event

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
                // Delayed call to avoid crash due to Window bug.
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Owner.Close();
                });
        }

        private void OnSizeLocationChanged()
        {
            Point offset = _placementTarget.TranslatePoint(new Point(), Owner);
            Point size = new Point(_placementTarget.ActualWidth, _placementTarget.ActualHeight);
            HwndSource hwndSource = (HwndSource)HwndSource.FromVisual(Owner);
            CompositionTarget ct = hwndSource.CompositionTarget;
            offset = ct.TransformToDevice.Transform(offset);
            size = ct.TransformToDevice.Transform(size);

            Win32.POINT screenLocation = new Win32.POINT(offset);
            Win32.ClientToScreen(hwndSource.Handle, ref screenLocation);
            Win32.POINT screenSize = new Win32.POINT(size);

            Win32.MoveWindow(((HwndSource)HwndSource.FromVisual(this)).Handle, screenLocation.X, screenLocation.Y, screenSize.X, screenSize.Y, true);
        }

        private void On_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GD_Root.Children.Remove(_web);
        }

        #endregion Window Event

        #region Private Method

        private void ResizeWebBrowser()
        {
        }

        #endregion Private Method
    }
}