using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Entities.Shared
{
    public class Filter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public Object Property { get; set; }
        public bool IsEntity { get; set; }
    }
}
