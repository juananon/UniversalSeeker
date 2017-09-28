using POC_UniversalSeeker.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Entities.Users
{
    public class Role
    {
        public int Id { get; set; }
        [Filter("Name")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
