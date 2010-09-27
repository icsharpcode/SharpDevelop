// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
            get 
            {
                if (StoreType == null || StoreType == SSDL.EntityType.StoreType.Tables)
                    return _definingQuery;
                
                if (string.IsNullOrEmpty(_definingQuery))
                {
                    string definingQuery = string.Empty;

                    for (int i = 0; i < _properties.Count; i++)
                    {
                        if (string.IsNullOrEmpty(definingQuery))
                            definingQuery += "SELECT \r";

                        definingQuery += string.Format("[{0}].[{1}] AS [{1}]", _entitySetName, _properties[i].Name);

                        if (i < _properties.Count - 1)
                        {
                            definingQuery += ", \r";
                        }
                        else
                        {
                            definingQuery += string.Format(" \rFROM [{0}].[{1}] AS [{1}]", Schema, _entitySetName);
                        }
                    }

                    return definingQuery;
                }

                return _definingQuery; 
            }
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
