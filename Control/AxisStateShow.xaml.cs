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
    /// AxisStateShow.xaml 的交互逻辑
    /// </summary>
    public partial class AxisStateShow : TextBlock
    {
        public AxisStateShow()
        {
            InitializeComponent();
            
          
        }

       public static readonly DependencyProperty StateProperty =
       DependencyProperty.Register("State", typeof(int), typeof(AxisStateShow), new PropertyMetadata(0));

        public int State
        {
            get { return (int)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
    }
}
