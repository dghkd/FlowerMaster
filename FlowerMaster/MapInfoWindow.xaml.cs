using FlowerMaster.Models;
using System.Windows;

namespace FlowerMaster
{
    /// <summary>
    /// MapInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MapInfoWindow
    {
        public MapInfoWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataUtil.Game.bossList == null) return;
            dgBossList.ItemsSource = DataUtil.Game.bossList;
        }
    }
}