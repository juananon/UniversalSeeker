using POC_UniversalSeeker.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Entities.Users
{
    public class User
    {
        public int Id { get; set; }
        [Filter("Name")]
        public string Name { get; set; }
        [Filter("SUrName")]
        public string SurName { get; set; }
        [Filter("Mail")]
        public string Mail { get; set; }
        public string Password { get; set; }
        [Filter("BirthDate")]
        public DateTime BirthDate { get; set; }
        [Filter("")]
        public Role Role { get; set; }
    }
}
