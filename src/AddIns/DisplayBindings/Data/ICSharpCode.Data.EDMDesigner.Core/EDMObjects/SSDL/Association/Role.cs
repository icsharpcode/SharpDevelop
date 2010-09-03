// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Interfaces;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Association
{
    public class Role : EDMObjectBase
    {
        public EntityType.EntityType Type { get; set; }
        public Cardinality Cardinality { get; set; }
        public EventedObservableCollection<Property.Property> Properties { get; set; }

        public Role()
        {
            Properties = new EventedObservableCollection<Property.Property>();
        }
    }
}
