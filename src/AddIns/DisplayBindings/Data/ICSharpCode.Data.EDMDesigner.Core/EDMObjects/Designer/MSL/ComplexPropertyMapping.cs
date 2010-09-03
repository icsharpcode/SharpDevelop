// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class ComplexPropertyMapping
    {
        public ComplexPropertyMapping(ComplexProperty complexProperty, MappingBase mapping, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
        {
            ComplexProperty = complexProperty;
            Mapping = mapping;
            Table = table;
        }

        public ComplexProperty ComplexProperty { get; private set; }
        public MappingBase Mapping { get; private set; }
        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType Table { get; private set; }
        public bool TPC { get; set; }

        public IEnumerable<PropertyMapping> Mappings
        {
            get
            {
                foreach (var property in ComplexProperty.ComplexType.AllScalarProperties)
                    yield return new PropertyMapping(property, Mapping.GetSpecificMappingCreateIfNull(ComplexProperty), Table);
            }
        }

        public ComplexPropertiesMapping ComplexPropertiesMapping
        {
            get 
            {
                return new ComplexPropertiesMapping(ComplexProperty.ComplexType, Mapping.GetSpecificMappingCreateIfNull(ComplexProperty), Table) { TPC = TPC }; 
            }
        }
    }
}
