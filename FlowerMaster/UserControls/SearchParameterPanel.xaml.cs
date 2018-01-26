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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FKGDataEditor;

namespace FlowerMaster.UserControls
{
    /// <summary>
    /// SearchParameterPanel.xaml 的互動邏輯
    /// </summary>
    public partial class SearchParameterPanel : UserControl
    {
        private ObservableCollection<GirlInfoEnum.Types> _colleType = new ObservableCollection<GirlInfoEnum.Types>();
        private ObservableCollection<GirlInfoEnum.Nationalities> _colleNationalities = new ObservableCollection<GirlInfoEnum.Nationalities>();

        public SearchParameterPanel()
        {
            InitializeComponent();
            InitParameters();
        }

        /// <summary>
        /// 取得搜尋參數
        /// </summary>
        public SearchParameterVM Parameter { get; private set; }

        /// <summary>
        /// 初始化各搜尋參數選項
        /// </summary>
        private void InitParameters()
        {
            //星數選項已於控制項初始化時建立，請於xaml中查閱"CMB_Rare"控制項

            //建立種類選項
            foreach (GirlInfoEnum.Types item in Enum.GetValues(typeof(GirlInfoEnum.Types)))
            {
                if (item == GirlInfoEnum.Types.NotCare)
                {
                    _colleType.Insert(0, item);
                }
                else
                {
                    _colleType.Add(item);
                }
            }

            //建立國家選項
            foreach (GirlInfoEnum.Nationalities item in Enum.GetValues(typeof(GirlInfoEnum.Nationalities)))
            {
                if (item == GirlInfoEnum.Nationalities.NotCare)
                {
                    _colleNationalities.Insert(0, GirlInfoEnum.Nationalities.NotCare);
                }
                else
                {
                    _colleNationalities.Add(item);
                }
            }

            //加入ComboBox列表
            CMB_Type.ItemsSource = _colleType;
            CMB_Nationality.ItemsSource = _colleNationalities;

            //初始化搜尋參數
            this.Parameter = new SearchParameterVM();
            

            this.DataContext = this.Parameter;
            this.Parameter.PropertyChanged += On_Parameter_PropertyChanged;
        }

        private void On_Parameter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine(this.Parameter);
        }

        private void On_TXTBOX_Keyword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
            {
                return;
            }

            //轉移焦點觸發Parameter PropertyChanged事件
            CMB_Nationality.Focus();
            TXTBOX_Keyword.Focus();

            e.Handled = true;
        }
    }
}
