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
