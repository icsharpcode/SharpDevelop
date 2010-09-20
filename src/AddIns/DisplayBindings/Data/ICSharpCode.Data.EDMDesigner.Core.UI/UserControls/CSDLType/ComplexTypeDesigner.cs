// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.CSDLType
{
    public class ComplexTypeDesigner : TypeBaseDesigner
    {
        public ComplexTypeDesigner(UIComplexType complexType)
            : base(complexType)
        {
            ComplexType = complexType;
        }

        public UIComplexType ComplexType { get; private set; }

        protected override void DrawRelation(UIRelatedProperty relatedProperty)
        {
            base.DrawRelation(relatedProperty);
            foreach (var typeDesigner in Designer.Children.OfType<TypeBaseDesigner>().ToList())
                typeDesigner.DrawComplexPropertyRelation(this, relatedProperty);
        }
    }
}
