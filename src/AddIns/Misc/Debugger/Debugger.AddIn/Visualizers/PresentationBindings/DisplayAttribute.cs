using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Debugger.AddIn.Visualizers
{
    public class DisplayAttribute : Attribute
    {
        public string DisplayValue { get; set; }

        public DisplayAttribute(string displayValue)
        {
            DisplayValue = displayValue;
        }
    }
}
