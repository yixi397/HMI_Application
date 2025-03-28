using HMI_ApplicationConfigClient.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_Application.Services.Communication.ModbusTcp
{
    public class InovanceH5UModbusAddress:IModbusAddress
    {
        public InovanceH5UModbusAddress()
        {
            AddressDic = new Dictionary<string, ushort>
            {
                { "M", 0X0000 },
                { "B", 0X3000 },
                { "S", 0XE000 },
                { "X", 0XF800 },
                { "Y", 0XFC00 },
                { "D", 0X0000 },
                { "R", 0X3000 }
            };

        }

        public Dictionary<string, ushort> AddressDic
        {
            get;
        }

        Dictionary<string, string> IModbusAddress.AddressDic => throw new NotImplementedException();

        public string[] GetAddressName(HMIVarInfo varInfo, int number = 1)
        {
            string address = varInfo.Address;
            string addType = address.Substring(0, 1);
            int AddressValue = 0;
            bool success = int.TryParse(address.Remove(0, 1), out AddressValue);

            if (success == false)
            {
                throw new ArgumentOutOfRangeException("地址格式错误:" + varInfo.Address);
            }
            if (AddressDic.ContainsKey(addType) == false)
            {
                throw new ArgumentOutOfRangeException("未查询到该地址:" + varInfo.Address);
            }
            string[] addrstrary = new string[number];
            for (int i = 0; i < number; i++)
            {
                if (addType == "X" || addType == "Y")
                {
                    string v = Convert.ToString(AddressValue + i, 8);
                    addrstrary[i] = addType + v;
                }
                else if (varInfo.Type == HMIVarInfo.TypeEnum.Dint || varInfo.Type == HMIVarInfo.TypeEnum.Real)
                {
                    addrstrary[i] = addType + (AddressValue + i * 2).ToString();
                }
                else
                {
                    addrstrary[i] = addType + (AddressValue + i).ToString();
                }
            }
            return addrstrary;
        }

        public string[] GetAddressName(string Address, int bitNumber, int number)
        {
            string address = Address;
            string addType = address.Substring(0, 1);
            int AddressValue = 0;
            bool success = int.TryParse(address.Remove(0, 1), out AddressValue);

            if (success == false)
            {
                throw new ArgumentOutOfRangeException("地址格式错误:" + Address);
            }
            if (AddressDic.ContainsKey(addType) == false)
            {
                throw new ArgumentOutOfRangeException("未查询到该地址:" + Address);
            }
            string[] addrstrary = new string[number];
            for (int i = 0; i < number; i++)
            {
                if (addType == "X" || addType == "Y")
                {
                    string v = Convert.ToString(AddressValue + i, 8);
                    addrstrary[i] = addType + v;
                }
                else if (bitNumber == 32)
                {
                    addrstrary[i] = addType + (AddressValue + i * 2).ToString();
                }
                else
                {
                    addrstrary[i] = addType + (AddressValue + i).ToString();
                }
            }
            return addrstrary;
        }

        public ushort GetAddressNum(string AddressName)
        {
            string addType = "";
            string addValue = "";
            foreach (var item in AddressName)
            {
                if (char.IsLetter(item))
                {
                    addType += item;
                }
                else
                {
                    addValue += item;
                }
            }

            if (AddressDic.ContainsKey(addType) == false)
            {
                throw new ArgumentOutOfRangeException("未查询到该地址:" + AddressName);
            }
            int v = AddressDic[addType] + Convert.ToUInt16(addValue);
            if (addType=="X" || addType=="Y")
            {
                
                v = AddressDic[addType] + Convert.ToUInt16( addValue,8);
            }
            
            return (ushort)v;

           
        }
    }
}
