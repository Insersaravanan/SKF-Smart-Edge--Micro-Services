using System;
using System.Collections.Generic;

namespace EMaintanance.Models
{
    public partial class Languages
    {
        public Languages()
        {
        }

        public int LanguageId { get; set; }
        public string Active { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string CountryCode { get; set; }
        public string Lname { get; set; }
    }
}
