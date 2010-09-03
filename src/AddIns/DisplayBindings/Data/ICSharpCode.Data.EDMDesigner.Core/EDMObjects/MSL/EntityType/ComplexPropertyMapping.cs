// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType
{
    public class ComplexPropertyMapping : MappingBase
    {
        public ComplexPropertyMapping(CSDL.Type.EntityType entityType, ComplexProperty complexProperty)
            : base(entityType)
        {
            ComplexProperty = complexProperty;
        }

        public ComplexProperty ComplexProperty { get; private set; }

        protected override MappingBase BaseMapping
        {
            get
            {
                if (EntityType.ComplexProperties.Contains(ComplexProperty))
                    return null;
                return EntityType.BaseType.Mapping;
            }
        }
    }
}
