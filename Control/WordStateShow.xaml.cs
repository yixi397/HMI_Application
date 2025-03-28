using HMI_ApplicationConfigClient.DataModels;
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

namespace HMI_Application.Control
{
    /// <summary>
    /// WordStateShow.xaml 的交互逻辑
    /// </summary>
    public partial class WordStateShow : TextBlock
    {


        public class ShowInfos
        {
            public ShowInfos(string name, Brush brush)
            {
                this.Name = name;
                this.Brush = brush;
            }
            public string Name { get; set; }
            public Brush Brush { get; set; }

        }

       

      
        public WordStateShow()
        {
            InitializeComponent(); 
        }

        #region 依赖属性
        public short State
        {
            get { return (short)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(short), typeof(WordStateShow), new PropertyMetadata((short)1, PropertyChangedCallback));
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WordStateShow textBlock = d as WordStateShow;
            textBlock.Text = textBlock.keyValuePairs[(short)e.NewValue].Name;
            textBlock.Background = textBlock.keyValuePairs[(short)e.NewValue].Brush;
        }

        public Dictionary<short, ShowInfos> keyValuePairs
        {
            get { return (Dictionary<short, ShowInfos>)GetValue(keyValuePairsProperty); }
            set { SetValue(keyValuePairsProperty, value); }
        }
        public static readonly DependencyProperty keyValuePairsProperty =
        DependencyProperty.Register("keyValuePairs", typeof(Dictionary<short, ShowInfos>), typeof(WordStateShow), new PropertyMetadata(null));

        #endregion

       
    }
}
