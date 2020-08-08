using System;
using System.Collections.Generic;

namespace EMaintanance.Models
{
    public partial class Users
    {
        public Users()
        {
            Languages = new HashSet<Languages>();
        }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public int? UserType { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public ICollection<Cmssetup> Cmssetup { get; set; }
        public ICollection<Languages> Languages { get; set; }
    }
}
