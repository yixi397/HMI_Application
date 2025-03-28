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
    /// BitButton.xaml 的交互逻辑
    /// </summary>
    public partial class BitButton : Button
    {
        /// <summary>
        /// 控制模式
        /// </summary>
        public enum PressDownModeEnum
        {
            Set, Rest, Alt, PressdownON, PressdownOFF
        }

        
        #region 依赖属性
        //地址
        public static readonly DependencyProperty AddressProperty =
        DependencyProperty.Register("Address", typeof(string), typeof(BitButton), new PropertyMetadata(null));
        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }

        //确认信息
        public static readonly DependencyProperty ConfirmInformationProperty =
        DependencyProperty.Register("ConfirmInformation", typeof(string), typeof(BitButton), new PropertyMetadata(null));
        /// <summary>
        /// 确认信息
        /// </summary>
        public string ConfirmInformation
        {
            get { return (string)GetValue(ConfirmInformationProperty); }
            set { SetValue(ConfirmInformationProperty, value); }
        }

        //控制模式
       

        public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register("Mode", typeof(PressDownModeEnum), typeof(BitButton), new PropertyMetadata(PressDownModeEnum.Set));
        /// <summary>
        /// 确认信息
        /// </summary>
        public PressDownModeEnum Mode
        {
            get { return (PressDownModeEnum)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        #endregion





        private ISetValue setValue
        {
            get {return App.Current.Services.GetService<ISetValue>(); }
        }
       
        public BitButton()
        {
            InitializeComponent();
        }

        bool state;

        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            

            if (Mode == PressDownModeEnum.Set)
            {
                if (ConfirmInformation != null && ConfirmInformation != "")
                {
                    MessageBoxResult bt = MessageBox.Show(ConfirmInformation, "请确认", MessageBoxButton.OKCancel);
                    if (bt == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }
            }
            switch (Mode)
            {
                case PressDownModeEnum.Set:
                    state = true;
                    break;
                case PressDownModeEnum.Rest:
                    state = false;
                    break;
                case PressDownModeEnum.Alt:
                    state = !state;
                    break;
                case PressDownModeEnum.PressdownON:
                    state = true;
                    break;
                case PressDownModeEnum.PressdownOFF:
                    state = false;
                    break;
                default:
                    break;
            }
            setValue.SetValue(Address,state);
        }

        private void Button_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (Mode)
            {
                case PressDownModeEnum.Set:
                    //Data.SetValue = true;
                    return;
                case PressDownModeEnum.Rest:
                    //Data.SetValue = false;
                    return;
                case PressDownModeEnum.Alt:
                    //Data.SetValue = !Data.Value;
                    return;
                case PressDownModeEnum.PressdownON:
                    state = false;
                    break;
                case PressDownModeEnum.PressdownOFF:
                    state = true;
                    break;
                default:
                    break;
            }
            setValue.SetValue(Address, state);
        }

        
    }
}
