using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using FKGDataEditor;
using FlowerMaster.Helpers;

namespace FlowerMaster.ViewModel
{
    public class GameFrameVM : ViewModelBase
    {
        #region Const

        public const double OriginalWidth = 1145;
        public const double OriginalHeight = 650;
        public const int OriginalAutoClick_X = 1020;
        public const int OriginalAutoClick_Y = 600;

        public const string XmlFileName = "GameFrameSize.xml";

        #endregion Const

        #region Private Member

        private double _width;
        private double _height;
        private double _zoomLevel;

        private int _autoClick_X;
        private int _autoClick_Y;

        #endregion Private Member

        #region Constuctor

        public GameFrameVM()
        {
            _width = OriginalWidth;
            _height = OriginalHeight;
            _zoomLevel = 0;

            _autoClick_X = OriginalAutoClick_X;
            _autoClick_Y = OriginalAutoClick_Y;
        }

        #endregion Constuctor

        #region Public Member

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));

                double ratio = OriginalWidth / OriginalHeight;
                double newHeight = Math.Round(_width / ratio);

                this.AutoClick_X = Convert.ToInt32(OriginalAutoClick_X * (_width / OriginalWidth));

                if (newHeight.CompareTo(_height) != 0)
                {
                    _height = newHeight;
                    OnPropertyChanged(nameof(Height));

                    this.AutoClick_Y = Convert.ToInt32(OriginalAutoClick_Y * (_height / OriginalHeight));

                    _zoomLevel = CefSharpHelper.Scale2ZoomLevel(_height / OriginalHeight);
                    OnPropertyChanged(nameof(ZoomLevel));
                }
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));

                double ratio = OriginalWidth / OriginalHeight;
                double newWidth = Math.Round(_height * ratio);

                this.AutoClick_Y = Convert.ToInt32(OriginalAutoClick_Y * (_height / OriginalHeight));

                if (newWidth.CompareTo(_width) != 0)
                {
                    _width = newWidth;
                    OnPropertyChanged(nameof(Width));

                    this.AutoClick_X = Convert.ToInt32(OriginalAutoClick_X * (_width / OriginalWidth));

                    _zoomLevel = CefSharpHelper.Scale2ZoomLevel(_width / OriginalWidth);
                    OnPropertyChanged(nameof(ZoomLevel));
                }
            }
        }

        public double ZoomLevel
        {
            get { return _zoomLevel; }
        }

        public int AutoClick_X
        {
            get { return _autoClick_X; }
            set { _autoClick_X = value; OnPropertyChanged(nameof(AutoClick_X)); }
        }

        public int AutoClick_Y
        {
            get { return _autoClick_Y; }
            set { _autoClick_Y = value; OnPropertyChanged(nameof(AutoClick_Y)); }
        }

        #endregion Public Member

        #region Public Method

        public void SaveSettings()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add(nameof(this.Width), this.Width.ToString());
            dict.Add(nameof(this.Height), this.Height.ToString());
            dict.Add(nameof(this.AutoClick_X), this.AutoClick_X.ToString());
            dict.Add(nameof(this.AutoClick_Y), this.AutoClick_Y.ToString());

            XElement el = new XElement("root", dict.Select(kv => new XElement(kv.Key, kv.Value)));
            el.Save(XmlFileName);
        }

        public bool LoadSettings()
        {
            bool ret = false;

            if (File.Exists(XmlFileName))
            {
                string xmlString = File.ReadAllText(XmlFileName);
                XElement rootElement = XElement.Parse(xmlString);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                foreach (var el in rootElement.Elements())
                {
                    dict.Add(el.Name.LocalName, el.Value);
                }

                this.Width = Convert.ToDouble(dict[nameof(this.Width)]);
                this.Height = Convert.ToDouble(dict[nameof(this.Height)]);
                this.AutoClick_X = Convert.ToInt32(dict[nameof(this.AutoClick_X)]);
                this.AutoClick_Y = Convert.ToInt32(dict[nameof(this.AutoClick_Y)]);

                ret = true;
            }

            return ret;
        }

        #endregion Public Method
    }
}