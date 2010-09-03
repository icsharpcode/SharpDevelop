// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public class EntityPropertiesMapping : PropertiesMapping
    {
        private bool _tpc;
        public bool TPC
        {
            get { return _tpc; }
            set
            {
                _tpc = value;
                if (Mappings != null)
                    MappingEnumerable = Mappings.Mappings;
                propertiesGrid.ItemsSource = MappingEnumerable;
            }
        }
    }
}
