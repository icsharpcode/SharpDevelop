// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Property;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.UI.UserControls.Mapping
{
    public class ColumnConditionComboBox : ColumnComboBox
    {
        protected override string NullValueText
        {
            get { return "<Delete>"; }
        }

        protected override void OnColumnComboBoxValueChanged(Property column)
        {
            ((ColumnConditionMapping)DataContext).Column = column;
            Column = column;
            if (column == null)
                OnDelete();
        }

        protected virtual void OnDelete()
        {
            if (Deleted != null)
                Deleted((ColumnConditionMapping)DataContext);
        }
        public event Action<ColumnConditionMapping> Deleted;
    }
}
