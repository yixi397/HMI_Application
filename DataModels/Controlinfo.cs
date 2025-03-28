using System.ComponentModel.DataAnnotations;

namespace HMI_ApplicationConfigClient.DataModels
{
    public class Controlinfo
    {
        
        [Key]
        public long Id { get; set; }
        public int Number { get; set; }
        public string ?Name { get; set; }
        public string ?ToolTip { get; set; }
        public string Address { get; set; }

        public string? Describe { get; set; }
        public ControlTypeEnum ControlType { get; set; }
        public RegionEnum Region { get; set; }
        public enum ControlTypeEnum
        {
            Set, Rest, Alt, PressdownON, PressdownOFF,IputIntBox, IputDintBox, IputRealBox
        }

        public enum RegionEnum
        {
            Jogcontrol, Config, Function
        }

    }
}
