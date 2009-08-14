#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Enums;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IConstraint : IDatabaseObjectBase
    {
        string PKColumnName { get; set; }
        string PKTableName { get; set; }
        string FKColumnName { get; set; }
        string FKTableName { get; set; }
        IColumn PKColumn { get; }
        ITable PKTable { get; }
        IColumn FKColumn { get; }
        ITable FKTable { get; }
        Cardinality PKCardinality { get; }
        Cardinality FKCardinality { get; }
    }
}
