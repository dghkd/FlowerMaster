using Newtonsoft.Json.Linq;
using System;

namespace FlowerMaster.Models
{
    internal interface IGameInfo
    {
        int gameServer { set; get; }
        string gameUrl { get; }
        bool isOnline { get; set; }
        bool isAuto { get; set; }
        bool canAuto { get; set; }
        DateTime serverTime { get; set; }
        string gameNewsUrl { get; }

        void CalcPlayerMaxAPExp();

        void CalcPlayerGamePoint(GameInfo.PlayerPointType timeType, JToken newVal, JToken newTime);

        void InitDaliyInfo();
    }
}