// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Type;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL
{
    public class EntityPropertiesMapping : PropertiesMapping
    {
        public EntityPropertiesMapping(EntityType entityType, ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType.EntityType table)
            : base(entityType, table)
        {
        }

        private bool _tpc;
        public bool TPC 
        {
            get { return _tpc; }
            set 
            { 
                _tpc = value;
                if (! value)
                    EntityType.Mapping.RemoveTPCMapping();
            }
        }

        public override IEnumerable<PropertyMapping> Mappings
        {
            get
            {
                IEnumerable<ScalarProperty> scalarProperties;
                if (TPC)
                    scalarProperties = EntityType.AllScalarProperties;
                else
                {
                    scalarProperties = EntityType.ScalarProperties;
                    if (EntityType.BaseType != null)
                        scalarProperties = EntityType.BaseType.Keys.Union(scalarProperties);
                }
                return scalarProperties.Except(EntityType.Mapping.ConditionsMapping.OfType<PropertyConditionMapping>().Select(pcm => pcm.CSDLProperty)).Select(property => new PropertyMapping(property, EntityType.Mapping, Table));
            }
        }
    }
}
