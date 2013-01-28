// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
