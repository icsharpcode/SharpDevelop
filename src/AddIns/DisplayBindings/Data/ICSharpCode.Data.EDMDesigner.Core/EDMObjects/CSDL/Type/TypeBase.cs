// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;
using System.ComponentModel;
using System.Collections.ObjectModel;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type
{
    public class TypeBase : EDMObjectBase
    {
        #region Fields

        private Visibility _visibility;
        private EventedObservableCollection<ScalarProperty> _scalarProperties;
        private EventedObservableCollection<ComplexProperty> _complexProperties;

        #endregion

        #region Properties

        [DisplayName("Access")]
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
            }
        }

        public EventedObservableCollection<ScalarProperty> ScalarProperties
        {
            get
            {
                if (_scalarProperties == null)
                {
                    _scalarProperties = new EventedObservableCollection<ScalarProperty>();
                    _scalarProperties.ItemAdded += scalarProperty => scalarProperty.EntityType = this;
                    _scalarProperties.ItemRemoved += scalarProperty => OnScalarPropertyRemoved(scalarProperty);
                }
                return _scalarProperties;
            }
        }

        public EventedObservableCollection<ComplexProperty> ComplexProperties
        {
            get
            {
                if (_complexProperties == null)
                {
                    _complexProperties = new EventedObservableCollection<ComplexProperty>();
                    _complexProperties.ItemAdded += complexProperty => complexProperty.EntityType = this;
                }
                return _complexProperties;
            }
        }

        public virtual IEnumerable<ScalarProperty> AllScalarProperties
        {
            get
            {
                return ScalarProperties;
            }
        }

        public virtual IEnumerable<ComplexProperty> AllComplexProperties
        {
            get
            {
                return ComplexProperties;
            }
        }

        internal CSDLContainer Container { get; set; }

        #endregion

        #region Constructor

        internal TypeBase()
        {
        }

        #endregion

        #region Methods

        public virtual PropertyBase DuplicateProperty(PropertyBase property)
        {
            var propertyBase = property.Duplicate();
            propertyBase.AddToType(this);
            return propertyBase;
        }

        protected virtual void OnScalarPropertyRemoved(ScalarProperty scalarProperty)
        {
        }

        #endregion
    }
}
