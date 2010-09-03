// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function
{
    public class FunctionParameter : EDMObjectBase
    {
        public PropertyType Type { get; set; }
        public ParameterMode? Mode { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public int? MaxLength { get; set; }
    }
}
