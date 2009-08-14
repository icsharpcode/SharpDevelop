#region Usings

using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.MSL;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public class PropertyMappingColumnComboBox : ColumnComboBox
    {
        public PropertyMappingColumnComboBox()
        {
            Loaded += delegate { columnComboBox.SelectNullItemItemIfSelectedItemIsNull(); };
        }

        protected override void OnColumnComboBoxValueChanged(Property column)
        {
            ((PropertyMapping)DataContext).Column = column;
            Column = column;
        }
    }
}
