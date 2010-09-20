// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
