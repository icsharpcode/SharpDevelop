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
