#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.Enums;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IProcedureParameter : IDatabaseObjectBase
    {
        string DataType { get; set; }
        ParameterMode ParameterMode { get; set; }
    }
}
