using HMI_Application.Services.DataProcessing;
using Microsoft.Extensions.DependencyInjection;
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
    /// CheckButton.xaml 的交互逻辑
    /// </summary>
    public partial class CheckButton : CheckBox
    {
        private ISetValue setValue
        {
            get { return App.Current.Services.GetService<ISetValue>(); }
        }
        public CheckButton()
        {
            InitializeComponent();
        }

        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        //状态显示依赖属性
        public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(bool), typeof(CheckButton), new PropertyMetadata(false, PropertyChangedCallback));
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CheckButton checkBox = d as CheckButton;
            checkBox.IsChecked = checkBox.State;
        }

        //地址依赖属性
        public static readonly DependencyProperty AddressProperty =
        DependencyProperty.Register("Address", typeof(string), typeof(CheckButton), new PropertyMetadata(null));
        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
           
            setValue.SetValue(Address, !State);
        }
    }
}
