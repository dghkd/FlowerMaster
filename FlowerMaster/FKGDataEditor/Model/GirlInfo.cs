using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace FKGDataEditor
{
    public class GirlInfo
    {
        public int ID { get; set; }
        public ImageSource ImageSrc { get; set; }
        public String ImgBase64 { get; set; }
        public String Names { get; set; }
        public String NamesJPN { get; set; }
        public String NamesCHT { get; set; }
        public String NamesCHS { get; set; }
        public String NamesENU { get; set; }
        public int FKGID { get; set; }

        public int Rare { get; set; }
        public GirlInfoEnum.Types Type { get; set; }
        public GirlInfoEnum.Nationalities Nationality { get; set; }
        public String Note { get; set; }
        
        public static String ImgToBase64(BitmapImage bmp)
        {
            byte[] bytes = null;
            String ret = "";
            try
            {
                Stream smarket = bmp.StreamSource;

                if (smarket != null && smarket.Length > 0)
                {
                    smarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(smarket))
                    {
                        bytes = br.ReadBytes((int)smarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling
            }

            ret = Convert.ToBase64String(bytes);

            return ret;
        }


        public static BitmapImage Base642Image(String strBase64)
        {
            if (strBase64 == null || strBase64 == "")
            {
                return null;
            }
            byte[] bytes = Convert.FromBase64String(strBase64);
            BitmapImage bmp = null;
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(bytes);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }
            return bmp;
        }

        public static String ImgFileToBase64(String imgFilename)
        {
            try
            {
                Bitmap bmp = new Bitmap(imgFilename);

                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
        
    }
}
