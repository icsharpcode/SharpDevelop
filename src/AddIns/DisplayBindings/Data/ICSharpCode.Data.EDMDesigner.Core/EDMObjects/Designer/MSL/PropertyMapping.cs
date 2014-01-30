// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
