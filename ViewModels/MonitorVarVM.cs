using CommunityToolkit.Mvvm.ComponentModel;
using HMI_Application.Services.Data;
using HMI_Application.Services.DataProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_Application.ViewModels
{
    partial class MonitorVarVM: ObservableObject
    {
        [ObservableProperty]
        private IDatas datas;

        #region 显示数据
        [ObservableProperty]
        private Dictionary<string, object> showkeyValuePairs = new Dictionary<string, object>();

        [ObservableProperty]
        private object showCurrItemsSource;
        private string showCurrSelect;
        public  string showCurrSelectedSelect
        {
            get { return showCurrSelect; } 
            set 
            {
                showCurrSelect = value;
                ShowCurrItemsSource = ShowkeyValuePairs[showCurrSelect];
                OnPropertyChanged();
            }
        }
        #endregion



        public MonitorVarVM(IDatas data)
        {
            this.datas = data;
            showkeyValuePairs.Add("Wbool",datas.Wbool);
            showkeyValuePairs.Add("Rbool", datas.Rbool);
            showkeyValuePairs.Add("Wint", datas.Wint);
            showkeyValuePairs.Add("Rint", datas.Rint);
            showkeyValuePairs.Add("Wdint", datas.Wdint);
            showkeyValuePairs.Add("Rdint", datas.Rdint);
            showkeyValuePairs.Add("Wreal", datas.Wreal);
            showkeyValuePairs.Add("Rreal", datas.Rreal);

        }

       
       
        
    }
}
