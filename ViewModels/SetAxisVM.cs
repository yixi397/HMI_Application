using CommunityToolkit.Mvvm.ComponentModel;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;

namespace HMI_Application.ViewModels
{
    partial class SetAxisVM : ObservableObject
    {
        private readonly ISetValue setValue;
        private readonly HMIDbContext db;
        [ObservableProperty]
        private IDatas datas;

        

        public SetAxisVM(IDatas data,ISetValue setValue, HMIDbContext db)
        {
            this.Datas = data;
            this.setValue = setValue;
            this.db = db;
            laadAxisinfo();
            data.Wint[0].PropertyChanged += SetAxisVM_PropertyChanged;
        }


        bool flag = false;
        private void SetAxisVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var v = sender as HMIVarInfoExtend<short>;
            flag = true;
            SelectIndex = v.Value;
            
        }

        #region 加载轴数据

        public string SpeedUnit
        {
            get { return AxisinfoList[SelectIndex].Property2; }
        }

       
        public string PosUnit
        {
            get { return AxisinfoList[SelectIndex].Property1; }
        }

        public string HomeWayDes
        {
            get { return AxisinfoList[SelectIndex].Describe; }
        }

        [ObservableProperty]
        private List<Deviceinfo> axisinfoList=new List<Deviceinfo>();

       

        
        public short SelectIndex
        {
            get { return Datas.Wint[0].Value; }
            set 
            {
                //if (value != SelectIndex)
                //{
                if(flag)
                {
                    flag = false;
                }
                else
                {
                    setValue.SetValue(Datas.Wint[0].Address, value);
                }
                    
                    OnPropertyChanged("SelectIndex");
                    OnPropertyChanged("SpeedUnit");
                    OnPropertyChanged("PosUnit");
                    OnPropertyChanged("HomeWayDes");
                //}
            }
        }

        private void laadAxisinfo()
        {
            var t = db.DeviceInfoDbSet.ToList();
             AxisinfoList = (from d in t
                         where d.DeviceType == Deviceinfo.DeviceTypeEnum.Axis
                         select d).ToList();    
        }
        #endregion
    }
}
