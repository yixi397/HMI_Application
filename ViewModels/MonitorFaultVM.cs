using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using HMI_Application.Services;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using NLog;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static HMI_Application.ViewModels.MonitorFaultVM;

namespace HMI_Application.ViewModels
{
    partial class MonitorFaultVM:ObservableObject
    { 
        private readonly IDatas data;
        Dictionary<int,FaultInfo> faultDictionary= new Dictionary<int, FaultInfo>();

        Timer timer;
        public MonitorFaultVM(IDatas data)
        {
            
            this.data = data;
            loadFaultsKeyValue();
            // 创建并配置线程
            Thread backgroundThread = new Thread(refresh);
            // 将线程设置为后台线程
            backgroundThread.IsBackground = true;
            // 启动线程
            backgroundThread.Start();
           
           
        }

     



        #region 加载故障字典

        private void loadFaultsKeyValue()
        {
            List<FaultInfo> faults = new List<FaultInfo>();
            
            try
            {
                string path = "config//" + Settings1.Default.Recipe + "//FaultDictionary.csv";
                using (var reader = new StreamReader(path, Encoding.Default))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    faults = csv.GetRecords<FaultInfo>().ToList();
                }
                for (int i = 0; i < faults.Count; i++)
                {
                    faultDictionary.Add(faults[i].Id, faults[i]);
                } 
                    
            }
            catch (Exception ex)
            {
               Tools.logger.Error("加载FaultDictionary.csv文件异常: " + ex.Message);
            }
            if(faults.Count==0)
            {
                Tools.logger.Error("加载FaultDictionary.csv文件异常");
            }
        }

        public class FaultInfo
        {
            
            public int Id { get; set; }
            public string Name { get; set; }
            public string Level { get; set; }
        }

        #endregion


        #region 查询故障状态
        [ObservableProperty]
        private ObservableCollection<ShowFaultInfo> currentFaults = new ObservableCollection<ShowFaultInfo>();

       
        public class ShowFaultInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string SatrtTime {  get; set; }
            public string Level { get; set; }
          
        }
        ShowFaultInfo[] faultInfoarr = new ShowFaultInfo[20];
        private void refresh()
        {
            for (int i = 0; i < faultInfoarr.Length; i++)
            {
                faultInfoarr[i] = new ShowFaultInfo();
            }
        lb1:
            int number = 0;
            for (int i = 0; i < 20; i++)
            {
                if (data.Rdint[20+i].Value!=0)
                {
                    if (faultDictionary.ContainsKey(data.Rdint[20+i].Value) ==true)
                    {
                        faultInfoarr[i].Name = faultDictionary[data.Rdint[20 + i].Value].Name;
                        faultInfoarr[i].Level = faultDictionary[data.Rdint[20 + i].Value].Level.ToString();
                        faultInfoarr[i].Id= faultDictionary[data.Rdint[20 + i].Value].Id;
                        number++;
                    }
                  
                }
                if (data.Rdint[20 + i].Value == 0)
                {
                    faultInfoarr[i].Name = "";
                    faultInfoarr[i].Level = "-1";
                    faultInfoarr[i].Id = 0;

                }
            }
            //新增
            for (int i = 0; i < 20; i++)
            {
                
                bool flag = CurrentFaults.Any(p => p.Id == faultInfoarr[i].Id);
                if(flag==false && number!=0 && faultInfoarr[i].Id!=0)
                {
                    ShowFaultInfo var = new ShowFaultInfo();
                    var.Name = faultDictionary[faultInfoarr[i].Id].Name;
                    var.Level = faultDictionary[faultInfoarr[i].Id].Level.ToString();
                    var.Id = faultDictionary[faultInfoarr[i].Id].Id;
                    var.SatrtTime = DateTime.Now.ToString("G");
                    Tools.logger.Error($"故障触发-ID:{faultInfoarr[i].Id}-{faultInfoarr[i].Name}");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 在这里执行对 SourceCollection 的更改操作，比如向 ObservableCollection 添加元素等
                        CurrentFaults.Add(var);
                        
                    });
                   
                }
                
            }
            //移除
            int index = 0;
            for(int i=0;i< CurrentFaults.Count-index; i++)
            {
                bool flag=false;
                for (int j = 0; j < 20; j++)
                {
                    if (CurrentFaults[i].Id== faultInfoarr[j].Id)
                    {
                        flag = true;
                    }
                }
                if(flag==false)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // 在这里执行对 SourceCollection 的更改操作，比如向 ObservableCollection 添加元素等
                        Tools.logger.Warn($"故障解除-ID:{CurrentFaults[i].Id}-{CurrentFaults[i].Name}");
                        CurrentFaults.RemoveAt(i);
                        
                    });
                   
                    index++;
                }

            }
            OnPropertyChanged("CurrentFaults");
            System.Threading.Thread.Sleep(100);
            goto lb1;
        }
        
        #endregion

    }



}
