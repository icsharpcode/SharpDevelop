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

using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer
{
    public class DesignerView : IEnumerable<ITypeDesigner>
    {
        private EventedObservableCollection<ITypeDesigner> _typeDesignersLocations;

        public static DesignerView NewView()
        {
            return new DesignerView { Name = "NewDesignerView", Zoom = 100 };
        }

        public bool ArrangeTypeDesigners { get; set; } 
        
        public string Name { get; set; }

        public int Zoom { get; set; }

        public DesignerView DesignerViewCloned { get; set; }

        public DesignerView CloneOrThis
        {
            get
            {
                return DesignerViewCloned ?? this;
            }
        }

        public EventedObservableCollection<ITypeDesigner> TypeDesignersLocations
        {
            get
            {
                if (_typeDesignersLocations == null)
                    _typeDesignersLocations = new EventedObservableCollection<ITypeDesigner>();
                return _typeDesignersLocations;
            }
        }

        public bool AddTypeDesigner(ITypeDesigner typeDesigner)
        {
            if (TypeDesignersLocations.Contains(typeDesigner))
                return false;
            TypeDesignersLocations.Add(typeDesigner);
            return true;
        }

        public bool RemoveTypeDesigner(ITypeDesigner typeDesigner)
        {
            if (!TypeDesignersLocations.Contains(typeDesigner))
                return false;
            TypeDesignersLocations.Remove(typeDesigner);
            return true;
        }

        public bool ContainsEntityType(IUIType uiType)
        {
            return TypeDesignersLocations.Any(td => td.UIType == uiType);
        }
        public bool ContainsEntityType(TypeBase type)
        {
            return TypeDesignersLocations.Any(td => td.UIType.BusinessInstance == type);
        }

        public IEnumerator<ITypeDesigner> GetEnumerator()
        {
            return TypeDesignersLocations.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
