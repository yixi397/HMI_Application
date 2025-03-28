using CommunityToolkit.Mvvm.ComponentModel;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_Application.Services.Data
{
    public class DataProcessingBase:IDatas
    {
        private readonly HMIDbContext db;

        public List<HMIVarInfoExtend<bool>> Wbool { get; set; } = new List<HMIVarInfoExtend<bool>>();
        public List<HMIVarInfoExtend<bool>> Rbool { get; set; } = new List<HMIVarInfoExtend<bool>>();
        public List<HMIVarInfoExtend<short>> Wint { get; set; } = new List<HMIVarInfoExtend<short>>();
        public List<HMIVarInfoExtend<short>> Rint { get; set; } = new List<HMIVarInfoExtend<short>>();
        public List<HMIVarInfoExtend<Int32>>  Wdint { get; set; } = new List<HMIVarInfoExtend<Int32>>();
        public List<HMIVarInfoExtend<Int32>> Rdint { get; set; } = new List<HMIVarInfoExtend<Int32>>();
        public List<HMIVarInfoExtend<float>> Wreal { get; set; } = new List<HMIVarInfoExtend<float>>();
        public List<HMIVarInfoExtend<float>> Rreal { get; set; } = new List<HMIVarInfoExtend<float>>();
        public List<IOVarinfoExtend >  Input { get; set; } = new List<IOVarinfoExtend>();
        public List<IOVarinfoExtend> Output { get; set; } = new List<IOVarinfoExtend>();

       
        public bool ISconnect { get; set; }
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public double GatherTime {  get; set; }
        
        Timer timer;
        public DataProcessingBase(HMIDbContext db)
        {
           
            this.db = db;
            loadData();
            
        }

        private void loadData()
        {
            List<HMIVarInfo> Listvars = db.HMIvarInfoDbSet.ToList();
            for (int i = 0; i < Listvars.Count; i++)
            {
                if (Listvars[i].Type == HMIVarInfo.TypeEnum.Bool && Listvars[i].Name[0] == 'W')
                {
                    Wbool.Add(new HMIVarInfoExtend<bool>(Listvars[i]) { Value=false});
                    
                    
                }
                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Bool && Listvars[i].Name[0] == 'R')
                {
                    Rbool.Add(new HMIVarInfoExtend<bool>(Listvars[i]) { Value=false});
                }
                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Int && Listvars[i].Name[0] == 'W')
                {
                    Wint.Add(new HMIVarInfoExtend<short>(Listvars[i]) { Value = 0 });

                }
                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Int && Listvars[i].Name[0] == 'R')
                {
                    Rint.Add(new HMIVarInfoExtend<short>(Listvars[i]) { Value = 0 });

                }

                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Dint && Listvars[i].Name[0] == 'W')
                {
                    Wdint.Add(new HMIVarInfoExtend<Int32>(Listvars[i]) { Value = 0 });

                }
                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Dint && Listvars[i].Name[0] == 'R')
                {
                    Rdint.Add(new HMIVarInfoExtend<Int32>(Listvars[i]) { Value = 0 });

                }

                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Real && Listvars[i].Name[0] == 'W')
                {
                    Wreal.Add(new HMIVarInfoExtend<float>(Listvars[i]) { Value = 0 });

                }
                else if (Listvars[i].Type == HMIVarInfo.TypeEnum.Real && Listvars[i].Name[0] == 'R')
                {
                    Rreal.Add(new HMIVarInfoExtend<float>(Listvars[i]) { Value = 0 });

                }
            }

            List<IOVarinfo> listio = db.IOVarInfoDbSet.ToList();
            int indexin = 0;
            int indexout=0;
            for (int i = 0; i < listio.Count; i++)
            {
                if (listio[i].InOut == IOVarinfo.InOutEnum.input)
                {
                    if(indexin >= App.InputNumber)
                    {
                        continue;
                    }
                    Input.Add(new IOVarinfoExtend( listio[i]));
                    indexin++;
                }
                else
                {
                    if (indexout >= App.OutputNumber)
                    {
                        continue;
                    }
                    Output.Add(new IOVarinfoExtend(listio[i]));
                    indexout++;
                }
            }

        }
        

    

        ~DataProcessingBase()
        {
            timer.Dispose();
        }
    }
}
