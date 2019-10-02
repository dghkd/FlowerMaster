using CefSharp;
using CefSharp.WinForms;
using FlowerMaster.Models;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FlowerMaster.Helpers
{
    public class CefSharpHelper
    {
        #region Win32 API

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        #endregion Win32 API

        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        #region Public Member

        /// <summary>
        /// 取得或設定CEF暫存資料夾路徑
        /// </summary>
        public static string CefCachePath { get; set; }

        #endregion Public Member

        #region Public Method

        /// <summary>
        /// CEF全域設定初始化
        /// <para>注意:只能初始化一次，且必須於ChromiumWebBrowser宣告建立實體之前進行初始化</para>
        /// </summary>
        public static void CefInitialize()
        {
            if (CefCachePath == null
                || CefCachePath == "")
            {
                CefCachePath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
               "FlowerMaster",
               "Chromium");
            }

            var cefSettings = new CefSettings()
            {
                CachePath = CefCachePath
            };

            cefSettings.CefCommandLineArgs.Add("proxy-server", GetLocalProxySettingString());
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            Cef.EnableHighDPISupport();
            Cef.Initialize(cefSettings);
        }

        /// <summary>
        /// 程式退出時呼叫關閉CEF
        /// </summary>
        public static void Close()
        {
            Cef.Shutdown();
        }

        /// <summary>
        /// 抽取遊戲區畫面
        /// </summary>
        /// <param name="webBrowser">已載入遊戲的ChromiumWebBrowser實體</param>
        public static bool TakeoutGameFrame(ChromiumWebBrowser webBrowser)
        {
            var browser = webBrowser.GetBrowser();
            var gameFrame = browser.GetFrame("game_frame");
            if (gameFrame == null)
            {
                return false;
            }

            IFrame frame = browser.GetFrameIdentifiers()
                .Select(x => browser.GetFrame(x))
                .FirstOrDefault(x => x.Url.Contains("/flower"));  //TODO: 依使用者選擇的伺服器URL做Contains
            if (frame != null)
            {
                var js = $"var style = document.createElement(\"style\"); style.innerHTML = \"{DataUtil.Config.sysConfig.userCSS.Replace("\r\n", " ")}\"; document.body.appendChild(style);";
                webBrowser.GetMainFrame().ExecuteJavaScriptAsync(js);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 嘗試取得ChromiumWebBrowser的Handle值。
        /// </summary>
        public static bool TryFindHandle(IntPtr browserHandle, out IntPtr chromeWidgetHostHandle)
        {
            var classDetails = new ClassDetails();
            var gcHandle = GCHandle.Alloc(classDetails);

            var childProc = new EnumWindowProc(EnumWindow);
            EnumChildWindows(browserHandle, childProc, GCHandle.ToIntPtr(gcHandle));

            chromeWidgetHostHandle = classDetails.DescendantFound;

            gcHandle.Free();

            return classDetails.DescendantFound != IntPtr.Zero;
        }

        public static double ZoomLevel2Scale(double zoomLevel)
        {
            double ret = Math.Pow(1.2, zoomLevel);
            return ret;
        }

        public static double Scale2ZoomLevel(double scale)
        {
            double ret = Math.Log(scale, 1.2);
            return ret;
        }

        public static void ClearCache()
        {
            if (Directory.Exists(CefCachePath))
            {
                Directory.Delete(CefCachePath, true);
            }
        }

        #endregion Public Method

        #region Private Method

        /// <summary>
        /// 取得代理設定字串
        /// </summary>
        private static String GetLocalProxySettingString()
        {
            if (DataUtil.Config.sysConfig.proxyType == SysConfig.ProxySettingsType.DirectAccess)
            {
                return String.Format($"http=127.0.0.1:{DataUtil.Config.localProxyPort}", "local");
            }
            else if (DataUtil.Config.sysConfig.proxyType == SysConfig.ProxySettingsType.UseSystemProxy)
            {
                return Convert.ToString((DataUtil.Config.localProxyPort));
            }
            else if (DataUtil.Config.sysConfig.proxyType == SysConfig.ProxySettingsType.UseUserProxy)
            {
                return String.Format($"http=127.0.0.1:{DataUtil.Config.localProxyPort};"
                                    + $"https={DataUtil.Config.sysConfig.proxyServer}:{DataUtil.Config.sysConfig.proxyPort};"
                                    , "local");
            }
            return "";
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            const string chromeWidgetHostClassName = "Chrome_RenderWidgetHostHWND";

            var buffer = new StringBuilder(128);
            GetClassName(hWnd, buffer, buffer.Capacity);

            if (buffer.ToString() == chromeWidgetHostClassName)
            {
                var gcHandle = GCHandle.FromIntPtr(lParam);

                var classDetails = (ClassDetails)gcHandle.Target;

                classDetails.DescendantFound = hWnd;
                return false;
            }

            return true;
        }

        #endregion Private Method
    }

    public class ClassDetails
    {
        public IntPtr DescendantFound { get; set; }
    }

    public class JsDialogHandler : IJsDialogHandler
    {
        public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload, IJsDialogCallback callback)
        {
            return true;
        }

        public void OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public bool OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            System.Diagnostics.Debug.WriteLine($"[OnJSDialog] Url:{originUrl} msg:{messageText}");
            callback.Continue(false);
            return true;
        }

        public void OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }
    }
}