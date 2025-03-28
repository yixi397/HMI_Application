using CommunityToolkit.Mvvm.ComponentModel;
using HMI_Application.Control;
using HMI_Application.Dialog_box;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static HMI_ApplicationConfigClient.DataModels.Controlinfo;

namespace HMI_Application.ViewModels
{
    partial class SetOtherVM:ObservableObject
    {
        private readonly IDatas data;
        private readonly HMIDbContext db;

        public SetOtherVM(IDatas data, HMIDbContext db)
        {
            this.data = data;
            this.db = db;
            loadconfig();
        }

        #region 设置功能

        public class ConfigInfo
        {
            public string? Name { get; set; }
            public string? ToolTip { get; set; }

            public  InputBoxDialog.DataTypeEnum DataTypeEnum { get; set; }
            public object Value { get; set; }
        }
        [ObservableProperty]
        private List<ConfigInfo> listConFigButton = new List<ConfigInfo>();

        [ObservableProperty]
        private List<ConfigInfo> listConFigInputBox = new List<ConfigInfo>();

        
        private void loadconfig()
        {
            var t = db.ControlinfoDbSet.ToList();
            var infos = (from d in t
                         where d.Region == RegionEnum.Config
                         select d).ToList();
            for (int i = 0; i < infos.Count; i++)
            {
                ConfigInfo configInfo = new ConfigInfo();
                configInfo.Name = infos[i].Name;
                configInfo.ToolTip = infos[i].ToolTip;
                int offset = Convert.ToInt16(infos[i].Address);
              
                switch (infos[i].ControlType)
                {
                    case ControlTypeEnum.IputIntBox:
                        configInfo.Value = data.Wint[offset];
                        configInfo.DataTypeEnum = InputBoxDialog.DataTypeEnum.Short;
                        ListConFigInputBox.Add(configInfo);
                        break;
                    case ControlTypeEnum.IputDintBox:
                        configInfo.Value = data.Wdint[offset];
                        configInfo.DataTypeEnum = InputBoxDialog.DataTypeEnum.Dint;
                        ListConFigInputBox.Add(configInfo);
                        break;
                    case ControlTypeEnum.IputRealBox:
                        configInfo.Value = data.Wreal[offset];
                        configInfo.DataTypeEnum = InputBoxDialog.DataTypeEnum.Real;
                        ListConFigInputBox.Add(configInfo);
                        break;
                    case ControlTypeEnum.Set:
                        configInfo.Value = data.Wbool[offset];
                        ListConFigButton.Add(configInfo);
                        break;
                    case ControlTypeEnum.Alt:
                        configInfo.Value = data.Wbool[offset];
                        ListConFigButton.Add(configInfo);
                        break;     
                }
            }
        }

        #endregion
    }
}
