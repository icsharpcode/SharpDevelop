// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System.Collections.Generic;
using System.Windows.Controls;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public abstract class TableMapping : UserControl
    {
        protected abstract void SetTablesValues(IEnumerable<EntityType> tables);

        private IEnumerable<EntityType> _tables;

        public IEnumerable<EntityType> Tables
        {
            get { return _tables; }
            set
            {
                _tables = value;
                SetTablesValues(value);
            }
        }

        public virtual EntityType Table { get; set; }

        private ComboBoxSelectedValueBindingWithNull<EntityType> _tableComboBoxValue;
        public ComboBoxSelectedValueBindingWithNull<EntityType> TableComboBoxValue
        {
            get
            {
                if (_tableComboBoxValue == null)
                    _tableComboBoxValue = new ComboBoxSelectedValueBindingWithNull<EntityType>(null, table => TableValueChange(table));
                return _tableComboBoxValue;
            }
        }

        protected virtual void TableValueChange(EntityType table)
        {
            Table = table;
        }
    }
}
