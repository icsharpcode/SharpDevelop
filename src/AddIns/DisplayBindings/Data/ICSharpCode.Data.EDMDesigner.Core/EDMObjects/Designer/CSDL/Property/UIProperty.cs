// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
