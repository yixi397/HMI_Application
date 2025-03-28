using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HMI_ApplicationConfigClient.DataModels
{
    public class IOVarinfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        public string ShowAddress { get; set; }
        public string IOAddress {get; set;}

        public int BitNo { get; set; }
        public string Note { get; set;}

        public DataTypeEnum DataType { get; set; }

        public InOutEnum InOut { get; set; }

        public enum InOutEnum
        {
            input,output
        }

        public enum DataTypeEnum
        {
            Bool, Int, Dint
        }
    }
    public class IOVarinfoExtend:IOVarinfo, INotifyPropertyChanged
    {
        private bool _value;
        public bool Value
        {
            get { return _value; }
            set
            {
                if (value!=_value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }

        }
        public  IOVarinfo IOinfo { get; set; }
        public IOVarinfoExtend(IOVarinfo iO)
        {
            this.IOinfo = iO;
            this.Id = iO.Id;
            this.ShowAddress = iO.ShowAddress;
            this.IOAddress = iO.IOAddress;
            this.Note = iO.Note;
            this.BitNo = iO.BitNo;
            this.InOut = iO.InOut;
            this.DataType = iO.DataType;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
