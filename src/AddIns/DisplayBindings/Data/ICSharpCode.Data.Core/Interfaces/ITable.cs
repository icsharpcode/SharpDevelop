// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using ICSharpCode.Data.Core.DatabaseObjects;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface ITable : IDatabaseObjectBase<IColumn>
    {
        string TableName { get; set; }
        string SchemaName { get; set; }
        DatabaseObjectsCollection<IConstraint> Constraints { get; }
        bool HasKeyDefined { get; }
        bool HasCompositeKey { get; }
    }
}
