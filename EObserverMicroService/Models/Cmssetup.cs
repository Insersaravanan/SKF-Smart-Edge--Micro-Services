using System;
using System.Collections.Generic;

namespace EMaintanance.Models
{
    public partial class Cmssetup
    {
        public int Cmsid { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string Descriptions { get; set; }
        public string TypeText { get; set; }
        public int? TypeOrder { get; set; }
        public string Active { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? Createdon { get; set; }
    }
}
