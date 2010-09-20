// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class ComplexPropertiesMapping
    {
        public ComplexPropertiesMapping(TypeBase type, MappingBase mapping, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
        {
            Type = type;
            Mapping = mapping;
            Table = table;
            TPC = true;
        }

        public TypeBase Type { get; private set; }
        public MappingBase Mapping { get; private set; }
        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType Table { get; private set; }
        public bool TPC { get; set; }

        public IEnumerable<ComplexPropertyMapping> Mappings
        {
            get
            {
                foreach (var complexProperty in (TPC ? Type.AllComplexProperties : Type.ComplexProperties))
                    yield return new ComplexPropertyMapping(complexProperty, Mapping, Table) { TPC = TPC };
            }
        }
    }
}
