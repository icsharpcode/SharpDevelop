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
