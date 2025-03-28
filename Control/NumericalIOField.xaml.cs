using HMI_Application.Dialog_box;
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
    /// NumericalIOField.xaml 的交互逻辑
    /// </summary>
    public partial class NumericalIOField : TextBox
    {
        private ISetValue setValue
        {
            get { return App.Current.Services.GetService<ISetValue>(); }
        }

        public NumericalIOField()
        {
            InitializeComponent();
        }

        
        //地址
        public static readonly DependencyProperty AddressProperty =
        DependencyProperty.Register("Address", typeof(string), typeof(NumericalIOField), new PropertyMetadata(null));
        public string Address
        {
            get { return (string)GetValue(AddressProperty); }
            set { SetValue(AddressProperty, value); }
        }


        //输入类型
        public static readonly DependencyProperty InDataTypeProperty =
        DependencyProperty.Register("InDataType", typeof(InputBoxDialog.DataTypeEnum), typeof(NumericalIOField), new PropertyMetadata(InputBoxDialog.DataTypeEnum.Real));
        public InputBoxDialog.DataTypeEnum InDataType 
        {
            get { return (InputBoxDialog.DataTypeEnum)GetValue(InDataTypeProperty); }
            set { SetValue(InDataTypeProperty, value);}
        }


        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            InputBoxDialog inputBoxDialog = new InputBoxDialog(Text, InDataType);

            inputBoxDialog.ShowDialog();
            string strvalue = inputBoxDialog.InputValue;
            if (inputBoxDialog.OKflag == true)
            {
                if (InDataType == InputBoxDialog.DataTypeEnum.Short)
                {
                    short value= Convert.ToInt16(strvalue);
                    setValue.SetValue(Address, value);
                }
                else if (InDataType == InputBoxDialog.DataTypeEnum.Dint)
                {
                    Int32 value = Convert.ToInt32(strvalue);
                    setValue.SetValue(Address, value);
                }
                else if (InDataType == InputBoxDialog.DataTypeEnum.Real)
                {
                    float value = Convert.ToSingle(strvalue);
                    setValue.SetValue(Address, value);
                }
                else
                {
                    MessageBox.Show("数据类型异常");
                }

            }

        }

    }
}
