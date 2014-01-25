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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;
using System.ComponentModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using System.Collections;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property
{
    public class NavigationProperty : PropertyBase
    {
        #region Fields

        private Cardinality _cardinality;
        private Visibility _setVisibility;
        private bool _generate = true;
        private EntityType _relatedEntityType;
        private AssociationRoleMapping _mapping;

        #endregion

        #region Constructor

        public NavigationProperty(Association.Association association)
        {
            Association = association;
        }

        #endregion

        #region Properties

        public new EntityType EntityType
        {
            get { return base.EntityType as EntityType; }
            internal set { base.EntityType = value; }
        }

        protected override Func<TypeBase, IList> GetPropertyCollection
        {
            get
            {
                return
                    baseType =>
                    {
                        var entityType = baseType as EntityType;
                        if (entityType == null)
                            return new List<NavigationProperty>();
                        return entityType.NavigationProperties;
                    };
            }
        }

        public Association.Association Association { get; private set; }

        public EntityType RelatedEntityType
        {
            get
            {
                if (_relatedEntityType == null)
                    _relatedEntityType = Association.PropertiesEnd.First(np => np != this).EntityType;
                return _relatedEntityType;
            }
        }

        [DisplayName("Multiplicity")]
        public Cardinality Cardinality
        {
            get { return _cardinality; }
            set
            {
                _cardinality = value;
                OnPropertyChanged("Cardinality");
                OnCardinalityChanged();
            }
        }

        [DisplayName("Setter")]
        public Visibility SetVisibility
        {
            get { return _setVisibility; }
            set
            {
                _setVisibility = value;
                OnPropertyChanged("SetVisibility");
            }
        }

        [DisplayName("Generate")]
        public bool Generate
        {
            get { return _generate; }
            set
            {
                _generate = value;
                OnPropertyChanged("Generate");
            }
        }

        public AssociationRoleMapping Mapping
        {
            get
            {
                if (_mapping == null)
                    _mapping = new AssociationRoleMapping(this);
                return _mapping;
            }
        }

        internal bool IsDeleted { get; set; }

        #endregion

        protected override PropertyBase Create()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnCardinalityChanged()
        {
            if (CardinalityChanged != null)
                CardinalityChanged();
        }
        public event Action CardinalityChanged;
    }
}
