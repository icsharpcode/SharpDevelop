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

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Common
{
    public interface IMapping
    {
        IEnumerable<PropertyMapping> GetSpecificMappingForTable(SSDL.EntityType.EntityType table);
        void AddMapping(ScalarProperty property, SSDL.Property.Property column);
        void ChangeMapping(ScalarProperty property, SSDL.Property.Property column);
        void RemoveMapping(ScalarProperty property, SSDL.EntityType.EntityType table);
    }
}
