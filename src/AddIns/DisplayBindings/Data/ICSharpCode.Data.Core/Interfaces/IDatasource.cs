// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

#endregion

namespace ICSharpCode.Data.Core.Interfaces
{
    public interface IDatasource : IDatabaseObjectBase
    {
        IDatabaseDriver DatabaseDriver { get; }
        ObservableCollection<IDatabase> Databases { get; set; }
        string ConnectionString { get; }
        string UserDefinedConnectionString { get; set; }
        bool UseUserDefinedConnectionString { get; set; }
        string ProviderManifestToken { get; set; }
        bool PopulateDatabases();
    }
}
