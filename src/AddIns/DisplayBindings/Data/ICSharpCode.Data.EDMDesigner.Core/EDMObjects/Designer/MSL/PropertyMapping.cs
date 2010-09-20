// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class PropertyMapping
    {
        public PropertyMapping(ScalarProperty property, IMapping mapping, EntityType table)
        {
            if (property == null || mapping == null)
                throw new ArgumentNullException();
            Property = property;
            Mapping = mapping;
            Table = table;
            if (BusinessPropertyMapping != null)
                _column = BusinessPropertyMapping.Column;
        }

        public ScalarProperty Property { get; private set; }
        private IMapping Mapping { get; set; }
        private EntityType Table { get; set; }

        private ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType.PropertyMapping BusinessPropertyMapping
        {
            get { return GetBusinessPropertyMapping(Table); }
        }
        private ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.EntityType.PropertyMapping GetBusinessPropertyMapping(EntityType table)
        {
            return Mapping.GetSpecificMappingForTable(table).FirstOrDefault(pm => pm.Property == Property);
        }

        private Property _column;
        public Property Column
        {
            get { return _column; }
            set
            {
                _column = value;
                if (value != null)
                {
                    var propertyMapping = GetBusinessPropertyMapping(value.EntityType);
                    if (propertyMapping == null)
                        Mapping.AddMapping(Property, value);
                    else
                        Mapping.ChangeMapping(Property, value);
                }
                else
                    Mapping.RemoveMapping(Property, Table);
            }
        }
    }
}
