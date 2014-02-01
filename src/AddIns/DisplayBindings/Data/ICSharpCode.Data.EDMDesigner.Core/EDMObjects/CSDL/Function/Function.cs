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
