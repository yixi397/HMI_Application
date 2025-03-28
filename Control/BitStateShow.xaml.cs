using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// BitStateShow.xaml 的交互逻辑
    /// </summary>
    public partial class BitStateShow : TextBlock
    {
        public BitStateShow()
        {
            InitializeComponent();

          
            this.Text = this.FalseText;
            this.Background = this.FalseBackground;

            State = true;
        }
        #region 依赖属性
        
        public bool State
        {
            get { return (bool)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }
        public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register("State", typeof(bool), typeof(BitStateShow), new PropertyMetadata(false, PropertyChangedCallback));
        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BitStateShow textBlock = d as BitStateShow;
            if ((bool)e.NewValue)
            {
                textBlock.Text = textBlock.TrueText;
                textBlock.Background = textBlock.TrueBackground;

            }
            else
            {
                textBlock.Text = textBlock.FalseText;
                textBlock.Background = textBlock.FalseBackground;
            }

        }


        public string TrueText
        {
            get { return (string)GetValue(TrueTextProperty); }
            set { SetValue(TrueTextProperty, value); }
        }
        public static readonly DependencyProperty TrueTextProperty =
        DependencyProperty.Register("TrueText", typeof(string), typeof(BitStateShow), new PropertyMetadata("True"));

       

        public static readonly DependencyProperty FalseTextProperty =
        DependencyProperty.Register("FalseText", typeof(string), typeof(BitStateShow), new PropertyMetadata("False"));
        public string FalseText
        {
            get { return (string)GetValue(FalseTextProperty); }
            set { SetValue(FalseTextProperty, value); }
        }

        public static readonly DependencyProperty TrueBackgroundProperty =
        DependencyProperty.Register("TrueBackground", typeof(Brush), typeof(BitStateShow), new PropertyMetadata(Brushes.LightGreen));
        public Brush TrueBackground
        {
            get { return (Brush)GetValue(TrueBackgroundProperty); }
            set { SetValue(TrueBackgroundProperty, value); }
        }

        public static readonly DependencyProperty FalseBackgroundProperty =
        DependencyProperty.Register("FalseBackground", typeof(Brush), typeof(BitStateShow), new PropertyMetadata(Brushes.LightGray));
        public Brush FalseBackground
        {
            get { return (Brush)GetValue(FalseBackgroundProperty); }
            set { SetValue(FalseBackgroundProperty, value); }
        }
        #endregion
    }
}
