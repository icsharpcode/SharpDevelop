// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
