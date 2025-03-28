using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMI_ApplicationConfigClient.DataModels;

namespace HMI_Application.Services.DataProcessing
{
    public interface IDatas
    {
        public List<HMIVarInfoExtend<bool>> Wbool { get; set; } 
        public List<HMIVarInfoExtend<bool>> Rbool { get; set; } 
        public List<HMIVarInfoExtend<short>> Wint { get; set; } 
        public List<HMIVarInfoExtend<short>> Rint { get; set; } 
        public List<HMIVarInfoExtend<Int32>> Wdint { get; set; } 
        public List<HMIVarInfoExtend<Int32>> Rdint { get; set; } 
        public List<HMIVarInfoExtend<float>> Wreal { get; set; }
        public List<HMIVarInfoExtend<float>> Rreal { get; set; } 
        public List<IOVarinfoExtend> Input { get; set; } 
        public List<IOVarinfoExtend> Output { get; set; } 

        public bool ISconnect { get; set; }

        public double GatherTime { get; set; }
    }
}
