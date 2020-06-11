using System;
using System.Collections.Generic;
using System.Text;

namespace Uber_Rider.ReferenceModels
{
  public  class RideType
    {
        public string Action { get; set; }
        public byte TypeId { get; set; }
        public string TypeNameEnglish { get; set; }
        public string TypeNameNepali { get; set; }
        public Nullable<bool> IsActive { get; set; }


    }
}
