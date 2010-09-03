// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property
{
    public class UIRelatedProperty : UIProperty
    {
        public UIRelatedProperty(IUIType parentType, PropertyBase property)
            : base(parentType, property)
        {
        }

        public IUIType RelatedType 
        {
            get
            {
                var navigationProperty = BusinessInstance as NavigationProperty;
                if (navigationProperty != null)
                    return ParentType.View.EntityTypes[navigationProperty.RelatedEntityType];
                var complexProperty = BusinessInstance as ComplexProperty;
                if (complexProperty != null)
                    return ParentType.View.ComplexTypes[complexProperty.ComplexType];
                throw new NotImplementedException();
            }
        }
    }
}
