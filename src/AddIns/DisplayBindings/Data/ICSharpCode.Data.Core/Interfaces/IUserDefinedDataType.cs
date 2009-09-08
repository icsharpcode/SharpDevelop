#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IUserDefinedDataType : IDatabaseObjectBase
    {
        string SystemType { get; set; }
        int Length { get; set; }
        bool IsNullable { get; set; }
    }
}
