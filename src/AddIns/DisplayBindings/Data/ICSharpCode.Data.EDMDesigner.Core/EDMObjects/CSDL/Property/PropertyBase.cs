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
using System.Collections;
using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property
{
    public abstract class PropertyBase : EDMObjectBase
    {
        #region Fields

        private TypeBase _entityType;
        private Visibility _getVisibility;

        #endregion

        #region Properties

        public TypeBase EntityType
        {
            get { return _entityType; }
            internal set
            {
                if (_entityType == value)
                    return;
                if (_entityType != null)
                    GetPropertyCollection(_entityType).Remove(this);
                _entityType = value;
            }
        }

        internal void AddToType(TypeBase type)
        {
            if (type != null)
            {
                EntityType = type;
                GetPropertyCollection(type).Add(this);
            }
        }
        protected abstract Func<TypeBase, IList> GetPropertyCollection { get; }

        [DisplayName("Getter")]
        public Visibility GetVisibility
        {
            get { return _getVisibility; }
            set
            {
                _getVisibility = value;
                OnPropertyChanged("GetVisibility");
            }
        }

        #endregion

        #region Methods

        internal virtual PropertyBase Duplicate()
        {
            var value = Create();
            value.Name = Name;
            value.GetVisibility = GetVisibility;
            return value;
        }

        protected abstract PropertyBase Create();

        #endregion
    }
}
