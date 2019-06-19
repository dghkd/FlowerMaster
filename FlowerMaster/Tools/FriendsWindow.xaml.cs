using FlowerMaster.Models;
using System.Windows;

namespace FlowerMaster
{
    /// <summary>
    /// FriendsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FriendsWindow
    {
        public FriendsWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataUtil.Game.friendList == null) return;
            dgFriendsList.ItemsSource = DataUtil.Game.friendList;
        }
    }
}