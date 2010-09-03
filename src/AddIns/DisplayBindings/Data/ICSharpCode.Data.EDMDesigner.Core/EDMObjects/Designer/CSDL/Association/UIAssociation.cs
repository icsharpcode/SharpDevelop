// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Association
{
    public class UIAssociation
    {
        public UIRelatedProperty NavigationProperty1 { get; set; }
        public UIRelatedProperty NavigationProperty2 { get; set; }
    }
}
