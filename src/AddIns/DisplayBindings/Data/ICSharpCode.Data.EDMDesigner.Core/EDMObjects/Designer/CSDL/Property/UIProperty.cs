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
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Property
{
    public class UIProperty : UIBusinessType<PropertyBase>
    {
        public UIProperty(IUIType parentType, PropertyBase property)
            : base(property)
        {
            ParentType = parentType;
        }

        protected override void BusinessInstancePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.BusinessInstancePropertyChanged(sender, e);
            switch (e.PropertyName)
            {
                case "IsKey":
                    OnPropertyChanged("IsKey");
                    break;
            }
        }

        public IUIType ParentType { get; private set; }

        public bool IsKey
        {
            get
            {
                var scalarProperty = BusinessInstance as ScalarProperty;
                return scalarProperty == null ? false : scalarProperty.IsKey;
            }
            set
            {
                var scalarProperty = BusinessInstance as ScalarProperty;
                if (scalarProperty == null)
                    throw new InvalidOperationException();
                scalarProperty.IsKey = value;
            }
        }
        public virtual bool CanBeKey
        {
            get
            {
                var scalarProperty = BusinessInstance as ScalarProperty;
                return scalarProperty.CanBeKey;
            }
        }

        public string PropertyType
        {
            get
            {
                if (BusinessInstance is ScalarProperty)
                    return "Scalar properties";
                if (BusinessInstance is NavigationProperty)
                    return "Navigation properties";
                if (BusinessInstance is ComplexProperty)
                    return "Complex properties";
                throw new NotImplementedException();
            }
        }

        public int PropertyTypeValue
        {
            get
            {
                if (BusinessInstance is ScalarProperty)
                    return 1;
                if (BusinessInstance is NavigationProperty)
                    return 3;
                if (BusinessInstance is ComplexProperty)
                    return 2;
                throw new NotImplementedException();
            }
        }

        public int Index
        {
            get
            {
                return ParentType.Properties.IndexOf(this);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
