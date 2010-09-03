// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
