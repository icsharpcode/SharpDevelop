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
    public interface IDatabase : IDatabaseObjectBase
    {
        string ConnectionString { get; }
        IDatasource Datasource { get; }
        DatabaseObjectsCollection<ITable> Tables { get; }
        DatabaseObjectsCollection<IView> Views { get; }
        DatabaseObjectsCollection<IProcedure> Procedures { get; }
        DatabaseObjectsCollection<IConstraint> Constraints { get; }

        bool LoadDatabase();
    }
}
