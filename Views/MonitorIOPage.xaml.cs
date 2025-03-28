using HMI_Application.Dialog_box;
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

namespace HMI_Application.Views
{
    /// <summary>
    /// MonitorIOPage.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorIOPage : Page
    {
        public MonitorIOPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(LogOn.IsEnable)
            {
                this.contextMenu.IsEnabled = true;
                this.input.IsReadOnly = false;
                this.output.IsReadOnly = false;

            }
            else
            {
                this.contextMenu.IsEnabled = false;
                this.input.IsReadOnly = true;
                this.output.IsReadOnly = true;
            }
        }
    }
}
