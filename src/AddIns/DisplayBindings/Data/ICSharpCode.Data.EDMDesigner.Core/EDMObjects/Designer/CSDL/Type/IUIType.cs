// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type
{
    public interface IUIType
    {
        TypeBase BusinessInstance { get; }
        string Name { get; set; }
        IndexableUIBusinessTypeObservableCollection<PropertyBase, UIProperty> Properties { get; }
        UIProperty AddScalarProperty(string propertyName, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property.PropertyType propertyType);
        UIRelatedProperty AddComplexProperty(string propertyName, ComplexType complexType);
        void DeleteProperty(UIProperty property);
        CSDLView View { get; }
        event Action<UIRelatedProperty> RelatedPropertyDeleted;
    }
}
