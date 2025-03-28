using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HMI_Application.Services.Communication;
using HMI_Application.Services.Data;
using HMI_ApplicationConfigClient.DataModels;
using NLog;
using Opc.Ua;
using Opc.Ua.Client;

namespace HMI_Application.Services.DataProcessing
{
    public class DataProcessingOpcUa : DataProcessingBase, ISetValue
    {
        MOpcUaHelper opcuaHelper=new MOpcUaHelper();
        private readonly Action<MOpcUaHelper> connect;
        public DataProcessingOpcUa(HMIDbContext db,Action<MOpcUaHelper> connect) : base(db)
        {
            
            this.connect = connect;
            logger.Debug("OpcUa初始化完成");
            Thread thread = new Thread(Refresh);
            thread.IsBackground = true;
            thread.Start();
        }
        #region 写入接口实现
        public void SetValue(string Address, bool Value)
        {
            if (opcuaHelper.ConnectStatus == false)
            {
                logger.Error($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:写入失败PLC未连接");
                return;
            }
            opcuaHelper.WriteSingleNodeIdOfSync(Address, Value);
            
            logger.Debug($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:");
        }

        public void SetValue(string Address, short Value)
        {
            if (opcuaHelper.ConnectStatus == false)
            {
                logger.Error($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:写入失败PLC未连接");
                return;
            }
            opcuaHelper.WriteSingleNodeIdOfSync(Address, Value);

            logger.Debug($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:");
        }

        public void SetValue(string Address, int Value)
        {
            if (opcuaHelper.ConnectStatus == false)
            {
                logger.Error($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:写入失败PLC未连接");
                return;
            }
            opcuaHelper.WriteSingleNodeIdOfSync(Address, Value);
            logger.Debug($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:");
        }

        public void SetValue(string Address, float Value)
        {
            if (opcuaHelper.ConnectStatus == false)
            {
                logger.Error($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}写入失败PLC未连接");
                return;
            }
            opcuaHelper.WriteSingleNodeIdOfSync(Address, Value);
            logger.Debug($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:");
        }

        public void SetValue16(string Address, int BitNO, bool Value)
        {
            if (opcuaHelper.ConnectStatus == false)
            {
                logger.Error($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}写入失败PLC未连接");
                return;
            }
            opcuaHelper.WriteSingleNodeIdOfSync(Address, Value);
            logger.Debug($"WriteSingleNodeId:{Address}-{BitNO}-{Value}");
        }

        public void SetValue32(string Address, int BitNO, bool Value)
        {
            if (opcuaHelper.ConnectStatus == false)
            {
                logger.Error($"WriteSingleNodeId:{Address}-{Value}-{Value.GetType()}:写入失败PLC未连接");
                return;
            }
            opcuaHelper.WriteSingleNodeIdOfSync(Address, Value);
            logger.Debug($"WriteSingleNodeId:{Address}-{BitNO}-{Value}");
        }
        #endregion

        


       
        //获取连接状态
        protected  void Refresh()
        {
            mlb0:
            DateTime startTime = DateTime.Now;
            //判断是否连接成功，未连接自动从新连接
            if (opcuaHelper.ConnectStatus)
            {
                if(iscallBack==false)
                {
                    readdata();
                }
                ISconnect = true;
            }
            else
            {
                ISconnect = false;
                System.Threading.Thread.Sleep(5000);
                connect(opcuaHelper);
                if(opcuaHelper.ConnectStatus==false)
                { logger.Error("连接PLC失败，尝试重新连接中......"); }
                
            }
            // 获取程序结束时间
            DateTime endTime = DateTime.Now;
            // 计算时间差，得到程序运行时间
            TimeSpan runTime = endTime - startTime;
            GatherTime = runTime.TotalMilliseconds;
            System.Threading.Thread.Sleep(100);
            goto mlb0;
        }

        bool iscallBack = false;

        Dictionary<string, object> keys = new Dictionary<string, object>();
        private void addData<T>(string keyName,List< HMIVarInfoExtend<T>> value)
        {
            string[] nodes = new string[value.Count];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = value[i].Address;
                keys.Add(value[i].Address, value[i]);
            }
            opcuaHelper.BatchNodeIdDatasSubscription(keyName, nodes, callback);
        }

        private void addData<T>(string keyName, List<IOVarinfoExtend> value)
        {
            string[] nodes = new string[value.Count];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = value[i].IOAddress;
                keys.Add(value[i].IOAddress, value[i]);
            }
            opcuaHelper.BatchNodeIdDatasSubscription(keyName, nodes, callback);
        }
        private void readdata()
        {
            logger.Debug("开始订阅数据");
            iscallBack = true;
            addData(nameof(Wbool), Wbool);
            addData(nameof(Rbool), Rbool);
            addData(nameof(Wint), Wint);
            addData(nameof(Rint), Rint);
            addData(nameof(Wdint), Wdint);
            addData(nameof(Rdint), Rdint);
            addData(nameof(Wreal), Wreal);
            addData(nameof(Rreal), Rreal);
            addData<IOVarinfoExtend>(nameof(Input), Input);
            addData<IOVarinfoExtend>(nameof(Output), Output);


            logger.Debug("订阅数据完成"); 
        }
        

        private void callback(string key, MonitoredItem item, MonitoredItemNotificationEventArgs args)
        {
            MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
            //logger.Debug($"{key}-{item.DisplayName}-{notification.Value.ToString()}");
            var value = notification.Value.Value;
            if (key== nameof(Wbool)|| key == nameof(Rbool))
            {
                var ob = keys[item.DisplayName] as HMIVarInfoExtend<bool>;
                ob.Value = (bool)value;
            }
            else if (key == nameof(Wint) || key == nameof(Rint))
            {
                var ob = keys[item.DisplayName] as HMIVarInfoExtend<short>;
                ob.Value = (short)value;
            }
            else if (key == nameof(Wdint) || key == nameof(Rdint))
            {
                var ob = keys[item.DisplayName] as HMIVarInfoExtend<Int32>;
                ob.Value = (Int32)value;
            }
            else if (key == nameof(Wreal) || key == nameof(Rreal))
            {
                var ob = keys[item.DisplayName] as HMIVarInfoExtend<float>;
                ob.Value = (float)value;
            }
            else if (key == nameof(Input) || key == nameof(Output))
            {
                var ob = keys[item.DisplayName] as IOVarinfoExtend;
                ob.Value = (bool)value;
            }





        }
    }
}
