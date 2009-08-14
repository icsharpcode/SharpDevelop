#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Interfaces;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType
{
    public class EntityType : EDMObjectBase
    {
        #region Fields

        private string _entitySetName;
        private string _definingQuery;
        private List<Property.Property> _properties;

        #endregion

        #region Properties

        public SSDLContainer Container { get; internal set; }

        public StoreType? StoreType { get; set; }
        public string Schema { get; set; }
        public string StoreName { get; set; }
        public string StoreSchema { get; set; }
        public string Table { get; set; }

        public string EntitySetName
        {
            get { return _entitySetName; }
            set
            {
                _entitySetName = value;
                OnPropertyChanged("EntitySetName");
            }
        }

        public string DefiningQuery
        {
            get { return _definingQuery; }
            set
            {
                _definingQuery = value;
                OnPropertyChanged("DefiningQuery");
            }
        }

        public List<Property.Property> Properties
        {
            get
            {
                if (_properties == null)
                    _properties = new List<Property.Property>();
                return _properties;
            }
        }

        #endregion
    }
}
