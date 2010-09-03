// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class AssociationPropertiesMapping : PropertiesMapping
    {
        public AssociationPropertiesMapping(NavigationProperty navigationProperty, EntityType table)
            : base(navigationProperty.EntityType, table)
        {
            NavigationProperty = navigationProperty;
        }

        public NavigationProperty NavigationProperty { get; private set; }

        public override IEnumerable<PropertyMapping> Mappings
        {
            get 
            {
                return EntityType.Keys.Select(k => new PropertyMapping(k, NavigationProperty.Mapping, Table));
            }
        }
    }
}
