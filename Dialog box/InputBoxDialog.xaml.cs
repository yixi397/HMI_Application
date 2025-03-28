using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using System.Windows.Shapes;

namespace HMI_Application.Dialog_box
{
    /// <summary>
    /// InputBoxDialog.xaml 的交互逻辑
    /// </summary>
    public partial class InputBoxDialog : Window
    {
        // 定义一个枚举类型，表示输入限制的类型

        public enum DataTypeEnum
        {
            None, Short, Dint, Real
        }
        private DataTypeEnum type;


        
        public InputBoxDialog(string InitShowData, DataTypeEnum type = DataTypeEnum.None, string Title = "请输入数据")
        {
            this.Title = Title;
            Owner = App.Current.Services.GetService<MainWindow>();
            if (Owner != null)
            {
                double left = Owner.Left + (Owner.Width - 500) / 2;
                double top = Owner.Top + (Owner.Height - 300) / 2;
                Left = left;
                Top = top;
            }

            InitializeComponent();
          

            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
         

           

            this.inputTextBox.Text = InitShowData;
            this.type = type;
            OKflag = false;
            inputTextBox.Focus();
            
            inputTextBox.SelectAll();
        }

       

        public string InputValue { get; set; }
        public bool OKflag { get; private set; }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                okButton_Click(sender, e);
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (type == DataTypeEnum.None)
            {
                InputValue = inputTextBox.Text;
                OKflag = true;
                this.Close();
                return;
            }
            if (type == DataTypeEnum.Short)
            {
                short temp;
                if (short.TryParse(inputTextBox.Text, out temp))
                {
                    InputValue = inputTextBox.Text;
                    OKflag = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("输入错误");
                    inputTextBox.Text = "";
                    return;
                }

            }

            if (type == DataTypeEnum.Dint)
            {
                Int32 temp;
                if (Int32.TryParse(inputTextBox.Text, out temp))
                {
                    InputValue = inputTextBox.Text;
                    OKflag = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("输入错误");
                    inputTextBox.Text = "";
                    return;
                }

            }
            if (type == DataTypeEnum.Real)
            {
                float temp;
                if (float.TryParse(inputTextBox.Text, out temp))
                {
                    InputValue = inputTextBox.Text;
                    OKflag = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("输入错误");
                    inputTextBox.Text = "";
                    return;
                }

            }


        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

    }
}
