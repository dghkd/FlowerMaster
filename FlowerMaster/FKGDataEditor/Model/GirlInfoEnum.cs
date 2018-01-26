using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FKGDataEditor
{

    public class GirlInfoEnum
    {
        public enum Types
        {
            /// <summary>
            /// 斬
            /// </summary>
            Slash,
            /// <summary>
            /// 打
            /// </summary>
            Blunt,
            /// <summary>
            /// 突
            /// </summary>
            Pierce,
            /// <summary>
            /// 魔
            /// </summary>
            Magic,

            /// <summary>
            /// 不限
            /// </summary>
            NotCare,
        }


        public enum Nationalities
        {
            BlossomHill,
            LilyWood,
            BananaOcean,
            BergamotValley,
            WinterRose,
            LotusLake,

            /// <summary>
            /// 不限
            /// </summary>
            NotCare,
        }

        public static Dictionary<String, Types> String2Types = new Dictionary<String, Types>()
        {
            {Types.Slash.ToString(),Types.Slash},
            {Types.Blunt.ToString(),Types.Blunt},
            {Types.Pierce.ToString(),Types.Pierce},
            {Types.Magic.ToString(),Types.Magic}
        };

        public static Dictionary<String, Nationalities> String2Nationality = new Dictionary<String, Nationalities>()
        {
            {"Blossom Hill",Nationalities.BlossomHill},
            {"Lily Wood",Nationalities.LilyWood},
            {"Banana Ocean",Nationalities.BananaOcean},
            {"Bergamot Valley",Nationalities.BergamotValley},
            {"Winter Rose",Nationalities.WinterRose},
            {"Lotus Lake",Nationalities.LotusLake},
        };
    }
}
