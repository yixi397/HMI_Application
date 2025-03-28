using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMI_Application.Dialog_box;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_Application.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows.Controls;

namespace HMI_Application
{
    partial class MainWindowVM: ObservableObject
    {
       

        [ObservableProperty]
        private IDatas datas;
       
        Timer timer;
        public MainWindowVM(IDatas data)
        {
            this.datas = data;
            loadkeyValuePairs();
            timer = new Timer(refresh, null, 0, 1000);
            pageNavigate("主界面");
            //Datas.Rbool[20].Value

        }

        #region 显示状态功能
        [ObservableProperty]
        private bool showIsconnect;
        [ObservableProperty]
        private double showGatherTime;
        [ObservableProperty]
        private String showDate;
       
        [ObservableProperty]
        private double showMemoryUsageInM;


        static bool isExecuting = false;//回调函数执行中标志
        private void refresh(object ob)
        {
            if (isExecuting==true)
            {
                return;
            }

            try
            {
                isExecuting = true;
                ShowGatherTime = Datas.GatherTime;
                ShowIsconnect = Datas.ISconnect;

                // 获取当前进程
                Process currentProcess = Process.GetCurrentProcess();
                // 获取内存使用情况（以 M 为单位并保留两位小数）
                long memoryUsage = currentProcess.WorkingSet64;
                ShowMemoryUsageInM = memoryUsage / 1024.0 / 1024.0;

                ShowDate = DateTime.Now.ToString("G");
            }
            finally 
            {
                isExecuting = false;

            }

           
            
        }





        #endregion

        #region 导航功能


        [ObservableProperty]
        private Dictionary<string, Page> keyValuePairs = new Dictionary<string, Page>();
        
        private void loadkeyValuePairs()
        {
            if(LogOn.CurrendUser=="unicomp")
            {
                keyValuePairs = new Dictionary<string, Page>()
                {
                   {"主界面",App.Current.Services.GetService<MainPage>() },
                   {"变量监控",App.Current.Services.GetService<MonitorVarPage>() },
                   {"IO监控",App.Current.Services.GetService<MonitorIOPage>() },
                   {"故障监控",App.Current.Services.GetService<MonitorFaultPage>() },
                   {"轴设置",App.Current.Services.GetService<SetAxisPage>() },
                   {"其他设置",App.Current.Services.GetService<SetOtherPage>() },
                };
            }
            else
            {
                keyValuePairs = new Dictionary<string, Page>()
                {
                   {"主界面",App.Current.Services.GetService<MainPage>() },
                   {"IO监控",App.Current.Services.GetService<MonitorIOPage>() },
                   {"故障监控",App.Current.Services.GetService<MonitorFaultPage>() },
                };
            }
            
        }

        [ObservableProperty]
        private string currUrl;

        [RelayCommand]
        private void pageNavigate(string url)
        {
            var fix = App.Current.Services.GetService<MainWindow>().mainFrame.Navigate(KeyValuePairs[url]);
            CurrUrl = url;
        }
        #endregion

        #region 日志功能

        [RelayCommand]
        private void printfLog()
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Debug("MainWindowVM初始化完成");
            logger.Debug("MainWindowVMThis is a debug message.");
            logger.Info("MainWindowVMThis is an info message.");
            logger.Error("MainWindowVMThis is an error message.");
        }
        #endregion



    }
}
