// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association
{
    public class AssociationMapping : EDMObjectBase
    {
        #region Fields

        private CSDL.Association.Association _association;
        private SSDL.EntityType.EntityType _ssdlTableMapped;
        private EventedObservableCollection<ColumnConditionMapping> _conditionsMapping;

        #endregion

        #region Events

        public event Action TableChanged;

        #endregion

        #region Properties

        internal bool MappingInit { get; private set; }

        public bool IsCompletlyMapped
        {
            get
            {
                return SSDLTableMapped != null && _association.PropertiesEnd.All(np => np.EntityType.Keys.All(k => np.Mapping.Any(pm => pm.Property == k && pm.Column != null && pm.Column.EntityType == SSDLTableMapped)));
            }
        }

        public SSDL.EntityType.EntityType SSDLTableMapped
        {
            get { return _ssdlTableMapped; }
            set
            {
                if (_ssdlTableMapped == value)
                    return;
                _ssdlTableMapped = value;
                OnTableChanged();
            }
        }

        public EventedObservableCollection<ColumnConditionMapping> ConditionsMapping
        {
            get
            {
                if (_conditionsMapping == null)
                    _conditionsMapping = new EventedObservableCollection<ColumnConditionMapping>();
                return _conditionsMapping;
            }
        }

        #endregion

        #region Constructor

        public AssociationMapping(CSDL.Association.Association association)
        {
            _association = association;
        }

        #endregion

        #region Methods

        internal void BeginInit()
        {
            MappingInit = true;
        }

        internal void EndInit()
        {
            MappingInit = false;
        }

        protected virtual void OnTableChanged()
        {
            if (MappingInit)
                return;
            if (TableChanged != null)
            {
                ConditionsMapping.Clear();
                TableChanged();
            }
            OnPropertyChanged("SSDLTableMapped");
            OnPropertyChanged("IsCompletlyMapped");
        }

        public void OnIsCompletelyMappedChanged()
        {
            OnPropertyChanged("IsCompletlyMapped");
        }

        #endregion
    }
}
