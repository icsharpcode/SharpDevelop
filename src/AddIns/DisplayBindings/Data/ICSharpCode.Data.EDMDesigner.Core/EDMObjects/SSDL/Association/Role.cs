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
        public Property.Property Property { get; set; }
    }
}
