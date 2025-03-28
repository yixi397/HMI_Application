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
    public class HMIVarInfo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public enum TypeEnum
        {
            Bool, Int, Dint, Real
        }
        public TypeEnum Type { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
    }
    public class HMIVarInfoExtend<T>: HMIVarInfo, INotifyPropertyChanged
    {
        public HMIVarInfo HMIVar { get; set; }
        private T _value;
        public T Value
        {
            get { return _value; }
            set
            {
                if (!EqualityComparer<T>.Default.Equals(value, _value))
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }

        }
        public HMIVarInfoExtend(HMIVarInfo hMIVar)
        {
            HMIVar = hMIVar;
            this.Id = hMIVar.Id;
            this.Address = hMIVar.Address;
            this.Note = hMIVar.Note;
            this.Type = hMIVar.Type;
            this.Name = hMIVar.Name;
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
