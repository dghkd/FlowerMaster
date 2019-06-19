using FKGDataEditor;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace FlowerMaster
{
    /// <summary>
    /// GirlsNamesListWindow.xaml 的互動邏輯
    /// </summary>
    public partial class GirlsNamesListWindow : MetroWindow
    {
        /// <summary>
        /// 顯示資料集合
        /// </summary>
        private ObservableCollection<GirlInfoVM> _girlColle = new ObservableCollection<GirlInfoVM>();

        /// <summary>
        /// 取消資料集合更新用Token Source
        /// </summary>
        private CancellationTokenSource _cancelColleUpdateToken;

        public GirlsNamesListWindow()
        {
            InitializeComponent();

            Task.Factory.StartNew(() =>
            {
                //初始化資料庫控制器
                SQLiteCtrl.Data.Init();

                //讀取所有資料
                List<GirlInfo> list = SQLiteCtrl.Data.LoadData();

                UpdateGirlColle(list);
            });

            //監聽搜尋參數改變事件
            PANEL_SearchParam.Parameter.PropertyChanged += On_SearchParameter_PropertyChanged;

            //設定顯示資料集合並產生內容
            PANEL_NamesList.ItemsSource = _girlColle;
        }

        /// <summary>
        /// 搜尋參數改變後事件處理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void On_SearchParameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            SearchParameterVM vm = sender as SearchParameterVM;
            if (sender != null)
            {
                List<GirlInfo> searchRet = SQLiteCtrl.Data.SearchData(vm.Keyword, vm.Type, vm.Nationality, vm.Rare);

                UpdateGirlColle(searchRet);
            }
        }

        private void UpdateGirlColle(List<GirlInfo> list)
        {
            Task.Factory.StartNew(() =>
            {
                //判斷目前顯示集合是否正在更新中 可取消更新
                if (_cancelColleUpdateToken != null)
                {
                    //發出取消要求
                    _cancelColleUpdateToken.Cancel();

                    //等待上一個執行緒完全退出
                    SpinWait.SpinUntil(() => _cancelColleUpdateToken == null, 3000);
                }

                //建立新token
                _cancelColleUpdateToken = new CancellationTokenSource();

                //清除集合
                this.Dispatcher.Invoke(() =>
                {
                    _girlColle.Clear();
                    SV_Scroller.ScrollToVerticalOffset(0);
                });

                //開始將原始資料轉化為ViewModel並加入顯示集合
                foreach (GirlInfo info in list)
                {
                    //若收到取消要求則離開迴圈
                    if (_cancelColleUpdateToken.IsCancellationRequested)
                    {
                        break;
                    }

                    //加入顯示集合
                    GirlInfoVM vm = new GirlInfoVM(info);
                    this.Dispatcher.Invoke(() =>
                    {
                        _girlColle.Add(vm);
                    });

                    //釋放CPU，減緩UI執行緒連續處理新增項目顯示造成視窗假死狀態
                    SpinWait.SpinUntil(() => false, 10);
                }

                //更新結束，清除token
                _cancelColleUpdateToken = null;
            });
        }
    }
}