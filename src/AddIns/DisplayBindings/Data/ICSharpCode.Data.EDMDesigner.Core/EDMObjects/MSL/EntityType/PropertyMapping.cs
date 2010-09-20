// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType
{
    public class PropertyMapping
    {
        public ScalarProperty Property { get; internal set; }
        public Property Column { get; internal set; }
    }
}
