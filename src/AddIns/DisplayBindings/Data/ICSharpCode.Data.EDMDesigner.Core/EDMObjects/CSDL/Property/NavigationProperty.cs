// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
