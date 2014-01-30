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
using System.Collections.ObjectModel;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL
{
    public class SSDLContainer : EDMObjectBase
    {
        #region Fields

        private EventedObservableCollection<EntityType.EntityType> _entityTypes;
        private EventedObservableCollection<Association.Association> _associationSets;
        private EventedObservableCollection<Function.Function> _functions;

        #endregion

        #region Properties

        public string Namespace { get; set; }
        public string Provider { get; set; }
        public string ProviderManifestToken { get; set; }

        public EventedObservableCollection<EntityType.EntityType> EntityTypes
        {
            get
            {
                if (_entityTypes == null)
                {
                    _entityTypes = new EventedObservableCollection<EntityType.EntityType>();
                    _entityTypes.ItemAdded += entityType => entityType.Container = this;
                }

                return _entityTypes;
            }
        }

        public EventedObservableCollection<Association.Association> AssociationSets
        {
            get
            {
                if (_associationSets == null)
                {
                    _associationSets = new EventedObservableCollection<Association.Association>();
                    _associationSets.ItemAdded += association => association.Container = this;
                }

                return _associationSets;
            }
        }

        public EventedObservableCollection<Function.Function> Functions
        {
            get
            {
                if (_functions == null)
                {
                    _functions = new EventedObservableCollection<Function.Function>();
                }

                return _functions;
            }
        }

        #endregion
    }
}
