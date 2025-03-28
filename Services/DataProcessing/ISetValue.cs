using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMI_Application.Services.DataProcessing
{
    public interface  ISetValue
    {
        public void SetValue(string Address,bool Value);

        public void SetValue(string Address, short Value);

        public void SetValue(string Address, Int32 Value);

        public void SetValue(string Address, float Value);

        public void SetValue16(String Address,int BitNO,bool Value );

        public void SetValue32(String Address, int BitNO, bool Value);



    }
}
