using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using HMI_Application.Control;
using HMI_Application.Services;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using HMI_ApplicationConfigClient.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static HMI_Application.Control.WordStateShow;
using static HMI_Application.ViewModels.MonitorFaultVM;
using static HMI_ApplicationConfigClient.DataModels.Controlinfo;

namespace HMI_Application.ViewModels
{
    partial class MainVM : ObservableObject
    {
        [ObservableProperty]
        private IDatas datas;
        private readonly HMIDbContext db;
        private readonly ISetValue setValue;

        public MainVM(IDatas data, HMIDbContext db, ISetValue setValue)
        {
            this.datas = data;
            this.db = db;
            this.setValue = setValue;
            laadAxisinfo();
            loadOtherDevice();
            LoadJogButton();
            LoadFunButton();
            loadStepDictionary();
            loadSpeedFun();
        }



        #region 轴相关功能
        public class AxisTemplate : Deviceinfo
        {
            public HMIVarInfoExtend<float> ActSpeed { get; set; }
            public HMIVarInfoExtend<float> ActPos { get; set; }
            public HMIVarInfoExtend<float> ActTorque { get; set; }
            public HMIVarInfoExtend<short> ActState { get; set; }
            public string JogPAddress { get; set; }
            public string JogNAddress { get; set; }

            private string tarGetPos;
            public string TarGetPos 
            {
                get { return tarGetPos; }
                set { tarGetPos = value; OnPropertyChanged(); }
            }

            public AxisTemplate(Deviceinfo deviceinfo)
            {
                this.Id = deviceinfo.Id;
                this.Name = deviceinfo.Name;
                this.Number = deviceinfo.Number;
                this.Describe = deviceinfo.Describe;
                this.DeviceType = deviceinfo.DeviceType;
                this.Property1 = deviceinfo.Property1;
                this.Property2 = deviceinfo.Property2;
                this.Property3 = deviceinfo.Property3;
            }


        }

        [ObservableProperty]
        private List<AxisTemplate> listAxisTemplate = new List<AxisTemplate>();


        private void laadAxisinfo()
        {
            var t = db.DeviceInfoDbSet.ToList();
            var infos = (from d in t
                         where d.DeviceType == Deviceinfo.DeviceTypeEnum.Axis
                         select d).ToList();

            for (int i = 0; i < infos.Count; i++)
            {
                ListAxisTemplate.Add(new AxisTemplate(infos[i])
                {
                    ActPos = Datas.Rreal[10 + i],
                    ActSpeed = Datas.Rreal[30 + i],
                    ActTorque = Datas.Rreal[50 + i],
                    ActState = Datas.Rint[20 + i],
                    JogPAddress = Datas.Wbool[40 + i * 2].Address,
                    JogNAddress = Datas.Wbool[40 + i * 2 + 1].Address,
                });

            }
            absPositioningCommand= new AsyncRelayCommand<AxisTemplate>(absPositioning);
            OnPropertyChanged(nameof(ListAxisTemplate));
        }

        public IAsyncRelayCommand absPositioningCommand { get;private set; }

        private async Task absPositioning(AxisTemplate axis)
        {
            if (Datas.Rbool[10].Value==false)
            {
                MessageBox.Show("未就绪");
                return;
            }
            float result = 0;
            if (float.TryParse(axis.TarGetPos, out result))
            {
                setValue.SetValue(Datas.Wint[0].Address, axis.Number);
                setValue.SetValue(Datas.Wreal[6].Address, result);
                await Task.Delay(100);
                setValue.SetValue(Datas.Wbool[4].Address, true);
            }
            else
            {
                axis.TarGetPos = "0";
                MessageBox.Show("值输入错误");
                return;
            }
        }
        #endregion

        #region 气缸和其他
        public class OtherDeviceTemplate: Deviceinfo
        {
            public string JogPAddress { get; set; }
            public string JogNAddress { get; set; }

            public string JogPtext { get; set; }
            public string JogNtext { get; set; }
            public HMIVarInfoExtend<short> ActState { get; set; }

            public OtherDeviceTemplate(Deviceinfo deviceinfo)
            {
                this.Id = deviceinfo.Id;
                this.Name = deviceinfo.Name;
                this.Number = deviceinfo.Number;
                this.Describe = deviceinfo.Describe;
                this.DeviceType = deviceinfo.DeviceType;
                this.Property1 = deviceinfo.Property1;
                this.Property2 = deviceinfo.Property2;
                this.Property3 = deviceinfo.Property3;
            }
           
        }

        [ObservableProperty]
        private List<OtherDeviceTemplate> lisOtherDeviceTemplate = new List<OtherDeviceTemplate>();

        private void loadOtherDevice()
        {

            var t = db.DeviceInfoDbSet.ToList();
            var infosCy = (from d in t
                         where d.DeviceType == Deviceinfo.DeviceTypeEnum.Cylinder
                         select d).ToList();


            int index = 0;
            for ( var i = 0; i < infosCy.Count; i++ )
            {
                LisOtherDeviceTemplate.Add(new OtherDeviceTemplate(infosCy[i])
                {
                    JogPAddress = Datas.Wbool[80 + index * 2].Address,
                    JogNAddress = Datas.Wbool[80 + index * 2 + 1].Address,
                    ActState = Datas.Rint[40 + index],
                    DeviceType = Deviceinfo.DeviceTypeEnum.Cylinder,
                    JogPtext ="伸出",
                    JogNtext = "缩回",
                });
                LisOtherDeviceTemplate[i].DeviceType = Deviceinfo.DeviceTypeEnum.Cylinder;
                //OnPropertyChanged(nameof(LisOtherDeviceTemplate[i]));
                index++;
            }

            var infosDo = (from d in t
                           where d.DeviceType == Deviceinfo.DeviceTypeEnum.Door
                           select d).ToList();
            
            for (var i = 0; i < infosDo.Count; i++)
            {
                LisOtherDeviceTemplate.Add(new OtherDeviceTemplate(infosDo[i])
                {
                    JogPAddress = Datas.Wbool[80 + index * 2].Address,
                    JogNAddress = Datas.Wbool[80 + index * 2 + 1].Address,
                    ActState = Datas.Rint[40 + index],
                    DeviceType = Deviceinfo.DeviceTypeEnum.Door,
                    JogPtext = "打开",
                    JogNtext = "关闭",
                });
                //OnPropertyChanged(nameof(LisOtherDeviceTemplate[i]));
                index++;
            }
           
        }

        #endregion

        #region 点动按钮
        [ObservableProperty]
        private List<BitButton> listJogButton=new List<BitButton>();

        private void LoadJogButton()
        {
            var t = db.ControlinfoDbSet.ToList();
            var infos = (from d in t
                         where d.Region == RegionEnum.Jogcontrol
                         select d).ToList();
            infos.Sort((p1, p2) => p1.Number.CompareTo(p2.Number));
            for (var i = 0;i < infos.Count;i++)
            {
                BitButton bitButton = new BitButton();
                bitButton.Content = infos[i].Name;
                bitButton.Address = infos[i].Address;
                bitButton.ToolTip = infos[i].ToolTip;
               
                int add = Convert.ToInt16(infos[i].Address);
                bitButton.Address = Datas.Wbool[add].Address;
                bitButton.Mode = BitButton.PressDownModeEnum.PressdownON;
                ListJogButton.Add(bitButton);
            }
        }

        #endregion

        #region 功能按钮
        [ObservableProperty]
        private List<BitButton> listFunButton = new List<BitButton>();

        private void LoadFunButton()
        {
            var t = db.ControlinfoDbSet.ToList();
            var infos = (from d in t
                         where d.Region == RegionEnum.Function 
                         && (d.ControlType==  ControlTypeEnum.Set 
                         || d.ControlType == ControlTypeEnum.Alt
                         || d.ControlType == ControlTypeEnum.PressdownON
                         || d.ControlType == ControlTypeEnum.PressdownOFF
                         || d.ControlType == ControlTypeEnum.Rest)
                         select d).ToList();
            infos.Sort((p1, p2) => p1.Number.CompareTo(p2.Number));
            for (var i = 0; i < infos.Count; i++)
            {
                BitButton bitButton = new BitButton();
                bitButton.Content = infos[i].Name;
                bitButton.Address = infos[i].Address;
                //bitButton.ToolTip = infos[i].ToolTip;
               
                int add =Convert.ToInt16( infos[i].Address);
                bitButton.Address = Datas.Wbool[add].Address;

                //后台代码绑定
                System.Windows.Data.Binding binding = new("MyText");
                binding.Source = this;
                binding.Path =new PropertyPath ("Datas.Rbool[10].Value");
                if(bitButton.Content.ToString()=="停止" || bitButton.Content.ToString() == "设备使能"
                    || bitButton.Content.ToString() == "复位")
                {
                    
                }
                else
                {
                    bitButton.SetBinding(BitButton.IsEnabledProperty, binding);
                }
                
               
                bitButton.Mode = BitButton.PressDownModeEnum.Set;
                bitButton.ConfirmInformation = infos[i].ToolTip;
                ListFunButton.Add(bitButton);
            }
        }
        #endregion

        #region 速度设置功能
        [ObservableProperty]
        List<SpeedInputBox> speedInputBoxList = new List<SpeedInputBox>();
        public class SpeedInputBox : ObservableObject
        {
            private readonly HMIVarInfoExtend<short> hMIVar;
            private readonly ISetValue setValue;

            public SpeedInputBox(HMIVarInfoExtend<short> hMIVar, ISetValue setValue)
            {
                this.hMIVar = hMIVar;
                this.setValue = setValue;
                hMIVar.PropertyChanged += HMIVar_PropertyChanged;
            }

            private void HMIVar_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                flag = true;
                var sen = sender as HMIVarInfoExtend<short>;
                Value = sen.Value;
                
            }

            bool flag = false;
            public string Name { get; set; }
            public short Value
            {
                get { return hMIVar.Value; }
                set 
                {
                    if(flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        setValue.SetValue(hMIVar.Address, value);
                    }
                    OnPropertyChanged();
                }
            }
        }
        
        private void loadSpeedFun()
        {
            var t = db.ControlinfoDbSet.ToList();
            var infos = (from d in t
                         where d.Region == RegionEnum.Function && d.Describe=="速度输入框"
                         select d).ToList();
            foreach (var info in infos)
            {
                int addoffset = Convert.ToInt32(info.Address);
                SpeedInputBox inputBox = new SpeedInputBox(Datas.Wint[addoffset], setValue);
                inputBox.Name = info.Name;
                SpeedInputBoxList.Add(inputBox);
            }
        }

        #endregion

        #region 流程步显示功能


        //模型

        public class StepDictionary
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public int Key { get; set; }
            public string Value { get; set; }
            
        }

        
        private List<List<StepDictionary>> stepGroups = new List<List<StepDictionary>>();

        public class StepShowInfo:ObservableObject
        {
            public StepShowInfo()
            {
                KeyValuePairs = new Dictionary<int, string>();
            }
            public int ID { get;set; }
            public string ProcessName { get;set; }

            private string stepName = "";
            public string StepName
            {
                get { return stepName; }
                set
                {
                    stepName = value;
                    OnPropertyChanged();
                }
            }

            public Dictionary <int,string> KeyValuePairs { get; set; }


        }
        [ObservableProperty]
        private List<StepShowInfo> stepShowInfos = new List<StepShowInfo>();

       
        //加载数据
        private void loadStepDictionary()
        {
            List<StepDictionary> stepList = new List<StepDictionary>();

            try
            {
                using (var reader = new StreamReader("config\\"+Settings1.Default.Recipe+"\\StepDictionary.csv", new UTF8Encoding(true)))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {

                    stepList = csv.GetRecords<StepDictionary>().ToList();
                }

                //分类数据
                var group = stepList.GroupBy(s => s.Name).ToList();
                foreach (var items in group)
                {
                    stepGroups.Add(items.ToList());
                    StepShowInfos.Add(new StepShowInfo
                    {
                        ProcessName = items.Key
                    });

                }

                for (int i = 0; i < stepGroups.Count; i++)
                {
                    string addstr = stepGroups[i][0].Address;
                    int addint = Convert.ToInt32(addstr);
                    StepShowInfos[i].ID = Datas.Rint[addint].Id;
                    for (int j = 0; j < stepGroups[i].Count; j++)
                    {
                        StepShowInfos[i].KeyValuePairs.Add(stepGroups[i][j].Key, stepGroups[i][j].Value);
                    }
                    Datas.Rint[addint].PropertyChanged += MainVM_PropertyChanged;
                    MainVM_PropertyChanged(Datas.Rint[addint], null);
                }
            }
            catch (Exception ex)
            {
                Tools.logger.Error ("加载StepDictionary.csv文件异常: " + ex.Message);
            }

          



        }

        private void MainVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
           var value=sender as HMIVarInfoExtend<short>;

            for (int i = 0; i < stepGroups.Count; i++)
            {
                if (StepShowInfos[i].ID==value.Id)
                {
                    if (StepShowInfos[i].KeyValuePairs.ContainsKey(value.Value))
                    {
                        StepShowInfos[i].StepName=StepShowInfos[i].KeyValuePairs[value.Value];
                        Tools.logger.Info($"{StepShowInfos[i].ProcessName}-{value.Value}-{StepShowInfos[i].StepName}-");
                        //OnPropertyChanged(StepShowInfos[i].StepName);
                    }
                }
            }

        }



        #endregion
    }
}
