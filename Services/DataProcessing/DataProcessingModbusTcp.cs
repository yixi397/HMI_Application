using HMI_Application.Services.Communication;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;
using NLog;
using NModbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HMI_Application.Services.Data
{
    public class DataProcessingModbusTcp : DataProcessingBase,ISetValue
    {
        private readonly ModbusTcpHelper mbMaster;
        private readonly IModbusAddress address;
        public DataProcessingModbusTcp(HMIDbContext db,ModbusTcpHelper tcpMaster,IModbusAddress address) : base(db)
        {
            
           
            this.mbMaster = tcpMaster;
            this.address = address;
            Thread thread = new Thread(Refresh);
            thread.IsBackground = true;
            thread.Start();
            logger.Debug("构造");
            listbool = new List<HMIVarInfoExtend<bool>>();
            listbool.AddRange(Wbool);
            listbool.AddRange(Rbool);
        }
        #region 写入接口实现
        public void SetValue(string Address, bool Value)
        {
            if(this.mbMaster.IsConnected==false)
            {
                logger.Error("WriteSingleCoil-" + Address + "-" + Value+"-"+"写入失败，PLC未连接");
                return;
            }
            ushort mbaddress=this.address.GetAddressNum(Address);
            logger.Debug("WriteSingleCoil-"+ Address+"-"+ Value);
            mbMaster.WriteSingleCoil(mbaddress, Value, 1); 
        }

        public void SetValue(string Address, short Value)
        {
            if (this.mbMaster.IsConnected == false)
            {
                logger.Error("WriteSingleShort-" + Address + "-" + Value + "-" + "写入失败，PLC未连接");
                return;
            }
            ushort mbaddress = this.address.GetAddressNum(Address);
            logger.Debug("WriteSingleShort-" + Address + "-" + Value);
            mbMaster.WriteSingleShort(mbaddress, Value, 1);
        }

        public void SetValue(string Address, Int32 Value)
        {
            if (this.mbMaster.IsConnected == false)
            {
                logger.Error("WriteInt32-" + Address + "-" + Value + "-" + "写入失败，PLC未连接");
                return;
            }
            ushort mbaddress = this.address.GetAddressNum(Address);
            logger.Debug("WriteInt32-" + Address + "-" + Value);
            mbMaster.WriteInt32(mbaddress, Value, 1);
        }

        public void SetValue(string Address, float Value)
        {
            if (this.mbMaster.IsConnected == false)
            {
                logger.Error("WriteFloat-" + Address + "-" + Value + "-" + "写入失败，PLC未连接");
                return;
            }
            ushort mbaddress = this.address.GetAddressNum(Address);
            logger.Debug("WriteFloat-" + Address + "-" + Value);
            mbMaster.WriteFloat(mbaddress, Value, 1);
        }

        public void SetValue16(String Address, int BitNO, bool Value)
        {
            if (BitNO >15)
            {
                throw new ArgumentException("位编号错误", "bitNO");
            }
            if (this.mbMaster.IsConnected == false)
            {
                logger.Error("SetValue16bit-" + Address + "."+BitNO+"-" + Value + "-" + "写入失败，PLC未连接");
                return;
            }

            short currValue = mbMaster.ReadSingleShort(address.GetAddressNum(Address));
            short setValue = (short)setValueBit(currValue, BitNO, Value);
            logger.Debug("SetValue16bit-" + Address + "."+BitNO+"-" + Value);
            mbMaster.WriteSingleShort(address.GetAddressNum(Address),setValue);
        }

        public void SetValue32(String Address, int BitNO, bool Value)
        {
            if (BitNO > 31)
            {
                throw new ArgumentException("位编号错误", "bitNO");
            }
            if (this.mbMaster.IsConnected == false)
            {
                logger.Error("SetValue32bit" + Address + "."+BitNO+"-" + Value + "-" + "写入失败，PLC未连接");
                return;
            }
            Int32 currValue = mbMaster.ReadInt32(address.GetAddressNum(Address));
            Int32 setValue = setValueBit(currValue, BitNO, Value);
            logger.Debug("SetValue32bit-" + Address + "." + BitNO + "-" + Value);
            mbMaster.WriteInt32(address.GetAddressNum(Address), setValue);
        }

        private int setValueBit(int number, int bitNO, bool valueToSet)
        {
            if (bitNO < 0)
            {
                throw new ArgumentException("位编号不能为负数", "bitNO");
            }
            // 创建一个掩码，用于定位到指定的二进制位
            int mask = 1 << bitNO;

            if (valueToSet)
            {
                // 如果要设置的值为 true，使用按位或操作将指定位置为 1
                number |= mask;
            }
            else
            {
                // 如果要设置的值为 false，使用按位与操作结合取反后的掩码将指定位置为 0
                number &= ~mask;
            }
            return number;
        }

        #endregion



        #region 定期刷新数据
     

        private List< HMIVarInfoExtend<bool>> listbool;
       

        
        protected  void Refresh()
        {
        mylabel:
            try
            {
               
                // 获取程序开始时间
                DateTime startTime = DateTime.Now;
                //判断是否连接成功，未连接自动从新连接
                while (mbMaster.IsConnected==false)
                {
                    ISconnect = false;
                    mbMaster.Connect();
                    System.Threading.Thread.Sleep(100);
                    if (mbMaster.IsConnected == true)
                    {
                       
                        break;
                    }
                    logger.Error("连接plc失败，尝试重新连接中........");
                    System.Threading.Thread.Sleep(3000);
                }
                ISconnect = true;
                refreshData();
                System.Threading.Thread.Sleep(80);
                // 获取程序结束时间
                DateTime endTime = DateTime.Now;
                // 计算时间差，得到程序运行时间
                TimeSpan runTime = endTime - startTime;
                GatherTime = runTime.TotalMilliseconds;
               
            }
            catch (Exception ex)
            {
                ISconnect = false;
                logger.Error(ex.Message);
                System.Threading.Thread.Sleep(6000);
            }
         goto mylabel;

        }

        void refreshData()
        {
            readData(listbool, mbMaster.ReadCoils, 1000);
            //System.Threading.Thread.Sleep(10);
            readData(Wint, mbMaster.ReadShorts, 120);
            //System.Threading.Thread.Sleep(10);
            readData(Rint, mbMaster.ReadShorts, 120);
            //System.Threading.Thread.Sleep(10);
            readData(Wdint, mbMaster.ReadInt32s, 50);
            //System.Threading.Thread.Sleep(10);
            readData(Rdint, mbMaster.ReadInt32s, 50);
            //System.Threading.Thread.Sleep(10);
            readData(Wreal, mbMaster.ReadFloats, 50);
            //System.Threading.Thread.Sleep(10);
            readData(Rreal, mbMaster.ReadFloats, 50);
           
            readIOs(Input);
            readIOs(Output);
        }

        #region 私有方法
        delegate T[] readDataDelegate<T>(ushort address, ushort number, byte slaveID = 1);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="get"></param>
        /// <param name="readLen">读取长度</param>
        /// <param name="offset">寄存器偏移值</param>
        private void readData<T>(List<HMIVarInfoExtend<T>> datas, readDataDelegate<T> get, ushort readLen=100)
        {
            
            int index = 0;
            T[] ValueArr;
            for (int i = 0; i + index < datas.Count; i++)
            {
                ushort addressno = address.GetAddressNum(datas[index].Address);
                ValueArr = get(addressno, readLen);
                string[] addressArr=address.GetAddressName(datas[index], readLen);
                for (int j = 0; j < ValueArr.Length; j++)
                {

                    if (datas[index].Address== addressArr[j])
                    {
                        datas[index].Value = ValueArr[j];
                        index++;
                    }
                    if (index == datas.Count - 1)
                    {
                        return;
                    }
                }
            }
        }


        private void readData(List<IOVarinfoExtend> datas, ushort readLen = 100)
        {

            int index = 0;
            bool[] ValueArr;
            for (int i = 0; i + index < datas.Count; i++)
            {
                ushort addressno = address.GetAddressNum(datas[index].IOAddress);
                ValueArr = mbMaster.ReadCoils(addressno, readLen);
                HMIVarInfo hMIVarInfo = new HMIVarInfo() { Address = datas[index].IOAddress, Type = HMIVarInfo.TypeEnum.Bool };
                string[] addressArr = address.GetAddressName(hMIVarInfo, readLen);
                for (int j = 0; j < ValueArr.Length; j++)
                {

                    if (datas[index].IOAddress == addressArr[j])
                    {
                        datas[index].Value = ValueArr[j];
                        index++;
                    }
                    if (index == datas.Count - 1)
                    {
                        return;
                    }
                }
            }
        }

        private static Dictionary<Type, int> typeReadNumberMap = new Dictionary<Type, int>()
        {
            {typeof(bool), 1000},
            {typeof(short), 120},
            {typeof(Int32), 60},
            {typeof(float), 60}
        };
        private void readDatas<T>(List<HMIVarInfoExtend<T>> hMIVar, Func<ushort, ushort, byte , T[]> func)
        {
            if (hMIVar.Count==0)
            {
                return;
            }
            List<HMIVarInfoExtend<T>> list = new List<HMIVarInfoExtend<T>>();
            list.AddRange(hMIVar);

            int index = 0;
            //判断数据类型获取1次最大读取数量
            int readNumber=0;
            if (typeReadNumberMap.TryGetValue(typeof(T), out readNumber))
            {
                // 执行后续逻辑
            }
            else
            {
                throw new Exception("数据类型错误");
            }
            Dictionary<string, T> keys = new Dictionary<string, T>();
            while (true)
            {
                ushort addressno = address.GetAddressNum(list[index].Address);
                string[] strings = address.GetAddressName(list[index], readNumber);
                T[] values = func(addressno,(ushort)readNumber,1);
               
                for (int i = 0; i < readNumber; i++)
                {
                    keys.Add(strings[i], values[i]);
                }

                //筛选出匹配的元素
                var matchedDataList = from d in list
                                      where keys.ContainsKey(d.Address)
                                      select d;


                //查找匹配元素并赋值
                foreach (var matchedData in matchedDataList)
                {
                    matchedData.Value = keys[matchedData.Address];
                    index++;
                }
                //遍历完成
                if (index >= list.Count)
                {
                    return;
                }
                keys.Clear();


            }
        }

        private void readIOs(List<IOVarinfoExtend> iOs)
        {
            //筛选数据 读取Bool数据
            List<IOVarinfoExtend> matchedDataListbool = (from d in iOs
                                  where d.DataType == IOVarinfoExtend.DataTypeEnum.Bool
                                  select d).ToList();
            int index = 0;
            
            while (true)
            {

                //获取地址集合
                string[] strs = address.GetAddressName(new HMIVarInfo() 
                { Address = matchedDataListbool[index].IOAddress ,Type=HMIVarInfo.TypeEnum.Bool}
                ,200);
                //获取值集合
                ushort addressNo = address.GetAddressNum(matchedDataListbool[index].IOAddress);
                bool[] bools = mbMaster.ReadCoils(addressNo, 200);
                //将集合添加到字典
                Dictionary<string, bool> keys = new Dictionary<string, bool>();
                for(int i = 0; i < bools.Length; i++)
                {
                    keys.Add(strs[i], bools[i]);
                }
                //查找匹配元素赋值
                foreach (var item in matchedDataListbool)
                {
                    item.Value = keys[item.IOAddress];
                    index++;
                }
                if (index >= matchedDataListbool.Count)
                {
                    break;
                }
            }

            //筛选数据读取int值
            matchedDataListbool = (from d in iOs
                                   where d.DataType == IOVarinfoExtend.DataTypeEnum.Int
                                   select d).ToList();
            if (matchedDataListbool.Count > 0)
            {
                
                short[] values= mbMaster.ReadShorts(address.GetAddressNum(matchedDataListbool[0].IOAddress), 10);
                string[] strings = address.GetAddressName(matchedDataListbool[0].IOAddress,1,10);
                Dictionary<string, short> keys = new Dictionary<string, short>();
               
                for (int i = 0;i < 10;i++)
                {
                    keys.Add(strings[i], values[i]);
                }
                
                for (int i = 0;i<matchedDataListbool.Count ; i++)
                {
                    if (keys.ContainsKey(matchedDataListbool[i].IOAddress))
                    {
                        matchedDataListbool[i].Value = getBitValue(keys[matchedDataListbool[i].IOAddress], matchedDataListbool[i].BitNo);
                    }
                }
               

            }

            //筛选数据读取Dint值
            matchedDataListbool = (from d in iOs
                                   where d.DataType == IOVarinfoExtend.DataTypeEnum.Dint
                                   select d).ToList();
            if (matchedDataListbool.Count > 0)
            {

                Int32[] values = mbMaster.ReadInt32s(address.GetAddressNum(matchedDataListbool[0].IOAddress), 10);
                string[] strings = address.GetAddressName(matchedDataListbool[0].IOAddress, 1, 10);
                Dictionary<string, Int32> keys = new Dictionary<string, Int32>();

                for (int i = 0; i < 10; i++)
                {
                    keys.Add(strings[i], values[i]);
                }

                for (int i = 0; i < matchedDataListbool.Count; i++)
                {
                    if (keys.ContainsKey(matchedDataListbool[i].IOAddress))
                    {
                        matchedDataListbool[i].Value = getBitValue(keys[matchedDataListbool[i].IOAddress], matchedDataListbool[i].BitNo);
                    }
                }
            }


        }

        private bool getBitValue(int numberToParse, int bitPosition)
        {
            // 首先进行边界检查，确保位位置是合理的非负整数
            if (bitPosition < 0)
            {
                throw new ArgumentException("位位置不能为负数", bitPosition.ToString());
            }

            // 构造一个只有目标位为1，其余位为0的掩码
            int bitMask = 1 << bitPosition;
            // 进行按位与操作，获取指定位的值（结果中只有目标位的值保留下来）
            int result = numberToParse & bitMask;
            // 通过右移操作将目标位的值移到最低位，然后判断是否为0来确定该位的值
            return (result >> bitPosition) != 0;
        }

        #endregion
        #endregion




    }
}
