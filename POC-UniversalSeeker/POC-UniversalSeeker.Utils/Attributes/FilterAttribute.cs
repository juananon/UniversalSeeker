using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC_UniversalSeeker.Utils.Attributes
{
    /// <summary>
    /// Atributo para hacer notaciones en las propiedades que pretenden ser filtradas.
    /// </summary>
    public class FilterAttribute : Attribute
    {
        protected String propertydescription;

        public FilterAttribute(String PropertyDescription)
        {
            this.propertydescription = PropertyDescription;
        }

        public String Description
        {
            get
            {
                return this.propertydescription;
            }
        }
    }
}
