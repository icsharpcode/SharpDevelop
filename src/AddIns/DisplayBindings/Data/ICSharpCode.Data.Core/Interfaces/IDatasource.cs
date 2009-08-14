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
