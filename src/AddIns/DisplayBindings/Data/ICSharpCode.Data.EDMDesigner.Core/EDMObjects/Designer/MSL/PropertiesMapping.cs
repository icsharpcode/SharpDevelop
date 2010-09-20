// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public abstract class PropertiesMapping
    {
        public PropertiesMapping(EntityType entityType, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
        {
            EntityType = entityType;
            Table = table;
        }

        public EntityType EntityType { get; private set; }
        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType Table { get; private set; }

        public abstract IEnumerable<PropertyMapping> Mappings { get; }
    }
}
