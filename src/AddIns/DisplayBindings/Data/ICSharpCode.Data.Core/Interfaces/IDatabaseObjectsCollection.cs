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
