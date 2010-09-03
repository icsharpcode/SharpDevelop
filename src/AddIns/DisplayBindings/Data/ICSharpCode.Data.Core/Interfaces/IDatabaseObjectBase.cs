// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.Core.DatabaseObjects;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IDatabaseObjectBase
    {
        string Name { get; set; }
        IDatabaseObjectBase Parent { get; set; }
        bool IsSelected { get; set; }
    }

    public interface IDatabaseObjectBase<T> : IDatabaseObjectBase where T : IDatabaseObjectBase
    {
        DatabaseObjectsCollection<T> Items { get; set; }
    }
}
