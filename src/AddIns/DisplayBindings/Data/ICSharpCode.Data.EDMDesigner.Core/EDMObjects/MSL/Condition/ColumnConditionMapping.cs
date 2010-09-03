// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Condition;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.Association
{
    public class ColumnConditionMapping : ConditionMapping
    {
        private SSDL.Property.Property _column;
        public SSDL.Property.Property Column
        {
            get { return _column; }
            set
            {
                _column = value;
                if (value != null)
                    Table = value.EntityType;
            }
        }

        internal bool Generated { get; set; }
    }
}
