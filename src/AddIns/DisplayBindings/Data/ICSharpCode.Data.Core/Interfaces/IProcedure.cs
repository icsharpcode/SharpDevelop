#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.DatabaseObjects;
using ICSharpCode.Data.Core.Enums;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IProcedure : IDatabaseObjectBase
    {
        string SchemaName { get; set; }
        string DataType { get; set; }
        int Length { get; set; }
        ProcedureType ProcedureType { get; set; }
        DatabaseObjectsCollection<IProcedureParameter> Items { get; }
    }
}
