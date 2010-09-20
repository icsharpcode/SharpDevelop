// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Function
{
    public class Function : EDMObjectBase
    {
        public Function()
        {
        }

        private Visibility _visibility;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        private EntityType _entityType;
        public EntityType EntityType
        {
            get { return _entityType; }
            set
            {
                _entityType = value;
                OnPropertyChanged("EntityType");
            }
        }

        private Property.PropertyType? _scalarReturnType;
        public Property.PropertyType? ScalarReturnType
        {
            get { return _scalarReturnType; }
            set
            {
                _scalarReturnType = value;
                OnPropertyChanged("ScalarReturnType");
            }
        }

        public string ReturnType
        {
            get
            {
                if (EntityType == null)
                {
                    if (ScalarReturnType == null)
                        return null;
                    return string.Format("Collection({0})", ScalarReturnType.Value.ToString());
                }
                return string.Format("Collection({0}.{1})", EntityType.Container.Namespace, EntityType.Name);
            }
        }

        private EventedObservableCollection<FunctionParameter> _parameters;
        public EventedObservableCollection<FunctionParameter> Parameters
        {
            get
            {
                if (_parameters == null)
                    _parameters = new EventedObservableCollection<FunctionParameter>();
                return _parameters;
            }
        }

        public ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function.Function SSDLFunction { get; set; }
    }
}
