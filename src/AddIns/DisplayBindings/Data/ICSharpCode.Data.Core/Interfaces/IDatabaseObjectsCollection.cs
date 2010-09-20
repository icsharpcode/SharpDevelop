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
    public interface IDatabaseObjectsCollection
    {
        List<IDatabaseObjectBase> NonGenericItems { get; }
    }
}
