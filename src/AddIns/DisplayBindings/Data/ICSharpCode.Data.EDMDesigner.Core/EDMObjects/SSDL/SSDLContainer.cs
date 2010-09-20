// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
