using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMI_Application.Services;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace HMI_Application.ViewModels
{
    partial class MonitorIOVM : ObservableObject
    {
        private readonly ISetValue setValue;
        private readonly HMIDbContext db;
        [ObservableProperty]
        private IDatas datas;

        public MonitorIOVM(IDatas data,ISetValue setValue, HMIDbContext db)
        {
            this.Datas = data;
            this.setValue = setValue;
            this.db = db;
        }

        

        [RelayCommand]
        private void altValue(IOVarinfoExtend iOVarinfo)
        {
            if(iOVarinfo.DataType==IOVarinfoExtend.DataTypeEnum.Bool)
            {
                setValue.SetValue(iOVarinfo.IOAddress, !iOVarinfo.Value);
            }
            else if(iOVarinfo.DataType == IOVarinfoExtend.DataTypeEnum.Int)
            {
                setValue.SetValue16(iOVarinfo.IOAddress,iOVarinfo.BitNo,!iOVarinfo.Value);
            }
            else if (iOVarinfo.DataType == IOVarinfoExtend.DataTypeEnum.Dint)
            {
                setValue.SetValue32(iOVarinfo.IOAddress, iOVarinfo.BitNo, !iOVarinfo.Value);
            }

        }

        [RelayCommand]
        private void save()
        {
          
            List<IOVarinfo> ios = db.IOVarInfoDbSet.ToList();
            Dictionary<long, IOVarinfo> keys1 = new Dictionary<long, IOVarinfo>();
            foreach (IOVarinfo ite in ios)
            {
                keys1.Add(ite.Id, ite );
            }
            //int i = 0;
            foreach (var ite in Datas.Input)
            {

                keys1[ite.Id].Note=ite.Note;
            }

            foreach (var ite in Datas.Output)
            {

                keys1[ite.Id].Note = ite.Note;
            }

            db.SaveChanges();
            Tools.logger.Info("保存成功");
        }
    }
}
