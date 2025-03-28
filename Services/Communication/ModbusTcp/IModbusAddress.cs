using HMI_ApplicationConfigClient.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_Application.Services.Communication
{
    public interface IModbusAddress
    {
        public Dictionary<string, string> AddressDic { get; }
        public string[] GetAddressName(HMIVarInfo varInfo, int number);

        public string[] GetAddressName(string Address,int bitNumber , int number);

        public ushort GetAddressNum(string AddressName);
        
    }
}
