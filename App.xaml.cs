using HMI_Application.Control;
using HMI_Application.Dialog_box;
using HMI_Application.Services.Communication;
using HMI_Application.Services.Communication.ModbusTcp;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_Application.ViewModels;
using HMI_Application.Views;
using HMI_ApplicationConfigClient.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace HMI_Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int InputNumber = 100;
        public static int OutputNumber = 100;
        public App()
        {
            
            Services = ConfigureServices();
            this.InitializeComponent();
        }
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; }
        public static Logger Logger=NLog.LogManager.GetCurrentClassLogger();
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            //注册视图和数据模型
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowVM>();

            services.AddSingleton<MonitorVarPage>();
            services.AddSingleton<MonitorVarVM>();

            services.AddSingleton<MonitorIOPage>();
            services.AddSingleton<MonitorIOVM>();

            services.AddSingleton<MonitorFaultPage>();
            services.AddSingleton<MonitorFaultVM>();

            services.AddSingleton<MainPage>();
            services.AddSingleton<MainVM>();

            services.AddSingleton<SetAxisPage>();
            services.AddSingleton<SetAxisVM>();

            services.AddSingleton<SetOtherPage>();
            services.AddSingleton<SetOtherVM>();


            
            //加载配置
            String recipeName = Settings1.Default.Recipe;
            Logger.Debug("加载数据:"+recipeName);
            Dictionary<string, string> dataDictionary = loadConfig(recipeName);
            services.AddDbContext<HMIDbContext>(opt =>
            {
                string s = "Data Source =config\\" + recipeName + "\\HMI_Application.db";
                opt.UseSqlite(s);
            });
          
            //创建零时serviceProvider，获取数据库上下文
            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetRequiredService<HMIDbContext>();

            //注册接口和实现
            string ip= dataDictionary["IP"];
            int port=Convert.ToInt16(dataDictionary["Port"]);
            string type = dataDictionary["type"];
            if (dataDictionary.ContainsKey("InputNumber") && dataDictionary.ContainsKey("OutputNumber"))
            {
                InputNumber= Convert.ToInt16(dataDictionary["InputNumber"]);
                OutputNumber = Convert.ToInt16(dataDictionary["OutputNumber"]);
            }
            switch (type)
            {
                case "InovanceH5uModbustcp":
                    IModbusAddress address = new InovanceH5UModbusAddress();
                    DataProcessingModbusTcp modbusTcp = new DataProcessingModbusTcp(dbContext, new ModbusTcpHelper(ip, port), address);
                    services.AddSingleton<IDatas>(modbusTcp);
                    services.AddSingleton<ISetValue>(modbusTcp);
                    break;
                case "OPCUA":
                case "InovanceAM500OPCUA":
                    Action<MOpcUaHelper> connect = x =>
                    {
                       x.OpenConnectOfAnonymous($"opc.tcp://{ip}:{port}");
                    };
                    DataProcessingOpcUa ua = new DataProcessingOpcUa(dbContext, connect);
                    services.AddSingleton<IDatas>(ua);
                    services.AddSingleton<ISetValue>(ua);
                    break;
                default:
                    MessageBox.Show("加载配置异常");
                    throw new Exception("加载配置异常");
            }
            Logger.Info("加载配置完成");
            Logger.Info($"ip:{ip}-port:{port}-type:{type}");
            
            return services.BuildServiceProvider();
        }
        bool flag = false;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (flag)
            {
                return;
            }
            flag = true;
            

            Services.GetService<MonitorVarPage>().DataContext = Services.GetService<MonitorVarVM>();
            Services.GetService<MonitorIOPage>().DataContext = Services.GetService<MonitorIOVM>();
            Services.GetService<MonitorFaultPage>().DataContext = Services.GetService<MonitorFaultVM>();
            Services.GetService<MainPage>().DataContext = Services.GetService<MainVM>();
            Services.GetService<SetAxisPage>().DataContext = Services.GetService<SetAxisVM>();
            Services.GetService<SetOtherPage>().DataContext = Services.GetService<SetOtherVM>();

            //关联视图和视图模型
            //var mainWindow = Services.GetService<MainWindow>();
            //mainWindow.DataContext = Services.GetService<MainWindowVM>();
            //mainWindow.Show();
            LogOn logOn=new LogOn();
            logOn.Show();

            Logger.Info("初始化完成");
        }

        //加载配置字典
        private static Dictionary<string, string> loadConfig(String recipeName)
        {
            // 定义字典用于存储数据
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();
            // 文件路径，这里假设文件和可执行程序在同一目录下，可根据实际情况修改
            string filePath = "Config\\" + recipeName + "\\Settings.txt";
            try
            {
                // 使用StreamReader读取文件内容
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        // 按":"分割每行内容，获取键和值
                        string[] parts = line.Split(':');
                        if (parts.Length == 2)
                        {
                            string key = parts[0].Trim();
                            string value = parts[1].Trim();
                            dataDictionary[key] = value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return dataDictionary;
        }


    }

}
