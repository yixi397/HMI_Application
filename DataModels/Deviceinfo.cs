using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HMI_ApplicationConfigClient.DataModels
{
    public class Deviceinfo: INotifyPropertyChanged
    {
        
        [Key]
        public long Id {  get; set; }
        public int Number {  get; set; }
        public string ?Name { get; set; }
        public string ?Describe { get; set; }

        private DeviceTypeEnum deviceType;
        public DeviceTypeEnum DeviceType
        {
            get { return deviceType; }
            set { deviceType = value; OnPropertyChanged("DeviceType");
            }
        }
        public enum DeviceTypeEnum
        {
            Axis, Cylinder, Door
        }
        public string? Property1 { get; set; }
        public string? Property2 { get; set; }
        public string? Property3 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

   
   

   
}
