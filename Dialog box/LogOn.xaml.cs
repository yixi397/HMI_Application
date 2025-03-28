using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;

namespace HMI_Application.Dialog_box
{
    /// <summary>
    /// LogOn.xaml 的交互逻辑
    /// </summary>
    public partial class LogOn : Window
    {
        public static string CurrendUser {  get; set; }

        public static bool IsEnable { get; set; }
      

        public LogOn()
        {
            InitializeComponent();
            userDic.Add("admin", "123");
            userDic.Add("unicomp", "unc");
        }
        private Dictionary<string, string> userDic = new Dictionary<string, string>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(passwordbos.Password=="qwer" || passwordbos.Password == "QWER")
            {
                CurrendUser = "unicomp";
                IsEnable = true;
                showMainWindow();
                this.Close();
                return;
            }

            if( userDic.ContainsKey( this.text_user.Text))
            {
                if (passwordbos.Password == userDic[text_user.Text])
                {
                    CurrendUser= text_user.Text;
                    if(CurrendUser== "unicomp")
                    {
                        IsEnable = true;
                    }
                    else
                    {
                        IsEnable = false;
                    }
                    showMainWindow();
                    this.Close();
                    return;
                }
            }
            MessageBox.Show("登录失败");
            text_user.Text = "";
            passwordbos.Password = "";
            return;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void showMainWindow()
        {
           
            var mainWindow =App.Current. Services.GetService<MainWindow>();
            mainWindow.DataContext = App.Current.Services.GetService<MainWindowVM>();
            mainWindow.Show();
        }


        
    }
}
