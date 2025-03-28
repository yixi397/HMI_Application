using HMI_Application.Services;
using HMI_ApplicationConfigClient.DataModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HMI_Application.Control
{
    /// <summary>
    /// DeviceStateShow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceStateShow : TextBlock
    {
        #region 属性
     
        private class ShowInfos
        {
            public ShowInfos(string name,Brush brush)
            {
                this.Name = name;
                this.Brush = brush;
            }
            public string Name { get; set; }
            public Brush Brush { get; set; }

        }

        private Dictionary<short, ShowInfos> keyValuePairs;

        #endregion
        public DeviceStateShow()
        {
            InitializeComponent();
            loadDic();
            

        }
        private void loadDic()
        {
            
            keyValuePairs = new Dictionary<short, ShowInfos>();
            switch (Type)
            {
                case Deviceinfo.DeviceTypeEnum.Axis:
                    LoadAxis();
                    break;
                case Deviceinfo.DeviceTypeEnum.Cylinder:
                    LoadCylinder();
                    break;
                case Deviceinfo.DeviceTypeEnum.Door:
                    LoadDoor();
                    break;
                default:
                    break;
            }
            if(keyValuePairs.ContainsKey(State))
            {
                Text = keyValuePairs[State].Name;
                Background = keyValuePairs[State].Brush;
                
            }
           
        }

        #region 依赖属性
        public short State
        {
            get { return (short)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(short), typeof(DeviceStateShow), new PropertyMetadata((short)1, PropertyChangedCallback));
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            DeviceStateShow textBlock = d as DeviceStateShow;
            if(textBlock.keyValuePairs==null)
            {
                return;
            }
            if (textBlock.keyValuePairs.ContainsKey(textBlock.State)==false)
            {
                return;
            }
            textBlock.Text = textBlock.keyValuePairs[(short)e.NewValue].Name;
            textBlock.Background = textBlock.keyValuePairs[(short)e.NewValue].Brush;
            textBlock.Foreground = Brushes.Black;
        }



        

        public Deviceinfo.DeviceTypeEnum Type
        {
            get { return (Deviceinfo.DeviceTypeEnum)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty =
        DependencyProperty.Register("Type", typeof(Deviceinfo.DeviceTypeEnum), typeof(DeviceStateShow), new PropertyMetadata(Deviceinfo.DeviceTypeEnum.Axis, PropertyChangedCallback1));

        private static void PropertyChangedCallback1(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            DeviceStateShow textBlock = d as DeviceStateShow;
            textBlock.loadDic();
          
        }

        #endregion


        #region 状态字典
        private void LoadAxis()
        {
            keyValuePairs.Add(0, new ShowInfos("未使能", Brushes.LightGray));
            keyValuePairs.Add(1, new ShowInfos("故障停机", Brushes.Red));
            keyValuePairs.Add(2, new ShowInfos("停止中", Brushes.Yellow));
            keyValuePairs.Add(3, new ShowInfos("就绪", Brushes.LightGreen));
            keyValuePairs.Add(4, new ShowInfos("离散运动中", Brushes.Yellow));
            keyValuePairs.Add(5, new ShowInfos("连续运动中", Brushes.Yellow));
            keyValuePairs.Add(6, new ShowInfos("同步运行中", Brushes.Yellow));
            keyValuePairs.Add(7, new ShowInfos("原点回归中", Brushes.Yellow));
        }

        private void LoadCylinder()
        {
            keyValuePairs.Add(0, new ShowInfos("未使能", Brushes.LightGray));
            keyValuePairs.Add(1, new ShowInfos("故障", Brushes.Red));
            keyValuePairs.Add(2, new ShowInfos("停止中", Brushes.Yellow));
            keyValuePairs.Add(3, new ShowInfos("就绪", Brushes.LightGreen));
            keyValuePairs.Add(4, new ShowInfos("伸出中", Brushes.Yellow));
            keyValuePairs.Add(5, new ShowInfos("缩回中", Brushes.Yellow));
            keyValuePairs.Add(6, new ShowInfos("伸出到位", Brushes.LightGreen));
            keyValuePairs.Add(7, new ShowInfos("缩回到位", Brushes.LightGreen));
        }

        private void LoadDoor()
        {
            keyValuePairs.Add(0, new ShowInfos("未使能", Brushes.LightGray));
            keyValuePairs.Add(1, new ShowInfos("故障", Brushes.Red));
            keyValuePairs.Add(2, new ShowInfos("停止中", Brushes.Yellow));
            keyValuePairs.Add(3, new ShowInfos("就绪", Brushes.LightGreen));
            keyValuePairs.Add(4, new ShowInfos("打开中", Brushes.Yellow));
            keyValuePairs.Add(5, new ShowInfos("关闭中", Brushes.Yellow));
            keyValuePairs.Add(6, new ShowInfos("打开到位", Brushes.LightGreen));
            keyValuePairs.Add(7, new ShowInfos("关闭到位", Brushes.LightGreen));
        }

        #endregion
    }
}
