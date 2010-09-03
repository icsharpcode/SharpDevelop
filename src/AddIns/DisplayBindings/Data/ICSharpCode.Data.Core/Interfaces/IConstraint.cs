// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Enums;
using ICSharpCode.Data.Core.DatabaseObjects;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IConstraint : IDatabaseObjectBase
    {
        List<string> PKColumnNames { get; set; }
        string PKTableName { get; set; }
        List<string> FKColumnNames { get; set; }
        string FKTableName { get; set; }
        DatabaseObjectsCollection<IColumn> PKColumns { get; }
        ITable PKTable { get; }
        DatabaseObjectsCollection<IColumn> FKColumns { get; }
        ITable FKTable { get; }
        Cardinality PKCardinality { get; }
        Cardinality FKCardinality { get; }
    }
}
