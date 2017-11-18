using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;

using MahApps.Metro.Controls.Dialogs;

using FlowerMaster.Helpers;

namespace FlowerMaster
{
    /// <summary>
    /// BalconyGardenWindows.xaml 的互動邏輯
    /// </summary>
    public partial class BalconyGardenWindows : Window
    {
        #region Win32 Helper

        public const int WM_WINDOWPOSCHANGING = 0x46;

        public struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }
        
        #endregion


        #region Private Member
        private WebBrowser _web;
        private WebBrowserOverlay _wnd;

        /// <summary>
        /// 自動吸附螢幕邊緣距離
        /// <para>SNAP_DIST = 20</para>
        /// </summary>
        private readonly int SNAP_DIST = 20;
        #endregion


        #region Constructor

        public BalconyGardenWindows(WebBrowser web)
        {
            InitializeComponent();
            this.SourceInitialized += On_Windows_SourceInitialized;
            _web = web;
        }

        #endregion


        #region Window Event
        
        private void On_Windows_SourceInitialized(object sender, EventArgs e)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            HwndSource source = HwndSource.FromHwnd(helper.Handle);
            
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_WINDOWPOSCHANGING:
                    WINDOWPOS wPos = (WINDOWPOS)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

                    Debug.WriteLine(String.Format("Top:{0} Left:{1}", wPos.y, wPos.x));
                    WINDOWPOS wPos2 = SnapToScreenEdge(wPos);
                    Debug.WriteLine(String.Format("Top1:{0} Left1:{1}", wPos2.y, wPos2.x));

                    System.Runtime.InteropServices.Marshal.StructureToPtr(wPos2, lParam, true);
                    break;
            }
            return IntPtr.Zero;
        }
        
        private void On_Window_Loaded(object sender, RoutedEventArgs e)
        {
            //在WPF中Webrowser無法在AllowsTransparency="True"的情況下正常顯示，因此放入另一個視窗中，以疊層方式顯示
            _wnd = new WebBrowserOverlay(_web, GD_Content);
            ResizeWindow(_web);
        }

        private void On_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _wnd.Close();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed
                && BTN_Fix.IsChecked == false)
            {
                DragMove();
            }
        }
        #endregion


        #region Control Event

        private void On_BTN_Fix_Click(object sender, RoutedEventArgs e)
        {
            if (BTN_Fix.IsChecked == true)
            {
                this.ShowInTaskbar = false;
                this.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                this.ShowInTaskbar = true;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
        }

        private void On_BTN_Topmost_Clicked(object sender, RoutedEventArgs e)
        {
            if (BTN_Topmost.IsChecked == true)
            {
                this.Topmost = true;
            }
            else
            {
                this.Topmost = false;
            }
        }

        private void On_BTN_Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion


        #region Private Method
        private void ResizeWindow(WebBrowser web)
        {
            this.Width = web.Width;
            this.Height = web.Height + GD_FootBar.ActualHeight;
        }

        private WINDOWPOS SnapToScreenEdge(WINDOWPOS wPos)
        {
            double screenRigth = ScreenHelper.Primary.WorkingArea.Right;
            double screenBottom = ScreenHelper.Primary.WorkingArea.Bottom;

            //Stick to Left
            if (wPos.x < SNAP_DIST && wPos.x > -SNAP_DIST)
            {
                wPos.x = 0;
            }

            //Stick to Top
            if (wPos.y < SNAP_DIST && wPos.y > -SNAP_DIST)
            {
                wPos.y = 0;
            }

            //Stick to Right
            if ((wPos.x + this.Width) > (screenRigth - SNAP_DIST)
                && (wPos.x + this.Width) < (screenRigth + SNAP_DIST))
            {
                wPos.x = Convert.ToInt32(screenRigth - this.Width);
            }

            //Stick to Bottom
            if ((wPos.y + this.Height) > (screenBottom - SNAP_DIST)
                && (wPos.y + this.Height) < (screenBottom + SNAP_DIST))
            {
                wPos.y = Convert.ToInt32(screenBottom - this.Height);
            }
            
            return wPos;
        }

        #endregion

    }
}
